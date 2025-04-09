using UnityEngine;
using System;
using System.IO;

public class FileDataHandler {
        
    string dataDirPath = "";
    string dataFileName = "";
    string extension = ".json";
    string fullPath;
    
    
    
    public FileDataHandler(string _dataDirPath, string _dataFileName) {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        fullPath = Path.Combine(dataDirPath, dataFileName) + extension;
    }
    
    public GameData Load() {
        GameData loadedData = null;
        if (File.Exists(fullPath)) {
            try {
                string dataToLoad = "";
                using (FileStream stream = new(fullPath, FileMode.Open)) {
                    using (StreamReader reader = new(stream)) {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            } catch (Exception e) {
                Debug.LogError("Error occured when trying to load file at path: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }
    
    public void Save(GameData data) {
        try {
            // Create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            
            string dataToStore = JsonUtility.ToJson(data, true);
            using (FileStream stream = new FileStream(fullPath, FileMode.Create)) {
                using (StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(dataToStore);
                }
            }
        } catch (Exception e) {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
    
    public void Delete() {
        try {
            if (File.Exists(fullPath)) {
                Directory.Delete(Path.GetDirectoryName(fullPath), true);
            } else {
                Debug.LogWarning("Tried to delete game data file, but data was not found at path: " + fullPath);
            }
        } catch (Exception e) {
            Debug.LogError("Failed to delete game data file at path: " + fullPath + "\n" + e);
        }
    }
    
}