using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePanel : UIPanel {
    
    [Header("Parameters")]
    [Range(0.01f, 10)]
    public float swf = 2;
    [Range(0, 1)]
    public float swz = 0.6f;
    [Range(-2, 2)]
    public float swr = 2;
    [SerializeField]
    [Range(5, 25)]
    float maxLookDelta = 7;
    [Range(5, 50)]
    [SerializeField]
    float maxSwayDist = 30;
    [SerializeField]
    float swayYExaggerateFactor = 2f;
    [Range(0.01f, 10)]
    public float scf = 3;
    [Range(0, 1)]
    public float scz = 1;
    [Range(-2, 2)]
    public float scr = 1;
    [SerializeField]
    float AdditionalScale = 0.17f; // The scale will reach this amount at a speed of HighAdditionalScaleSpeed
    [SerializeField]
    float HighAdditionalScaleSpeed = 50; // The scale will reach AdditionalScale at this speed
    [Range(0.01f, 10)]
    public float fillf = 5;
    [Range(0, 1)]
    public float fillz = 1;
    [Range(0, 1)]
    public float fillr = 1;
    SecondOrderDynamicsF sodLookX;
    SecondOrderDynamicsF sodLookY;
    SecondOrderDynamicsF sodSpeed;
    SecondOrderDynamicsF sodFuelFill;
    SecondOrderDynamicsF sodHealthFill;
    const float BENT_BAR_MAX_FILL = 0.2f;
    // RAD = SLIDER_BG.Width / 2 * SLIDER_BG.Scale - SLIDER_LEVEL.Width / 2
    const float BENT_BAR_CENTERED_RADIUS = 115f * 1.927799f - 22.16969f / 2f;
    
    [Header("Fuel Gauge")]
    public Slider FuelSlider;
    public Animator FuelOutlineAnimator;
    public Animator FuelFillAnimator;
    public Image FuelSlider2;
    public RectTransform FuelSliderLevel;
    
    [Header("Healthbar")]
    public Slider HealthSlider;
    public TMP_Text HealthText;
    public Animator HealthFillAnimator;
    public Image HealthSlider2;
    public RectTransform HealthSliderLevel;
    
    [Header("Crosshairs")]
    public RectTransform MainVacuumCrosshair;
    public RectTransform MainCannonCrosshair;
    public Animator CannonHitmarkerAnim;
    public Animator VacuumHitmarkerAnim;
    
    [Header("Bike Spinning")]
    public Material SpinTileMatDefault;
    public Material SpinTileMatRecent;
    public Material SpinTileMatProgress;
    // public Material SpinTileMat;
    public MeshRenderer TileRendW;
    public MeshRenderer TileRendA;
    public MeshRenderer TileRendS;
    public MeshRenderer TileRendD;
    public TMP_Text SpinCounterText;
    // public GameObject SpinCounterBG;
    public Image SpinCounterOutline;
    [SerializeField]
    Color SpinOutlineExpiringColor = Color.magenta;
    [SerializeField]
    Color SpinOutlineHoldColor = Color.black;
    
    [Header("Killfeed")]
    public RectTransform KillfeedArea;
    float killFeedElemHeight;
    int currKillElem;
    public RectTransform[] KillfeedElements;
    RawImage[] killfeedEntryImages; // Groups of 4: card bg, wp icon, kill type icon, enemy icon
    Texture2D[] killfeedTextures; // Weapon, Weapon, Weapon, KillType, KillType, Enemies...
    
    [Header("Misc.")]
    public Animator LavaWarning; // TODO
    public TMP_Text RoundLabel;
    public Animator RoundLabelAnimator;
    public RectTransform MomentumPanel;
    
    [Header("Input Overlay")]
    public bool inputOverlayStartsOn = false;
    public GameObject InputOverlay;
    public Image KeyImageW;
    public Image KeyImageA;
    public Image KeyImageS;
    public Image KeyImageD;
    public Image KeyImageM1;
    public Image KeyImageM2;
    public Image KeyImageSpace;
    public Image KeyImageShift;
    
    float currFuel = 1;
    float currHlth = 1;
    
    
    
    public override void Init() {
        base.Init();
        sodLookX = new SecondOrderDynamicsF(swf, swz, swr, 0);
        sodLookY = new SecondOrderDynamicsF(swf, swz, swr, 0);
        sodSpeed = new SecondOrderDynamicsF(scf, scz, scr, 0);
        sodFuelFill = new SecondOrderDynamicsF(fillf, fillz, fillr, 1);
        sodHealthFill = new SecondOrderDynamicsF(fillf, fillz, fillr, 1);
#if !UNITY_EDITOR && !KEEP_DEBUG
        Destroy(InputOverlay);
#else
        InputOverlay.SetActive(inputOverlayStartsOn);
#endif
        initKillfeed();
    }
    
    void Start() {
        StartCoroutine(loadKFIconsAsync());
    }
    
    void Update() {
        if (!GameManager.CurrentPlayer) return;
        if (Time.deltaTime > 0) {
            setFuelSliderFill();
            setHealthSliderFill();
        }
    }
    
    void LateUpdate() {
        if (!GameManager.CurrentPlayer) return;
        if (Time.deltaTime > 0) {
            lerpSway();
            lerpSpeed();
            lerpKillfeedElemPosns();
        }
    }
    
    void setFuelSliderFill() {
        float fill = Mathf.Clamp(sodFuelFill.Update(currFuel, Time.deltaTime), 0, 1);
        FuelSlider2.fillAmount = fill * BENT_BAR_MAX_FILL;
        float ang = (2 * fill - 1) * (180f * BENT_BAR_MAX_FILL);
        FuelSliderLevel.localPosition = new Vector3(
            BENT_BAR_CENTERED_RADIUS * Mathf.Cos(ang * Mathf.Deg2Rad),
            BENT_BAR_CENTERED_RADIUS * Mathf.Sin(ang * Mathf.Deg2Rad),
            0
        );
        FuelSliderLevel.localRotation = Quaternion.Euler(0, 0, ang);
    }
    
    void setHealthSliderFill() {
        float fill = Mathf.Clamp(sodHealthFill.Update(currHlth, Time.deltaTime), 0, 1);
        HealthSlider2.fillAmount = fill * BENT_BAR_MAX_FILL;
        float ang = 180 - (2 * fill - 1) * (180f * BENT_BAR_MAX_FILL);
        HealthSliderLevel.localPosition = new Vector3(
            BENT_BAR_CENTERED_RADIUS * Mathf.Cos(ang * Mathf.Deg2Rad),
            BENT_BAR_CENTERED_RADIUS * Mathf.Sin(ang * Mathf.Deg2Rad),
            0
        );
        HealthSliderLevel.localRotation = Quaternion.Euler(0, 0, ang);
    }
    
    void lerpSway() {
#if UNITY_EDITOR
        sodLookX.SetDynamics(swf, swz, swr);
        sodLookY.SetDynamics(swf, swz, swr);
#endif
        Vector2 newPos = new Vector2(
            sodLookX.Update(Mathf.Clamp(-GameManager.CurrentPlayer.lookDelta.x / maxLookDelta, -1, 1), Time.deltaTime),
            sodLookY.Update(Mathf.Clamp(-GameManager.CurrentPlayer.lookDelta.y / maxLookDelta, -1, 1), Time.deltaTime) * swayYExaggerateFactor
        );
        MomentumPanel.anchoredPosition = newPos * maxSwayDist;
    }
    
    void lerpSpeed() {
#if UNITY_EDITOR
        sodSpeed.SetDynamics(scf, scz, scr);
#endif
        float speed = GameManager.CurrentPlayer.rb.velocity.magnitude;
        float newScale = Mathf.LerpUnclamped(1, 1 - AdditionalScale, sodSpeed.Update(speed, Time.deltaTime) / HighAdditionalScaleSpeed);
        MomentumPanel.localScale = new Vector3(newScale, newScale, 1);
    }
    
#if UNITY_EDITOR || KEEP_DEBUG
    public void OnFireCannon(bool started) {
        KeyImageM2.color = started ? Color.white : Color.gray;
    }
    
    public void OnFireVacuum(bool started) {
        KeyImageM1.color = started ? Color.white : Color.gray;
    }
#endif
    
    public void UpdateCrosshairPositions(Vector3 screenPointVacuum, Vector3 screenPointCannon) {
        if (screenPointVacuum.z > 0.01f) {
            if (!MainVacuumCrosshair.gameObject.activeSelf) MainVacuumCrosshair.gameObject.SetActive(true);
            MainVacuumCrosshair.position = screenPointVacuum;
        } else {
            if (MainVacuumCrosshair.gameObject.activeSelf) MainVacuumCrosshair.gameObject.SetActive(false);
        }
        if (screenPointCannon.z > 0.01f) {
            if (!MainCannonCrosshair.gameObject.activeSelf) MainCannonCrosshair.gameObject.SetActive(true);
            MainCannonCrosshair.position = screenPointCannon;
        } else {
            if (MainCannonCrosshair.gameObject.activeSelf) MainCannonCrosshair.gameObject.SetActive(false);
        }
    }
    
#if UNITY_EDITOR || KEEP_DEBUG
    public void OnTurnInputChanged(Vector2 v) {
        if (v.y > 0.001f) { // Forward/backward
            KeyImageW.color = Color.white;
            KeyImageS.color = Color.gray;
        } else if (v.y < -0.001f) {
            KeyImageW.color = Color.gray;
            KeyImageS.color = Color.white;
        } else {
            KeyImageW.color = Color.gray;
            KeyImageS.color = Color.gray;
        }
        if (v.x > 0.001f) { // Right/left
            KeyImageD.color = Color.white;
            KeyImageA.color = Color.gray;
        } else if (v.x < -0.001f) {
            KeyImageD.color = Color.gray;
            KeyImageA.color = Color.white;
        } else {
            KeyImageD.color = Color.gray;
            KeyImageA.color = Color.gray;
        }
    }
    
    public void OnVertInputChanged(float y) {
        if (y > 0.001f) { // Up/down
            KeyImageSpace.color = Color.white;
            KeyImageShift.color = Color.gray;
        } else if (y < -0.001f) {
            KeyImageSpace.color = Color.gray;
            KeyImageShift.color = Color.white;
        } else {
            KeyImageSpace.color = Color.gray;
            KeyImageShift.color = Color.gray;
        }
    }
#endif
    
    public void OnFuelAdded(float changeAmnt, float perc) {
        // FuelSlider.value = perc;
        // TODO
        currFuel = perc;
        
        if (!gameObject.activeSelf) return;
        // if (perc == 1f || true) FuelOutlineAnimator.SetTrigger("RefillFuel");
        
        // Temporary?
        GameManager.Instance.Audio2D.PlayClipSFX(AudioPlayer2D.EClipSFX.Plr_PickupFuel);
    }
    
    public void OnFuelSpent(float amnt, float perc, bool spentAsHealth) {
        // FuelSlider.value = perc;
        // if (spentAsHealth)
        //     HealthFillAnimator.SetTrigger("HealthAsFuel");
        // else
        //     FuelFillAnimator.SetTrigger("SpendFuel");
        
        // TODO
        currFuel = perc;
    }
    
    public void OnDamageTaken(float amnt) {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        updateHealthUI(plr.CurrentHealth, plr.MaxHealth);
    }
    
    public void OnPlayerHealed(float amnt) {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        updateHealthUI(plr.CurrentHealth, plr.MaxHealth);
    }
    
    void onSpinProgressed(int progress) {
        switch (progress) {
        case 1:
            TileRendD.material = SpinTileMatRecent;
            break;
        case 2:
            TileRendS.material = SpinTileMatRecent;
            TileRendD.material = SpinTileMatProgress;
            break;
        case 3:
            TileRendA.material = SpinTileMatRecent;
            TileRendS.material = SpinTileMatProgress;
            break;
        case -1:
            TileRendA.material = SpinTileMatRecent;
            break;
        case -2:
            TileRendS.material = SpinTileMatRecent;
            TileRendA.material = SpinTileMatProgress;
            break;
        case -3:
            TileRendD.material = SpinTileMatRecent;
            TileRendS.material = SpinTileMatProgress;
            break;
        }
    }
    
    void onSpinProgressReset(int progressBeforeReset) {
        resetSpinTileColors();
    }
    
    void onSpinCompleted(int newSpinCount, bool isGrounded) {
        resetSpinTileColors();
        // TileRendW.material = SpinTileMatProgress;
        setSpinCounterText(newSpinCount);
        if (isGrounded) {
            SpinCounterOutline.color = SpinOutlineExpiringColor;
            updateSpinDecayFill(0);
        } else {
            SpinCounterOutline.color = SpinOutlineHoldColor;
            updateSpinDecayFill(1);
        }
        if (!SpinCounterOutline.gameObject.activeSelf)
            SpinCounterOutline.gameObject.SetActive(true);
    }
    
    void onSpinsSpent(int prevSpinCount, int newSpinCount) {
        setSpinCounterText(newSpinCount);
        SpinCounterOutline.gameObject.SetActive(false);
    }
    
    void onSpinsDecaying(float runoutTimerPerc) {
        updateSpinDecayFill(1 - runoutTimerPerc);
    }
    
    void onSpinsExpired() {
        setSpinCounterText(0);
        SpinCounterOutline.gameObject.SetActive(false);
    }
    
    void onGroundednessChanged(bool newGrounded) {
        if (!newGrounded) {
            SpinCounterOutline.color = SpinOutlineHoldColor;
        } else {
            SpinCounterOutline.color = SpinOutlineExpiringColor;
        }
    }
    
    void updateHealthUI(float currH, float maxH) {
        // HealthSlider.value = currH / maxH;
        // HealthText.text = Mathf.CeilToInt(currH).ToString();
        
        // TODO
        currHlth = currH / maxH;
    }
    
    public void OnOutOfFuel() {
        AnimatorStateInfo asi = FuelOutlineAnimator.GetCurrentAnimatorStateInfo(0);
        if (!asi.IsName("OutOfFuelBlink") || asi.normalizedTime >= 0.8f)
            FuelOutlineAnimator.SetTrigger("OutOfFuel");
    }
    
    public void OnPlayerDamagedEnemy(EDamageType dtype, bool wasKill, EnemyBase enemy) {
        if (wasKill) {
            CannonHitmarkerAnim.SetTrigger("Hit");
        } else {
            if (dtype == EDamageType.Vacuum) {
                VacuumHitmarkerAnim.SetTrigger("Kill");
            } else {
                CannonHitmarkerAnim.SetTrigger("Kill");
            }
        }
        
        // Increment current card index
        currKillElem = (currKillElem + 1) % KillfeedElements.Length;
        
        // Position card directly at the bottom
        KillfeedElements[currKillElem].anchoredPosition = Vector2.zero;
        
        // Set color of card background
        killfeedEntryImages[currKillElem * 4].color = wasKill ? Color.red : Color.yellow;
        
        // Set images for card icons
        killfeedEntryImages[currKillElem * 4 + 1].texture = killfeedTextures[dtype switch {
            EDamageType.Vacuum => 0,
            EDamageType.Projectile => 1,
            EDamageType.ProjectileRicochet => 2,
            _ => throw new Exception($"ERROR: Player damage type when damaging enemy is invalid. Type given: {dtype}.")
        }];
        killfeedEntryImages[currKillElem * 4 + 2].texture = killfeedTextures[4];
        killfeedEntryImages[currKillElem * 4 + 3].texture = killfeedTextures[enemy.etypeid < EEnemyType.COUNT ? (int)enemy.etypeid + 5 : 5];
        
        // Enable new card, disable oldest card
        KillfeedElements[currKillElem].gameObject.SetActive(true);
        KillfeedElements[(currKillElem - 4 + KillfeedElements.Length) % KillfeedElements.Length].gameObject.SetActive(false);
    }
    
    public void OnRoundCompleted() {
        RoundLabelAnimator.SetTrigger("Completed");
    }
    
    public void OnUpdateRoundNumber(int newRound) {
        RoundLabel.text = "Round: " + newRound;
        RoundLabelAnimator.SetTrigger("NewRound");
    }
    
    void resetSpinTileColors() {
        TileRendW.material = SpinTileMatDefault;
        TileRendA.material = SpinTileMatDefault;
        TileRendS.material = SpinTileMatDefault;
        TileRendD.material = SpinTileMatDefault;
    }
    
    void setSpinCounterText(int spins) {
        SpinCounterText.text = spins.ToString();
    }
    
    void updateSpinDecayFill(float perc) {
        // float b = 0.05f;
        // SpinCounterOutline.fillAmount = b * Mathf.Sin(8 * Mathf.PI * perc) / (8 * Mathf.PI * (1 - b)) + perc;
        SpinCounterOutline.fillAmount = perc;
    }
    
    void lerpKillfeedElemPosns() {
        for (int i = 0; i < 4; i++) {
            int curr = (currKillElem - i + KillfeedElements.Length) % KillfeedElements.Length;
            KillfeedElements[curr].anchoredPosition = Vector2.Lerp(
                KillfeedElements[curr].anchoredPosition,
                new(0, i * killFeedElemHeight),
                0.1f
            );
        }
    }
    
    public override void OnPlayerSpawned(PlayerCharacterCtrlr plr) {
        plr.A_FuelAdded += OnFuelAdded;
        plr.A_FuelSpent += OnFuelSpent;
        plr.A_PlayerTakenDamage += OnDamageTaken;
        plr.A_PlayerHealed += OnPlayerHealed;
        plr.A_SpinProgressed += onSpinProgressed;
        plr.A_SpinProgressReset += onSpinProgressReset;
        plr.A_SpinCompleted += onSpinCompleted;
        plr.A_SpinsSpent += onSpinsSpent;
        plr.A_SpinsDecaying += onSpinsDecaying;
        plr.A_SpinsExpired += onSpinsExpired;
        plr.A_GroundednessChanged += onGroundednessChanged;
        ResetUIElements(plr);
    }
    
    // public override void OnPlayerDestroying(PlayerCharacterCtrlr plr) {
    //     unloadKFIcons();
    // }
    
    public void ResetUIElements(PlayerCharacterCtrlr plr) {
        OnFuelAdded(0, 1);
        updateHealthUI(100, 100);
        resetSpinTileColors();
        setSpinCounterText(plr.currentBikeSpins);
        RoundLabel.text = "Round: --";
        if (plr.currentBikeSpins == 0)
            SpinCounterOutline.gameObject.SetActive(false);
        if (gameObject.activeSelf) FuelOutlineAnimator.SetTrigger("Reset");
#if UNITY_EDITOR || KEEP_DEBUG
        OnTurnInputChanged(Vector2.zero);
        OnVertInputChanged(0);
        OnFireVacuum(false);
        OnFireCannon(false);
#endif
    }
    
    void initKillfeed() {
        int numKFElems = KillfeedElements.Length;
        currKillElem = numKFElems - 1;
        killFeedElemHeight = KillfeedElements[0].rect.height;
        killfeedEntryImages = new RawImage[numKFElems * 4];
        for (int i = 0; i < numKFElems; i++) {
            // GetComponentsInChildren() is supposed to only look in children but for some reason this one includes the current gameObject
            RawImage[] elemImages = KillfeedElements[i].GetComponentsInChildren<RawImage>();
            for (int ri = 0; ri < 4; ri++)
                killfeedEntryImages[i * 4 + ri] = elemImages[ri];
            KillfeedElements[i].gameObject.SetActive(false);
        }
    }
    
    IEnumerator loadKFIconsAsync() {
        string[] iconPaths = {
            // Weapons
            "Killfeed Icons/Vacuum",
            "Killfeed Icons/Cannon",
            "Killfeed Icons/Ricochet",
            // Arrows
            "Killfeed Icons/SmallArrow",
            "Killfeed Icons/BigArrow",
            // Enemy icons
            "Killfeed Icons/Bug",
            "Killfeed Icons/Bird",
            "Killfeed Icons/BirdOutline",
            "Killfeed Icons/Crab",
            "Killfeed Icons/CrabOutline",
            "Killfeed Icons/Turtle",
            "Killfeed Icons/CentipedeIcon"
        };
        killfeedTextures = new Texture2D[iconPaths.Length];
        ResourceRequest rr;
        for (int i = 0; i < iconPaths.Length; i++) {
            rr = Resources.LoadAsync<Texture2D>(iconPaths[i]);
            yield return rr;
            killfeedTextures[i] = rr.asset as Texture2D;
            // if (!killfeedTextures[i])
            //     Debug.LogWarning($"FAILED TO LOAD KILLFEED ICON: \"{iconPaths[i]}\"");
            // else
            //     print($"Loaded '{killfeedTextures[i].name}' (path: '{iconPaths[i]}')");
        }
    }
    
    void unloadKFIcons() {
        if (killfeedTextures == null) return;
        for (int i = 0; i < killfeedTextures.Length; i++) {
            Resources.UnloadAsset(killfeedTextures[i]);
        }
    }
    
}