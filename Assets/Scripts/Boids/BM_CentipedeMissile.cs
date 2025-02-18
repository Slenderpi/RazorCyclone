using UnityEngine;

public class BM_CentipedeMissile : BoidMover {
    
    public SO_CentipedeMissile centMissileData;
    
    
    
    protected override void Init() {
        generalBoidData = centMissileData;
    }
    
    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return Quaternion.LookRotation(forward);
    }
    
    public override Vector3 CalculateSteering() {
        Vector3 steer;
        if (centMissileData.UseSeek) {
            steer = BoidSteerer.Seek(
                transform.position,
                GameManager.CurrentPlayer.transform.position,
                rb.velocity,
                centMissileData
            );
        } else {
            steer = BoidSteerer.Pursuit(
                transform.position,
                GameManager.CurrentPlayer.transform.position,
                rb.velocity,
                GameManager.CurrentPlayer.rb.velocity,
                centMissileData
            );
        }
        return steer;
    }
    
}