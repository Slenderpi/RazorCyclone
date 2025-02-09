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
    
}