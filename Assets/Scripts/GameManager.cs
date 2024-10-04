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
    SceneRunner currentSceneRunner;
    
    // Pause menu input actions
    PlayerInputActions.PauseMenuActions PauseInputActions;
    
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
        
        PauseInputActions = new PlayerInputActions().PauseMenu;
        PauseInputActions.Escape.Enable();
        PauseInputActions.Escape.started += PauseInputPressed;
        PausePanel.SetActive(false);
        GamePanel.SetActive(false);
        A_GamePaused += GamePanel.OnGamePaused;
        A_GamePaused += PausePanel.OnGamePaused;
        A_GameResumed += GamePanel.OnGameResumed;
        A_GameResumed += PausePanel.OnGameResumed;
        A_PlayerSpawned += GamePanel.OnPlayerSpawned;
        A_PlayerSpawned += PausePanel.OnPlayerSpawned;
        A_PlayerDestroying += GamePanel.OnPlayerDestroying;
        A_PlayerDestroying += PausePanel.OnPlayerDestroying;
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
    
    public void OnSceneStarted(SceneRunner sr) {
        currentSceneRunner = sr;
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
    
    public void OnEnemyDied() {
        CurrentPlayer.AddFuel(100f);
        //Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
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
        if (gameIsPaused)
            ResumeGame();
        else
            PauseGame();
    }
    
    
    
    /******  Pause menu  ******/
    
    public void OnMouseSenseSliderChanged() {
        CurrentMouseSensitivity = Mathf.Lerp(LowestSensitivity, HighestSensitivity, PausePanel.MouseSenseSlider.value);
    }
    
    public void TestSceneChange() {
        ResumeGame();
        string sceneToToggleTo = "TestScene";
        if (SceneManager.GetSceneByName(sceneToToggleTo).IsValid()) {
            SceneManager.UnloadSceneAsync(sceneToToggleTo);
            SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Additive);
        } else {
            SceneManager.UnloadSceneAsync("SampleScene");
            SceneManager.LoadScene(sceneToToggleTo, LoadSceneMode.Additive);
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