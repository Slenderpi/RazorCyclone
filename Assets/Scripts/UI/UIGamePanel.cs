using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePanel : UIPanel {
    
    [Header("Fuel Gauge")]
    public Slider FuelSlider;
    public Animator FuelOutlineAnimator;
    public Animator FuelFillAnimator;
    
    [Header("Healthbar")]
    public Slider HealthSlider;
    public TMP_Text HealthText;
    public Animator HealthFillAnimator;
    
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
    public TMP_Text SpinCounterText_1;
    
    [Header("Misc.")]
    public TMP_Text RoundLabel;
    
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
        FuelSlider.value = perc;
        if (!gameObject.activeSelf) return;
        if (perc == 1f || true) FuelOutlineAnimator.SetTrigger("RefillFuel");
        
        // Temporary?
        GameManager.Instance.Audio2D.PlayClipSFX(AudioPlayer2D.EClipSFX.Plr_PickupFuel);
    }
    
    public void OnFuelSpent(float amnt, float perc, bool spentAsHealth) {
        FuelSlider.value = perc;
        if (spentAsHealth)
            HealthFillAnimator.SetTrigger("HealthAsFuel");
        else
            FuelFillAnimator.SetTrigger("SpendFuel");
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
        setSpinCounterText(newSpinCount);
    }
    
    void onSpinsSpent(int prevSpinCount, int newSpinCount) {
        setSpinCounterText(newSpinCount);
    }
    
    void updateHealthUI(float currH, float maxH) {
        HealthSlider.value = currH / maxH;
        HealthText.text = Mathf.CeilToInt(currH).ToString();
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
        SpinCounterText_1.text = SpinCounterText.text;
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
        HealthSlider.value = 100;
        HealthText.text = "100";
        resetSpinTileColors();
        setSpinCounterText(plr.currentBikeSpins);
        if (gameObject.activeSelf) FuelOutlineAnimator.SetTrigger("Reset");
    }

}