using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SREndlessMode : SceneRunner, IDataPersistence {
    
    [Header("References")]
    public WaveSpawnerManager WaveSpawnManager;
    
    [HideInInspector]
    public int EnemiesKilled;
    [HideInInspector]
    public int[] SpawnedEnemyCounts = new int[(int)EEnemyType.COUNT];
    
    UIMainCanvas mainCanvas;
    GameData recordData;
    int wavesCompleted = 0;
    // bool currWaveIsActive = false;
    bool playerIsDead = true;
    List<float> TimesSpentEachWave;
    
    // Likely temp
    [HideInInspector]
    public float _SceneStartTime;
    [HideInInspector]
    public float _TimeSinceSceneStarted { get { return Time.time - _SceneStartTime; }}
    
    
    
    public override void BeginScene() {
        GameManagerOLD.Instance.MainCanvas.TutorialPanel.SetActive(false);
        WaveSpawnManager.OwningEndlessMode = this;
        WaveSpawnManager.A_OnWaveActivated += OnWaveActivated;
        WaveSpawnManager.A_OnWaveFinished += OnWaveFinished;
        WaveSpawnManager.InitWaveSpawner();
        mainCanvas = GameManagerOLD.Instance.MainCanvas;
        GameManagerOLD.A_EnemyKilled += () => { EnemiesKilled++; };
        // mainCanvas.GamePanel.SetReadTimerOn(true);
        mainCanvas.GamePanel.RoundLabel.gameObject.SetActive(true);
        TimesSpentEachWave = new List<float>();
#if UNITY_EDITOR || KEEP_DEBUG
        _SceneStartTime = Time.time;
#endif
        playerIsDead = false;
        GameManagerOLD.Instance.MainCanvas.FadeToClear();
        SpawnPlayerAndConnect();
        StartCoroutine(delayedBeginEndless());
    }
    
    IEnumerator delayedBeginEndless() {
        yield return new WaitForSecondsRealtime(UIMainCanvas.FADER_FADE_DURATION);
        WaveSpawnManager.StartWaveSpawner();
    }
    
    protected override void OnPlayerDied() {
        playerIsDead = true;
        GameManagerOLD.Instance.SetPauseInputActionsEnabled(false);
        GameData runData = calcCurrentRunResults();
        mainCanvas.DeathPanel.SetEndScreenInfo(recordData, runData, determineAndSetRecords(runData));
        mainCanvas.SetCanvasState(UIMainCanvas.ECanvasState.DiedEndless);
        DataPersistenceManager.Instance.SaveGame();
    }
    
    GameData calcCurrentRunResults() {
        float gameTimeSpent = 0;
        for (int i = 0; i < wavesCompleted; i++)
            gameTimeSpent += TimesSpentEachWave[i];
        return new(wavesCompleted, gameTimeSpent);
    }
    
    int determineAndSetRecords(GameData runData) {
        int recordEnum = 0;
        if (runData.HighestWaveCompleted > recordData.HighestWaveCompleted) { // New wave record
            recordEnum = 1;
            recordData.HighestWaveCompleted = runData.HighestWaveCompleted;
            recordData.GameTimeSpent = runData.GameTimeSpent;
        } else if (runData.HighestWaveCompleted == recordData.HighestWaveCompleted && runData.GameTimeSpent < recordData.GameTimeSpent) { // Only new time record
            recordEnum = 2;
            recordData.GameTimeSpent = runData.GameTimeSpent;
        }
        return recordEnum;
    }
    
    public void SaveGameNow() {
        determineAndSetRecords(calcCurrentRunResults());
        DataPersistenceManager.Instance.SaveGame();
    }
    
    public void LoadData(GameData data) {
        recordData = new GameData(data);
    }
    
    public void SaveData(GameData data) {
        data.HighestWaveCompleted = recordData.HighestWaveCompleted;
        data.GameTimeSpent = recordData.GameTimeSpent;
        // data.TimeSpentEachWave = recordData.TimeSpentEachWave;
    }
    
    public override void AddEnemyToList(EnemyBase en) {
        base.AddEnemyToList(en);
        int eti = (int)en.etypeid;
        if (eti >= (int)EEnemyType.COUNT) return;
        SpawnedEnemyCounts[eti]++;
    }
    
    public override void RemoveEnemyFromList(EnemyBase en) {
        base.RemoveEnemyFromList(en);
        int eti = (int)en.etypeid;
        if (eti >= (int)EEnemyType.COUNT) return;
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
    
    void OnApplicationQuit() {
        SaveGameNow();
    }
    
}