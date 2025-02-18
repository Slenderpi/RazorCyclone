using UnityEngine;

public class BM_Lava : BoidMover {
    
    public SO_LavaEnemy LavaEnemyData;
    
    
    
    protected override void Init() {
        generalBoidData = LavaEnemyData;
    }
    
    public override Vector3 CalculateSteering() {
        StepWanderPoint2D(LavaEnemyData);
        return BoidSteerer.Wander(
            transform.position, rb.velocity, wanderPoint,
            LavaEnemyData
        );
    }
    
    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return BoidRotator.YawOnly(forward);
    }
    
}