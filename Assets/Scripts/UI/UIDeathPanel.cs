using TMPro;
using UnityEngine;

public class UIDeathPanel : UIPanel {
    
    [Header("References")]
    public TMP_Text WaveReachedValue;
    public TMP_Text RecordWaveReachedValue;
    public TMP_Text RecordTimeSpentValue;
    
    
    
    public void OnButton_Retry() {
        GameManager.Instance.SetPauseInputActionsEnabled(true);
        GameManager.Instance.currentSceneRunner.ReloadCurrentScene();
    }
    
    public void OnButton_ReturnMain() {
        GameManager.Instance.SetPauseInputActionsEnabled(true);
        GameManager.Instance.currentSceneRunner.SwitchToScene("MainMenuScene");
    }
    
    public void SetEndscreenInfo(GameData data, int waveReached, bool isNewWaveRecord, bool isNewTimeRecord) {
        WaveReachedValue.text = $"{waveReached}";
        RecordWaveReachedValue.text = isNewWaveRecord ? $"! {data.HighestWaveSurvived}" : $"{data.HighestWaveSurvived}";
        RecordTimeSpentValue.text = isNewTimeRecord ? $"! {textifyTime(data.TimeSpent)}" : $"{textifyTime(data.TimeSpent)}";
    }
    
    public void SetEndscreenInfo(float timeSurvived, float highestTimeSurvived, bool isNewTimeRecord, int enemiesKilled) {
        // TimeSpentValue.text = textifyTime(timeSurvived);
        // WaveReachedValue.text = (isNewTimeRecord ? "! " : "") + textifyTime(highestTimeSurvived);
        // EnemiesKilledValue.text = enemiesKilled.ToString();
    }
    
    string textifyTime(float time) {
        float rounded = Mathf.Round(time);
        int mins = (int)rounded / 60;
        int secs = (int)(rounded - mins * 60f);
        return mins.ToString("00") + ":" + secs.ToString("00");
    }
    
}