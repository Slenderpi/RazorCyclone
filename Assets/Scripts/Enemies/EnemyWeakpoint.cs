using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeakpoint : EnemyBase {
    
    public Action<EDamageType> A_WeakpointDefeated; // EDamageType dmgtype
    
    EDamageType killingDType;
    
    
    
    public override void TakeDamage(float amnt, EDamageType damageType) {
        if (health <= 0) return;
        health -= amnt;
        if (health <= 0) {
            killingDType = damageType;
            OnDefeated(damageType);
        } else {
            GameManager.Instance.OnEnemyTookDamage(this, damageType, false);
        }
    }

    protected override void OnDefeated(EDamageType damageType) {
        DropFuel();
        gameObject.SetActive(false);
        A_WeakpointDefeated.Invoke(killingDType);
    }

}