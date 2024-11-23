// UNCOMMENT THE LINE BELOW TO PRINT WAVES AS STRINGS
#define DEBUG_WAVE_STRS

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.EditorTools;
using UnityEngine;

public struct WaveEntry {
    public int num;
    public float spawnTime;
    public int[] enemyCounts;

    public override readonly string ToString() {
        string s = $"Wave: {num, 2} | time: {spawnTime, 3} | Counts:";
        for (int i = 0; i < enemyCounts.Length; i++) {
            if (enemyCounts[i] == 0) continue;
            s += string.Format("\n{2, 11}{0, -11}: {1, 3}", WaveSpawnerManager.EnemyStrs[i], enemyCounts[i], "> ");
        }
        return s;
    }
}

public class WaveSpawnerManager : MonoBehaviour {
    
    public static readonly string[] EnemyStrs = { // ENSURE THE SPREADSHEET COLUMNS AND THIS ARRAY LINE UP
        "EnemyBase",
        "Hunter",
        "Laser",
        "Lava"
    };
    
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
    
    public void ActivateWave() {
        if (CurrentPreloadedWaveNumber == -1)
            return;
        CurrentWaveNumber = CurrentPreloadedWaveNumber;
#if DEBUG_WAVE_STRS && UNITY_EDITOR
        Debug.Log("DEBUG: Activating wave number: " + CurrentWaveNumber);
#endif
        // TODO: Adjust locations of enemies before activating them
        foreach (EnemyBase en in loadedWave) {
            en.gameObject.SetActive(true);
        }
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