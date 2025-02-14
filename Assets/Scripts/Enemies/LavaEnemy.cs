public class LavaEnemy : WeakpointedEnemy {
    
    // [Header("Lava Enemy Configuration")]
    
    
    
    protected override void Init() {
        base.Init();
    }
    
    protected override void LateInit() {
        base.LateInit();
        lava.OnLavaEnemySpawned();
    }

    public override void TakeDamage(float amnt, EDamageType damageType) {
        if (damageType == EDamageType.Projectile) {
            // TODO: Expose weakpoint. Use a function call and maybe states?
            print("Moment");
        }
    }

    protected override void onFixedUpdate() {
        if (transform.position.y < lava.currentHeight) {
            if (!rb.constraints.HasFlag(UnityEngine.RigidbodyConstraints.FreezePositionY))
                rb.constraints |= UnityEngine.RigidbodyConstraints.FreezePositionY;
            transform.position = new(transform.position.x, lava.currentHeight, transform.position.z);
        }
    }
    
    protected override void OnDefeated(EDamageType damageType) {
        lava.OnLavaEnemyDefeated();
        base.OnDefeated(damageType);
    }
    
}