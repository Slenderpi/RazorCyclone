using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPointedEnemy : EnemyBase {
    
    [Header("Weakpoints")]
    [Tooltip("List of weakpoints for this enemy. The health of this enemy will be set to the number of weakpoints they have.")]
    public EnemyWeakpoint[] weakpoints;
    
    
    
    protected override void Init() {
        MaxHealth = weakpoints.Length;
        base.Init();
        foreach (EnemyWeakpoint wkp in weakpoints) {
            wkp.A_WeakpointDefeated += (EDamageType damageType) => {
                TakeWeakpointDamage(damageType);
            };
        }
    }

    public override void TakeDamage(float amnt, EDamageType damageType) {
        // WeakPointedEnemies cannot take direct damage
        return;
    }
    
    protected virtual void TakeWeakpointDamage(EDamageType damageType) {
        health--;
        if (health <= 0) {
            GameManager.Instance.OnEnemyDied(this, damageType);
            OnDefeated();
        }
    }

}