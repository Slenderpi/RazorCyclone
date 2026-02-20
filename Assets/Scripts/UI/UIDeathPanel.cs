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
    
    /// <summary>
    /// Record enum is an int with the following meanings:<br/>
    /// 0: No new records<br/>
    /// 1: New highest wave<br/>
    /// 2: Same wave, but faster time<br/>
    /// </summary>
    /// <param name="recordData"></param>
    /// <param name="runData"></param>
    /// <param name="recordEnum"></param>
    public void SetEndScreenInfo(GameData recordData, GameData runData, int recordEnum) {
        DefeatedWavesValue.text = $"{runData.HighestWaveCompleted}";
        RecordDefeatedWavesValue.text = recordEnum == 1 ? $"! {recordData.HighestWaveCompleted}" : $"{recordData.HighestWaveCompleted}";
        RecordTimeSpentValue.text = (recordEnum >= 1 ? "! " : "") + textifyTime(runData.GameTimeSpent);
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
        GameManagerOLD.Instance.MainCanvas.FadeToBlack();
        yield return new WaitForSecondsRealtime(UIMainCanvas.FADER_FADE_DURATION + 0.05f);
        if (isRetrying)
            GameManagerOLD.Instance.currentSceneRunner.ReloadCurrentScene();
        else {
            GameManagerOLD.Instance.MainCanvas.FadeToClear();
            GameManagerOLD.Instance.currentSceneRunner.SwitchToScene("MainMenuScene");
        }
    }
    
    string textifyTime(float time) {
        float rounded = Mathf.Round(time);
        int mins = (int)rounded / 60;
        int secs = (int)(rounded - mins * 60f);
        return mins.ToString("00") + ":" + secs.ToString("00");
    }
    
}