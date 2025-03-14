using UnityEngine;

public class WeakpointedEnemy : EnemyBase {
    
    [Header("Weakpoints")]
    [Tooltip("List of weakpoints for this enemy. The health of this enemy will be set to the number of weakpoints they have.")]
    public EnemyWeakpoint[] weakpoints;
    
    
    
    protected override void Init() {
        // CanGetVacuumKilled = false;
        MaxHealth = weakpoints.Length;
        health = MaxHealth;
        foreach (EnemyWeakpoint wkp in weakpoints) {
            wkp.A_WeakpointDefeated += (EDamageType damageType) => {
                TakeWeakpointDamage(damageType);
            };
        }
    }
    
    protected virtual void TakeWeakpointDamage(EDamageType damageType) {
        if (health <= 0) return;
        if (--health <= 0) {
            GameManager.Instance.OnEnemyTookDamage(this, damageType, true);
            OnDefeated(EDamageType.Any);
        }
    }

}