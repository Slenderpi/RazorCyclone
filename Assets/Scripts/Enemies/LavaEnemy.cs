using System.Collections;
using UnityEngine;

public class LavaEnemy : WeakpointedEnemy {
    
    [Header("Lava Enemy Configuration")]
    public float WeakpointExposeDuration = 5;
    WaitForSeconds exposeWaiter;
    
    LavaWeakpoint weakpoint;
    bool isArmored = true;
    
    
    
    protected override void Init() {
        base.Init();
        if (MaxHealth > 1)
            Debug.LogWarning(">> Lava Enemy has more than one weakpoint!");
        weakpoint = weakpoints[0] as LavaWeakpoint;
        exposeWaiter = new WaitForSeconds(WeakpointExposeDuration);
    }
    
    protected override void LateInit() {
        base.LateInit();
        lava.OnLavaEnemySpawned();
    }

    protected override void onFixedUpdate() {
        if (transform.position.y < lava.currentHeight) {
            if (!rb.constraints.HasFlag(RigidbodyConstraints.FreezePositionY))
                rb.constraints |= RigidbodyConstraints.FreezePositionY;
            transform.position = new(transform.position.x, lava.currentHeight, transform.position.z);
        }
    }
    
    protected override void OnDefeated(EDamageType damageType) {
        lava.OnLavaEnemyDefeated();
        // Lava enemy does not drop its own fuel cell. Instead, its weakpoint(s) will.
        // base.OnDefeated(damageType);
        gameObject.SetActive(false);
        Destroy(gameObject, 1);
    }

    public override void TakeDamage(float amnt, EDamageType damageType) {
        if (damageType == EDamageType.Projectile && isArmored) {
            OnShotByCanon();
        }
    }
    
    void OnShotByCanon() {
        isArmored = false;
        weakpoint.BeginExpose();
        StartCoroutine(hideWeakpointTimer());
    }
    
    void ReArmor() {
        weakpoint.BeginHide();
        isArmored = true;
    }
    
    IEnumerator hideWeakpointTimer() {
        yield return exposeWaiter;
        if (health > 0)
            ReArmor();
    }
    
    /*
    Two states, armored and exposed
    During armored state:
        - 
    During exposed state:
        - 
    Enter exposed state:
        - Enable weakpoint
        - Start timer to hide weakpoint
    */
    
}