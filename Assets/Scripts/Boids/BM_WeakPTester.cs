using UnityEngine;

public class BM_WeakPTester : BoidMover {
    
    public SO_WeakPTest WeakPTestData;
    
    
    
    protected override void Init() {
        generalBoidData = WeakPTestData;
    }
    
    public override Vector3 CalculateSteering() {
        StepWanderPoint2D(WeakPTestData);
        return BoidSteerer.Wander(
            transform.position, rb.velocity, wanderPoint, WeakPTestData
        );
    }
    
    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return BoidRotator.YawOnly(forward);
    }
    
}