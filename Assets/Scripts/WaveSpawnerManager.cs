// UNCOMMENT THE LINE BELOW TO PRINT WAVES AS STRINGS
// #define DEBUG_WAVE_STRS

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = System.Random;

/*
 * ADDING AN ENEMY
 * Add column to wave csv file
 * Add enemy type to GameManager.EnemyType enum
 * Drag/drop reference to respective prefab into WaveSpawnerManager.EnemyPrefabs
 */

public struct WaveEntry {
    public int num;
    public int[] enemyCounts;
    
    public override readonly string ToString() {
        string s = $"WAVE: {num, 2} | {"TYPE", -11} | COUNTS:";
        for (int i = 0; i < enemyCounts.Length; i++) {
            if (enemyCounts[i] == 0) continue;
            s += string.Format("\n{0, 10} {1, -12}: {2, 2}", ">", (EEnemyType)i, enemyCounts[i]);
        }
        return s;
    }
}

public class WaveSpawnerManager : MonoBehaviour {
    
    /// <summary>
    /// int: Activated wave number<br/>
    /// float: Activation time (Time.time)
    /// </summary>
    public event Action<int, float> A_OnWaveActivated;
    /// <summary>
    /// Called when all enemies in the current wave have been killed.<br/>
    /// int: Wave number of finished wave<br/>
    /// float: Time the wave was finished (Time.time)
    /// </summary>
    public event Action<int, float> A_OnWaveFinished;
    
    [Header("Wave Spawner Config")]
    public TextAsset WaveTableFile;
    [Tooltip("List of enemy prefab references.\n\nORDER MATTERS. ASK FOR PRESTON'S APPROVAL BEFORE TOUCHING.")]
    public EnemyBase[] EnemyPrefabs;
    
    [HideInInspector]
    public SREndlessMode OwningEndlessMode;
    [HideInInspector]
    public int CurrentWaveNumber = 0;
    [HideInInspector]
    public int CurrentPreloadedWaveNumber = 0;
    
    [HideInInspector]
    public WaveEntry[] waveEntries;
    [HideInInspector]
    public WaveEntry currPreloadedWave;
    readonly List<Spawner> spawners = new();
    readonly List<EnemyBase> loadedWave = new();
    
    const float MAX_WAIT_TIME_FOR_UNFINISHED_PRELOAD = 3;
    const int PRELOAD_MAX_BATCH_SIZE = 100;
    const int ACTIVATE_MAX_BATCH_SIZE = 10;
    
    bool preloadWaveFinished = true;
    public bool activateWaveFinished = true;
    bool hasDefeatedActiveWave = false;
    bool activatedWaveIsOnlyFodder = false;
    
    
    
    void Awake() {
        spawners.AddRange(FindObjectsOfType<Spawner>());
    }
    
    // void Update() {
    //     // if (OwningEndlessMode.TimeSurvived >= currentWaveSpawnTime && CurrentPreloadedWaveNumber != -1) {
    //     //     ActivateWave();
    //     //     PreloadWave(CurrentWaveNumber + 1);
    //     // }
    //     // TODO
    // }
    
    public void InitWaveSpawner() {
        if (WaveTableFile == null) {
            Debug.LogError("ERROR: No wave table file was set for the wave spawner!");
        }
        
        StringReader stringReader = new(WaveTableFile.text);
        List<string[]> data = new();
        char[] splitStr = ",".ToCharArray();
        
        while (stringReader.Peek() != -1) {
            string line = stringReader.ReadLine();
            string[] items = line.Split(splitStr, StringSplitOptions.None);
            data.Add(items);
        }
        
        int numWaveEntries = data.Count - 1;
        waveEntries = new WaveEntry[numWaveEntries];
        for (int i = 0; i < numWaveEntries; i++) {
            string[] strs = data[i + 1];
            // Entries are in format "count for base, count for hunter, count for laser, ..."
            // Set wave number
            waveEntries[i].num = i + 1; // First wave number is on 1
            // Set enemyCounts
            waveEntries[i].enemyCounts = new int[strs.Length - 1];
            for (int j = 0; j < strs.Length - 1; j++) {
                waveEntries[i].enemyCounts[j] = int.Parse(strs[j + 1]);
            }
        }
        
#if DEBUG_WAVE_STRS && UNITY_EDITOR
        PrintWaveEntries();
#endif
#if UNITY_EDITOR
        CurrentWaveNumber = GameManager.Instance.StartRound - 1;
#endif
        PreloadWave(CurrentWaveNumber + 1);
    }
    
