using UnityEngine;

public class SREndlessMode : SceneRunner, IDataPersistence {
    
    [Header("References")]
    public WaveSpawnerManager WaveSpawnManager;
    
    [HideInInspector]
    public float EndlessStartTime;
    [HideInInspector]
    public float TimeSurvived { get { return Time.time - EndlessStartTime; }}
    [HideInInspector]
    public int EnemiesKilled;
    GameData recordData;
    [HideInInspector]
    public int[] SpawnedEnemyCounts = new int[(int)EnemyType.COUNT];
    
    UIMainCanvas mainCanvas;
    
    
    
    public override void BeginScene() {
        WaveSpawnManager.OwningEndlessMode = this;
        WaveSpawnManager.InitWaveSpawner();
        mainCanvas = GameManager.Instance.MainCanvas;
        GameManager.A_EnemyKilled += () => { EnemiesKilled++; };
        base.BeginScene();
        // mainCanvas.GamePanel.SetReadTimerOn(true);
        mainCanvas.GamePanel.RoundLabel.gameObject.SetActive(true);
#if UNITY_EDITOR
        recordData = new GameData();
#endif
    }
    
    protected override void OnPlayerDied() {
        GameManager.Instance.SetPauseInputActionsEnabled(false);
        int survivedWave = WaveSpawnManager.CurrentWaveNumber;
        bool isNewWaveRecord = false;
        bool isNewTimeRecord = false;
        if (survivedWave > recordData.HighestWaveSurvived) {
            recordData.HighestWaveSurvived = survivedWave;
            recordData.TimeSpent = TimeSurvived;
            isNewWaveRecord = true;
            isNewTimeRecord = true;
        } else if (survivedWave == recordData.HighestWaveSurvived) {
            if (TimeSurvived < recordData.TimeSpent) { // Wave counts better if time spent was shorter
                recordData.TimeSpent = TimeSurvived;
                isNewTimeRecord = true;
            }
        }
        mainCanvas.DeathPanel.SetEndscreenInfo(recordData, survivedWave, isNewWaveRecord, isNewTimeRecord);
        mainCanvas.SetCanvasState(UIMainCanvas.ECanvasState.DiedEndless);
        DataPersistenceManager.Instance.SaveGame();
    }
    
    public void LoadData(GameData data) {
        recordData = new GameData(data);
    }
    
    public void SaveData(GameData data) {
        data.HighestWaveSurvived = recordData.HighestWaveSurvived;
        data.TimeSpent = recordData.TimeSpent;
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

}