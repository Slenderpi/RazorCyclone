using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for Boid steering functions.
/// </summary>
public class BoidSteer {
    
    public static Vector3 Seek(Vector3 pos, Vector3 targetPos, Vector3 velocity, float maxSteeringVelocity, float maxSteeringForce) {
        // Find desired velocity via |targ - pos| * maxSteerVel
        // Steer force is then desired - currVel, clamped by maxSteerForce
        return Vector3.ClampMagnitude((targetPos - pos).normalized * maxSteeringVelocity - velocity, maxSteeringForce);
    }
    
    public static Vector3 Seek(Vector3 pos, Vector3 targetPos, Vector3 velocity, GeneralBoidSO boidData) {
        return Seek(pos, targetPos, velocity, boidData.MaxSteeringVelocity, boidData.MaxSteeringForce);
    }
    
    public static Vector3 Flee(Vector3 pos, Vector3 targetPos, Vector3 velocity, float maxSteeringVelocity, float maxSteeringForce) {
        return -Seek(pos, targetPos, velocity, maxSteeringVelocity, maxSteeringForce);
    }
    
    public static Vector3 Flee(Vector3 pos, Vector3 targetPos, Vector3 velocity, GeneralBoidSO boidData) {
        return Flee(pos, targetPos, velocity, boidData.MaxSteeringVelocity, boidData.MaxSteeringForce);
    }
    
    public static Vector3 Pursuit(Vector3 pos, Vector3 targetPos, Vector3 velocity, Vector3 targetVel, float maxSteeringVelocity, float maxSteeringForce) {
        float predictTime = calculatePredictTime(pos, targetPos, velocity, targetVel);
        return Seek(pos + velocity * predictTime, targetPos + targetVel * predictTime, velocity, maxSteeringVelocity, maxSteeringForce)
        + Seek(pos, targetPos, velocity, maxSteeringVelocity, maxSteeringForce);
        // float predictTime = Mathf.Sqrt((targetPos - transform.position).sqrMagnitude / (rb.velocity - targetVel).sqrMagnitude);
        // return Seek(targetPos + targetVel * predictTime);
    }
    
    public static Vector3 Pursuit(Vector3 pos, Vector3 targetPos, Vector3 velocity, Vector3 targetVel, GeneralBoidSO boidData) {
        return Pursuit(pos, targetPos, velocity, targetVel, boidData.MaxSteeringVelocity, boidData.MaxSteeringForce);
    }
    
    public static Vector3 Evade(Vector3 pos, Vector3 targetPos, Vector3 velocity, Vector3 targetVel, float maxSteeringVelocity, float maxSteeringForce) {
        // TODO
        // Debug.LogWarning("Evade not yet implemented.");
        float predictTime = calculatePredictTime(pos, targetPos, velocity, targetVel);
        return Flee(pos + velocity * predictTime, targetPos + targetVel * predictTime, velocity, maxSteeringVelocity, maxSteeringForce)
        + Flee(pos, targetPos, velocity, maxSteeringVelocity, maxSteeringForce);
    }
    
    public static Vector3 Evade(Vector3 pos, Vector3 targetPos, Vector3 velocity, Vector3 targetVel, GeneralBoidSO boidData) {
        return Evade(pos, targetPos, velocity, targetVel, boidData.MaxSteeringVelocity, boidData.MaxSteeringForce);
    }
    
    static float calculatePredictTime(Vector3 pos, Vector3 targetPos, Vector3 velocity, Vector3 targetVel) {
        return Mathf.Sqrt((targetPos - pos).sqrMagnitude / (velocity - targetVel).sqrMagnitude);
    }
    
    /// <summary>
    /// This function DOES NOT call a StepWander() function. You must call it first.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="velocity"></param>
    /// <param name="wanderPoint">Point to wander to AFTER you've ALREADY stepped it.</param>
    /// <param name="wanderLimitRadius"></param>
    /// <param name="wanderLimitDist"></param>
    /// <returns></returns>
    public static Vector3 Wander(Vector3 pos, Vector3 velocity, Vector3 wanderPoint, float wanderLimitDist, float maxSteeringVelocity, float maxSteeringForce) {
        return Seek(pos, pos + wanderLimitDist * velocity.normalized + wanderPoint, velocity, maxSteeringVelocity, maxSteeringForce);
    }
    
    public static Vector3 Wander(Vector3 pos, Vector3 velocity, Vector3 wanderPoint, GeneralBoidSO boidData) {
        return Wander(
            pos, velocity, wanderPoint,
            boidData.WanderLimitDist,
            boidData.MaxSteeringVelocity,
            boidData.MaxSteeringForce
        );
    }
    
    public static Vector3 StepWanderPoint2D(Vector3 wanderPoint, float wanderLimitRadius, float wanderChangeDist) {
        wanderPoint.x += (UnityEngine.Random.Range(0, 2) * 2 - 1) * wanderChangeDist;
        wanderPoint.z += (UnityEngine.Random.Range(0, 2) * 2 - 1) * wanderChangeDist;
        return wanderPoint * wanderLimitRadius / wanderPoint.magnitude;
    }
    
    public static Vector3 StepWanderPoint2D(Vector3 wanderPoint, GeneralBoidSO boidData) {
        return StepWanderPoint2D(wanderPoint, boidData.WanderLimitRadius, boidData.WanderChangeDist);
    }
    
    public static Vector3 StepWanderPoint3D(Vector3 wanderPoint, float wanderLimitRadius, float wanderChangeDist) {
        // Choose random 1 or -1 for x, y, and z separately
        // Scale to WanderChangeDist
        // Add to lastWanderPoint
        // Limit lastWanderPoint to WanderLimitRadius
        wanderPoint.x += (UnityEngine.Random.Range(0, 2) * 2 - 1) * wanderChangeDist;
        wanderPoint.z += (UnityEngine.Random.Range(0, 2) * 2 - 1) * wanderChangeDist;
        wanderPoint.y += (UnityEngine.Random.Range(0, 2) * 2 - 1) * wanderChangeDist;
        return wanderPoint * wanderLimitRadius / wanderPoint.magnitude;
    }
    
    public static Vector3 StepWanderPoint3D(Vector3 wanderPoint, GeneralBoidSO boidData) {
        return StepWanderPoint3D(wanderPoint, boidData.WanderLimitRadius, boidData.WanderChangeDist);
    }
    
}