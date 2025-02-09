using UnityEngine;

/// <summary>
/// This class handles the framework for the movement and rotation of a Boid. Indpendent boid objects should
/// create a child class from the BoidMover to implement movement logic specific to that Boid.
/// 
/// T
/// </summary>
public abstract class BoidMover : MonoBehaviour {
    
    [Header("General BoidMover parameters")]
    [Tooltip("Reference to the model to be rotated.\n\nIf none set, BoidMover will NOT apply any rotations.")]
    public Transform ModelToRotate;
    public float rotationPerFrameLerpAlpha = 0.1f;
    
    protected Rigidbody rb;
    protected Quaternion calculatedRotation;
    protected Vector3 wanderPoint;
    protected float lastWanderStepTime = -1000;
    
    
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    
    void Update() {
        if (ModelToRotate)
            ModelToRotate.rotation = Quaternion.Lerp(ModelToRotate.rotation, calculatedRotation, rotationPerFrameLerpAlpha);
    }
    
    void FixedUpdate() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return;
        Vector3 steer = CalculateSteering();
        rb.AddForce(steer, ForceMode.Acceleration);
        if (ModelToRotate)
            calculatedRotation = _calcSteering(rb.velocity, steer);
    }
    
    Quaternion _calcSteering(Vector3 forward, Vector3 steer) {
        if (forward.sqrMagnitude <= 0.0001f) forward = ModelToRotate.forward; // If vel is 0, use model's current forward
        if (forward.sqrMagnitude <= 0.0001f) return ModelToRotate.rotation; // If forward is 0, maintain prev rot
        if (steer.sqrMagnitude <= 0.0001f) return ModelToRotate.rotation; // If steer is 0, maintain prev rot
        return CalculateRotation(forward, steer);
    }
    
    public abstract Vector3 CalculateSteering();
    
    public abstract Quaternion CalculateRotation(Vector3 forward, Vector3 steer);
    
    public abstract void StepWanderPoint();
    
}