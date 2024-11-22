using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour {
    
    public static DataPersistenceManager Instance { get; private set; }
    
    [Header("Debugging")]
    [SerializeField]
    bool disableDataPersistence = false;
    [SerializeField]
    bool initializeDataIfNull = true;
    
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    GameData gameData;
    List<IDataPersistence> dataPersistenceObjects;
    FileDataHandler dataHandler;
    
    
    
    void Awake() {
        if (Instance != null) {
            Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (disableDataPersistence) {
            Debug.LogWarning("Data Persistence is currently disabled. Loaded GameData will use default values.");
            gameData = new GameData();
        }

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }

    public void NewGame() {
        if (disableDataPersistence) return;
        
        gameData = new GameData();
    }

    public void LoadGame() {
        if (disableDataPersistence) return;
        
        gameData = dataHandler.Load();
        if (gameData == null && initializeDataIfNull) {
            NewGame();
        }
        
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) {
            dataPersistenceObj.LoadData(gameData);
        }
        print("Game loaded!");
    }

    public void SaveGame() {
        if (disableDataPersistence) return;
        
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) {
            dataPersistenceObj.SaveData(gameData);
        }
        
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit() {
        SaveGame();
    }
    
    public bool HasGameData() {
        return gameData != null;
    }
    
    public void OnSceneLoaded() {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }
    
    List<IDataPersistence> FindAllDataPersistenceObjects() {
        // FindObjectsofType takes in an optional boolean to include inactive gameobjects
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
    
}