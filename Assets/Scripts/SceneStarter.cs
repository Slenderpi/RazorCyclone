using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStarter : MonoBehaviour {
    
    public GameObject playerSpawnPoint;
    
    void Awake() {
        if (SceneManager.GetSceneByName("CoreScene").IsValid()) {
            GameManager.Instance.OnSceneStarted(this);
        } else {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("CoreScene", LoadSceneMode.Additive);
        }
    }
    
    void Start() {
        
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode == LoadSceneMode.Additive && scene.name == "CoreScene") {
            GameManager.Instance.OnSceneStarted(this);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    
}