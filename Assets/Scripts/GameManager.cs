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

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance;
    public static PlayerCharacterCtrlr CurrentPlayer;
    public static PlayerInputActions PInputActions;
    
    [Header("Player Settings")]
    float _currentMouseSensitivity;
    public float CurrentMouseSensitivity {
        get { return _currentMouseSensitivity; }
        set {
            _currentMouseSensitivity = value;
            if (CurrentPlayer != null) CurrentPlayer.mouseSensitivity = _currentMouseSensitivity;
            MouseSenseLabel.text = "Mouse Sensitivity: " + _currentMouseSensitivity.ToString("0.00");
        }
    }
    public float DefaultMouseSensitivity = 0.7f;
    public float LowestSensitivity = 0.02f;
    public float HighestSensitivity = 1.2f;
    
    [Header("UI References")]
    public GameObject GamePanel;
    public GameObject PausePanel;
    public Slider FuelSlider;
    public RectTransform MainVacuumCrosshair;
    public RectTransform MainCanonCrosshair;
    public TMP_Text MouseSenseLabel;
    public Slider MouseSenseSlider;
    public Image KeyImageW;
    public Image KeyImageA;
    public Image KeyImageS;
    public Image KeyImageD;
    public TMP_Text TextKeyM1;
    public TMP_Text TextKeyM2;
    public Image KeyImageSpace;
    public Image KeyImageShift;
    
    [Header("Other")]
    [SerializeField]
    Transform spawnPoint;
    [SerializeField]
    PlayerCharacterCtrlr playerPrefab;
    [SerializeField]
    EnemyBase enemyPrefab;
    [SerializeField]
    Transform enemySpawnPoint;
    public Camera rearCamera;
    
    bool hasSpawnedPlayer = false;
    
    public bool gameIsPaused = false;
    
    void Awake() {
        if (!Instance) {
            Instance = this;
            PInputActions = new PlayerInputActions();
        } else {
            Debug.LogWarning("GameManager singleton instantiated more than once!");
        }
        
        PInputActions.Player.Escape.Enable();
        PInputActions.Player.Escape.started += PauseInputPressed;
        
        PausePanel.SetActive(false);
        GamePanel.SetActive(false);
        CurrentMouseSensitivity = DefaultMouseSensitivity;
        MouseSenseSlider.value = (CurrentMouseSensitivity - LowestSensitivity) / (HighestSensitivity - LowestSensitivity);
    }
    
    void Start() {
        
    }
    
    void Update() {
        if (!hasSpawnedPlayer && Time.time > 0.1) {
            hasSpawnedPlayer = true;
            spawnPlayer();
        }
    }
    
    void spawnPlayer() {
        CurrentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<PlayerCharacterCtrlr>();
        GamePanel.SetActive(true);
    }
    
    public void OnEnemyDied() {
        CurrentPlayer.AddFuel(100f);
        //Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
    }
    
    void PauseGame() {
        CurrentPlayer.OnPauseGame();
        PausePanel.SetActive(true);
        GamePanel.SetActive(false);
        SetPauseControlsEnabled(true);
        Time.timeScale = 0;
    }
    
    void ResumeGame() {
        CurrentPlayer.OnResumeGame();
        PausePanel.SetActive(false);
        GamePanel.SetActive(true);
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
        CurrentMouseSensitivity = Mathf.Lerp(LowestSensitivity, HighestSensitivity, MouseSenseSlider.value);
    }
    
    public void OnCloseGameButtonClicked() {
        Application.Quit();
    }
    
}
