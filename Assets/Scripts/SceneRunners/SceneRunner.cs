using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRunner : MonoBehaviour {
    
    [Header("Consistent References")] // References consistent for all SceneRunners
    public Transform playerSpawnPoint;
    public Lava lava;
    
    [HideInInspector]
    public List<EnemyBase> SpawnedEnemies;
    
    
    
    void Awake() {
        SpawnedEnemies = new List<EnemyBase>();
        if (SceneManager.GetSceneByName("CoreScene").IsValid()) {
            startScene();
        } else {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("CoreScene", LoadSceneMode.Additive);
        }
    }

    /// <summary>
    /// This method begins the logic and gameplay loop for the current scene. To change what
    /// happens at the beginning, create a new child class of SceneRunner and override this method.
    /// By default, this method spawns the player.
    /// </summary>
    public virtual void BeginScene() {
        GameManager.A_PlayerSpawned += _onPlayerSpawned;
        GameManager.Instance.SpawnPlayer();
    }
    
    protected virtual void OnPlayerDied() {
        // GameManager.Instance.SpawnPlayer();
        GameManager.Instance.DestroyPlayer();
        StartCoroutine(delayedRespawn());
    }
    
    public void SwitchToScene(string sceneName) {
        onSceneAboutToUnload();
        Scene curr = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(curr);
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }
    
    public void ReloadCurrentScene() {
        SwitchToScene(SceneManager.GetActiveScene().name);
    }
    
    public void AddEnemyToList(EnemyBase en) {
        SpawnedEnemies.Add(en);
    }
    
    public void RemoveEnemyFromList(EnemyBase en) {
        SpawnedEnemies.Remove(en);
    }
    
    public EnemyBase GetClosestEnemy(Vector3 pos, EnemyBase ignore) {
        int c = SpawnedEnemies.Count;
        if (c == 0) return null;
        EnemyBase closestEn = null;
        float closestSqrd = 9999999f;
        for (int i = 0; i < c; i++) {
            EnemyBase en = SpawnedEnemies[i];
            if (!en.gameObject.activeSelf || !en.ConsiderForRicochet || en == ignore) continue;
            float distSqrd = (pos - en.TransformForRicochetToAimAt.position).sqrMagnitude;
            if (distSqrd < closestSqrd) {
                closestSqrd = distSqrd;
                closestEn = en;
            }
        }
        return closestEn;
    }
    
    void startScene() {
        if (GameManager.Instance == null) {
            Debug.LogError("!! SceneRunner awoke before GameManager !!  --  " + 
                           "Did you run the game with the CoreScene already open? " +
                           "If so, double click the CoreScene in the hierarchy to " +
                           "make it the active scene. Its name should become bolded.");
            return;
        }
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
    
    protected virtual void onSceneAboutToUnload() {
        GameManager.A_PlayerSpawned -= _onPlayerSpawned;
    }
    
    void _onPlayerSpawned(PlayerCharacterCtrlr plr) {
        plr.A_PlayerDied += _onPlayerDied;
    }
    
    void _onPlayerDied() {
        GameManager.CurrentPlayer.A_PlayerDied -= _onPlayerDied;
        OnPlayerDied();
    }
    
    IEnumerator delayedRespawn() {
        yield return new WaitForSecondsRealtime(2);
        GameManager.Instance.SetPauseInputActionsEnabled(true);
        GameManager.Instance.SpawnPlayer();
    }
    
}