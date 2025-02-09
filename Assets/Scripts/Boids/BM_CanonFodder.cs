using UnityEngine;

public class BM_CanonFodder : BoidMover {
    
    public SO_CanonFodder CanonFodderData;
    
    
    
    public override Vector3 CalculateSteering() {
        Vector3 plrPos = GameManager.CurrentPlayer.transform.position;
        if ((plrPos - transform.position).sqrMagnitude <= CanonFodderData.FleeTriggerDistance * CanonFodderData.FleeTriggerDistance) {
            Vector3 steer = BoidSteer.Flee(transform.position, plrPos, rb.velocity, CanonFodderData);
            steer.y = 0;
            return steer;
        } else {
            StepWanderPoint();
            return BoidSteer.Wander(transform.position, rb.velocity, wanderPoint, CanonFodderData);
        }
    }
    
    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return BoidRotator.YawOnly(forward);
    }

    public override void StepWanderPoint() {
        if (Time.fixedTime - lastWanderStepTime <= CanonFodderData.WanderMinimumDelay)
            return;
        lastWanderStepTime = Time.fixedTime;
        wanderPoint = BoidSteer.StepWanderPoint2D(wanderPoint, CanonFodderData);
    }
    
}