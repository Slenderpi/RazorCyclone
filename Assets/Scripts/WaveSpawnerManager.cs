// UNCOMMENT THE LINE BELOW TO PRINT WAVES AS STRINGS
#define DEBUG_WAVE_STRS

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.EditorTools;
using UnityEngine;
using Random = System.Random;

public struct WaveEntry {
    public int num;
    public float spawnTime;
    public int[] enemyCounts;
    public int[] typesInWave; // List of indices for GameManager.EnemyStrs[]

    public override readonly string ToString() {
        string s = $"Wave: {num, 2} | time: {spawnTime, 3} | Counts:";
        for (int i = 0; i < enemyCounts.Length; i++) {
            if (enemyCounts[i] == 0) continue;
            s += string.Format("\n{2, 11}{0, -11}: {1, 3}", GameManager.EnemyStrs[i], enemyCounts[i], "> ");
        }
        return s;
    }
}

public class WaveSpawnerManager : MonoBehaviour {
    
    [Header("Wave Spawner Config")]
    public TextAsset WaveTableFile;
    [Tooltip("List of enemy prefab references. ORDER MATTERS. ASK FOR PRESTON'S APPROVAL BEFORE TOUCHING.")]
    public EnemyBase[] EnemyPrefabs; // ENSURE THIS LINES UP WITH "enemyStrs" ARRAY
    
    [HideInInspector]
    public SREndlessMode OwningEndlessMode;
    [HideInInspector]
    public int CurrentWaveNumber = -1;
    [HideInInspector]
    public int CurrentPreloadedWaveNumber = -1;

    readonly List<Spawner> spawners = new();
    WaveEntry[] waveEntries;
    readonly List<EnemyBase> loadedWave = new();
    [HideInInspector]
    public float currentWaveSpawnTime = float.MaxValue;
    
    
    
    void Awake() {
        spawners.AddRange(FindObjectsOfType<Spawner>());
    }
    
    void Update() {
        if (OwningEndlessMode.TimeSurvived >= currentWaveSpawnTime && CurrentPreloadedWaveNumber != -1) {
            ActivateWave();
            PreloadWave(CurrentWaveNumber + 1);
        }
    }
    
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
            // Entries are in format "spawnTime, count for base, count for hunter, count for laser, ..."
            // Set wave number
            waveEntries[i].num = i + 1;
            // Set spawnTime
            waveEntries[i].spawnTime = float.Parse(strs[0]);
            // Set enemyCounts
            waveEntries[i].enemyCounts = new int[strs.Length - 1];
            for (int j = 0; j < strs.Length - 1; j++) {
                waveEntries[i].enemyCounts[j] = int.Parse(strs[j + 1]);
            }
            // NOTE---------------
            // Set typesInWave
            waveEntries[i].typesInWave = new int[strs.Length - 1];
        }
        
#if DEBUG_WAVE_STRS && UNITY_EDITOR
        PrintWaveEntries();
