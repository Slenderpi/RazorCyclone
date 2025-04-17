using System.Collections;
using UnityEngine;

public class TUT_EnemySpawner : MonoBehaviour {
    
    [Header("Config")]
    public TUT_EnemyDefeatedDetector ThingToSpawn;
    public float RespawnDelay = 1;
    
    [Header("References")]
    [SerializeField]
    Transform spawnTransform;
    
    
    
    public void SpawnEnemy() {
        TUT_EnemyDefeatedDetector edd = Instantiate(ThingToSpawn, spawnTransform.position, spawnTransform.rotation);
        edd.SetOwningSpawner(this);
    }
    
    public void DelaySpawnEnemy() {
        StartCoroutine(waitAndSpawn());
    }
    
    IEnumerator waitAndSpawn() {
        yield return new WaitForSeconds(RespawnDelay);
        SpawnEnemy();
    }
    
}