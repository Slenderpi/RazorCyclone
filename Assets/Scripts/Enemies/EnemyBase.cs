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
    [Tooltip("Determines if this enemy allows vacuum forces to be applied on it.\n\nNote: certain enemies (e.g. Hunter) will set this value on their own, and do not need this to be touched.")]
    public bool CanGetVacuumSucked = true;
    public int FuelAmount = 50; // The value of the fuel this enemy will drop
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public BoidMover boid;
    // public PlayerCharacterCtrlr player = GameManager.CurrentPlayer;
    
    [Header("References")]
    [SerializeField]
    private FuelPickup fuelPickupPrefab;
    
    [Header("For testing")]
    [SerializeField]
    protected bool invincible = false;
    // [Header("Enemy movement force")]
    // [SerializeField]
    // float MovementForce = 425f; 
    
    
    
    
    void Awake() {
        if (MaxHealth <= 0) Debug.LogWarning("Enemy MaxHealth set to a value <= 0 (set to " + MaxHealth + ").");
        if (fuelPickupPrefab == null)
            Debug.LogWarning("This enemy's fuelPickupPrefab was not set!");
        rb = GetComponent<Rigidbody>();
        boid = GetComponent<BoidMover>();
        health = MaxHealth;
        Init();
    }
    
    void OnTriggerEnter(Collider collider) {
        if (DealDamageOnTouch && collider.CompareTag("Player")) {
            Attack();
        }
    }
    
    public virtual void Attack() {
        if (Damage <= 0) return;
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return;
        if (Time.time - lastAttackTime <= AttackDelay) return;
        lastAttackTime = Time.time;
        plr.TakeDamage(Damage);
    }
    
    public virtual void TakeDamage(float amnt, EDamageType damageType) {
        if (invincible) return;
        if (health <= 0) return;
        health = Mathf.Max(health - amnt, 0);
        if (health == 0) {
            GameManager.Instance.OnEnemyTookDamage(this, damageType, true);
            OnDefeated(damageType);
        } else {
            GameManager.Instance.OnEnemyTookDamage(this, damageType, false);
        }
    }
    
    protected virtual void OnDefeated(EDamageType damageType) {
        if (damageType == EDamageType.Vacuum) {
            // Give player fuel immediately if killed by vacuum
            GameManager.CurrentPlayer.AddFuel(FuelAmount);
        } else {
            DropFuel();
        }
        gameObject.SetActive(false);
        Destroy(gameObject, 1);
    }

    public void DropFuel() {
        FuelPickup fuel = Instantiate(fuelPickupPrefab, transform.position, Quaternion.identity);
        fuel.FuelValue = FuelAmount;
    }
    
    protected virtual void Init() {
        
    }
    
}
