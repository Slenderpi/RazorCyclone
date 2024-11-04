using System.Collections;
using UnityEngine;

public class ProjectileBase : MonoBehaviour {
    
    [Tooltip("Damage of this projectile.")]
    public float damage;
    // public GameObject explosionEffect;
    [Tooltip("Maximum lifetime in seconds of this projectile to prevent projectiles that go into the void from living too long.")]
    public float MaxLifetime = 10f;
    
    void Start() {
        StartCoroutine(ProjectileLifetime());
    }
    
    void Update() {
        
    }
    
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "Projectile") {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy != null) {
                OnHitEnemy(enemy);
            } else {
                OnHitNonEnemy(collision.gameObject);
            }
            // ExplosionBase exp = Instantiate(explosionEffect, transform.position, Quaternion.identity).GetComponent<ExplosionBase>();
            // exp.damage = damage;
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