using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    
    [Header("References")]
    public Rigidbody rb;
    [SerializeField]
    GameObject explosionEffect;
    [SerializeField]
    GameObject MeshAndCollider;
    
    
    [HideInInspector]
    public float Damage;
    
    bool hasHit = false;
    
    
    
    void OnCollisionEnter(Collision collision) {
        if (hasHit) return;
        if (collision.collider.CompareTag("Player")) {
            hasHit = true;
            GameManager.CurrentPlayer.TakeDamage(Damage, EDamageType.Enemy);
            onHitSomething();
        } else if (collision.collider.CompareTag("Untagged")) {
            hasHit = true;
            onHitSomething();
        }
    }
    
    void onHitSomething() {
        rb.velocity *= 0;
        MeshAndCollider.SetActive(false);
        explosionEffect.SetActive(true);
        Destroy(gameObject, 5);
    }
    
}