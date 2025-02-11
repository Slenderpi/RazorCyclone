using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BM_WeakPTester : BoidMover {
    
    //
    
    

    protected override void Init() {
        
    }

    public override Vector3 CalculateSteering() {
        return Vector3.zero;
    }
    
    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return Quaternion.identity;
    }
    
}