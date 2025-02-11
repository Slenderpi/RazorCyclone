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
    [Tooltip(@"Determines the number of raycasts to perform:
- None:
        No avoidance checking at all.
- SingleFlat:
        One cast in the direction of the Boid's velocity.
        However, any y-component in the velocity will
        be ignored, effectively locking the velocity
        vector in the x-z plane.
- Single3D:
        One cast in the direction of the Boid's velocity.
- TripleFlat:
        One cast from the center, one on the right, and
        one on the left. All of these vectors are
        locked in the x-z plane.
- Triple3D:
        One cast from the center, one on the right, and
        one on the left, with respect to Boid rotation.
        That is, if the Boid is rolled 90 degrees,
        the right cast will now be upward, and the
        left cast downward.
- FivePoints:
        NOT YET IMPLEMENTED, DO NOT USE.")]
    public AvoidanceTestMode AvoidanceTestType;
    [Tooltip("Maximum look-ahead distance to check if this Boid is heading for a wall.")]
    public float AvoidanceMaxLookDist = 4;
    [Tooltip("For Boids that use multi-point avoidance checking. Determines the angle of extra 'whiskers' from the forward whisker.\n\nIf using single-point avoidance checking, this value does not matter.")]
    public float AvoidanceWhiskerAngle = 30f;
    [Tooltip("The intensity of the Boid avoiding a wall when far away from a wall (intensity increases as they get closer).")]
    public float AvoidanceMinIntensity = 1;
    [Tooltip("The intensity of the Boid avoiding a wall when very close to a wall (intensity increases as they get closer).")]
    public float AvoidanceMaxIntensity = 10;
    [Tooltip("Limits the avoidance forces applied.\nWhen using single-point avoidance, you should set this to be at least AvoidanceMaxIntensity.\n\n" +
             "When using multi-point avoidance, this value limits the sum of the calculated avoidance forces, so might want to set it higher (e.g. for 3-point, it may be better to set this to AvoidanceMaxIntensity * 3)." +
             "You can still set it lower if you want.")]
    public float AvoidanceMaxSteeringForce = 10;
    [Tooltip("If true, the Boid will also avoid colliding into objects that are on the InvisBoidWall layer.")]
    public bool AvoidInvisBoidWalls = false;
    
}

[System.Serializable]
public enum AvoidanceTestMode {
    None,
    SingleFlat,
    Single3D,
    TripleFlat,
    Triple3D,
    FivePoints
}