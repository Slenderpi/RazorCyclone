using System;
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
    public RectTransform MainCanonCrosshair;
    public Animator CanonHitmarkerAnim;
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
    public GameObject SpinCounterBG;
    
    [Header("Misc.")]
    public TMP_Text RoundLabel;
    public Animator RoundLabelAnimator;
    public RectTransform MomentumPanel;
    
    [Header("Input Overlay")]
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
    }
    
    void Update() {
        if (!GameManager.CurrentPlayer) return;
        if (Time.deltaTime > 0) {
            setFuelSliderFill();
            setHealthSliderFill();
        }
    }
    
    void setFuelSliderFill() {
        float fill = sodFuelFill.Update(currFuel, Time.deltaTime);
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
        float fill = sodHealthFill.Update(currHlth, Time.deltaTime);
        HealthSlider2.fillAmount = fill * BENT_BAR_MAX_FILL;
        float ang = 180 - (2 * fill - 1) * (180f * BENT_BAR_MAX_FILL);
        HealthSliderLevel.localPosition = new Vector3(
            BENT_BAR_CENTERED_RADIUS * Mathf.Cos(ang * Mathf.Deg2Rad),
            BENT_BAR_CENTERED_RADIUS * Mathf.Sin(ang * Mathf.Deg2Rad),
            0
        );
        HealthSliderLevel.localRotation = Quaternion.Euler(0, 0, ang);
    }
    
    void LateUpdate() {
        if (!GameManager.CurrentPlayer) return;
        if (Time.deltaTime > 0) {
            lerpSway();
            lerpSpeed();
        }
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
    
    public void OnFireCanon(bool started) {
        KeyImageM2.color = started ? Color.white : Color.gray;
    }
    
    public void OnFireVacuum(bool started) {
        KeyImageM1.color = started ? Color.white : Color.gray;
    }
    
    public void UpdateCrosshairPositions(Vector3 screenPointVacuum, Vector3 screenPointCanon) {
        if (screenPointVacuum.z > 0.01f) {
            if (!MainVacuumCrosshair.gameObject.activeSelf) MainVacuumCrosshair.gameObject.SetActive(true);
            MainVacuumCrosshair.position = screenPointVacuum;
        } else {
            if (MainVacuumCrosshair.gameObject.activeSelf) MainVacuumCrosshair.gameObject.SetActive(false);
        }
        if (screenPointCanon.z < 0.01f) {
            if (!MainCanonCrosshair.gameObject.activeSelf) MainCanonCrosshair.gameObject.SetActive(true);
            MainCanonCrosshair.position = screenPointCanon;
        } else {
            if (MainCanonCrosshair.gameObject.activeSelf) MainCanonCrosshair.gameObject.SetActive(false);
        }
    }
    
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
    
    void onSpinCompleted(int newSpinCount) {
        resetSpinTileColors();
        // TileRendW.material = SpinTileMatProgress;
        if (newSpinCount == 1)
            SpinCounterBG.SetActive(true);
        setSpinCounterText(newSpinCount);
    }
    
    void onSpinsSpent(int prevSpinCount, int newSpinCount) {
        setSpinCounterText(newSpinCount);
        SpinCounterBG.SetActive(false);
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
    
    public void OnPlayerDamagedEnemy(EnemyBase enemy) {
        CanonHitmarkerAnim.SetTrigger("Hit");
    }
    
    public void OnPlayerKilledEnemy(EnemyBase enemy, bool wasCanon) {
        if (wasCanon) {
            CanonHitmarkerAnim.SetTrigger("Kill");
        } else {
            VacuumHitmarkerAnim.SetTrigger("Kill");
        }
    }
    
    public void OnRoundCompleted() {
        RoundLabelAnimator.SetTrigger("Completed");
    }
    
    public void OnUpdateRoundNumber(int newRound) {
        RoundLabel.text = "Round: " + newRound;
        RoundLabelAnimator.SetTrigger("NewRound");
    }
    
    // public void SetReadTimerOn(bool setReadOn) {
    //     if (setReadOn) {
            
    //     } else {
            
    //     }
    // }
    
    void resetSpinTileColors() {
        TileRendW.material = SpinTileMatDefault;
        TileRendA.material = SpinTileMatDefault;
        TileRendS.material = SpinTileMatDefault;
        TileRendD.material = SpinTileMatDefault;
    }
    
    void setSpinCounterText(int spins) {
        SpinCounterText.text = spins.ToString();
    }
    
    public override void OnGameResumed() {
        // SetActive(true);
    }
    
    public override void OnGamePaused() {
        // SetActive(false);
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
        ResetUIElements(plr);
    }
    
    public override void OnPlayerDestroying(PlayerCharacterCtrlr plr) {
        // NOTE: Not sure if these unsubscriptions are necessary since the player's getting destroyed anyway
        plr.A_FuelAdded -= OnFuelAdded;
        plr.A_FuelSpent -= OnFuelSpent;
        plr.A_PlayerTakenDamage -= OnDamageTaken;
        plr.A_PlayerHealed -= OnPlayerHealed;
        plr.A_SpinProgressed -= onSpinProgressed;
        plr.A_SpinProgressReset -= onSpinProgressReset;
        plr.A_SpinCompleted -= onSpinCompleted;
        plr.A_SpinsSpent -= onSpinsSpent;
    }
    
    public void ResetUIElements(PlayerCharacterCtrlr plr) {
        OnFuelAdded(0, 1);
        OnTurnInputChanged(Vector2.zero);
        OnVertInputChanged(0);
        OnFireVacuum(false);
        OnFireCanon(false);
        updateHealthUI(100, 100);
        resetSpinTileColors();
        setSpinCounterText(plr.currentBikeSpins);
        RoundLabel.text = "Round: --";
        if (plr.currentBikeSpins == 0)
            SpinCounterBG.SetActive(false);
        if (gameObject.activeSelf) FuelOutlineAnimator.SetTrigger("Reset");
    }

}