using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ExplosionBase : MonoBehaviour {
    
    int MaxHits = 20;
    float ExplosionRadius = 3f; // Area of effect for damage and knockback. Effect sizes are scaled with this
    float ExplosionRadForEffectSize = 0.98f; // Expected radius for explosion when effect is made. Determines scaling of effects given explosionRadius
    float ExplosionForce = 0f;
    float AdditionalUpwardForce = 190f; // Rigidbodies hit will experience an additional upward boost
    
    public float damage;
    int ExplosionLayerMask;
    
    Collider[] hitColliders;
    
    void Awake() {
        ExplosionLayerMask = 1 << LayerMask.NameToLayer("Movable");
        hitColliders = new Collider[MaxHits];
        ParticleSystem[] particleEffects = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in particleEffects)
            p.transform.localScale = Vector3.one * ExplosionRadius / ExplosionRadForEffectSize;
    }
    
    void Start() {
        Physics.OverlapSphereNonAlloc(transform.position, ExplosionRadius, hitColliders, ExplosionLayerMask);
        foreach (Collider co in hitColliders) {
            if (co && co.TryGetComponent(out Rigidbody rb)) {
                float dist = Vector3.Distance(transform.position, co.transform.position);
                if (!Physics.Raycast(transform.position, (co.transform.position - transform.position).normalized, dist, ~ExplosionLayerMask)) {
                    rb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius + 0.5f);
                    rb.AddForce(Vector3.up * AdditionalUpwardForce, ForceMode.Impulse);
                    if (co.TryGetComponent(out EnemyBase en)) {
                        if (damage > 0) en.TakeDamage(damage, EDamageType.ProjectileExplosion);
                    }
                }
            }
        }
    }
    
}