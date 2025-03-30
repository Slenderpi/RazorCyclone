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
    [HideInInspector]
    public float HighestTimeSurvived;
    [HideInInspector]
    public int[] SpawnedEnemyCounts = new int[(int)EnemyType.COUNT];
    
    UIMainCanvas mainCanvas;
    
    
    
    public override void BeginScene() {
        WaveSpawnManager.OwningEndlessMode = this;
        WaveSpawnManager.InitWaveSpawner();
        EndlessStartTime = Time.time;
        mainCanvas = GameManager.Instance.MainCanvas;
        GameManager.A_EnemyKilled += () => { EnemiesKilled++; };
        base.BeginScene();
        // mainCanvas.GamePanel.SetReadTimerOn(true);
        mainCanvas.GamePanel.RoundLabel.gameObject.SetActive(true);
    }
    
    protected override void OnPlayerDied() {
        GameManager.Instance.SetPauseInputActionsEnabled(false);
        bool isNewTimeRecord = false;
        if (TimeSurvived > HighestTimeSurvived) {
            isNewTimeRecord = true;
            HighestTimeSurvived = TimeSurvived;
        }
        mainCanvas.DeathPanel.SetEndscreenInfo(TimeSurvived, HighestTimeSurvived, isNewTimeRecord, EnemiesKilled);
        mainCanvas.SetCanvasState(UIMainCanvas.ECanvasState.DiedEndless);
        DataPersistenceManager.Instance.SaveGame();
    }
    
    public void LoadData(GameData data) {
        HighestTimeSurvived = data.HighestTimeSurvived;
    }
    
    public void SaveData(GameData data) {
        data.HighestTimeSurvived = HighestTimeSurvived;
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