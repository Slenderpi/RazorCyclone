using System;

public class EnemyWeakpoint : EnemyBase {
    
    public Action<EDamageType> A_WeakpointDefeated; // EDamageType dmgtype
    
    
    
    protected override void Init() {
        // DealDamageOnTouch = false;
    }
    
    protected override void OnTakeDamage(float amnt, EDamageType damageType) {
        health -= amnt;
        if (health <= 0) {
            OnDefeated(damageType);
        } else {
            // GameManager.Instance.OnEnemyTookDamage(this, damageType, false);
        }
    }
    
    protected override void OnDefeated(EDamageType damageType) {
        if (damageType == EDamageType.Vacuum) {
            // Give player fuel immediately if killed by vacuum
            GameManager.CurrentPlayer.AddFuel(100);
        } else {
            DropFuel();
        }
        gameObject.SetActive(false);
        A_WeakpointDefeated.Invoke(damageType);
    }
    
}