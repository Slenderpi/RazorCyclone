using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// This class handles the framework for the movement and rotation of a Boid. Indpendent boid objects should
/// create a child class from the BoidMover to implement movement logic specific to that Boid.
/// </summary>
public abstract class BoidMover : MonoBehaviour {
    
    [Header("General BoidMover parameters")]
    [Tooltip("Reference to the model to be rotated.\n\nIf none set, BoidMover will NOT apply any rotations.")]
    public Transform ModelToRotate;
    public float rotationPerFrameLerpAlpha = 0.1f;
    
    // Should be set by child classes in Init()
    protected GeneralBoidSO generalBoidData;
    protected Rigidbody rb;
    protected Quaternion calculatedRotation;
    protected Vector3 wanderPoint;
    protected float lastWanderStepTime = -1000;
    
    protected bool enableAvoidanceTest = true;
    // protected delegate Vector3 AvoidanceTester(Vector3 pos, Vector3 velocity, GeneralBoidSO boidData);
    // protected AvoidanceTester AvoidanceTestFunction;
    
    
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
        if (ModelToRotate)
            ResetWanderPoint(1);
        else
            wanderPoint = transform.forward;
        Init();
    }
    
    void Update() {
        if (ModelToRotate)
            ModelToRotate.rotation = Quaternion.Lerp(ModelToRotate.rotation, calculatedRotation, rotationPerFrameLerpAlpha);
        else
            calculatedRotation = transform.rotation;
    }
    
    void FixedUpdate() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return;
        Vector3 steer = CalculateSteering();
        if (ModelToRotate)
            calculatedRotation = _calcRotation(rb.velocity, steer);
        if (enableAvoidanceTest)
            steer += testAvoidance();
        rb.AddForce(steer, ForceMode.Acceleration);
    }
    
    Quaternion _calcRotation(Vector3 forward, Vector3 steer) {
        if (forward.sqrMagnitude <= 0.0001f) forward = ModelToRotate.forward; // If vel is 0, use model's current forward
        if (forward.sqrMagnitude <= 0.0001f) return ModelToRotate.rotation; // If forward is 0, maintain prev rot
        if (steer.sqrMagnitude <= 0.0001f) return ModelToRotate.rotation; // If steer is 0, maintain prev rot
        return CalculateRotation(forward, steer);
    }
    
    /// <summary>
    /// Called in Awake().
    /// </summary>
    protected abstract void Init();
    
    public abstract Vector3 CalculateSteering();
    
    public abstract Quaternion CalculateRotation(Vector3 forward, Vector3 steer);
    
    public void ResetWanderPoint(float wanderLimitRadius) {
        wanderPoint = ModelToRotate.forward * wanderLimitRadius;
    }
    
    protected void StepWanderPoint2D(float wanderMinimumDelay, float wanderLimitRadius, float wanderChangeDist) {
        if (Time.fixedTime - lastWanderStepTime <= wanderMinimumDelay)
            return;
        lastWanderStepTime = Time.fixedTime;
        wanderPoint = BoidSteerer.StepWanderPoint2D(wanderPoint, wanderLimitRadius, wanderChangeDist);
    }
    
    protected void StepWanderPoint2D(GeneralBoidSO boidData) {
        StepWanderPoint2D(boidData.WanderMinimumDelay, boidData.WanderLimitRadius, boidData.WanderChangeDist);
    }
    
    protected void StepWanderPoint3D(float wanderMinimumDelay, float wanderLimitRadius, float wanderChangeDist) {
        if (Time.fixedTime - lastWanderStepTime <= wanderMinimumDelay)
            return;
        lastWanderStepTime = Time.fixedTime;
        wanderPoint = BoidSteerer.StepWanderPoint3D(wanderPoint, wanderLimitRadius, wanderChangeDist);
    }
    
    protected void StepWanderPoint3D(GeneralBoidSO boidData) {
        StepWanderPoint3D(boidData.WanderMinimumDelay, boidData.WanderLimitRadius, boidData.WanderChangeDist);
    }
    
    protected Vector3 testAvoidance() {
        return generalBoidData.AvoidanceTestType switch {
            AvoidanceTestMode.SingleFlat => BoidSteerer.Avoidance1PFlat(transform.position, rb.velocity, generalBoidData),
            AvoidanceTestMode.Single3D => BoidSteerer.Avoidance1P(transform.position, rb.velocity, generalBoidData),
            AvoidanceTestMode.TripleFlat => BoidSteerer.Avoidance3PFlat(transform.position, rb.velocity, generalBoidData),
            AvoidanceTestMode.Triple3D => BoidSteerer.Avoidance3P3D(transform.position, rb.velocity, calculatedRotation, generalBoidData),
            AvoidanceTestMode.FivePoints => BoidSteerer.Avoidance5P(transform.position, rb.velocity, calculatedRotation, generalBoidData),
            _ => Vector3.zero,
        };
    }
    
}