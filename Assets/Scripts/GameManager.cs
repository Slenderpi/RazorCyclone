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
    bool _plrInv = false;
    public bool plrInvincible {
        get { return _plrInv; }
        set {
            _plrInv = value;
            MainCanvas.DebugPanel.TogIndInvincibility.SetActive(_plrInv);
            if (CurrentPlayer) CurrentPlayer.IsInvincible = _plrInv;
        }
    }
    bool _plrNFC = false;
    public bool plrNoFuelCost {
        get { return _plrNFC; }
        set {
            _plrNFC = value;
            MainCanvas.DebugPanel.TogIndInfFuel.SetActive(_plrNFC);
            if (CurrentPlayer) CurrentPlayer.NoFuelCost = _plrNFC;
        }
    }
#endif
#if UNITY_EDITOR
    [HideInInspector]
    public int StartRound = 1;
    ProgrammerPreferences _prefs;
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
            DataPersistenceManager.Instance.usettings.MouseSensitivity = _currentMouseSensitivity;
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
    public float LowestSensitivity = 0.02f;
    public float HighestSensitivity = 1.2f;
    
    [Header("Other")]
    [SerializeField]
    PlayerCharacterCtrlr playerPrefab;
    [SerializeField]
    EnemyBase enemyPrefab;
    public Camera rearCamera;
    
    [HideInInspector]
    public GameCamera GCam = null;
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
#if UNITY_EDITOR
        /******  PROGRAMMER SPECIFIC  ******/
        TextAsset programmerPreferenceJson = Resources.Load<TextAsset>("ProgrammerPreferences");
        if (programmerPreferenceJson != null) {
            _prefs = JsonUtility.FromJson<ProgrammerPreferences>(programmerPreferenceJson.text);
            if (_prefs != null) {
                _prefs.SetPreferencesAwake();
                // Debug.Log("Note: a 'ProgrammerPreferences' file was found in the Resources folder and will be loaded in.");
            } else Debug.LogWarning("Programmer preferences failed to load. Make sure your json file is written correctly.");
        } else {
            // Debug.Log("Note: no 'ProgrammerPreferences' file found in Resources folder, so no preferences were loaded.");
        }
#endif
    }
    
    void Start() {
#if UNITY_EDITOR
        if (_prefs != null && _prefs.UsePreferences) {
            _prefs.SetPreferencesStart();
        } else {
            DataPersistenceManager.Instance.LoadSettings();
        }
#else
        DataPersistenceManager.Instance.LoadSettings();
#endif
        Audio2D.asMusic.Play();
    }
    
    void initializeUI() {
        SettingsPanel = MainCanvas.SettingsPanel;
        MainCanvas.GamePanel.Init();
        MainCanvas.PausePanel.Init();
        SettingsPanel.Init();
        MainCanvas.MainMenuPanel.Init();
        MainCanvas.Init();
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
        );
#if UNITY_EDITOR || KEEP_DEBUG
        CurrentPlayer.IsInvincible = plrInvincible;
        CurrentPlayer.NoFuelCost = plrNoFuelCost;
        MainCanvas.DebugPanel.TogIndInvincibility.SetActive(plrInvincible);
        MainCanvas.DebugPanel.TogIndInfFuel.SetActive(plrNoFuelCost);
#endif
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
                MainCanvas.GamePanel.OnPlayerKilledEnemy(enemy, true);
                Audio2D.PlayClipSFX(AudioPlayer2D.EClipSFX.Cannon_Kill);
                break;
            case EDamageType.Vacuum:
                MainCanvas.GamePanel.OnPlayerKilledEnemy(enemy, false);
                Audio2D.PlayClipSFX(AudioPlayer2D.EClipSFX.Vacuum_Kill);
                break;
            }
        } else {
            switch (damageType) {
            case EDamageType.Projectile:
                MainCanvas.GamePanel.OnPlayerDamagedEnemy(enemy);
                Audio2D.PlayClipSFX(AudioPlayer2D.EClipSFX.Cannon_Hit);
                break;
            }
        }
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
    
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        GCam = FindObjectOfType<GameCamera>();
        GCam?.SetFOV(CurrentFOV);
        if (scene.name != "CoreScene") {
            SceneManager.SetActiveScene(scene);
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
        GCam?.SetFOV(value);
        DataPersistenceManager.Instance.usettings.FOV = value;
    }
    
    
    
    /******  DEBUGGING  ******/
    
    public void SetPreferredTimeScale(float scale) {
        gameTimeScale = scale;
        if (!gameIsPaused) {
            Time.timeScale = gameTimeScale;
        }
    }
    
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
    CannonFodder,
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
    
    public bool UsePreferences; // If false, all these preferences will be ignored
    public float MouseSensitivity; // The sensitivity that you, the developer, prefers
    public float MasterVolume = 100; // Value between 0 and 100
    public bool EnableMusic = true; // If false, music volume will be set to 0
    public bool PlayerInvincible = false; // If true, the player will spawn with invincibility on
    public bool PlayerNoFuelCost = false; // If true, the player will spawn with no fuel cost
    public int StartRound = 1;
        
    internal void SetPreferencesAwake() {
        if (!UsePreferences) return;
        GameManager.Instance.plrInvincible = PlayerInvincible;
        GameManager.Instance.plrNoFuelCost = PlayerNoFuelCost;
        GameManager.Instance.StartRound = StartRound;
    }
    
    internal void SetPreferencesStart() {
        if (!UsePreferences) return;
        float highSens = GameManager.Instance.HighestSensitivity;
        float lowSens = GameManager.Instance.LowestSensitivity;
        GameManager.Instance.CurrentMouseSensitivity = Mathf.Clamp(MouseSensitivity, lowSens, highSens);
        GameManager.Instance.SettingsPanel.MouseSenseSlider.value = (GameManager.Instance.CurrentMouseSensitivity - lowSens) / (highSens - lowSens);
        if (MasterVolume == 1f) Debug.LogWarning(">> Programmer preferences file has MasterVolume set to 1. Did you mean 100? Currently, volume is on a scale from 0 to 100 rather than 0 to 1.");
        else if (MasterVolume < 1f && MasterVolume > 0f) Debug.LogWarning(">> Programmer preferences file has MasterVolume between 0 and 1. Make sure you set the volume to be between 0 and 100--volume is on a scale from 0 to 100 rather than 0 to 1.");
        GameManager.Instance.Audio2D.SetMasterVolume(MasterVolume);
        GameManager.Instance.Audio2D.SetMusicVolume(EnableMusic ? 100 : 0);
    }
    
}
#endif