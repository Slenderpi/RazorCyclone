using UnityEngine;

public class CTRL_Projectile : MonoBehaviour {
    
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
            CTRL_Enemy enemy = collision.gameObject.GetComponent<CTRL_Enemy>();
            if (enemy != null) {
                // enemy.TakeDamage(damage);
                // print("Hit enemy! New enemy health: " + enemy.health);
            } else {
                // print("Hit other: " + collision.gameObject.name);
            }
            CTRL_Explosion exp = Instantiate(explosionEffect, transform.position, Quaternion.identity).GetComponent<CTRL_Explosion>();
            exp.damage = damage;
            Destroy(gameObject);
        }
    }
    
}