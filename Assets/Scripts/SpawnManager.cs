using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public float SpawnDelay = 3f;
    public int enemiesPerSpawn = 3;
    public int maxEnemies = 10;
    public GameObject enemyPrefab;

    // private List<Spawner> spawners = new List<Spawner>();
    // private int currentEnemyCount = 0;
    // float lastSpawnTime = 0f;
    
    // void Awake() {
    //     spawners.AddRange(FindObjectsOfType<Spawner>());
    // }

    // void Start() {
    //     lastSpawnTime = Time.fixedTime;
    // }

    // void FixedUpdate() {
    //     if (Time.fixedTime - lastSpawnTime >= SpawnDelay) {
    //         lastSpawnTime = Time.fixedTime;
    //         SpawnEnemies();
    //     }
    // }

    // void SpawnEnemies()
    // {
    //     int enemiesToSpawn = Mathf.Min(enemiesPerSpawn, maxEnemies - currentEnemyCount);
    //     if (enemiesToSpawn <= 0)
    //         return;

    //     List<Spawner> validSpawners = new List<Spawner>();
    //     foreach (Spawner spawner in spawners)
    //     {
    //         if (spawner.CanSpawn())
    //         {
    //             validSpawners.Add(spawner);
    //         }
    //     }

    //     while (validSpawners.Count < enemiesToSpawn)
    //     {
    //         Spawner furthestInvalidSpawner = GetFurthestInvalidSpawner();
    //         if (furthestInvalidSpawner != null)
    //         {
    //             validSpawners.Add(furthestInvalidSpawner);
    //         }
    //     }

    //     for (int i = 0; i < enemiesToSpawn; i++)
    //     {
    //         Spawner selectedSpawner = validSpawners[Random.Range(0, validSpawners.Count)];
    //         Instantiate(enemyPrefab, selectedSpawner.transform.position, Quaternion.identity);
    //         currentEnemyCount++;
    //     }
    // }

    // Spawner GetFurthestInvalidSpawner() {
    //     PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
    //     if (!plr) return null;
    //     Transform plrTrans = plr.transform;
        
    //     Spawner furthestSpawner = null;
    //     float maxDistance = -1f;

    //     foreach (Spawner spawner in spawners)
    //     {
    //         if (!spawner.CanSpawn())
    //         {
    //             float distanceToPlayer = Vector3.Distance(spawner.transform.position, plrTrans.position);
    //             if (distanceToPlayer > maxDistance)
    //             {
    //                 maxDistance = distanceToPlayer;
    //                 furthestSpawner = spawner;
    //             }
    //         }

    //         return furthestSpawner;
    //     }

    //     return furthestSpawner;
    // }

    // public void EnemyDefeated()
    // {
    //     currentEnemyCount--;
    // }

}