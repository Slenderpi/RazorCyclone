using System.Collections;
using UnityEngine;

public class CentipedeMissile : EnemyBase {
    
    [Header("Missile Config")]
    public CentMissileEnemySO CentMissConfig;
    [SerializeField]
    TrailRenderer MissileTrail;
    // [Tooltip("Determines the amount of seconds this missile can exist before it automatically explodes.")]
    // public float MaximumMissileLifetime = 10;
    // [Tooltip("When a missile is spawned in, it will start off without tracking and just burst forward, because that looks cool. This is the 'arming' phase.\nThe missile is still able to collide during this phase.\n\nThis variable determines how long arming lasts.")]
    // public float ArmingTime = 0.5f;
    // [Tooltip("Arming-phase burst strength.")]
    // public float ArmingBoost = 30;
    // [Tooltip("Arming-phase drag.\nBy having high drag and high boost, the missile looks like it bursts out quickly, gets weighed down by gravity, and then turns on its boosters to track the player.")]
    // public float ArmingDrag = 5;
    // [Tooltip("Prefab of explosion to use when the missile explodes.")]
    // public GameObject ExplosionEffectPrefab;
    GameObject pooledExplosion;
    
    float drag;
    bool hasTriggered = false;
    
    
    
    protected override void Init() {
        boid.enabled = false;
        drag = rb.drag;
        rb.drag = CentMissConfig.ArmingDrag;
        pooledExplosion = Instantiate(CentMissConfig.ExplosionEffectPrefab);
        pooledExplosion.SetActive(false);
        ConsiderForRicochet = false;
    }
    
    protected override void LateInit() {
        base.LateInit();
        rb.AddForce(transform.forward * CentMissConfig.ArmingBoost, ForceMode.VelocityChange);
        StartCoroutine(armTimer());
        StartCoroutine(missileLifeTimer());
    }
    
    void OnTriggerEnter(Collider collider) {
        if (Dead || hasTriggered) return;
        if (collider.CompareTag("Vacuum")) // Ignore vacuum
            return;
        hasTriggered = true;
        if (collider.CompareTag("Player"))
            Attack();
        explode();
        Model.SetActive(false);
        Destroy(gameObject, 1);
    }
    
    protected override void OnTakeDamage(float amnt, EDamageType damageType) {
        if (damageType != EDamageType.Vacuum)
            base.OnTakeDamage(amnt, damageType);
    }
    
    protected override void OnDestroying() {
        base.OnDestroying();
        if (pooledExplosion) {
            Destroy(pooledExplosion);
            pooledExplosion = null;
        }
    }
    
    void explode() {
        pooledExplosion.transform.SetPositionAndRotation(transform.position, transform.rotation);
        pooledExplosion.SetActive(true);
        pooledExplosion = null;
    }
    
    IEnumerator armTimer() {
        yield return new WaitForSeconds(CentMissConfig.ArmingTime);
        rb.useGravity = false;
        rb.drag = drag;
        boid.enabled = true;
    }
    
    IEnumerator missileLifeTimer() {
        yield return new WaitForSeconds(CentMissConfig.MaximumMissileLifetime);
        if (pooledExplosion) {
            explode();
        }
        Destroy(gameObject);
    }
    
}