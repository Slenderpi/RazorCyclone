using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance;
    public static PlayerCharacterCtrlr CurrentPlayer;
    
    // Events
    public static event Action A_GamePaused;
    public static event Action A_GameResumed;
    public static event Action<PlayerCharacterCtrlr> A_PlayerSpawned;
    public static event Action<PlayerCharacterCtrlr> A_PlayerDestroying;
    public static event Action A_EnemyKilled;
    
    // Current scene runner
    [HideInInspector]
    public SceneRunner currentSceneRunner;
    
    // Pause menu input actions
    PlayerInputActions.PauseMenuActions PauseInputActions;
#if UNITY_EDITOR || KEEP_DEBUG
    PlayerInputActions.DEBUGActions DebugActions;
#endif
    
    [Header("Core References")]
    public UIMainCanvas MainCanvas;
    [HideInInspector]
    public UISettingsPanel SettingsPanel;
    public AudioPlayer2D Audio2D;
    
    float _currentMouseSensitivity;
    public float CurrentMouseSensitivity {
        get { return _currentMouseSensitivity; }
        set {
            _currentMouseSensitivity = value;
            if (CurrentPlayer != null) CurrentPlayer.mouseSensitivity = _currentMouseSensitivity;
            SettingsPanel.SetMouseSenseText(_currentMouseSensitivity);
        }
    }
    int _currentFOV = 90;
    public int CurrentFOV {
        get => _currentFOV;
        set {
            _currentFOV = value;
            onFOVChanged(value);
        }
    }
    [Header("Player Settings")]
    public float DefaultMouseSensitivity = 0.7f;
    public float LowestSensitivity = 0.02f;
    public float HighestSensitivity = 1.2f;
    
    [Header("Other")]
    [SerializeField]
    PlayerCharacterCtrlr playerPrefab;
    [SerializeField]
    EnemyBase enemyPrefab;
    public Camera rearCamera;
    
    [HideInInspector]
    public bool gameIsPaused = false;
    [HideInInspector]
    public float gameTimeScale = 1;
    
    
    
    void Awake() {
        if (Instance != null) {
            Debug.LogError("GameManager singleton instantiated more than once!");
            return;
        }
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        initializeUI();
        
        PauseInputActions = new PlayerInputActions().PauseMenu;
        SetPauseInputActionsEnabled(true);
        
#if UNITY_EDITOR || KEEP_DEBUG
        setupDebugActions();
#endif
    }
    
    void Start() {
#if UNITY_EDITOR
        /******  PROGRAMMER SPECIFIC  ******/
        TextAsset programmerPreferenceJson = Resources.Load<TextAsset>("ProgrammerPreferences");
        if (programmerPreferenceJson != null) {
            ProgrammerPreferences _prefs = JsonUtility.FromJson<ProgrammerPreferences>(programmerPreferenceJson.text);
            if (_prefs != null) {
                _prefs.SetPreferences();
                // Debug.Log("Note: a 'ProgrammerPreferences' file was found in the Resources folder and will be loaded in.");
            } else Debug.LogWarning("Programmer preferences failed to load. Make sure your json file is written correctly.");
        } else {
            // Debug.Log("Note: no 'ProgrammerPreferences' file found in Resources folder, so no preferences were loaded.");
        }
#endif
    }
    
    void initializeUI() {
        SettingsPanel = MainCanvas.SettingsPanel;
        MainCanvas.GamePanel.Init();
        MainCanvas.PausePanel.Init();
        SettingsPanel.Init();
        MainCanvas.MainMenuPanel.Init();
        MainCanvas.Init();
        
        CurrentMouseSensitivity = DefaultMouseSensitivity;
        SettingsPanel.MouseSenseSlider.value = (CurrentMouseSensitivity - LowestSensitivity) / (HighestSensitivity - LowestSensitivity);
    }
    
    public void OnSceneStarted(SceneRunner sr) {
        currentSceneRunner = sr;
        MainCanvas.SetCanvasState(UIMainCanvas.ECanvasState.None);
        DataPersistenceManager.Instance.OnSceneLoaded();
    }
    
    public void SpawnPlayer() {
        CurrentPlayer = Instantiate(
            playerPrefab,
            currentSceneRunner.playerSpawnPoint != null ? currentSceneRunner.playerSpawnPoint.position : Vector3.zero,
            currentSceneRunner.playerSpawnPoint != null ? currentSceneRunner.playerSpawnPoint.rotation : Quaternion.identity
        ).GetComponent<PlayerCharacterCtrlr>();
        CurrentPlayer.A_PlayerDied += onPlayerDied;
        A_PlayerSpawned?.Invoke(CurrentPlayer);
    }
    
    public void DestroyPlayer() {
        A_PlayerDestroying?.Invoke(CurrentPlayer);
        Destroy(CurrentPlayer.gameObject);
        CurrentPlayer = null;
    }
    
    // public void OnEnemyDied(EnemyBase enemy, EDamageType damageType) {
    //     switch (damageType) {
    //     case EDamageType.Projectile:
    //         Audio2D.PlayClipSFX(AudioPlayer2D.EClipSFX.Kill_DirectHit);
    //         break;
    //     }
    //     MainCanvas.GamePanel.OnPlayerDamagedEnemy(enemy);
    // }
    
    public void OnEnemyTookDamage(EnemyBase enemy, EDamageType damageType, bool wasKillingBlow) {
        if (wasKillingBlow) {
            A_EnemyKilled?.Invoke();
            switch (damageType) {
            case EDamageType.Projectile:
                Audio2D.PlayClipSFX(AudioPlayer2D.EClipSFX.Kill_DirectHit);
                break;
            case EDamageType.Vacuum: // TODO: Vacuum kill sfx
                Audio2D.PlayClipSFX(AudioPlayer2D.EClipSFX.Kill_DirectHit);
                break;
            }
        }
        MainCanvas.GamePanel.OnPlayerDamagedEnemy(enemy);
    }
    
    public void PauseGame() {
        gameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        A_GamePaused?.Invoke();
        gameTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }
    
    public void ResumeGame() {
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        A_GameResumed?.Invoke();
        Time.timeScale = gameTimeScale;
    }
    
    public void PauseInputPressed(InputAction.CallbackContext context) {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenuScene")) {
            if (MainCanvas.CurrentCanvasState == UIMainCanvas.ECanvasState.Settings)
                MainCanvas.SetCanvasState(UIMainCanvas.ECanvasState.MainMenu);
        } else {
            if (gameIsPaused) {
                if (MainCanvas.CurrentCanvasState == UIMainCanvas.ECanvasState.Settings)
                    MainCanvas.SetCanvasState(UIMainCanvas.ECanvasState.Paused);
                else
                    ResumeGame();
            } else {
                PauseGame();
            }
        }
    }
    
    
    
    /******  Functions Involving UI or Scenes  ******/
    
    public void OnMouseSenseSliderChanged() {
        CurrentMouseSensitivity = Mathf.Lerp(LowestSensitivity, HighestSensitivity, SettingsPanel.MouseSenseSlider.value);
    }
    
    // public void OnFOVChanged(int newfov) {
    //     Camera.main.fieldOfView = newfov;
    // }
    
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name != "CoreScene") {
            SceneManager.SetActiveScene(scene);
            // Camera.main.fieldOfView = _currentFOV;
        }
    }
    
    public void SetPauseInputActionsEnabled(bool newEnabled) {
        if (newEnabled) {
            PauseInputActions.Escape.Enable();
            PauseInputActions.Escape.started += PauseInputPressed;
        } else {
            PauseInputActions.Escape.Disable();
            PauseInputActions.Escape.started -= PauseInputPressed;
        }
    }
    
    void onPlayerDied() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // SetPauseInputActionsEnabled(false);
    }
    
    void onFOVChanged(int value) {
        Camera.main.fieldOfView = value;
    }
    
    
    
    /******  DEBUGGING  ******/
    
