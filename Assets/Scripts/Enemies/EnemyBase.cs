using UnityEngine;

public class EnemyBase : MonoBehaviour {
    
    [Tooltip("FOR PRESTON TO SET.")]
    public EnemyType etypeid;
    
    [Header("Enemy Configuration")]
    public float MaxHealth = 50;
    [HideInInspector]
    public float health;
    public float Damage = 10;
    [Tooltip("A cooldown for attacking the player")]
    public float AttackDelay = 1;
    float lastAttackTime = -1000;
    public bool DealDamageOnTouch = true;
    [Tooltip("Determines if this enemy can be killed when touched by the vacuum's killbox.\n\nNote: certain enemies (e.g. Hunter) will set this value on their own, and do not need this to be touched.")]
    public bool CanGetVacuumKilled = true;
    [Tooltip("If enabled, this enemy will call its OnSubmerged() method when it detects that it is below lava.")]
    public bool AffectedByLava = true;
    // [Tooltip("If enabled, projectiles will ricochet when they hit this enemy.\n\nNote: this variable does not determine if this enemy can be targeted by ricochet aimbot, that is done by an internal variable.")]
    [HideInInspector]
    public bool RicochetCanon = true;
    [HideInInspector]
    // If enabled, ricochets can target this enemy. Note: if an enemy sets this value to false in its LateInit() override or earlier, the enemy will not be added to the SceneRunner's list.
    public bool ConsiderForRicochet = true;
    [Tooltip("If left null, will default to the gameobject's transform. This is primarily for the EnemyWeakpoint type.")]
    public Transform TransformForRicochetToAimAt = null;
    [Tooltip("This enemy will be affected by lava if its y position + HeightOffset is below the lava. This is to allow objects to sink lower before actually being counted as submerged.")]
    public float LavaSubmergeOffset = 1;
    public int FuelAmount = 50; // The value of the fuel this enemy will drop
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public BoidMover boid;
    [HideInInspector]
    public Suckable suckable;
    // public PlayerCharacterCtrlr player = GameManager.CurrentPlayer;
    
    [Header("References")]
    [SerializeField]
    FuelPickup fuelPickupPrefab;
    
    protected Lava lava;
    
    bool wasEverStarted = false;
    
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
        suckable = GetComponent<Suckable>();
        health = MaxHealth;
        if (TransformForRicochetToAimAt == null) TransformForRicochetToAimAt = transform;
        Init();
    }
    
    /// <summary>
    /// Called by Awake().
    /// </summary>
    protected virtual void Init() {}
    
    void Start() {
        wasEverStarted = true;
        LateInit();
        GameManager.Instance.currentSceneRunner.AddEnemyToList(this);
    }
    
    /// <summary>
    /// Called by Start().
    /// </summary>
    protected virtual void LateInit() {
        lava = GameManager.Instance.currentSceneRunner.lava;
    }
    
    void FixedUpdate() {
        onFixedUpdate();
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
        plr.TakeDamage(Damage, EDamageType.Enemy);
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
        Destroy(gameObject);
    }
    
    public void DropFuel() {
        DropFuel(transform.position);
    }
    
    public void DropFuel(Vector3 position) {
        FuelPickup fuel = Instantiate(fuelPickupPrefab, position, Quaternion.identity);
        fuel.FuelValue = FuelAmount;
    }
    
    protected virtual void onFixedUpdate() {
        if (AffectedByLava && lava) {
            // Check if below lava
            if (transform.position.y + LavaSubmergeOffset < lava.currentHeight) {
                OnSubmerged();
            }
        }
    }
    
    /// <summary>
    /// Called when this enemy detects that its y position + LavaSubmergeOffset is below the current lava height.
    /// </summary>
    protected virtual void OnSubmerged() {
        gameObject.SetActive(false); // TODO: Set inactive or just kill?
        Destroy(gameObject);
    }
    
    void OnDestroy() {
        OnDestroying();
    }
    
    protected virtual void OnDestroying() {
        if (wasEverStarted)
            GameManager.Instance.currentSceneRunner.RemoveEnemyFromList(this);
    }
    
}
