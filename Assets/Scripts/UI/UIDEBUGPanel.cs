using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIDEBUGPanel : UIPanel {
    
    public static UIDEBUGPanel inst;
    
    public bool ShowPanel = false;
    
    [Header("General References")]
    public TMP_Text ShowHideButtonText;
    public GameObject DebugPanelContainer;
    public TMP_Text SurvivalTimerText;
    public GameObject F1Hint;
    
    [Header("References for Buttons")]
    public GameObject TogIndInputOverlay;
    public GameObject TogIndInvincibility;
    public GameObject TogIndInfFuel;
    
    WaveSpawnerManager wsm = null;
    
    
    
    void Start() {
#if !UNITY_EDITOR // Automatically destroy debug panel if in build version
        Destroy(gameObject);
#else
        DebugPanelContainer.SetActive(ShowPanel);
        ShowHideButtonText.text = (ShowPanel ? "HIDE" : "SHOW") + "\nDebug Panel";
        setWSM();
        GameManager.A_PlayerSpawned += (PlayerCharacterCtrlr plr) => {
            setWSM();
        };
        inst = this;
#endif
    }
    
    void LateUpdate() {
        if (wsm) {
            SurvivalTimerText.text = "Time Survived: " + wsm.OwningEndlessMode.TimeSurvived;
        }
    }
    
    public void OnButton_ToggleDebugPanel() {
        ShowPanel = !ShowPanel;
        DebugPanelContainer.SetActive(ShowPanel);
        ShowHideButtonText.text = (ShowPanel ? "HIDE" : "SHOW") + "\nDebug Panel";
    }
    
    public void OnButton_ToggleInputOverlay() {
        UIGamePanel gp = GameManager.Instance.MainCanvas.GamePanel;
        gp.InputOverlay.SetActive(!gp.InputOverlay.activeSelf);
        TogIndInputOverlay.SetActive(gp.InputOverlay.activeSelf);
    }
    
    public void OnButton_ToggleInvincible() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return;
        plr.IsInvincible = !plr.IsInvincible;
        TogIndInvincibility.SetActive(plr.IsInvincible);
    }
    
    public void OnButton_ToggleInfFuel() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return;
        plr.NoFuelCost = !plr.NoFuelCost;
        TogIndInfFuel.SetActive(plr.NoFuelCost);
    }
    
    public void OnButton_SpawnNextWave() {
        if (!wsm) {
            Debug.LogWarning("WARN: There is no wave spawner in the current scene. Cannot spawn a 'next wave'.");
            return;
        }
        if (wsm.CurrentPreloadedWaveNumber != -1) {
            wsm.ActivateWave();
            wsm.PreloadWave(wsm.CurrentPreloadedWaveNumber + 1);
            Debug.Log($"DEBUG: Clicked debug button for force spawning next wave. Next wave ({wsm.CurrentPreloadedWaveNumber}) will spawn at time {wsm.currentWaveSpawnTime}.");
        } else {
            Debug.Log("DEBUG: Clicked debug button for force spawning next wave but there is no preloaded wave left.");
        }
    }
    
    void setWSM() {
        SREndlessMode sre = GameManager.Instance.currentSceneRunner as SREndlessMode;
        wsm = sre ? sre.WaveSpawnManager : null;
        initToggles();
    }
    
    void initToggles() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        TogIndInputOverlay.SetActive(GameManager.Instance.MainCanvas.GamePanel.InputOverlay.activeSelf);
        TogIndInvincibility.SetActive(plr.IsInvincible);
        TogIndInfFuel.SetActive(plr.NoFuelCost);
    }
    
}