using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEditor;
using System;
using Palmmedia.ReportGenerator.Core;
using UnityEngine.Windows;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance;
    public static PlayerCharacterCtrlr CurrentPlayer;
    public static PlayerInputActions PInputActions;
    
    // Events
    public static event Action A_GamePaused;
    public static event Action A_GameResumed;
    public static event Action<PlayerCharacterCtrlr> A_PlayerSpawned;
    
    [Header("UI References")]
    public UIGamePanel GamePanel;
    public UIPausePanel PausePanel;
    
    [Header("Player Settings")]
    float _currentMouseSensitivity;
    public float CurrentMouseSensitivity {
        get { return _currentMouseSensitivity; }
        set {
            _currentMouseSensitivity = value;
            if (CurrentPlayer != null) CurrentPlayer.mouseSensitivity = _currentMouseSensitivity;
            PausePanel.SetMouseSenseText(_currentMouseSensitivity);
        }
    }
    public float DefaultMouseSensitivity = 0.7f;
    public float LowestSensitivity = 0.02f;
    public float HighestSensitivity = 1.2f;
    
    [Header("Other")]
    // [SerializeField]
    Transform spawnPoint;
    [SerializeField]
    PlayerCharacterCtrlr playerPrefab;
    [SerializeField]
    EnemyBase enemyPrefab;
    // [SerializeField]
    Transform enemySpawnPoint;
    public Camera rearCamera;
    
    // bool hasSpawnedPlayer = false;
    
    public bool gameIsPaused = false;
    
    void Awake() {
        if (!Instance) {
            Instance = this;
            PInputActions = new PlayerInputActions();
        } else {
            Debug.LogWarning("GameManager singleton instantiated more than once!");
            return;
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        PInputActions.Player.Escape.Enable();
        PInputActions.Player.Escape.started += PauseInputPressed;
        
        PausePanel.SetActive(false);
        GamePanel.SetActive(false);
        A_GamePaused += PausePanel.OnGamePaused;
        A_GamePaused += GamePanel.OnGamePaused;
        A_GameResumed += PausePanel.OnGameResumed;
        A_GameResumed += GamePanel.OnGameResumed;
        A_PlayerSpawned += PausePanel.OnPlayerSpawned;
        A_PlayerSpawned += GamePanel.OnPlayerSpawned;
        CurrentMouseSensitivity = DefaultMouseSensitivity;
        PausePanel.MouseSenseSlider.value = (CurrentMouseSensitivity - LowestSensitivity) / (HighestSensitivity - LowestSensitivity);
        
        /******  PROGRAMMER SPECIFIC  ******/
        TextAsset programmerPreferenceJson = Resources.Load<TextAsset>("ProgrammerPreferences");
        if (programmerPreferenceJson != null) {
            ProgrammerPreferences _prefs = JsonUtility.FromJson<ProgrammerPreferences>(programmerPreferenceJson.text);
            if (_prefs != null) {
                _prefs.SetPreferences();
                Debug.Log("Note: a 'ProgrammerPreferences' file was found in the Resources folder and will be loaded in.");
            } else Debug.LogWarning("Programmer preferences failed to load. Make sure your json file is written correctly.");
        } else {
            // Debug.Log("Note: no 'ProgrammerPreferences' file found in Resources folder, so no preferences were loaded.");
        }
    }
    
    void Start() {
        
    }
    
    public void OnSceneStarted(SceneStarter ss) {
        spawnPoint = ss.playerSpawnPoint != null ? ss.playerSpawnPoint.transform : null;
        spawnPlayer();
    }
    
    void Update() {
        // if (!hasSpawnedPlayer && Time.time > 0.1) {
        //     hasSpawnedPlayer = true;
        //     spawnPlayer();
        // }
    }
    
    void spawnPlayer() {
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        if (spawnPoint != null) {
            spawnPos = spawnPoint.position;
            spawnRot = spawnPoint.rotation;
        }
        CurrentPlayer = Instantiate(playerPrefab, spawnPos, spawnRot).GetComponent<PlayerCharacterCtrlr>();
        A_PlayerSpawned?.Invoke(CurrentPlayer);
    }
    
    public void OnEnemyDied() {
        CurrentPlayer.AddFuel(100f);
        //Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
    }
    
    void PauseGame() {
        A_GamePaused?.Invoke();
        SetPauseControlsEnabled(true);
        Time.timeScale = 0;
    }
    
    void ResumeGame() {
        A_GameResumed?.Invoke();
        SetPauseControlsEnabled(false);
        Time.timeScale = 1;
    }

    private void PauseInputPressed(InputAction.CallbackContext context) {
        if (Time.timeScale == 0) {
            ResumeGame();
        } else {
            PauseGame();
        }
    }
    
    void SetPauseControlsEnabled(bool newEnabled) {
        if (newEnabled) {
            // PInputActions.PauseMenu.Escape.Enable();
        } else {
            
        }
    }
    
    
    
    /******  Pause menu  ******/
    
    public void OnMouseSenseSliderChanged() {
        CurrentMouseSensitivity = Mathf.Lerp(LowestSensitivity, HighestSensitivity, PausePanel.MouseSenseSlider.value);
    }
    
    public void TestSceneChange() {
        ResumeGame();
        if (SceneManager.GetSceneByName("TestScene").IsValid()) {
            SceneManager.UnloadSceneAsync("TestScene");
            SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Additive);
        } else {
            SceneManager.UnloadSceneAsync("SampleScene");
            SceneManager.LoadScene("TestScene", LoadSceneMode.Additive);
            // SceneManager.SetActiveScene(SceneManager.GetSceneByName());
        }
    }
    
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name != "CoreScene") {
            SceneManager.SetActiveScene(scene);
        }
    }
    
}



/******  PROGRAMMER SPECIFIC  ******/
[Serializable]
class ProgrammerPreferences {
    
    public bool UsePreferences;
    public float MouseSensitivity;

    internal void SetPreferences() {
        if (!UsePreferences) return;
        GameManager.Instance.CurrentMouseSensitivity = MouseSensitivity;
        float highSens = GameManager.Instance.HighestSensitivity;
        float lowSens = GameManager.Instance.LowestSensitivity;
        GameManager.Instance.PausePanel.MouseSenseSlider.value = (GameManager.Instance.CurrentMouseSensitivity - lowSens) / (highSens - lowSens);
    }
}