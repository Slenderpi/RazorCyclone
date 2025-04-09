using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SREndlessMode : SceneRunner, IDataPersistence {
    
    [Header("References")]
    public WaveSpawnerManager WaveSpawnManager;
    
#if UNITY_EDITOR || KEEP_DEBUG
    [HideInInspector]
    public float _SceneStartTime;
    [HideInInspector]
    public float _TimeSinceSceneStarted { get { return Time.time - _SceneStartTime; }}
#endif
    [HideInInspector]
    public int EnemiesKilled;
    [HideInInspector]
    public int[] SpawnedEnemyCounts = new int[(int)EnemyType.COUNT];
    
    UIMainCanvas mainCanvas;
    GameData recordData;
    int wavesCompleted = 0;
    // bool currWaveIsActive = false;
    bool playerIsDead = true;
    List<float> TimesSpentEachWave;
    
    
    
    public override void BeginScene() {
        WaveSpawnManager.OwningEndlessMode = this;
        WaveSpawnManager.OnWaveActivated += OnWaveActivated;
        WaveSpawnManager.OnWaveFinished += OnWaveFinished;
        WaveSpawnManager.InitWaveSpawner();
        mainCanvas = GameManager.Instance.MainCanvas;
        GameManager.A_EnemyKilled += () => { EnemiesKilled++; };
        // mainCanvas.GamePanel.SetReadTimerOn(true);
        mainCanvas.GamePanel.RoundLabel.gameObject.SetActive(true);
        TimesSpentEachWave = new List<float>();
#if UNITY_EDITOR || KEEP_DEBUG
        _SceneStartTime = Time.time;
#endif
        base.BeginScene();
        playerIsDead = false;
    }
    
    protected override void OnPlayerDied() {
        playerIsDead = true;
        GameManager.Instance.SetPauseInputActionsEnabled(false);
        
        bool isNewWaveRecord = false;
        bool[] recordTimes; // Determines if a time is a new record
        if (wavesCompleted > recordData.HighestWaveCompleted) { // Completed more waves than before
            recordData.HighestWaveCompleted = wavesCompleted;
            // Update any improved wave times
            isNewWaveRecord = true;
            recordTimes = new bool[wavesCompleted];
            float[] newTimeSpentEachWave = new float[wavesCompleted];
            for (int i = 0; i < wavesCompleted; i++) {
                if (i < recordData.TimeSpentEachWave.Length) {
                    if (TimesSpentEachWave[i] < recordData.TimeSpentEachWave[i]) {
                        recordTimes[i] = true;
                        newTimeSpentEachWave[i] = TimesSpentEachWave[i];
                    } else {
                        newTimeSpentEachWave[i] = recordData.TimeSpentEachWave[i];
                    }
                } else {
                    recordTimes[i] = true;
                    newTimeSpentEachWave[i] = TimesSpentEachWave[i];
                }
            }
            recordData.TimeSpentEachWave = newTimeSpentEachWave.ToArray();
        } else { // Did not complete more waves than before
            // Update any improved wave times
            int prevTimeCount = recordData.TimeSpentEachWave.Length;
            recordTimes = new bool[prevTimeCount];
            for (int i = 0; i < wavesCompleted; i++) {
                if (TimesSpentEachWave[i] < recordData.TimeSpentEachWave[i]) {
                    recordTimes[i] = true;
                    recordData.TimeSpentEachWave[i] = TimesSpentEachWave[i];
                }
            }
        }
        
        // string s = "[";
        // foreach (float f in recordData.TimeSpentEachWave)
        //     s += $"{f:f2}, ";
        // Debug.LogWarning(s[0..^2] + "]");
        // s = "[";
        // foreach (bool b in recordTimes)
        //     s += b ? "!, " : "-, ";
        // Debug.LogWarning(s[..^2] + "]");
        
        mainCanvas.DeathPanel.SetEndScreenInfo(recordData, wavesCompleted, isNewWaveRecord, recordTimes);
        
        // bool isNewWaveRecord = false;
        // bool isNewTimeRecord = false;
        // if (waveCompleted > recordData.HighestWaveSurvived) {
        //     recordData.HighestWaveSurvived = waveCompleted;
        //     // recordData.TimeSpent = TimeSurvived;
        //     isNewWaveRecord = true;
        //     isNewTimeRecord = true;
        // } else if (waveCompleted == recordData.HighestWaveSurvived) {
        //     // if (TimeSurvived < recordData.TimeSpent) { // Wave counts better if time spent was shorter
        //     //     recordData.TimeSpent = TimeSurvived;
        //     //     isNewTimeRecord = true;
        //     // }
        // }
        // mainCanvas.DeathPanel.SetEndScreenInfo(recordData, waveCompleted, isNewWaveRecord, isNewTimeRecord);
        
        mainCanvas.SetCanvasState(UIMainCanvas.ECanvasState.DiedEndless);
        DataPersistenceManager.Instance.SaveGame();
    }
    
    public void LoadData(GameData data) {
        recordData = new GameData(data);
    }
    
    public void SaveData(GameData data) {
        data.HighestWaveCompleted = recordData.HighestWaveCompleted;
        data.TimeSpentEachWave = recordData.TimeSpentEachWave;
    }

    public override void AddEnemyToList(EnemyBase en) {
        base.AddEnemyToList(en);
        int eti = (int)en.etypeid;
        if (eti >= (int)EnemyType.COUNT) return;
        SpawnedEnemyCounts[eti]++;
    }

    public override void RemoveEnemyFromList(EnemyBase en) {
        base.RemoveEnemyFromList(en);
        int eti = (int)en.etypeid;
        if (eti >= (int)EnemyType.COUNT) return;
        SpawnedEnemyCounts[eti]--;
        if (WaveSpawnManager)
            WaveSpawnManager.OnEnemyCountDecreased(SpawnedEnemyCounts);
    }
    
    void OnWaveActivated(int waveNum, float activateTime) {
        if (playerIsDead) return;
        while (TimesSpentEachWave.Count < waveNum) // This is for if the debug button for spawning a specific wave is used
            TimesSpentEachWave.Add(float.MaxValue);
        // Store the activate time into TimeSpentEachWave
        TimesSpentEachWave[waveNum - 1] = activateTime;
    }
    
    void OnWaveFinished(int waveNum, float finishTime) {
        if (playerIsDead) return;
        wavesCompleted = waveNum;
        // Use the stored activation time to calculate the spent duration
        TimesSpentEachWave[waveNum - 1] = Time.time - TimesSpentEachWave[waveNum - 1];
    }

}