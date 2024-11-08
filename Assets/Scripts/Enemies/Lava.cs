using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lava : MonoBehaviour {
    
    [Header("Lava Behavior and Stats")]
    public float LavaRiseSpeed = 0.5f;
    public float MaxLavaHeight = 25f;
    public float LavaLowerSpeed = 4f;
    float riseRate = 0;
    public float LavaDamage = 50;
#if UNITY_EDITOR
    public bool ForceDisable = false;
#endif
    
    [Header("References")]
    [SerializeField]
    CollisionNotifier LavaHitboxCollider;
    
    int livingLavaEnemies = 0;
    float startY;
    
    
    
    void Start() {
        startY = transform.position.y;
    }
    
    void FixedUpdate() {
        Vector3 newPosition = transform.position;
        newPosition.y += Time.fixedDeltaTime * riseRate;
        if (newPosition.y > MaxLavaHeight) {
            newPosition.y = MaxLavaHeight;
            riseRate = 0;
        } else if (livingLavaEnemies == 0 && newPosition.y <= startY) {
            riseRate = 0;
            newPosition.y = startY;
            gameObject.SetActive(false);
        }
        transform.position = newPosition;
    }
    
    void OnLavaCollisionEntered(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            GameManager.CurrentPlayer.TakeDamage(LavaDamage);
        }
    }
    
    void OnEnable() {
        LavaHitboxCollider.A_CollisionEntered += OnLavaCollisionEntered;
        // LavaHitboxCollider.A_CollisionExited += OnLavaCollisionExit;
    }
    
    void OnDisable() {
        LavaHitboxCollider.A_CollisionEntered -= OnLavaCollisionEntered;
        // LavaHitboxCollider.A_CollisionExited -= OnLavaCollisionExit;
    }
    
    public void OnLavaEnemySpawned() {
#if UNITY_EDITOR
        if (ForceDisable) return;
#endif
        if (++livingLavaEnemies == 1) {
            gameObject.SetActive(true);
            if (transform.position.y < MaxLavaHeight) {
                riseRate = LavaRiseSpeed;
            }
        }
    }
    
    public void OnLavaEnemyDefeated() {
#if UNITY_EDITOR
        if (ForceDisable) return;
#endif
        if (--livingLavaEnemies == 0) {
            riseRate = -LavaLowerSpeed;
        }
    }
    
}