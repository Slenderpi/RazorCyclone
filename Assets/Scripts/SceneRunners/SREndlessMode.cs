using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SREndlessMode : SceneRunner, IDataPersistence {
    
    [Header("References")]
    public WaveSpawnerManager WaveSpawnManager;
    
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
    
    // Likely temp
    [HideInInspector]
    public float _SceneStartTime;
    [HideInInspector]
    public float _TimeSinceSceneStarted { get { return Time.time - _SceneStartTime; }}
    
    
    
    public override void BeginScene() {
        GameManager.Instance.MainCanvas.TutorialPanel.SetActive(false);
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
        playerIsDead = false;
        GameManager.Instance.MainCanvas.FadeToClear();
        SpawnPlayer();
        StartCoroutine(delayedBeginEndless());
    }
    
    IEnumerator delayedBeginEndless() {
        yield return new WaitForSecondsRealtime(UIMainCanvas.FADER_FADE_DURATION);
        WaveSpawnManager.StartWaveSpawner();
    }
    
    protected override void OnPlayerDied() {
        playerIsDead = true;
        GameManager.Instance.SetPauseInputActionsEnabled(false);
        
        bool isNewWaveRecord = false;
        // bool[] recordTimes; // Determines if a time is a new record
        if (wavesCompleted > recordData.HighestWaveCompleted) { // Completed more waves than before
            recordData.HighestWaveCompleted = wavesCompleted;
            isNewWaveRecord = true;
        }
        float totalTimeSpent = 0;
        for (int i = 0; i < wavesCompleted; i++)
            totalTimeSpent += TimesSpentEachWave[i];
        mainCanvas.DeathPanel.SetEndScreenInfo(recordData, wavesCompleted, isNewWaveRecord, totalTimeSpent);
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