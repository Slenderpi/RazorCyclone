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
    
    // Current scene runner
    [HideInInspector]
    public SceneRunner currentSceneRunner;
    
    // Pause menu input actions
    PlayerInputActions.PauseMenuActions PauseInputActions;
    
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
    [Header("Player Settings")]
    public float DefaultMouseSensitivity = 0.7f;
    public float LowestSensitivity = 0.02f;
    public float HighestSensitivity = 1.2f;
    
    [Header("Other")]
    [SerializeField]
    PlayerCharacterCtrlr playerPrefab;
    [SerializeField]
    EnemyBase enemyPrefab;
    // [SerializeField]
    // Transform enemySpawnPoint;
    public Camera rearCamera;
    
    [HideInInspector]
    public bool gameIsPaused = false;
    
    void Awake() {
        if (Instance != null) {
            Debug.LogError("GameManager singleton instantiated more than once!");
            return;
        }
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        initializeUI();
        
        PauseInputActions = new PlayerInputActions().PauseMenu;
        PauseInputActions.Escape.Enable();
        PauseInputActions.Escape.started += PauseInputPressed;
        
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

    void Start() {
        
    }
    
    public void OnSceneStarted(SceneRunner sr) {
        currentSceneRunner = sr;
        MainCanvas.SetCanvasState(UIMainCanvas.ECanvasState.None);
    }
    
    void Update() {
        
    }
    
    public void SpawnPlayer() {
        CurrentPlayer = Instantiate(
            playerPrefab,
            currentSceneRunner.playerSpawnPoint != null ? currentSceneRunner.playerSpawnPoint.position : Vector3.zero,
            currentSceneRunner.playerSpawnPoint != null ? currentSceneRunner.playerSpawnPoint.rotation : Quaternion.identity
        ).GetComponent<PlayerCharacterCtrlr>();
        A_PlayerSpawned?.Invoke(CurrentPlayer);
    }
    
    public void DestroyPlayer() {
        A_PlayerDestroying?.Invoke(CurrentPlayer);
        Destroy(CurrentPlayer.gameObject);
        CurrentPlayer = null;
    }
    
    public void OnEnemyDied(EnemyBase enemy, EDamageType damageType) {
        switch (damageType) {
        case EDamageType.Projectile:
            Audio2D.PlayClipSFX(AudioPlayer2D.EClipSFX.Kill_DirectHit);
            break;
        }
    }
    
    public void OnEnemyTookDamage(EnemyBase enemy, EDamageType damageType) {
        
    }
    
    public void PauseGame() {
        gameIsPaused = true;
        A_GamePaused?.Invoke();
        Time.timeScale = 0;
    }
    
    public void ResumeGame() {
        gameIsPaused = false;
        A_GameResumed?.Invoke();
        Time.timeScale = 1;
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
        if (scene.name != "CoreScene") {
            SceneManager.SetActiveScene(scene);
        }
    }
    
}



/// <summary>
/// The type of damage applied on an enemy.
/// </summary>
public enum EDamageType {
    Projectile,
    Vacuum,
    ProjectileExplosion
}



#if UNITY_EDITOR
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
        GameManager.Instance.SettingsPanel.MouseSenseSlider.value = (GameManager.Instance.CurrentMouseSensitivity - lowSens) / (highSens - lowSens);
    }
}
#endif