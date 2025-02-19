using UnityEngine;

public class FuelPickup : MonoBehaviour {
    
    [HideInInspector]
    public float FuelValue = 100;
    
    public float AnimPeakHeight = 0.05f;
    [Tooltip("Time it takes to animate from low to high to low.")]
    public float AnimCycleDuration = 2;
    [Tooltip("In deg per sec.")]
    public float SpinRate = 180;
    [Tooltip(@"When a fuel pickup spawns, it will have a bit of a jump.
    - The Y-value determines the vertical velocity.
        The jump's Y will always be exactly this value.
    - The X-value determines the horizontal (x-z)
        velocity. The jump's horizontal velocity
        will lie on a circle formed by this value.")]
    public Vector2 SpawnJumpVelRange;
    
    [Header("References")]
    [SerializeField]
    Transform ModelPivot;
    public Rigidbody rb;
    
    bool hasBeenCollected = false;
    
    
    
    void Start() {
        float angle = Random.Range(0, Mathf.PI * 2);
        rb.AddForce(new(
            SpawnJumpVelRange.x * Mathf.Cos(angle),
            SpawnJumpVelRange.y,
            SpawnJumpVelRange.x * Mathf.Sin(angle)
        ), ForceMode.VelocityChange);
    }
    
    void OnTriggerEnter(Collider other) {
        if (hasBeenCollected) return;
        if (other.CompareTag("Player"))  {
            hasBeenCollected = true;
            PlayerCharacterCtrlr player = GameManager.CurrentPlayer;
            if (player != null)  {
                player.AddFuel(FuelValue);
                Destroy(gameObject);
            }
        }
    }
    
    void Update() {
        Vector3 newPos = ModelPivot.localPosition;
        float t = Time.time % AnimCycleDuration / AnimCycleDuration * 2;
        float x = t <= 1 ? t : 2 - t;
        newPos.y = Mathf.Lerp(-AnimPeakHeight, AnimPeakHeight, 3 * x * x - 2 * x * x * x);
        ModelPivot.localPosition = newPos;
        ModelPivot.Rotate(new(0, SpinRate * Time.deltaTime, 0));
    }
    
}
