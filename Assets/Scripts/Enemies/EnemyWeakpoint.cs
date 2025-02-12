using System;

public class EnemyWeakpoint : EnemyBase {
    
    public Action<EDamageType> A_WeakpointDefeated; // EDamageType dmgtype



    protected override void Init() {
        DealDamageOnTouch = false;
        CanGetVacuumSucked = false;
    }

    public override void TakeDamage(float amnt, EDamageType damageType) {
        if (invincible) return;
        if (health <= 0) return;
        health -= amnt;
        if (health <= 0) {
            OnDefeated(damageType);
        } else {
            GameManager.Instance.OnEnemyTookDamage(this, damageType, false);
        }
    }

    protected override void OnDefeated(EDamageType damageType) {
        if (damageType == EDamageType.Vacuum) {
            // Give player fuel immediately if killed by vacuum
            GameManager.CurrentPlayer.AddFuel(FuelAmount);
        } else {
            DropFuel();
        }
        gameObject.SetActive(false);
        A_WeakpointDefeated.Invoke(damageType);
    }

}