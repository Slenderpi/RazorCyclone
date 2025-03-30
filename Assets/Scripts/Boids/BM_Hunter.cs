using UnityEngine;

public class BM_Hunter : BoidMover {
    
    public SO_Hunter HunterData;
    
    float lastRunawayTime = -1000;
    
    
    
    protected override void Init() {
        generalBoidData = HunterData;
    }
    
    public override Vector3 CalculateSteering() {
        Vector3 ret = Vector3.zero;
        Transform plrTrans = GameManager.CurrentPlayer.transform;
        Vector3 toPlayer = plrTrans.position - transform.position;
        // Flee if still fleeing
        if (Time.fixedTime - lastRunawayTime <= HunterData.RunAwayDuration) {
            ret += hunterFlee(plrTrans.position);
        } else {
            float toPlrMag = toPlayer.sqrMagnitude;
            // If greater than WanderTriggerDist, include wander
            if (HunterData.IncludeWander && toPlrMag > HunterData.WanderTriggerDist * HunterData.WanderTriggerDist) {
                StepWanderPoint3D(HunterData);
                ret += BoidSteerer.Wander(transform.position, rb.velocity, wanderPoint, HunterData);
            }
            // If Hunter passed player, set lastRunawayTime and flee
            if (rb.velocity.sqrMagnitude > HunterData.RunAwayRequiredSpeed * HunterData.RunAwayRequiredSpeed &&
                toPlrMag <= HunterData.RunAwayRequiredDist * HunterData.RunAwayRequiredDist &&
                Vector3.Dot(toPlayer, rb.velocity) <= 0) {
                lastRunawayTime = Time.fixedTime;
                ret += hunterFlee(plrTrans.position);
            } else {
                enableAvoidanceTest = toPlrMag > HunterData.AvoidanceDisableDist * HunterData.AvoidanceDisableDist;
                ret += BoidSteerer.Seek(
                    transform.position,
                    plrTrans.position,
                    rb.velocity,
                    HunterData
                );
            }
        }
        return ret;
    }
    
    Vector3 hunterFlee(Vector3 playerPos) {
        return BoidSteerer.Flee(
            transform.position, playerPos, rb.velocity,
            HunterData.RunAwayMaxSteerVelocity, HunterData.RunAwayMaxSteerForce
        );
        // return BoidSteer.Evade(transform.position, playerPos, rb.velocity, GameManager.CurrentPlayer.rb.velocity, HunterData);
    }
    
    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return BoidRotator.Airplane(forward, steer);
    }
    
}