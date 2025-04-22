using UnityEngine;

public class Spawner : MonoBehaviour {
    
    [Header("Spawner Config")]
    [Tooltip("A list of the enemy types that are NOT allowed to spawn at this spawner.")]
    public EEnemyType[] EnemyTypes;
    [Tooltip("If set to true, then the EnemyTypes list now means the enemies that ARE ALLOWED to spawn at this spawner.")]
    public bool UseTypesAsInclude = false;
    
    // public Transform[] enemies;
    public float playerDetectionRange = 15f;
    public float enemyDetectionRange = 5f;
    
    
    
    /// <summary>
    /// Set canSpawn based on spawner-specific criteria, such as distance to player.
    /// </summary>
    public bool ValidateSpawnerSpecificCriteria() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return false;
        return (plr.transform.position - transform.position).sqrMagnitude > playerDetectionRange * playerDetectionRange;
    }
    
    public bool AcceptsEnemy(EEnemyType etype) {
        // If etype is in enemyTypes, then return !UseAsExc
        foreach (EEnemyType flag in EnemyTypes) {
            if (etype == flag) {
                return UseTypesAsInclude;
            }
        }
        // etype not found in list of flags, so return UseAsExc
        return !UseTypesAsInclude;
    }
    
}
