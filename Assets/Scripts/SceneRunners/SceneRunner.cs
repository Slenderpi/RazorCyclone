// UNCOMMENT THE LINE BELOW TO SEE THE PATH THE CENTIPEDE WILL ROAM TOWARDS
#define DRAW_CENTIPEDE_PATH

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRunner : MonoBehaviour {
    
    [Header("Consistent References")] // References consistent for all SceneRunners
    public Transform playerSpawnPoint;
    public Lava lava;
    [Tooltip("Center of the map the Centipede will roam around. Height matters.")]
    public Vector3 mapCenter;
    public float maxCentipedeRoamRadius = 50;
    public float minCentipedeRoamRadius = 20;
    public float maxCentipedeRoamHeightRange = 5;
    public float minCentipedeRoamHeightRange = 1.5f;
    [Tooltip("Amount of time it will take the Centipede's roam point to make one full circle.")]
    public float centipedeCircleCompleteTime = 70;
    [Tooltip("Amount of time for the Centipede's roam point to go both up and down once.")]
    public float centipedeHeightCompletionTime = 7f;
    
    // [HideInInspector]
    public List<EnemyBase> EnemiesForRicochet;
    
    LayerMask ricochetLOSMask;
    
    
    
    /// <summary>
    /// This method begins the logic and gameplay loop for the current scene. To change what
    /// happens at the beginning, create a new child class of SceneRunner and override this method.<br/>
    /// <br/>
    /// By default, this method spawns the player.<br/>
    /// <br/>
    /// <example>
    /// In an overridden function, you can call the original function by doing:<code>
    /// override MyCoolFunction(int paramA) {
    ///     // code...
    ///     base.MyCoolFunction(paramA);
    ///     // code...
    /// }</code></example>
    /// </summary>
    public virtual void BeginScene() {
        SpawnPlayerAndConnect();
    }

#if UNITY_EDITOR && DRAW_CENTIPEDE_PATH
    void FixedUpdate() {
        int numPoints = 360;
        float horiT = 0 * 2f * Mathf.PI;
        float vertT = 0 * 2f * centipedeCircleCompleteTime * Mathf.PI / centipedeHeightCompletionTime; 
        Vector3 point = mapCenter;
        point.x += maxCentipedeRoamRadius * Mathf.Cos(horiT);
        point.z += maxCentipedeRoamRadius * Mathf.Sin(horiT);
        point.y += maxCentipedeRoamHeightRange * Mathf.Sin(vertT);
        Vector3 prevP = point;
        for (int i = 1; i <= numPoints; i++) {
            horiT = (float)i / numPoints * 2f * Mathf.PI;
            vertT = (float)i / numPoints * centipedeCircleCompleteTime * 2f * Mathf.PI / centipedeHeightCompletionTime; 
            point = mapCenter;
            point.x += maxCentipedeRoamRadius * Mathf.Cos(horiT);
            point.z += maxCentipedeRoamRadius * Mathf.Sin(horiT);
            point.y += maxCentipedeRoamHeightRange * Mathf.Sin(vertT);
            GameManager.D_DrawPoint(point, Color.magenta);
            Debug.DrawRay(prevP, point - prevP, Color.red);
            prevP = point;
        }
        horiT = 0 * 2f * Mathf.PI;
        vertT = 0 * 2f * centipedeCircleCompleteTime * Mathf.PI / centipedeHeightCompletionTime; 
        point = mapCenter;
        point.x += minCentipedeRoamRadius * Mathf.Cos(horiT);
        point.z += minCentipedeRoamRadius * Mathf.Sin(horiT);
        point.y += maxCentipedeRoamHeightRange * Mathf.Sin(vertT);
        prevP = point;
        for (int i = 1; i <= numPoints; i++) {
            horiT = (float)i / numPoints * 2f * Mathf.PI;
            vertT = (float)i / numPoints * centipedeCircleCompleteTime * 2f * Mathf.PI / centipedeHeightCompletionTime; 
            point = mapCenter;
            point.x += minCentipedeRoamRadius * Mathf.Cos(horiT);
            point.z += minCentipedeRoamRadius * Mathf.Sin(horiT);
            point.y += minCentipedeRoamHeightRange * Mathf.Sin(vertT);
            GameManager.D_DrawPoint(point, Color.magenta);
            Debug.DrawRay(prevP, point - prevP, Color.yellow);
            prevP = point;
        }
    }
#endif

    protected void SpawnPlayerAndConnect() {
        GameManager.A_PlayerSpawned += _onPlayerSpawned;
        GameManager.Instance.SpawnPlayer();
    }
    
    protected virtual void OnPlayerDied() {
        GameManager.Instance.DestroyPlayer();
        StartCoroutine(delayedRespawn());
    }
    
    protected virtual void onSceneAboutToUnload() {
        GameManager.A_PlayerSpawned -= _onPlayerSpawned;
    }
    
    public virtual void AddEnemyToList(EnemyBase en) {
        EnemiesForRicochet.Add(en);
    }
    
    public virtual void RemoveEnemyFromList(EnemyBase en) {
        EnemiesForRicochet.Remove(en);
    }
    
    
    
    void Awake() {
        ricochetLOSMask = 1 << LayerMask.NameToLayer("Default");
        EnemiesForRicochet = new List<EnemyBase>();
        if (SceneManager.GetSceneByName("CoreScene").IsValid()) {
            startScene();
        } else {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("CoreScene", LoadSceneMode.Additive);
        }
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
    
    public EnemyBase GetClosestEnemy(Vector3 pos, EnemyBase ignore) {
        int c = EnemiesForRicochet.Count;
        if (c == 0) return null;
        EnemyBase closestEn = null;
        float closestSqrd = 9999999f;
        for (int i = 0; i < c; i++) {
            EnemyBase en = EnemiesForRicochet[i];
            if (!en.gameObject.activeSelf || !en.ConsiderForRicochet || en == ignore) continue;
            float distSqrd = (pos - en.TransformForRicochetToAimAt.position).sqrMagnitude;
            if (distSqrd < closestSqrd) {
                if (!checkEnLOS(pos, en.TransformForRicochetToAimAt.position, distSqrd))
                    continue;
                closestSqrd = distSqrd;
                closestEn = en;
            }
        }
        return closestEn;
    }
    
    bool checkEnLOS(Vector3 pos, Vector3 enPos, float maxDstSqrd) {
        // If raycasting for walls is a success, then no LOS
        return !Physics.Raycast(
            origin: pos,
            direction: enPos - pos,
            maxDistance: Mathf.Sqrt(maxDstSqrd),
            layerMask: ricochetLOSMask
        );
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