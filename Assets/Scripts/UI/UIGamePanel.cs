
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
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
    public Animator HitmarkerAnimator;
    
    [Header("Misc.")]
    public TMP_Text Speedometer;
    
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
    
    
    
    public void SetSpeedText(float speed) {
        Speedometer.text = string.Format("{0:0.0}", speed) + "";
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
        HitmarkerAnimator.SetTrigger("Show");
    }
    
    // public void SetReadTimerOn(bool setReadOn) {
    //     if (setReadOn) {
            
    //     } else {
            
    //     }
    // }

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
        ResetUIElements();
    }

    public override void OnPlayerDestroying(PlayerCharacterCtrlr plr) {
        plr.A_FuelAdded -= OnFuelAdded;
        plr.A_FuelSpent -= OnFuelSpent;
        plr.A_PlayerTakenDamage -= OnDamageTaken;
        plr.A_PlayerHealed -= OnPlayerHealed;
    }
    
    public void ResetUIElements() {
        OnFuelAdded(0, 1);
        OnTurnInputChanged(Vector2.zero);
        OnVertInputChanged(0);
        OnFireVacuum(false);
        OnFireCanon(false);
        HealthSlider.value = 100;
        HealthText.text = "100";
        if (gameObject.activeSelf) FuelOutlineAnimator.SetTrigger("Reset");
    }

}