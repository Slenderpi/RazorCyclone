using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBase : MonoBehaviour {
    
    
    public float MaxHealth = 50;
    public float health { private set; get; }
    public float lastVacuumHitTime = 0f;
    public int FuelAmount = 50; // The value of the fuel this enemy will drop
    protected Rigidbody rb;
    // public PlayerCharacterCtrlr player = GameManager.CurrentPlayer;
    
    [Header("References")]
    [SerializeField]
    private FuelPickup fuelPickupPrefab;
    
    // [Header("Enemy movement force")]
    // [SerializeField]
    // float MovementForce = 425f; 
    
    void Awake() {
        if (MaxHealth <= 0) Debug.LogWarning("Enemy MaxHealth set to a value <= 0 (set to " + MaxHealth + ").");
        health = MaxHealth;
        rb = GetComponent<Rigidbody>();
        
        if (fuelPickupPrefab == null)
            Debug.LogWarning("This enemy's fuelPickupPrefab was not set!");
    }
    
    void Start() {
        
    }
    
    void Update() {
        // player = GameManager.CurrentPlayer;
    }
    
    /*
    void FixedUpdate() {
        if (GameManager.CurrentPlayer) {
            rb.AddForce(
                // removeY(GameManager.CurrentPlayer.transform.position - transform.position).normalized * MovementForce,
                (GameManager.CurrentPlayer.transform.position - transform.position).normalized * MovementForce,
                ForceMode.Force
            );
        }
    }
    */

    // modify depending on enemy specifics / actual health system
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerCharacterCtrlr player = GameManager.CurrentPlayer;
            if (player != null)
            {
                player.TakeDamage(1);
                Debug.Log("player health reduced");
            }
        }
    }
    
    public virtual void TakeDamage(float amnt, EDamageType damageType) {
        if (health <= 0) return;
        health -= amnt;
        if (health <= 0) {
            GameManager.Instance.OnEnemyDied();
            DropFuel();
            Destroy(gameObject);
        } else {
            GameManager.Instance.OnEnemyTookDamage();
        }
    }

    public void DropFuel() {
        FuelPickup fuel = Instantiate(fuelPickupPrefab, transform.position, Quaternion.identity);
        fuel.FuelValue = FuelAmount;
    }
    
    Vector3 removeY(Vector3 vector) {
        vector.y = 0f;
        return vector;
    }
    
}