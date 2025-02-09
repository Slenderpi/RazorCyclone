using UnityEngine;

[CreateAssetMenu(fileName = "GeneralBoidData", menuName = "ScriptableObjects/GeneralBoid", order = 1)]
public class GeneralBoidSO : ScriptableObject {
    
    [Header("General Boid parameters")]
    [Tooltip("A sort of maximum speed for this Boid. Increasing this allows the Boid to reach higher speeds and sometimes accelerate faster.")]
    public float MaxSteeringVelocity = 15;
    [Tooltip("Determines the steering capability for this Boid. A higher maximum steering force allows sharper turns. If changing this value doesn't quite get the movement you want, consider adjusting the maximum velocity as well.")]
    public float MaxSteeringForce = 10;
    [Tooltip("Radius of wander's circle (or sphere with AllowFlight).")]
    public float WanderLimitRadius = 0.5f;
    [Tooltip("The distance wander's circle (or sphere) is from the front of the Boid.")]
    public float WanderLimitDist = 0.5f;
    [Tooltip("Maximum distance in an axis to step the wander point.")]
    public float WanderChangeDist = 0.15f;
    [Tooltip("Minimum time required to pass until a new wander point is calculated. Very low values could lead to jittery movement, depending on the other wander parameters.")]
    public float WanderMinimumDelay = 0;
    
}