#if UNITY_EDITOR || KEEP_DEBUG
    void setupDebugActions() {
        DebugActions = new PlayerInputActions().DEBUG;
        DebugActions.ToggleMouseLock.Enable();
        DebugActions.ToggleMouseLock.started += (InputAction.CallbackContext context) => {
            if (Cursor.lockState == CursorLockMode.None) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            } else {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // UIDEBUGPanel.inst.F1Hint.SetActive(false);
            }
        };
        DebugActions.KillPlayer.Enable();
        DebugActions.KillPlayer.started += (InputAction.CallbackContext context) => {
            if (CurrentPlayer != null && CurrentPlayer.CurrentHealth > 0) {
                print("Killing player from hotkey.");
                CurrentPlayer.TakeDamage(CurrentPlayer.MaxHealth, EDamageType.Any);
            }
        };
    }
    
    public void SetPreferredTimeScale(float scale) {
        gameTimeScale = scale;
        if (!gameIsPaused) {
            Time.timeScale = gameTimeScale;
        }
    }
    
    public static void D_DrawPoint(Vector3 position, Color c) {
        D_DrawPoint(position, c, Time.fixedDeltaTime);
    }
    
    public static void D_DrawPoint(Vector3 position, Color c, float t) {
        bool b = false;
        float rad = 0.15f;
        Debug.DrawRay(position + Vector3.forward * rad, 2 * rad * Vector3.back, c, t, b);
        Debug.DrawRay(position + Vector3.right * rad, 2 * rad * Vector3.left, c, t, b);
    }
