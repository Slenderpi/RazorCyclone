using System.Collections;
using UnityEngine;

public class TUT_FuelSpawner : MonoBehaviour {
    
    [Header("Config")]
    [SerializeField]
    TUT_FuelDestroyedDetector fuelPrefab;
    public float RespawnDelay = 3;
    
    [Header("References")]
    [SerializeField]
    Transform spawnTransform;
    
    
    
    void Start() {
        SpawnFuel();
    }
    
    public void SpawnFuel() {
        TUT_FuelDestroyedDetector thing = Instantiate(fuelPrefab, spawnTransform.position, spawnTransform.rotation);
        thing.owningSpawner = this;
    }
    
    public void DelaySpawnFuel() {
        StartCoroutine(waitAndSpawn());
    }
    
    IEnumerator waitAndSpawn() {
        yield return new WaitForSeconds(RespawnDelay);
        SpawnFuel();
    }
    
}