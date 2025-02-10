using UnityEngine;

public class BM_Laser : BoidMover {
    
    public GeneralBoidSO LaserData;
    
    

    protected override void Init() {
        generalBoidData = LaserData;
    }
    
    public override Vector3 CalculateSteering() {
        // TODO
        return Vector3.zero;
    }
    
    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return BoidRotator.YawOnly(forward);
    }
    
}