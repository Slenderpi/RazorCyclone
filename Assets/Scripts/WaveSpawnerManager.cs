using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnerManager : MonoBehaviour {
    
    List<Spawner> spawners = new List<Spawner>();
    
    
    
    void Awake() {
        spawners.AddRange(FindObjectsOfType<Spawner>());
    }
    
    
    
}