#endif
        // Automatically preload the first wave
        PreloadWave(1);
    }
    
    /// <summary>
    /// Preload a wave by choosing the wave number. The first wave is 1, not 0.
    /// </summary>
    /// <param name="waveNumber">The first wave is 1, not 0.</param>
    public void PreloadWave(int waveNumber) {
        // TODO: See if there's a way to use the Job system for this (and for UnloadWave() as well)
        if (waveNumber > waveEntries.Length || waveNumber <= 0) {
#if DEBUG_WAVE_STRS && UNITY_EDITOR
            Debug.LogWarning($"WARN: Attempted to preload wave number ({waveNumber}) but there are only {waveEntries.Length} determined waves.");
#endif
            CurrentPreloadedWaveNumber = -1;
            currentWaveSpawnTime = float.MaxValue;
            return;
        }
        CurrentPreloadedWaveNumber = waveNumber;
        WaveEntry wave = waveEntries[waveNumber - 1];
#if DEBUG_WAVE_STRS && UNITY_EDITOR
        Debug.Log("DEBUG: Preloading wave:\n" + wave);
#endif
        currentWaveSpawnTime = wave.spawnTime;
        // The loadedWave should already be cleared.
        for (int i = 0; i < EnemyPrefabs.Length; i++) {
            int count = wave.enemyCounts[i];
            for (int c = 0; c < count; c++) {
                EnemyBase en = Instantiate(EnemyPrefabs[i]);
                en.gameObject.SetActive(false);
                loadedWave.Add(en);
            }
        }
    }
    
    /// <summary>
    /// Destroy currently preloaded enemies.
    /// </summary>
    public void UnloadWave() {
        CurrentPreloadedWaveNumber = -1;
        currentWaveSpawnTime = float.MaxValue;
        foreach (EnemyBase en in loadedWave) {
            Destroy(en);
        }
        loadedWave.Clear();
    }
    
    /// <summary>
    /// Activate currently-loaded wave.
    /// </summary>
    public void ActivateWave() {
        if (CurrentPreloadedWaveNumber == -1 || loadedWave.Count == 0)
            return;
        CurrentWaveNumber = CurrentPreloadedWaveNumber;
#if DEBUG_WAVE_STRS && UNITY_EDITOR
        Debug.Log("DEBUG: Activating wave number: " + CurrentWaveNumber);
#endif
        /* PSUEDOCODE for creating an array of spawners to choose from when positioning an enemy:
        // Create a "map" where the key is the EnemyStr (but parallel to GameManager.EnemyStrs so
        /   just use the int index) and the value is a list of spawners that can spawn
        /   enemies with that EnemyStr
        List<Spawner>[] availableSpawners = new List<>[enemyStr.len]
        foreach spawner:
            ValidateSpawnerSpecificCriteria()
            for i = 0 to enemyStr.len:
                if spawner accepts enemyStr[i]:
                    add it to availableSpawners[i]
        // The availableSpawners array will now be populated with lists of spawners that accept
        /   respective EnemyStrs.
        */
        int numEStrs = GameManager.EnemyStrs.Length;
        WaveEntry wave = waveEntries[CurrentPreloadedWaveNumber - 1];
        Random rnd = new();
        List<Spawner>[] availableSpawners = new List<Spawner>[numEStrs];
        for (int i = 0; i < numEStrs; i++) availableSpawners[i] = new List<Spawner>(); // Initialize lists
        foreach (Spawner sp in spawners) {
            sp.ValidateSpawnerSpecificCriteria();
            if (!sp.canSpawn) continue;
            for (int si = 0; si < numEStrs; si++) {
                if (wave.enemyCounts[si] == 0) continue;
                if (sp.AcceptsEnemyStr(GameManager.EnemyStrs[si])) {
                    availableSpawners[si].Add(sp);
                }
            }
        }
        // Ensure a minimum of 1 randomly chosen spawner for each list
        for (int i = 0; i < numEStrs; i++)
            if (availableSpawners[i].Count == 0)
                availableSpawners[i].Add(spawners[rnd.Next(spawners.Count)]);
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
        for esi in enemyStr.len:
            List<Spawner> currSpawnerList = availableSpawners[esi] // list of spawners valid for this enemyStr
            for ci = 0 to wave.enemyCounts[esi] // There are wave.enemyCounts[esi] duplicates of the current enemy type
                loadedWave[lei].position = currSpawnerList[random from 0 to list length]
                loadedWave[lei].SetEnabled(true)
                lei++
        */
        int lei = 0; // Loaded Enemy i
        for (int esi = 0; esi < numEStrs; esi++) { // Enemy String i
            List<Spawner> currSpList = availableSpawners[esi];
            for (int ci = 0; ci < wave.enemyCounts[esi]; ci++) { // (Enemy) Count i
                EnemyBase en = loadedWave[lei++];
                en.transform.position = currSpList[rnd.Next(currSpList.Count)].transform.position;
                en.gameObject.SetActive(true);
            }
        }
        // Clear loadedWave so that if ActivateWave() is quickly called again (it shouldn't be) that we
        // don't perform uneccessary extra work.
        loadedWave.Clear();
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