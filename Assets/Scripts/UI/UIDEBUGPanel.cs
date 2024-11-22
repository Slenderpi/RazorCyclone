using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDEBUGPanel : UIPanel {
    
#if !UNITY_EDITOR // Automatically destroy debug panel if in build version
    void Start() {
        Destroy(gameObject);
    }
#endif
    
    public void OnButton_SpawnNextWave() {
        WaveSpawnerManager wsm = FindObjectOfType<WaveSpawnerManager>();
        if (!wsm) {
            Debug.LogWarning("WARN: There is no wave spawner in the current scene. Cannot spawn a 'next wave'.");
            return;
        }
        Debug.Log("DEBUG: Clicked debug button for force spawning next wave.");
        wsm.ActivateWave();
        wsm.PreloadWave(wsm.CurrentPreloadedWaveNumber + 1);
    }
    
}