#endif
    
}



/// <summary>
/// The type of damage applied on an enemy.
/// </summary>
public enum EDamageType {
    Any,
    Enemy,
    Projectile,
    Vacuum,
    ProjectileExplosion
}

/// <summary>
/// Enum representing an enemy type.
/// </summary>
public enum EnemyType {
    CanonFodder,
    HunterBasic,
    Hunter,
    CrabBasic,
    Crab,
    Turtle,
    Centipede,
    COUNT, // ALWAYS HAVE THIS BE LAST. Meant to be used for the number of EnemyTypes via (int)EnemyType.COUNT
    // ANY TYPES AFTER COUNT ARE EXTRANEOUS TYPES NOT MEANT TO BE CONSIDERED BY MOST SCRIPTS
    EnemyBase,
    Weakpoint,
    CentipedeMissile
}



#if UNITY_EDITOR
/******  PROGRAMMER SPECIFIC  ******/
[Serializable]
class ProgrammerPreferences {
    
    public bool UsePreferences;
    public float MouseSensitivity;
    public float MasterVolume = 100;
    
    internal void SetPreferences() {
        if (!UsePreferences) return;
        GameManager.Instance.CurrentMouseSensitivity = MouseSensitivity;
        float highSens = GameManager.Instance.HighestSensitivity;
        float lowSens = GameManager.Instance.LowestSensitivity;
        GameManager.Instance.SettingsPanel.MouseSenseSlider.value = (GameManager.Instance.CurrentMouseSensitivity - lowSens) / (highSens - lowSens);
        if (MasterVolume == 1f) Debug.LogWarning(">> Programmer preferences file has MasterVolume set to 1. Did you mean 100? Currently, volume is on a scale from 0 to 100 rather than 0 to 1.");
        else if (MasterVolume < 1f && MasterVolume > 0f) Debug.LogWarning(">> Programmer preferences file has MasterVolume between 0 and 1. Make sure you set the volume to be between 0 and 100--volume is on a scale from 0 to 100 rather than 0 to 1.");
        GameManager.Instance.Audio2D.SetMasterVolume(MasterVolume);
    }
}
#endif