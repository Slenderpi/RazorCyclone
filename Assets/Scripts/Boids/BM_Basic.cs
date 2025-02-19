using UnityEngine;

public class BM_Basic : BoidMover {
    
    public SO_BasicBoid BoidData;
    [Header("Overrides")]
    [SerializeField]
    [Tooltip("If set true, then BM_Basic will use the below values insetad of the ones provided by BoidData.\n\nThis is useful for changing an individual BoidMover.")]
    bool USE_OVERRIDES = false;
    [SerializeField]
    BoidBehaviour BehaviourType;
    [SerializeField]
    BoidRotationType RotationType;
    [SerializeField]
    bool CanFly = false;
    [SerializeField]
    float MaxSteeringVelocity = 15;
    [SerializeField]
    float MaxSteeringForce = 10;
    [SerializeField]
    float WanderLimitRadius = 0.5f;
    [SerializeField]
    float WanderLimitDist = 0.5f;
    [SerializeField]
    float WanderChangeDist = 0.15f;
    [SerializeField]
    float WanderMinimumDelay = 0;
    [SerializeField]
    float AvoidanceMaxLookDist = 4;
    [SerializeField]
    float AvoidanceWhiskerAngle = 30f;
    [SerializeField]
    float AvoidanceMinIntensity = 1;
    [SerializeField]
    float AvoidanceMaxIntensity = 7;
    [SerializeField]
    float AvoidanceMaxSteeringForce = 10;
    
    BoidBehaviour bt;
    BoidRotationType rt;
    bool cf;
    float msv;
    float msf;
    float wlr;
    float wld;
    float wcd;
    float wmd;
    float amld;
    float awa;
    float ami;
    float aMi;
    float amsf;
    
    
    
    void Start() {
        checkAndSetForOverride();
    }
    
    protected override void Init() {
        generalBoidData = BoidData;
    }
    
    public override Vector3 CalculateSteering() {
        checkAndSetForOverride();
        return bt switch {
            BoidBehaviour.Seek => BoidSteerer.Seek(transform.position, GameManager.CurrentPlayer.transform.position, rb.velocity, msv, msf),
            BoidBehaviour.Flee => BoidSteerer.Flee(transform.position, GameManager.CurrentPlayer.transform.position, rb.velocity, msv, msf),
            BoidBehaviour.Pursuit => BoidSteerer.Pursuit(transform.position, GameManager.CurrentPlayer.transform.position, rb.velocity, GameManager.CurrentPlayer.rb.velocity, msv, msf),
            BoidBehaviour.Evade => BoidSteerer.Evade(transform.position, GameManager.CurrentPlayer.transform.position, rb.velocity, GameManager.CurrentPlayer.rb.velocity, msv, msf),
            BoidBehaviour.Wander => doWander(),
            BoidBehaviour.TestState => testObstAvoid(),
            _ => Vector3.zero,
        };
    }
    
    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return rt switch {
            BoidRotationType.YawOnly => BoidRotator.YawOnly(forward),
            BoidRotationType.YawAndPitch => BoidRotator.YawAndPitch(forward),
            BoidRotationType.YawAndBank => BoidRotator.YawAndBank(forward, steer),
            BoidRotationType.Airplane => BoidRotator.Airplane(forward, steer),
            _ => ModelToRotate.rotation,
        };
    }
    
    Vector3 doWander() {
        StepWanderPoint2D(wmd, wlr, wcd);
        return BoidSteerer.Wander(transform.position, rb.velocity, wanderPoint, wld, msv, msf);
    }
    
    Vector3 testObstAvoid() {
        Vector3 forward = rb.velocity;
        if (forward.sqrMagnitude <= 0.0001f)
            forward = transform.forward;
        // Vector3 avoid = BoidSteerer.Avoidance3P(transform.position, forward, awa, amld, ai, amsf);
        Vector3 straight = BoidSteerer.Seek(transform.position, transform.position + forward.normalized * BoidData.MaxSteeringForce / 10f, rb.velocity, msv, msf);
        return straight;
        // return avoid + straight;
        // return doWander() + avoid;
        // return doWander();
    }
    
    // public override Vector3 AddObstacleAvoidance() {
    //     Vector3 forward = rb.velocity;
    //     if (forward.sqrMagnitude <= 0.001f)
    //         forward = transform.forward;
    //     return BoidSteerer.Avoidance3P(transform.position, forward, awa, amld, ai, amsf);
    // }
    
    void checkAndSetForOverride() {
        if (USE_OVERRIDES) {
            bt = BehaviourType;
            rt = RotationType;
            cf = CanFly;
            msv = MaxSteeringVelocity;
            msf = MaxSteeringForce;
            wlr = WanderLimitRadius;
            wld = WanderLimitDist;
            wcd = WanderChangeDist;
            wmd = WanderMinimumDelay;
            amld = AvoidanceMaxLookDist;
            awa = AvoidanceWhiskerAngle;
            aMi = AvoidanceMinIntensity;
            aMi = AvoidanceMaxIntensity;
            amsf = AvoidanceMaxSteeringForce;
        } else {
            bt = BoidData.BehaviourType;
            rt = BoidData.RotationType;
            cf = BoidData.CanFly;
            msv = BoidData.MaxSteeringVelocity;
            msf = BoidData.MaxSteeringForce;
            wlr = BoidData.WanderLimitRadius;
            wld = BoidData.WanderLimitDist;
            wcd = BoidData.WanderChangeDist;
            wmd = BoidData.WanderMinimumDelay;
            amld = BoidData.AvoidanceMaxLookDist;
            awa = BoidData.AvoidanceWhiskerAngle;
            aMi = BoidData.AvoidanceMinIntensity;
            aMi = BoidData.AvoidanceMaxIntensity;
            amsf = BoidData.AvoidanceMaxSteeringForce;
        }
    }
    
}