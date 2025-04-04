using UnityEngine;

public class HunterBasicEnemy : EnemyBase {
    
    [Header("Hunter Basic Config")]
    [SerializeField]
    bool CanGetVacuumKilled = true;
    
    
    
    protected override void OnTakeDamage(float amnt, EDamageType damageType) {
        if (!CanGetVacuumKilled && damageType == EDamageType.Vacuum) return;
        base.OnTakeDamage(amnt, damageType);
    }
    
}