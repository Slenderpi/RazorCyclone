using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public float spawnRate = 3f;
    public int enemiesPerSpawn = 3;
    public int maxEnemies = 10;
    public GameObject enemyPrefab;
    public Transform player;

    private List<Spawner> spawners = new List<Spawner>();
    private int currentEnemyCount = 0;
    private float spawnTimer;

    void Start()
    {
        spawnTimer = spawnRate;
        spawners.AddRange(FindObjectsOfType<Spawner>());
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnEnemies();
            spawnTimer = spawnRate;
        }
    }

    void SpawnEnemies()
    {
        int enemiesToSpawn = Mathf.Min(enemiesPerSpawn, maxEnemies - currentEnemyCount);
        if (enemiesToSpawn <= 0)
            return;

        List<Spawner> validSpawners = new List<Spawner>();
        foreach (Spawner spawner in spawners)
        {
            if (spawner.CanSpawn())
            {
                validSpawners.Add(spawner);
            }
        }

        while (validSpawners.Count < enemiesToSpawn)
        {
            Spawner furthestInvalidSpawner = GetFurthestInvalidSpawner();
            if (furthestInvalidSpawner != null)
            {
                validSpawners.Add(furthestInvalidSpawner);
            }
        }

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Spawner selectedSpawner = validSpawners[Random.Range(0, validSpawners.Count)];
            Instantiate(enemyPrefab, selectedSpawner.transform.position, Quaternion.identity);
            currentEnemyCount++;
        }
    }

    Spawner GetFurthestInvalidSpawner()
    {
        Spawner furthestSpawner = null;
        float maxDistance = -1f;

        foreach (Spawner spawner in spawners)
        {
            if (!spawner.CanSpawn())
            {
                float distanceToPlayer = Vector3.Distance(spawner.transform.position, player.position);
                if (distanceToPlayer > maxDistance)
                {
                    maxDistance = distanceToPlayer;
                    furthestSpawner = spawner;
                }
            }

            return furthestSpawner;
        }

        return furthestSpawner;
    }

    public void EnemyDefeated()
    {
        currentEnemyCount--;
    }

}