    public void StartWaveSpawner() {
        StartCoroutine(delaySpawnNextWave());
    }
    
    /// <summary>
    /// Preload a wave by choosing the wave number. The first wave is 1, not 0.
    /// </summary>
    /// <param name="waveNumber">The first wave is 1, not 0.</param>
    public void PreloadWave(int waveNumber) {
        if (waveNumber <= 0) {
#if DEBUG_WAVE_STRS && UNITY_EDITOR
            Debug.LogError($"WARN: Attempted to preload wave number ({waveNumber}) which is invalid.");
#endif
            CurrentPreloadedWaveNumber = -1;
            return;
        }
        preloadWaveFinished = false;
        CurrentPreloadedWaveNumber = waveNumber;
        // The loadedWave should already be cleared.
        StartCoroutine(preloadWaveStaggered(waveNumber));
    }
    
    IEnumerator preloadWaveStaggered(int waveNumber) { 
        if (waveNumber > waveEntries.Length) {
            // Generate wave
#if DEBUG_WAVE_STRS && UNITY_EDITOR
        Debug.Log($"DEBUG: Wave number ({waveNumber}) greater than pre-determined number of waves ({waveEntries.Length}), generating new wave.");
#endif
            generateWave(waveNumber);
        } else {
            currPreloadedWave = waveEntries[waveNumber - 1];     
        }
#if DEBUG_WAVE_STRS && UNITY_EDITOR
        Debug.Log("DEBUG: Preloading wave:\n" + currPreloadedWave);
        float startTime = Time.time;
#endif
        int currBatchSize = 0;
        for (int i = 0; i < EnemyPrefabs.Length; i++) {
            int count = currPreloadedWave.enemyCounts[i];
            for (int c = 0; c < count; c++) {
                EnemyBase en = Instantiate(EnemyPrefabs[i]);
                en.gameObject.SetActive(false);
                loadedWave.Add(en);
                if (++currBatchSize >= PRELOAD_MAX_BATCH_SIZE) {
                    currBatchSize = 0;   
#if DEBUG_WAVE_STRS && UNITY_EDITOR
                    Debug.Log($"DEBUG: Preload batch delay t = {Time.time - startTime}");
#endif
                    yield return null;
                }
            }
        }
        preloadWaveFinished = true;
    }
    
    void generateWave(int wnum) {
        currPreloadedWave = new() {
            num = wnum,
            enemyCounts = new int[(int)EEnemyType.COUNT]
        };
        currPreloadedWave.enemyCounts[0] = 3; // Hard code fodder to only be 3
        for (int i = 1; i < currPreloadedWave.enemyCounts.Length; i++) {
            currPreloadedWave.enemyCounts[i] = 1;
        }
    }
    
#if UNITY_EDITOR || KEEP_DEBUG
    IEnumerator delayUnloadWave() {
        float startTime = Time.time;
        do
            yield return null;
        while (!preloadWaveFinished && Time.time - startTime < MAX_WAIT_TIME_FOR_UNFINISHED_PRELOAD);
        if (preloadWaveFinished) {
            Debug.Log("DEBUG: Detected finished preload, allowing unloading of wave.");
            UnloadWave();
        } else
            Debug.LogError($"ERROR: Wave {CurrentPreloadedWaveNumber} has taken longer than {MAX_WAIT_TIME_FOR_UNFINISHED_PRELOAD} seconds to load! Cancelling wave unloader.");
    }
    
    /// <summary>
    /// Destroy currently preloaded enemies.
    /// </summary>
    public void UnloadWave() {
        if (!preloadWaveFinished) {
            StartCoroutine(delayUnloadWave());
            return;
        }
        CurrentPreloadedWaveNumber = -1;
        foreach (EnemyBase en in loadedWave) {
            Destroy(en);
        }
        loadedWave.Clear();
    }
#endif
    
