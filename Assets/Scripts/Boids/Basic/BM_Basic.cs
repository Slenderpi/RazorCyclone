using Unity.VisualScripting;
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
    
    BoidBehaviour bt;
    BoidRotationType rt;
    bool cf;
    float msv;
    float msf;
    float wlr;
    float wld;
    float wcd;
    float wmd;
    
    
    
    void Start() {
        checkAndSetForOverride();
    }
    
    public override Vector3 CalculateSteering() {
        checkAndSetForOverride();
        return bt switch {
            BoidBehaviour.Seek => BoidSteer.Seek(transform.position, GameManager.CurrentPlayer.transform.position, rb.velocity, msv, msf),
            BoidBehaviour.Flee => BoidSteer.Flee(transform.position, GameManager.CurrentPlayer.transform.position, rb.velocity, msv, msf),
            BoidBehaviour.Pursuit => BoidSteer.Pursuit(transform.position, GameManager.CurrentPlayer.transform.position, rb.velocity, GameManager.CurrentPlayer.rb.velocity, msv, msf),
            BoidBehaviour.Evade => BoidSteer.Evade(transform.position, GameManager.CurrentPlayer.transform.position, rb.velocity, GameManager.CurrentPlayer.rb.velocity, msv, msf),
            BoidBehaviour.Wander => doWander(),
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

    public override void StepWanderPoint() {
        if (Time.fixedTime - lastWanderStepTime <= wmd)
            return;
        lastWanderStepTime = Time.fixedTime;
        wanderPoint = cf ? BoidSteer.StepWanderPoint3D(wanderPoint, wlr, wcd) : BoidSteer.StepWanderPoint2D(wanderPoint, wlr, wcd);
    }
    
    Vector3 doWander() {
        StepWanderPoint();
        return BoidSteer.Wander(transform.position, rb.velocity, wanderPoint, wld, msv, msf);
    }
    
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
        }
    }
    
}