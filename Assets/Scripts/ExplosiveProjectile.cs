using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : ProjectileBase {
    
    [Header("Explosion Settings")]
    [Tooltip("The damage of the explosion.")]
    // NOTE: If ExplosionDamage is set to 0, the TakeDamage() method will not be called on enemies.
    public float ExplosionDamage = 50f;
    [Tooltip("Area of effect for damage and knockback. Effect sizes are scaled with this.")]
    public float ExplosionRadius = 3f;
    [Tooltip("Expected radius for explosion when effect is made. Determines scaling of effects given explosionRadius")]
    public float ExplosionRadForEffectSize = 0.98f;
    [Tooltip("Radial force applied on colliders by the explosion.")]
    public float ExplosionForce = 0f;
    [Tooltip("Rigidbodies hit can experience an additional upward boost from this.")]
    public float AdditionalUpwardForce = 190f;
    
    [Header("References")]
    public EffectInstance explosionEffect;
    EffectInstance expeffect;
    
    int MaxHits = 20;
    Collider[] hitColliders;
    int ExplosionLayerMask;

    
    
    void Awake() {
        ExplosionLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        hitColliders = new Collider[MaxHits];
        initExplosionEffect();
    }

    protected override void OnHitEnemy(EnemyBase enemy) {
        base.OnHitEnemy(enemy);
        explode(enemy);
    }

    protected override void OnHitNonEnemy(GameObject other) {
        explode();
    }

    protected override void OnProjectileLifetimeExpired() {
        explode();
        Destroy(gameObject);
    }
    
    void explode(EnemyBase enemyToNotDmg = null) {
        expeffect.transform.position = transform.position;
        expeffect.gameObject.SetActive(true);
        Physics.OverlapSphereNonAlloc(transform.position, ExplosionRadius, hitColliders, ExplosionLayerMask);
        foreach (Collider co in hitColliders) {
            if (co && co.TryGetComponent(out Rigidbody rb)) {
                float dist = Vector3.Distance(transform.position, co.transform.position);
                if (!Physics.Raycast(transform.position, (co.transform.position - transform.position).normalized, dist, ~ExplosionLayerMask)) {
                    rb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius + 0.5f);
                    rb.AddForce(Vector3.up * AdditionalUpwardForce, ForceMode.Impulse);
                    if (ExplosionDamage > 0)
                        if (co.TryGetComponent(out EnemyBase en))
                            if (en != enemyToNotDmg)
                                en.TakeDamage(ExplosionDamage, EDamageType.ProjectileExplosion);
                }
            }
        }
    }
    
    void initExplosionEffect() {
        expeffect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        expeffect.gameObject.SetActive(false);
        ParticleSystem[] particleEffects = expeffect.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in particleEffects)
            p.transform.localScale = Vector3.one * ExplosionRadius / ExplosionRadForEffectSize;
    }

}