using UnityEngine;

public class ProjectileBase : MonoBehaviour {
    
    public float damage;
    public GameObject explosionEffect;
    
    float MaxLifetime = 10f;
    
    void Start() {
        Destroy(gameObject, MaxLifetime);
    }
    
    void Update() {
        
    }
    
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "Projectile") {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy != null) {
                enemy.TakeDamage(damage);
                // print("Hit enemy! New enemy health: " + enemy.health);
            } else {
                // print("Hit other: " + collision.gameObject.name);
            }
            ExplosionBase exp = Instantiate(explosionEffect, transform.position, Quaternion.identity).GetComponent<ExplosionBase>();
            // exp.damage = damage;
            Destroy(gameObject);
        }
    }
    
}