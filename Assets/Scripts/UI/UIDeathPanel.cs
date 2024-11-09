using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDeathPanel : UIPanel {
    
    [Header("References")]
    public TMP_Text TimeSurvivedValue;
    public TMP_Text EnemiesKilledValue;
    
    
    
    public void OnButton_Retry() {
        GameManager.Instance.SetPauseInputActionsEnabled(true);
        GameManager.Instance.currentSceneRunner.SwitchToScene("EndlessLevel");
    }
    
    public void OnButton_ReturnMain() {
        GameManager.Instance.SetPauseInputActionsEnabled(true);
        GameManager.Instance.currentSceneRunner.SwitchToScene("MainMenuScene");
    }
    
    public void SetEndscreenInfo(float timeSurvived, int enemiesKilled) {
        timeSurvived = Mathf.Round(timeSurvived);
        int mins = (int)timeSurvived / 60;
        int secs = (int)(timeSurvived - mins * 60f);
        TimeSurvivedValue.text = mins.ToString("00") + ":" + secs.ToString("00");
        EnemiesKilledValue.text = enemiesKilled.ToString();
    }
    
}