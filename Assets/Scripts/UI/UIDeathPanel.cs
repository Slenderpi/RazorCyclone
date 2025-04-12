using System.Collections;
using TMPro;
using UnityEngine;

public class UIDeathPanel : UIPanel {
    
    [Header("References")]
    public TMP_Text DefeatedWavesValue;
    public TMP_Text RecordDefeatedWavesValue;
    public TMP_Text RecordTimeSpentValue;
    
    
    
    public void OnButton_Retry() {
        StartCoroutine(delayLoadScene(true));
    }
    
    public void OnButton_ReturnMain() {
        StartCoroutine(delayLoadScene(false));
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
    
    IEnumerator delayLoadScene(bool isRetrying) {
        GameManager.Instance.MainCanvas.FadeToBlack();
        yield return new WaitForSecondsRealtime(UIMainCanvas.FADER_FADE_DURATION + 0.05f);
        if (isRetrying)
            GameManager.Instance.currentSceneRunner.ReloadCurrentScene();
        else {
            GameManager.Instance.MainCanvas.FadeToClear();
            GameManager.Instance.currentSceneRunner.SwitchToScene("MainMenuScene");
        }
    }
    
    string textifyTime(float time) {
        float rounded = Mathf.Round(time);
        int mins = (int)rounded / 60;
        int secs = (int)(rounded - mins * 60f);
        return mins.ToString("00") + ":" + secs.ToString("00");
    }
    
}