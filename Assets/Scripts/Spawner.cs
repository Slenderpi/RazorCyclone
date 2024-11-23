using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    
    [Header("Spawner Config")]
    public EnemyType[] enemyTypes;
    public bool UseTypesAsExclude = false;
    [HideInInspector]
    public bool canSpawn = true;
    
    public Transform[] enemies;
    public float playerDetectionRange = 15f;
    public float enemyDetectionRange = 5f;

    void Start()
    {
        
    }

    void Update()
    {
        // CheckSpawnConditions();
    }

    void CheckSpawnConditions()
    {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return;
        Transform plrTrans = plr.transform;
        
        bool playerInRange = Vector3.Distance(transform.position, plrTrans.position) <= playerDetectionRange;
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

    public bool CanSpawn() {
        CheckSpawnConditions();
        return canSpawn;
    }
    
    /// <summary>
    /// Set canSpawn based on spawner-specific criteria, such as distance to player.
    /// </summary>
    public void ValidateSpawnerSpecificCriteria() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return;
        Transform plrTrans = plr.transform;
        
        canSpawn = (plrTrans.position - transform.position).magnitude > playerDetectionRange;
    }

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
        foreach (EnemyType flag in enemyTypes) {
            if (etype == flag) {
                return !UseTypesAsExclude;
            }
        }
        // etype not found in list of flags, so return UseAsExc
        return UseTypesAsExclude;
    }
}
