using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDeathPanel : UIPanel {
    
    [Header("References")]
    public TMP_Text TimeSurvivedValue;
    public TMP_Text HighestTimeSurvivedValue;
    public TMP_Text EnemiesKilledValue;
    
    
    
    public void OnButton_Retry() {
        GameManager.Instance.SetPauseInputActionsEnabled(true);
        GameManager.Instance.currentSceneRunner.SwitchToScene("EndlessLevel");
    }
    
    public void OnButton_ReturnMain() {
        GameManager.Instance.SetPauseInputActionsEnabled(true);
        GameManager.Instance.currentSceneRunner.SwitchToScene("MainMenuScene");
    }
    
    public void SetEndscreenInfo(float timeSurvived, float highestTimeSurvived, bool isNewTimeRecord, int enemiesKilled) {
        TimeSurvivedValue.text = textifyTime(timeSurvived);
        HighestTimeSurvivedValue.text = (isNewTimeRecord ? "! " : "") + textifyTime(highestTimeSurvived);
        EnemiesKilledValue.text = enemiesKilled.ToString();
    }
    
    string textifyTime(float time) {
        float rounded = Mathf.Round(time);
        int mins = (int)rounded / 60;
        int secs = (int)(rounded - mins * 60f);
        return mins.ToString("00") + ":" + secs.ToString("00");
    }
    
}