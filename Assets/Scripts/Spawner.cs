using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    
    [Header("Spawner Config")]
    [Tooltip("A list of the enemy types that are NOT allowed to spawn at this spawner.")]
    public EnemyType[] EnemyTypes;
    [Tooltip("If set to true, then the EnemyTypes list now means the enemies that ARE ALLOWED to spawn at this spawner.")]
    public bool UseTypesAsInclude = false;
    [HideInInspector]
    public bool canSpawn = true;
    
    // public Transform[] enemies;
    public float playerDetectionRange = 15f;
    public float enemyDetectionRange = 5f;

    // void Update() {
    //     // CheckSpawnConditions();
    // }

    // void CheckSpawnConditions()
    // {
    //     PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
    //     if (!plr) return;
    //     Transform plrTrans = plr.transform;
        
    //     bool playerInRange = Vector3.Distance(transform.position, plrTrans.position) <= playerDetectionRange;
    //     bool enemyInRange = false;

    //     // foreach (Transform enemy in enemies)
    //     // {
    //     //     if (Vector3.Distance(transform.position, enemy.position) <= enemyDetectionRange)
    //     //     {
    //     //         enemyInRange = true;
    //     //         break;
    //     //     }
    //     // }

    //     canSpawn = !(playerInRange || enemyInRange);
    // }

    // public bool CanSpawn() {
    //     CheckSpawnConditions();
    //     return canSpawn;
    // }
    
    /// <summary>
    /// Set canSpawn based on spawner-specific criteria, such as distance to player.
    /// </summary>
    public void ValidateSpawnerSpecificCriteria() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return;
        Transform plrTrans = plr.transform;
        
        canSpawn = (plrTrans.position - transform.position).magnitude > playerDetectionRange;
    }

    /// <summary>
    /// Determines if the provided string refers to a valid enemy type and if that type is accepted by this spawner.
    /// </summary>
    /// <param name="estr"></param>
    /// <returns></returns>
    public bool AcceptsEnemyStr(string estr) {
        // TODO: Find a better way to do this, perhaps by not using strings or the EnemyStrs system
        // Convert estr to EnemyType enum
        EnemyType etype;
        if (estr == "EnemyBase") etype = EnemyType.EnemyBase;
        else if (estr == "Hunter") etype = EnemyType.Hunter;
        else if (estr == "Laser") etype = EnemyType.Laser;
        else if (estr == "Lava") etype = EnemyType.FloorIsLava;
        else return false;
        
        // If etype is in enemyTypes, then return !UseAsExc
        foreach (EnemyType flag in EnemyTypes) {
            if (etype == flag) {
                return UseTypesAsInclude;
            }
        }
        // etype not found in list of flags, so return UseAsExc
        return !UseTypesAsInclude;
    }
}
