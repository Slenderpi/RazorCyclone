using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DataPersistenceManager : MonoBehaviour {
    
    public static DataPersistenceManager Instance { get; private set; }
    
    [Header("Debugging")]
    [SerializeField]
    bool disableGameData = true;
    [SerializeField]
    bool disableSaveSettings = true;
    [SerializeField]
    bool initializeDataIfNull = true;
    
    [Header("File Storage Config")]
    [SerializeField]
    string gameDataFileName = "rcGameData";
    [SerializeField]
    string userSettingsFileName = "rcUserSettings";
    
    
    GameData gameData;
    [HideInInspector]
    public UserSettings usettings;
    List<IDataPersistence> dataPersistenceObjects;
    FileDataHandler<GameData> gameDataHandler;
    FileDataHandler<UserSettings> usettingsHandler;
    
    
    
    void Awake() {
#if !UNITY_EDITOR
        disableGameData = false; // Force data if in build
        disableSaveSettings = false; // Force settings if in build
#endif
        if (Instance != null) {
            Debug.LogError("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        gameDataHandler = new FileDataHandler<GameData>(Application.persistentDataPath, gameDataFileName);
        usettingsHandler = new FileDataHandler<UserSettings>(Application.persistentDataPath, userSettingsFileName);
    }
    
    public void NewGame() {
        gameData = new();
    }
    
    public void LoadGameData() {
        gameData = gameDataHandler.Load();
        if (gameData == null && initializeDataIfNull) {
            NewGame();
        }
        
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) {
            dataPersistenceObj.LoadData(gameData);
        }
    }
    
    public void SaveGame() {
        if (disableGameData) return;
        
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) {
            dataPersistenceObj.SaveData(gameData);
        }
        
        gameDataHandler.Save(gameData);
    }
    
    public void LoadSettings() {
        usettings = usettingsHandler.Load();
        usettings ??= new(); // The ??= operator here means: IF usettings == null THEN usettings = new() END
        
        loadControlSettings();
        loadVideoSettings();
        loadAudioSettings();
    }
    
    public void SaveSettings() {
        if (disableSaveSettings) return;
        usettingsHandler.Save(usettings);
    }
    
    private void OnApplicationQuit() {
        SaveGame();
    }
    
    public bool HasGameData() {
        return gameData != null;
    }
    
    public void OnSceneLoaded() {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGameData();
    }
    
    List<IDataPersistence> FindAllDataPersistenceObjects() {
        // FindObjectsofType takes in an optional boolean to include inactive gameobjects
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>();
        
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
    
    void loadControlSettings() {
        float highSens = GameManager.Instance.HighestSensitivity;
        float lowSens = GameManager.Instance.LowestSensitivity;
        usettings.MouseSensitivity = Mathf.Clamp(usettings.MouseSensitivity, lowSens, highSens);
        GameManager.Instance.CurrentMouseSensitivity = usettings.MouseSensitivity;
        GameManager.Instance.SettingsPanel.MouseSenseSlider.value = (GameManager.Instance.CurrentMouseSensitivity - lowSens) / (highSens - lowSens);
    }
    
    void loadVideoSettings() {
        GameManager.Instance.CurrentFOV = Math.Clamp(usettings.FOV, GameCamera.MIN_FOV, GameCamera.MAX_FOV);
    }
    
    void loadAudioSettings() {
        AudioPlayer2D aud = GameManager.Instance.Audio2D;
        aud.SetMasterVolume(usettings.MasterVolume);
        aud.SetSFXVolume(usettings.SoundVolume);
        aud.SetMusicVolume(usettings.MusicVolume);
    }
    
}