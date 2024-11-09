using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SREndlessMode : SceneRunner {
    
    [HideInInspector]
    public float EndlessStartTime;
    [HideInInspector]
    public float TimeSurvived { get { return Time.unscaledTime - EndlessStartTime; }}
    [HideInInspector]
    public int EnemiesKilled;
    
    UIMainCanvas mainCanvas;
    
    
    
    public override void BeginScene() {
        base.BeginScene();
        EndlessStartTime = Time.unscaledTime;
        mainCanvas = GameManager.Instance.MainCanvas;
        // mainCanvas.GamePanel.SetReadTimerOn(true);
        GameManager.A_EnemyKilled += () => { EnemiesKilled++; };
        GameManager.CurrentPlayer.A_PlayerDied += OnPlayerDied;
    }
    
    void OnPlayerDied() {
        GameManager.CurrentPlayer.A_PlayerDied -= OnPlayerDied;
        mainCanvas.DeathPanel.SetEndscreenInfo(TimeSurvived, EnemiesKilled);
        mainCanvas.SetCanvasState(UIMainCanvas.ECanvasState.DiedEndless);
    }

}