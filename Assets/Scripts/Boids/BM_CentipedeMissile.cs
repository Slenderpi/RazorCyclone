using UnityEngine;

public class BM_CentipedeMissile : BoidMover {
    
    public SO_CentipedeMissile centMissileData;
    
    Vector3 finalTrackingDirection = Vector3.zero;
    float aimTime;
    
    
    
    protected override void Init() {
        generalBoidData = centMissileData;
    }

    void Start() {
        aimTime = Time.fixedTime;
    }

    public override Quaternion CalculateRotation(Vector3 forward, Vector3 steer) {
        return Quaternion.LookRotation(forward);
    }
    
    public override Vector3 CalculateSteering() {
        if (Time.fixedTime - aimTime > centMissileData.AimDuration) {
            // return BoidSteerer.Seek(
            //     transform.position,
            //     finalTrackingDirection,
            //     rb.velocity,
            //     centMissileData
            // );
            return Vector3.ClampMagnitude(
                finalTrackingDirection.normalized * centMissileData.MaxSteeringVelocity - rb.velocity, centMissileData.MaxSteeringForce
            );
        } else {
            finalTrackingDirection = BoidSteerer.PredictPosition(
                transform.position,
                GameManager.CurrentPlayer.transform.position,
                rb.velocity,
                GameManager.CurrentPlayer.rb.velocity
            ) - transform.position;
            return BoidSteerer.Pursuit(
                transform.position,
                GameManager.CurrentPlayer.transform.position,
                rb.velocity,
                GameManager.CurrentPlayer.rb.velocity,
                centMissileData
            );
        }
    }
    
}