    IEnumerator delayActivateWave() {
        float startTime = Time.time;
        do {
            yield return null;
        } while (!preloadWaveFinished && Time.time - startTime < MAX_WAIT_TIME_FOR_UNFINISHED_PRELOAD);
        if (preloadWaveFinished) {
#if DEBUG_WAVE_STRS && UNITY_EDITOR
            Debug.Log("DEBUG: Detected finished preload, allowing wave activation.");
#endif
            ActivateWave();
        } else
            Debug.LogError($"ERROR: Wave {CurrentPreloadedWaveNumber} has taken longer than {MAX_WAIT_TIME_FOR_UNFINISHED_PRELOAD} seconds to load! Cancelling next wave activation.");
    }
    
    /// <summary>
    /// Activate currently-loaded wave.
    /// </summary>
    public void ActivateWave() {
        if (CurrentPreloadedWaveNumber == -1 || loadedWave.Count == 0)
            return;
        if (!preloadWaveFinished) {
#if DEBUG_WAVE_STRS && UNITY_EDITOR
            Debug.LogWarning("DEBUG: Next wave not yet done preloading! Delaying wave activation.");
#endif
            StartCoroutine(delayActivateWave());
            return;
        }
        activateWaveFinished = false;
        CurrentWaveNumber = CurrentPreloadedWaveNumber;
        A_OnWaveActivated?.Invoke(CurrentWaveNumber, Time.time);
        GameManager.Instance.MainCanvas.GamePanel.OnUpdateRoundNumber(CurrentWaveNumber);
        hasDefeatedActiveWave = false;
#if DEBUG_WAVE_STRS && UNITY_EDITOR
        Debug.Log("DEBUG: Activating wave number: " + CurrentWaveNumber);
#endif
        // Check if the wave to activate only has fodders in it
        activatedWaveIsOnlyFodder = currPreloadedWave.enemyCounts[0] > 0;
        if (activatedWaveIsOnlyFodder)
            for (int ti = 1; ti < (int)EEnemyType.COUNT; ti++)
                if (currPreloadedWave.enemyCounts[ti] > 0) {
                    activatedWaveIsOnlyFodder = false;
                    break;
                }
        /* PSUEDOCODE for creating an array of spawners to choose from when positioning an enemy:
        // Create a "map" where the key is the EnemyType (but parallel to EnemyTypes enum so
        /   just use the int index) and the value is a list of spawners that can spawn
        /   enemies with that type
        List<Spawner>[] availableSpawners = new List<>[EnemyTypes.len]
        foreach spawner:
            ValidateSpawnerSpecificCriteria()
            for i = 0 to EnemyTypes.len:
                if spawner accepts (EnemyType)i:
                    add it to availableSpawners[i]
        // The availableSpawners array will now be populated with lists of spawners that accept
        /   respective EnemyTypes.
        */
        int numETypes = (int)EEnemyType.COUNT;
        Random rnd = new();
        List<Spawner>[] availableSpawners = new List<Spawner>[numETypes];
        for (int i = 0; i < numETypes; i++) availableSpawners[i] = new List<Spawner>(); // Initialize lists
        foreach (Spawner sp in spawners) {
            if (!sp.ValidateSpawnerSpecificCriteria()) continue;
            for (int ti = 0; ti < numETypes; ti++) { // type i
                if (currPreloadedWave.enemyCounts[ti] == 0) continue;
                if (sp.AcceptsEnemy((EEnemyType)ti)) {
                    availableSpawners[ti].Add(sp);
                }
            }
        }
        // Ensure a minimum of 1 randomly chosen spawner for each list
        for (int i = 0; i < numETypes; i++)
            if (availableSpawners[i].Count == 0)
                availableSpawners[i].Add(spawners[rnd.Next(spawners.Count)]);
        
        StartCoroutine(activateWaveStaggered(numETypes, availableSpawners, rnd));
    }
    
