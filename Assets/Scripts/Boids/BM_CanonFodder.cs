using UnityEngine;

public class BM_CanonFodder : BoidMover {
    
    public SO_CanonFodder CanonFodderData;
    
    
    
    protected override void Init() {
        generalBoidData = CanonFodderData;
    }
    
    public override Vector3 CalculateSteering() {
        Vector3 plrPos = GameManager.CurrentPlayer.transform.position;
        if ((plrPos - transform.position).sqrMagnitude <= CanonFodderData.FleeTriggerDistance * CanonFodderData.FleeTriggerDistance) {
            Vector3 steer = BoidSteerer.Flee(transform.position, plrPos, rb.velocity, CanonFodderData);
            steer.y = 0;
            enableAvoidanceTest = false;
            ResetWanderPoint(CanonFodderData.WanderLimitRadius);
            return steer;
        } else {
            enableAvoidanceTest = true;
            StepWanderPoint2D(CanonFodderData);
            return BoidSteerer.Wander(
                transform.position, rb.velocity, wanderPoint,
                CanonFodderData
            );
        }
    }
    
    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return BoidRotator.YawOnly(forward);
    }

}