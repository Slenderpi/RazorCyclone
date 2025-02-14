using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BM_Lava : BoidMover {
    
    public SO_LavaEnemy LavaEnemyData;
    
    

    protected override void Init() {
        generalBoidData = LavaEnemyData;
    }

    public override Vector3 CalculateSteering() {
        return Vector3.zero;
    }
    
    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return BoidRotator.YawOnly(forward);
    }
    
}