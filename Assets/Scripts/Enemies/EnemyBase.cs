using UnityEngine;

public class EnemyBase : MonoBehaviour {
    
    [Tooltip("FOR PRESTON TO SET.")]
    public EnemyType etypeid;
    
    [Header("Enemy Configuration")]
    [Tooltip("Reference to the common data of this enemy.")]
    public GeneralEnemySO EnConfig;
    protected float lastAttackTime = -1000;
    [Tooltip("GameObject holding the enemy's hitboxes.")]
    public GameObject Hitboxes;
    [Tooltip("GameObject holding the enemy's attack triggers.")]
    public GameObject EnemyTrigger;
    [Tooltip("GameObject holding the enemy's meshes.")]
    public GameObject Model;
    [Tooltip("If left null, will default to the gameobject's transform. This is primarily for the EnemyWeakpoint type.")]
    public Transform TransformForRicochetToAimAt = null;
    
    [Header("Audio")]
    [SerializeField]
    protected AudioSource AmbientAudio;
    [SerializeField]
    protected AudioSource DeathAudio;
    
    [HideInInspector]
    public bool Dead = false;
    [HideInInspector]
    public float MaxHealth = 50;
    [HideInInspector]
    public float health;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public BoidMover boid;
    [HideInInspector]
    public Suckable suckable;
    [HideInInspector]
    public bool RicochetCanon = true;
    [HideInInspector]
    // If enabled, ricochets can target this enemy. Note: if an enemy sets this value to false in its LateInit() override or earlier, the enemy will not be added to the SceneRunner's list.
    public bool ConsiderForRicochet = true;
    
    protected Lava lava;
    
    bool wasEverStarted = false;
    bool removedFromEList = false;
    
    [Header("For testing")]
    [SerializeField]
    protected bool invincible = false;
    
    
    
    void Awake() {
        if (EnConfig.FuelPickupPrefab == null)
            Debug.LogWarning("This enemy's fuelPickupPrefab was not set!");
        rb = GetComponent<Rigidbody>();
        boid = GetComponent<BoidMover>();
        suckable = GetComponent<Suckable>();
        health = 50;
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
        if (AmbientAudio)
            AmbientAudio.Play();
    }
    
    void FixedUpdate() {
        if (Dead) return;
        onFixedUpdate();
    }
    
    void OnTriggerEnter(Collider collider) {
        if (Dead) return;
        if (EnConfig.DealDamageOnTouch && collider.CompareTag("Player")) {
            Attack();
        }
    }
    
    public virtual void Attack() {
        if (Dead) return;
        if (EnConfig.Damage <= 0) return;
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return;
        if (Time.time - lastAttackTime <= EnConfig.AttackDelay) return;
        lastAttackTime = Time.time;
        plr.TakeDamage(EnConfig.Damage, EDamageType.Enemy);
    }
    
    public void TakeDamage(float amnt, EDamageType damageType) {
        if (Dead) return;
        if (invincible) return;
        if (health <= 0) return;
        OnTakeDamage(amnt, damageType);
    }
    
    protected virtual void OnTakeDamage(float amnt, EDamageType damageType) {
        health = Mathf.Max(health - amnt, 0);
        if (health == 0) {
            GameManager.Instance.OnEnemyTookDamage(this, damageType, true);
            OnDefeated(damageType);
        } else {
            GameManager.Instance.OnEnemyTookDamage(this, damageType, false);
        }
    }
    
    protected virtual void OnDefeated(EDamageType damageType) {
        if (Dead) return;
        if (damageType == EDamageType.Vacuum) {
            // Give player fuel immediately if killed by vacuum
            GameManager.CurrentPlayer.AddFuel(100);
        } else {
            DropFuel();
        }
        ShowDeath();
    }
    
    protected virtual void ShowDeath() {
        if (Dead) return;
        Dead = true;
        if (Hitboxes)
            Hitboxes.SetActive(false);
        if (EnemyTrigger)
            EnemyTrigger.SetActive(false);
        // NOTE: For now, the enemy's mesh will be disabled on death
        if (Model)
            Model.SetActive(false);
        if (boid)
            boid.enabled = false;
        if (!removedFromEList) {
            removedFromEList = true;
            GameManager.Instance.currentSceneRunner.RemoveEnemyFromList(this);
        }
        if (suckable)
            suckable.CanGetVacuumSucked = false;
        
        if (DeathAudio)
            DeathAudio.Play();
        if (AmbientAudio)
            AmbientAudio.Stop();
        
        enabled = false;
        Destroy(gameObject, EnConfig.DestroyDelay);
    }
    
    public void DropFuel() {
        DropFuel(transform.position);
    }
    
    public void DropFuel(Vector3 position) {
        Instantiate(EnConfig.FuelPickupPrefab, position, Quaternion.identity);
    }
    
    protected virtual void onFixedUpdate() {
        if (EnConfig.AffectedByLava && lava) {
            // Check if below lava
            if (transform.position.y + EnConfig.LavaSubmergeOffset < lava.currentHeight) {
                OnSubmerged();
            }
        }
    }
    
    /// <summary>
    /// Called when this enemy detects that its y position + LavaSubmergeOffset is below the current lava height.
    /// </summary>
    protected virtual void OnSubmerged() {
        // gameObject.SetActive(false); // TODO: Set inactive or just kill?
        // Destroy(gameObject);
        ShowDeath();
    }
    
    void OnDestroy() {
        OnDestroying();
    }
    
    protected virtual void OnDestroying() {
        if (wasEverStarted && !removedFromEList) {
            removedFromEList = true;
            GameManager.Instance.currentSceneRunner.RemoveEnemyFromList(this);
        }
    }
    
}
