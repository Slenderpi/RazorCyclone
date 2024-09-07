using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CTRL_Enemy : MonoBehaviour {
    
    public float MaxHealth { private set; get; } = 50;
    public float health { private set; get; }
    
    public float lastVacuumHitTime = 0f;
    public int vacuumArrayIndex = -1;
    
    [Header("Enemy movement force")]
    [SerializeField]
    float MovementForce = 425f;
    
    Rigidbody rb;
    
    void Awake() {
        if (MaxHealth <= 0) Debug.LogWarning("Enemy MaxHealth set to a value <= 0 (set to " + MaxHealth + ").");
        health = MaxHealth;
        rb = GetComponent<Rigidbody>();
    }
    
    void Start() {
        
    }
    
    void Update() {
        
    }
    
    void FixedUpdate() {
        if (GameManager.CurrentPlayer) {
            rb.AddForce(
                // removeY(GameManager.CurrentPlayer.transform.position - transform.position).normalized * MovementForce,
                (GameManager.CurrentPlayer.transform.position - transform.position).normalized * MovementForce,
                ForceMode.Force
            );
        }
    }
    
    public void TakeDamage(float amnt) {
        if (health <= 0) return;
        health -= amnt;
        GameManager.CurrentPlayer.AddFuel(amnt * 0.2f);
        if (health <= 0) {
            print("'" + gameObject.name + "' died.");
            GameManager.Instance.OnEnemyDied();
            Destroy(gameObject);
        } else {
            print("'" + gameObject.name + "' health now " + health);
        }
    }
    
    Vector3 removeY(Vector3 vector) {
        vector.y = 0f;
        return vector;
    }
    
}