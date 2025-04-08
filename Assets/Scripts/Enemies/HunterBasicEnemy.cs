using UnityEngine;

public class HunterBasicEnemy : EnemyBase {
    
    [Header("Hunter Basic Config")]
    [SerializeField]
    bool CanGetVacuumKilled = true;
    [SerializeField]
    TrailRenderer[] contrails;
    
    
    
    protected override void OnTakeDamage(float amnt, EDamageType damageType) {
        if (!CanGetVacuumKilled && damageType == EDamageType.Vacuum) return;
        base.OnTakeDamage(amnt, damageType);
    }

    protected override void ShowDeath() {
        foreach (TrailRenderer c in contrails)
            c.emitting = false;
        base.ShowDeath();
    }
    
}