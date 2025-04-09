using TMPro;
using UnityEngine;

public class UIDeathPanel : UIPanel {
    
    [Header("References")]
    public TMP_Text DefeatedWavesValue;
    public TMP_Text RecordDefeatedWavesValue;
    public TMP_Text RecordTimeSpentValue;
    
    
    
    public void OnButton_Retry() {
        GameManager.Instance.SetPauseInputActionsEnabled(true);
        GameManager.Instance.currentSceneRunner.ReloadCurrentScene();
    }
    
    public void OnButton_ReturnMain() {
        GameManager.Instance.SetPauseInputActionsEnabled(true);
        GameManager.Instance.currentSceneRunner.SwitchToScene("MainMenuScene");
    }
    
    public void SetEndScreenInfo(GameData recordData, int wavesCompleted, bool isNewWaveRecord, bool[] recordTimes) {
        DefeatedWavesValue.text = isNewWaveRecord ? $"! {wavesCompleted}" : $"{wavesCompleted}";
        RecordDefeatedWavesValue.text = isNewWaveRecord ? $"! {recordData.HighestWaveCompleted}" : $"{recordData.HighestWaveCompleted}";
    }
    
    // public void SetEndScreenInfo(GameData data, int waveReached, bool isNewWaveRecord, bool isNewTimeRecord) {
    //     DefeatedWavesValue.text = $"{waveReached}";
    //     RecordDefeatedWavesValue.text = isNewWaveRecord ? $"! {data.HighestWaveSurvived}" : $"{data.HighestWaveSurvived}";
    //     // RecordTimeSpentValue.text = isNewTimeRecord ? $"! {textifyTime(data.TimeSpent)}" : $"{textifyTime(data.TimeSpent)}";
    // }
    
    // public void SetEndScreenInfo(float timeSurvived, float highestTimeSurvived, bool isNewTimeRecord, int enemiesKilled) {
    //     // TimeSpentValue.text = textifyTime(timeSurvived);
    //     // WaveReachedValue.text = (isNewTimeRecord ? "! " : "") + textifyTime(highestTimeSurvived);
    //     // EnemiesKilledValue.text = enemiesKilled.ToString();
    // }
    
    string textifyTime(float time) {
        float rounded = Mathf.Round(time);
        int mins = (int)rounded / 60;
        int secs = (int)(rounded - mins * 60f);
        return mins.ToString("00") + ":" + secs.ToString("00");
    }
    
}