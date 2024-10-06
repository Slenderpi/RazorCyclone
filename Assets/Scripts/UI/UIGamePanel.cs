
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePanel : UIPanel {
    
    public Slider FuelSlider;
    public RectTransform MainVacuumCrosshair;
    public RectTransform MainCanonCrosshair;
    public TMP_Text Speedometer;
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

    internal void UpdateCrosshairPositions(Vector3 screenPointVacuum, Vector3 screenPointCanon) {
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
    
    public void OnFuelChanged(float changeAmnt, float perc) {
        FuelSlider.value = perc;
    }

    public override void OnGameResumed() {
        // SetActive(true);
    }

    public override void OnGamePaused() {
        // SetActive(false);
    }
    
    public override void OnPlayerSpawned(PlayerCharacterCtrlr plr) {
        plr.A_FuelChanged += OnFuelChanged;
        ResetUIElements();
        // SetActive(true);
    }

    public override void OnPlayerDestroying(PlayerCharacterCtrlr plr) {
        plr.A_FuelChanged -= OnFuelChanged;
        // SetActive(false);
    }
    
    public void ResetUIElements() {
        OnFuelChanged(0, 1);
        OnTurnInputChanged(Vector2.zero);
        OnVertInputChanged(0);
        OnFireVacuum(false);
        OnFireCanon(false);
    }

}