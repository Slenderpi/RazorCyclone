using UnityEngine;

public class Lava : MonoBehaviour {
    
    [Header("Lava Behavior and Stats")]
    public float LavaRiseSpeed = 0.5f;
    public float MaxLavaHeight = 25f;
    public float LavaLowerSpeed = 4f;
    float currRiseRate = 0;
    public float LavaDamage = 50;
    [Tooltip("When the player touches lava and bounces off of it, the vertical velocity of the bounce will be at least this value.")]
    public float MinimumVerticalBounceSpeed = 6;
#if UNITY_EDITOR
    public bool ForceDisable = false;
#endif
    
    [Header("References")]
    [SerializeField]
    TriggerNotifier LavaHitboxNotifier;
    
    [HideInInspector]
    public float currentHeight;
    
    int livingLavaEnemies = 0;
    float startY;
    
    int PICKUP_LAYER;



    void Awake() {
#if UNITY_EDITOR
        if (ForceDisable)
            gameObject.SetActive(false); 
#endif
    }

    void Start() {
        PICKUP_LAYER = LayerMask.NameToLayer("Pickup");
        startY = transform.position.y;
        currentHeight = startY;
    }
    
    void FixedUpdate() {
        // Vector3 newPosition = transform.position;
        currentHeight += Time.fixedDeltaTime * currRiseRate;
        // newPosition.y += Time.fixedDeltaTime * currRiseRate;
        if (currentHeight > MaxLavaHeight) {
            currentHeight = MaxLavaHeight;
            currRiseRate = 0;
        } else if (livingLavaEnemies == 0 && currentHeight <= startY) {
            currRiseRate = 0;
            currentHeight = startY;
            // gameObject.SetActive(false);
        }
        transform.position = new(transform.position.x, currentHeight, transform.position.y);
    }
    
    void OnObjectEnteredLava(Collider collider) {
        // if (collider.gameObject.layer == PICKUP_LAYER) {
        //     FuelPickup fp = collider.gameObject.GetComponent<FuelPickup>();
        //     print("Pickup landed in! PICKUP_LAYER: " + PICKUP_LAYER + "; layer: " + collider.gameObject.layer);
        // } else {
        //     print("Lava collision: " + collider.gameObject.name);
        // }
        // // if (collision.gameObject.CompareTag("Player")) {
        // //     GameManager.CurrentPlayer.TakeDamage(LavaDamage);
        // // }
    }
    
    void OnEnable() {
        // LavaHitboxNotifier.A_TriggerEntered += OnObjectEnteredLava;
        // LavaHitboxCollider.A_CollisionExited += OnLavaCollisionExit;
    }
    
    void OnDisable() {
        // LavaHitboxNotifier.A_TriggerEntered -= OnObjectEnteredLava;
        // LavaHitboxCollider.A_CollisionExited -= OnLavaCollisionExit;
    }
    
    public void OnLavaEnemySpawned() {
#if UNITY_EDITOR
        if (ForceDisable) return;
#endif
        if (++livingLavaEnemies == 1) {
            // gameObject.SetActive(true);
            if (currentHeight < MaxLavaHeight) {
                currRiseRate = LavaRiseSpeed;
            }
            // if (transform.position.y < MaxLavaHeight) {
            //     riseRate = LavaRiseSpeed;
            // }
        }
    }
    
    public void OnLavaEnemyDefeated() {
#if UNITY_EDITOR
        if (ForceDisable) return;
#endif
        if (--livingLavaEnemies == 0) {
            currRiseRate = -LavaLowerSpeed;
        }
    }
    
}