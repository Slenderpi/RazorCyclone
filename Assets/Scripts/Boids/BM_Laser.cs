using UnityEngine;

public class BM_Laser : BoidMover {
    
    public GeneralBoidSO CanonFodderData;
    
    
    
    public override Vector3 CalculateSteering() {
        // TODO
        return Vector3.zero;
    }
    
    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return BoidRotator.YawOnly(forward);
    }

    public override void StepWanderPoint() {
        if (Time.fixedTime - lastWanderStepTime > CanonFodderData.WanderMinimumDelay) {
            lastWanderStepTime = Time.fixedTime;
            wanderPoint = BoidSteer.StepWanderPoint3D(wanderPoint, CanonFodderData);
        }
    }
    
}