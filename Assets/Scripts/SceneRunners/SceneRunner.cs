using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRunner : MonoBehaviour {
    
    public Transform playerSpawnPoint;
    
    void Awake() {
        if (SceneManager.GetSceneByName("CoreScene").IsValid()) {
            startScene();
        } else {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("CoreScene", LoadSceneMode.Additive);
        }
    }
    
    /**
     * This method begins the logic and gameplay loop for the current scene. To change what
     * happens at the beginning, create a new child class of SceneRunner and override this
     * method.
     */
    public virtual void BeginScene() {
        GameManager.Instance.SpawnPlayer();
    }
    
    public void SwitchToScene(string sceneName) {
        Scene curr = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(curr);
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }
    
    void startScene() {
        GameManager.Instance.OnSceneStarted(this);
        GameManager.Instance.MainCanvas.SetCanvasState(UIMainCanvas.ECanvasState.None);
        BeginScene();
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode == LoadSceneMode.Additive && scene.name == "CoreScene") {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            startScene();
        }
    }
    
}