using UnityEngine;
using System.Collections;

public class Enemy : EnemyBase
{
    public GameObject hunterEnemy;
    public float spawnDelay = 3f;
    public float spawnInterval = 10f;

    private bool canSpawnHunter = false;

    void Start()
    {
        StartCoroutine(DelayedSpawn(spawnDelay));
    }

    void Update()
    {
        if (canSpawnHunter)
        {
            StartCoroutine(SpawnHunterEnemy());
        }
    }

    IEnumerator DelayedSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        canSpawnHunter = true;
    }

    IEnumerator SpawnHunterEnemy()
    {
        canSpawnHunter = false;

        Instantiate(hunterEnemy, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(spawnInterval);

        canSpawnHunter = true; 
    }

}