    IEnumerator activateWaveStaggered(int numETypes, List<Spawner>[] availableSpawners, Random rnd) {
        /* PSUEDOCODE for positioning and activating each enemy:
        // Now, for each enemy, choose a random valid spawner position and enable them
        // Since PreloadWave() preloads enemies in the order of WaveEntry.EnemyCounts, the order
        /   of enemies in loadedWave is in sync with the order of EnemyStrs. For example,
        /   if WaveEntry.EnemyCounts has 5 EnemyBase and 1 Hunter, then PreloadWave() instantiates
        /   5 EnemyBases first and then 1 hunter, which means loadedWave's first 5 enemies are
        /   EnemyBase. Now, instead of looping through loadedWave, we can loop through EnemyStrs
        /   because the current index i in EnemyStrs will be used to index the correct list of
        /   spawners in AvailableSpawners
        int lei = 0; // i representing the current loadedEnemy to position and activate
        for eti in EnemyTypes.len:
            List<Spawner> currSpawnerList = availableSpawners[eti] // list of spawners valid for this enemyStr
            for ci = 0 to wave.enemyCounts[eti] // There are wave.enemyCounts[eti] duplicates of the current enemy type
                loadedWave[lei].position = currSpawnerList[random from 0 to list length]
                loadedWave[lei].SetEnabled(true)
                lei++
        */
        int currBatchSize = 0;
#if DEBUG_WAVE_STRS && UNITY_EDITOR
        float startTime = Time.time;
#endif
        int lei = 0; // Loaded Enemy i
        for (int eti = 0; eti < numETypes; eti++) { // Enemy Type i
            List<Spawner> currSpList = availableSpawners[eti];
            for (int ci = 0; ci < currPreloadedWave.enemyCounts[eti]; ci++) { // (Enemy) Count i
                EnemyBase en = loadedWave[lei++];
                Transform spawnTrans = currSpList[rnd.Next(currSpList.Count)].transform;
                en.transform.SetPositionAndRotation(spawnTrans.position, spawnTrans.rotation);
                en.gameObject.SetActive(true);
                if (++currBatchSize > ACTIVATE_MAX_BATCH_SIZE) {
#if DEBUG_WAVE_STRS && UNITY_EDITOR
                    Debug.Log($"DEBUG: Activate batch delay t = {Time.time - startTime}");
#endif
                    currBatchSize = 0;
                    yield return null;
                }
            }
        }
        // Clear loadedWave so that if ActivateWave() is quickly called again (it shouldn't be) that we
        // don't perform uneccessary extra work.
        loadedWave.Clear();
        activateWaveFinished = true;
        PreloadWave(CurrentWaveNumber + 1);
    }
    
    public void OnEnemyCountDecreased(int[] counts) {
        if (hasDefeatedActiveWave) return;
        bool waveComplete = true;
        if (activatedWaveIsOnlyFodder) {
            if (counts[0] > 0)
                waveComplete = false;
        } else {
            for (int i = 1; i < (int)EEnemyType.COUNT; i++) // Start at enemy right after cannon fodder
                if (counts[i] > 0) {
                    waveComplete = false;
                    break;
                }
        }
        if (waveComplete) {
            hasDefeatedActiveWave = true;
            A_OnWaveFinished?.Invoke(CurrentWaveNumber, Time.time);
            GameManager.Instance.MainCanvas.GamePanel.OnRoundCompleted();
            GameManager.CurrentPlayer.HealHealth(GameManager.CurrentPlayer.MaxHealth);
            GameManager.CurrentPlayer.AddFuel(100);
            StartCoroutine(delaySpawnNextWave());
        }
    }
    
    IEnumerator delaySpawnNextWave() {
        yield return new WaitForSeconds(2);
        ActivateWave();
    }
    
#if DEBUG_WAVE_STRS && UNITY_EDITOR
    public void PrintWaveEntries() {
        string str = "DEBUG: Printing wave entries:\n";
        foreach (WaveEntry entry in waveEntries) {
            str += entry + "\n";
        }
        Debug.Log(str);
    }
#endif
    
}