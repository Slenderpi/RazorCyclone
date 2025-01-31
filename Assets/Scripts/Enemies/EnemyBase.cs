using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBase : MonoBehaviour {
    
    [Header("Enemy Configuration")]
    public float MaxHealth = 50;
    [HideInInspector]
    public float health;
    public float Damage = 10;
    [Tooltip("A cooldown for attacking the player")]
    public float AttackDelay = 1;
    float lastAttackTime = -1000;
    public bool DealDamageOnTouch = true;
    [HideInInspector]
    public float lastVacuumHitTime = 0f;
    public int FuelAmount = 50; // The value of the fuel this enemy will drop
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public BoidObject boid;
    // public PlayerCharacterCtrlr player = GameManager.CurrentPlayer;
    
    [Header("References")]
    [SerializeField]
    private FuelPickup fuelPickupPrefab;
    
    [Header("For testing")]
    [SerializeField]
    bool invincible = false;
    // [Header("Enemy movement force")]
    // [SerializeField]
    // float MovementForce = 425f; 
    
    
    
    
    void Awake() {
        if (MaxHealth <= 0) Debug.LogWarning("Enemy MaxHealth set to a value <= 0 (set to " + MaxHealth + ").");
        if (fuelPickupPrefab == null)
            Debug.LogWarning("This enemy's fuelPickupPrefab was not set!");
        rb = GetComponent<Rigidbody>();
        boid = GetComponent<BoidObject>();
        Init();
    }

    // modify depending on enemy specifics / actual health system
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player") && DealDamageOnTouch) {
            Attack();
        }
    }
    
    public virtual void Attack() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return;
        if (Time.time - lastAttackTime <= AttackDelay) return;
        AttackDelay = Time.time;
        plr.TakeDamage(Damage);
    }
    
    public virtual void TakeDamage(float amnt, EDamageType damageType) {
        if (invincible) return;
        if (health <= 0) return;
        health = Mathf.Max(health - amnt, 0);
        if (health == 0) {
            GameManager.Instance.OnEnemyTookDamage(this, damageType, true);
            OnDefeated();
        } else {
            GameManager.Instance.OnEnemyTookDamage(this, damageType, false);
        }
    }
    
    protected virtual void OnDefeated() {
        DropFuel();
        gameObject.SetActive(false);
        Destroy(gameObject, 1);
    }

    public void DropFuel() {
        FuelPickup fuel = Instantiate(fuelPickupPrefab, transform.position, Quaternion.identity);
        fuel.FuelValue = FuelAmount;
    }
    
    protected virtual void Init() {
        health = MaxHealth;
    }
    
}
