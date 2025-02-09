using UnityEngine;

public class BM_Hunter : BoidMover {
    
    public SO_Hunter HunterData;
    
    float lastRunawayTime = -1000;
    
    
    
    public override Vector3 CalculateSteering() {
        Vector3 ret = Vector3.zero;
        Transform plrTrans = GameManager.CurrentPlayer.transform;
        Vector3 toPlayer = plrTrans.position - transform.position;
        // Flee if still fleeing
        if (Time.fixedTime - lastRunawayTime <= HunterData.RunawayDuration) {
            ret += hunterFlee(plrTrans.position);
        } else {
            float toPlrMag = toPlayer.sqrMagnitude;
            // If greater than WanderTriggerDist, include wander
            if (HunterData.IncludeWander && toPlrMag > HunterData.WanderTriggerDist * HunterData.WanderTriggerDist) {
                StepWanderPoint();
                ret += BoidSteer.Wander(transform.position, rb.velocity, wanderPoint, HunterData);
            }
            // If Hunter passed player, set lastRunawayTime and flee
            if (rb.velocity.sqrMagnitude > HunterData.RunawayRequiredSpeed * HunterData.RunawayRequiredSpeed &&
                toPlrMag <= HunterData.RunawayRequiredDist * HunterData.RunawayRequiredDist &&
                Vector3.Dot(toPlayer, rb.velocity) <= 0) {
                // print("FLEEING");
                lastRunawayTime = Time.fixedTime;
                ret += hunterFlee(plrTrans.position);
            } else {
                ret += BoidSteer.Seek(
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
        return BoidSteer.Flee(transform.position, playerPos, rb.velocity, HunterData);
        // return BoidSteer.Evade(transform.position, playerPos, rb.velocity, GameManager.CurrentPlayer.rb.velocity, HunterData);
    }

    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return BoidRotator.Airplane(forward, steer);
    }

    public override void StepWanderPoint() {
        if (Time.fixedTime - lastWanderStepTime > HunterData.WanderMinimumDelay) {
            lastWanderStepTime = Time.fixedTime;
            wanderPoint = BoidSteer.StepWanderPoint3D(wanderPoint, HunterData);
        }
    }
}