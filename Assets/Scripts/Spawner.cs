using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public enum EnemyType {
        FlyingGrunt,
        GroundGrunt,
        Hunter,
        FloorIsLava,
        ShieldedTurret
    }
    
    public EnemyType[] enemyTypes;
    private bool canSpawn = true;

    public Transform player;
    public Transform[] enemies;
    public float playerDetectionRange = 15f;
    public float enemyDetectionRange = 5f;

    void Start()
    {
        
    }

    void Update()
    {
        CheckSpawnConditions();
    }

    void CheckSpawnConditions()
    {
        bool playerInRange = Vector3.Distance(transform.position, player.position) <= playerDetectionRange;
        bool enemyInRange = false;

        foreach (Transform enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.position) <= enemyDetectionRange)
            {
                enemyInRange = true;
                break;
            }
        }

        canSpawn = !(playerInRange || enemyInRange);
    }

    public bool CanSpawn()
    {
        return canSpawn;
    }

}
