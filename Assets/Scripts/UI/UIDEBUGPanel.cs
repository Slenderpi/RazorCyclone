using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDEBUGPanel : UIPanel {
    
    public TMP_Text ShowHideButtonText;
    public GameObject DebugPanelContainer;
    public bool ShowPanel = false;
    
    
    
    void Start() {
#if !UNITY_EDITOR // Automatically destroy debug panel if in build version
        Destroy(gameObject);
#else
        DebugPanelContainer.SetActive(ShowPanel);
        ShowHideButtonText.text = (ShowPanel ? "HIDE" : "SHOW") + "\nDebug Panel";
#endif
    }
    
    public void OnButton_ToggleDebugPanel() {
        ShowPanel = !ShowPanel;
        DebugPanelContainer.SetActive(ShowPanel);
        ShowHideButtonText.text = (ShowPanel ? "HIDE" : "SHOW") + "\nDebug Panel";
    }
    
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