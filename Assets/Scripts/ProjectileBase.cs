using System;
using System.Collections;
using UnityEngine;

public class ProjectileBase : MonoBehaviour {
    
    // [Tooltip("Damage of this projectile.")]
    [HideInInspector]
    public float damage;
    // public GameObject explosionEffect;
    [Tooltip("Maximum lifetime in seconds of this projectile to prevent projectiles that go into the void from living too long.")]
    public float MaxLifetime = 10f;
    
    bool hasHit = false;
    
    
    
    void Start() {
        StartCoroutine(ProjectileLifetime());
    }
    
    void Update() {
        
    }
    
    void OnCollisionEnter(Collision collision) {
        // TODO: For some reason the collision version does not work with enemy weakpoints
        onHitSomething(collision.gameObject);
    }
    
    void OnTriggerEnter(Collider collider) {
        onHitSomething(collider.gameObject);
    }
    
    // This function allows for a projectile's hitbox to be either a collider or a trigger
    void onHitSomething(GameObject hitObject) {
        if (hasHit) return;
        if (!hitObject.CompareTag("Player") &&
            !hitObject.CompareTag("Projectile") &&
            !hitObject.CompareTag("Pickup")) {
            hasHit = true;
            if (hitObject.TryGetComponent(out EnemyBase enemy)) {
                OnHitEnemy(enemy);
            } else {
                OnHitNonEnemy(hitObject);
            }
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Called after this projectile has hit an enemy. By default, this method
    /// calls TakeDamage() on the enemy.
    /// After calling OnHitEnemy(), the projectile will be destroyed.
    /// </summary>
    /// <param name="enemy">The enemy that the projectile hit.</param>
    protected virtual void OnHitEnemy(EnemyBase enemy) {
        enemy.TakeDamage(damage, EDamageType.Projectile);
    }
    
    /// <summary>
    /// Called after this projectile has hit something that wasn't an enemy. By default,
    /// this method does nothing. After calling OnHitNonEnemy(), this projectile
    /// will be destroyed.
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnHitNonEnemy(GameObject other) {}
    
    /// <summary>
    /// Called if the projectile lives up to its MaxLifetime without collision. By default,
    /// this method destroys the projectile.
    /// </summary>
    protected virtual void OnProjectileLifetimeExpired() {
        Destroy(gameObject);
    }
    
    IEnumerator ProjectileLifetime() {
        yield return new WaitForSeconds(MaxLifetime);
        OnProjectileLifetimeExpired();
    }
    
}