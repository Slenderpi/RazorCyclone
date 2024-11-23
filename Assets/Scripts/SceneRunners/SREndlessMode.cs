using System.Collections;
using System.Collections.Generic;
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
    
    UIMainCanvas mainCanvas;
    
    
    
    public override void BeginScene() {
        WaveSpawnManager.OwningEndlessMode = this;
        WaveSpawnManager.InitWaveSpawner();
        EndlessStartTime = Time.time;
        mainCanvas = GameManager.Instance.MainCanvas;
        base.BeginScene();
        // mainCanvas.GamePanel.SetReadTimerOn(true);
        GameManager.A_EnemyKilled += () => { EnemiesKilled++; };
        GameManager.CurrentPlayer.A_PlayerDied += OnPlayerDied;
    }
    
    void OnPlayerDied() {
        GameManager.CurrentPlayer.A_PlayerDied -= OnPlayerDied;
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
    
}