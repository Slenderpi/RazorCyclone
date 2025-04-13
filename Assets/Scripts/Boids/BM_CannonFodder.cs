using UnityEngine;

public class BM_CannonFodder : BoidMover {
    
    public SO_CannonFodder CannonFodderData;
    
    
    
    protected override void Init() {
        generalBoidData = CannonFodderData;
    }
    
    public override Vector3 CalculateSteering() {
        Vector3 plrPos = GameManager.CurrentPlayer.transform.position;
        if ((plrPos - transform.position).sqrMagnitude <= CannonFodderData.FleeTriggerDistance * CannonFodderData.FleeTriggerDistance) {
            Vector3 steer = BoidSteerer.Flee(transform.position, plrPos, rb.velocity, CannonFodderData);
            steer.y = 0;
            ResetWanderPoint(CannonFodderData.WanderLimitRadius);
            return steer;
        } else {
            StepWanderPoint2D(CannonFodderData);
            return BoidSteerer.Wander(
                transform.position, rb.velocity, wanderPoint,
                CannonFodderData
            );
        }
    }
    
    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return BoidRotator.YawOnly(forward);
    }
    
}