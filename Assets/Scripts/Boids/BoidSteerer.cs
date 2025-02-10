// UNCOMMENT THE BELOW LINE TO DRAW DEBUG RAYS
#define DEBUG_RAYS

using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Class for Boid steering functions.
/// </summary>
public class BoidSteerer {
    
    static LayerMask AVOIDANCE_LAYER_MASK = (1 << LayerMask.NameToLayer("Default")); // | (1 << LayerMask.NameToLayer("Enemy")));
    
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
        return Seek(pos + velocity * predictTime, targetPos + targetVel * predictTime, velocity, maxSteeringVelocity, maxSteeringForce) +
               Seek(pos, targetPos, velocity, maxSteeringVelocity, maxSteeringForce);
        // float predictTime = Mathf.Sqrt((targetPos - transform.position).sqrMagnitude / (rb.velocity - targetVel).sqrMagnitude);
        // return Seek(targetPos + targetVel * predictTime);
    }
    
    public static Vector3 Pursuit(Vector3 pos, Vector3 targetPos, Vector3 velocity, Vector3 targetVel, GeneralBoidSO boidData) {
        return Pursuit(pos, targetPos, velocity, targetVel, boidData.MaxSteeringVelocity, boidData.MaxSteeringForce);
    }
    
    public static Vector3 Evade(Vector3 pos, Vector3 targetPos, Vector3 velocity, Vector3 targetVel, float maxSteeringVelocity, float maxSteeringForce) {
        float predictTime = calculatePredictTime(pos, targetPos, velocity, targetVel);
        return Flee(pos + velocity * predictTime, targetPos + targetVel * predictTime, velocity, maxSteeringVelocity, maxSteeringForce) +
               Flee(pos, targetPos, velocity, maxSteeringVelocity, maxSteeringForce);
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
    
    /// <summary>
    /// Calculates an avoidance force. Assumes velocity to be the current facing direction.
    /// <br/><br/>
    /// Uses one central raycast.
    /// </summary>
    public static Vector3 Avoidance1P(Vector3 pos, Vector3 velocity, float lookDist, float minAvoidIntensity, float maxAvoidIntensity, float maxSteeringForce) {
        return Vector3.ClampMagnitude(calcAvoid(pos, velocity, lookDist, minAvoidIntensity, maxAvoidIntensity), maxSteeringForce);
    }
    
    /// <summary>
    /// Calculates an avoidance force. Assumes velocity to be the current facing direction.
    /// <br/><br/>
    /// Uses one central raycast.
    /// </summary>
    public static Vector3 Avoidance1P(Vector3 pos, Vector3 velocity, GeneralBoidSO boidData) {
        return Avoidance1P(
            pos, velocity,
            boidData.AvoidanceMaxLookDist, boidData.AvoidanceMinIntensity, boidData.AvoidanceMaxIntensity, boidData.AvoidanceMaxSteeringForce
        );
    }
    
    /// <summary>
    /// Calculates an avoidance force. Assumes velocity to be the current facing direction.<br/>
    /// Uses a velocity with y of 0 before doing a distance check.
    /// <br/><br/>
    /// Uses one central raycast.
    /// </summary>
    public static Vector3 Avoidance1PFlat(Vector3 pos, Vector3 velocity, float lookDist, float minAvoidIntensity, float maxAvoidIntensity, float maxSteeringForce) {
        velocity.y = 0;
        return Avoidance1P(
            pos, velocity,
            lookDist, minAvoidIntensity, maxAvoidIntensity, maxSteeringForce
        );
    }
    
    /// <summary>
    /// Calculates an avoidance force. Assumes velocity to be the current facing direction.<br/>
    /// Uses a velocity with y of 0 before doing a distance check.
    /// <br/><br/>
    /// Uses one central raycast.
    /// </summary>
    public static Vector3 Avoidance1PFlat(Vector3 pos, Vector3 velocity, GeneralBoidSO boidData) {
        velocity.y = 0;
        return Avoidance1P(
            pos, velocity,
            boidData.AvoidanceMaxLookDist, boidData.AvoidanceMinIntensity, boidData.AvoidanceMaxIntensity, boidData.AvoidanceMaxSteeringForce
        );
    }
    
    /// <summary>
    /// Avoidance using two additional whiskers.<br/>
    /// All whiskers (forward, right, left) are locked in the x-z plane.
    /// </summary>
    public static Vector3 Avoidance3PFlat(Vector3 pos, Vector3 velocity, float angleDeg, float lookDist, float minAvoidIntensity, float maxAvoidIntensity, float maxSteeringForce) {
        velocity.y = 0;
        float speed = velocity.magnitude;
        Vector3 forward = velocity / speed;
        Vector3 right = -Vector3.Cross(forward, Vector3.up); // ??? Why tf is this backward
        Vector3 velFactor = forward * Mathf.Cos(angleDeg * Mathf.Deg2Rad);
        Vector3 rightFactor = right * Mathf.Sin(angleDeg * Mathf.Deg2Rad);
        return Vector3.ClampMagnitude(
            calcAvoid(pos, velocity, lookDist, minAvoidIntensity, maxAvoidIntensity) + // Center whisker
            calcAvoid(pos, (velFactor + rightFactor) * speed, lookDist, minAvoidIntensity, maxAvoidIntensity) + // Right whisker
            calcAvoid(pos, (velFactor - rightFactor) * speed, lookDist, minAvoidIntensity, maxAvoidIntensity), // Left whisker
            maxSteeringForce
        );
    }
    
    /// <summary>
    /// Avoidance using two additional whiskers.<br/>
    /// All whiskers (forward, right, left) are locked in the x-z plane.
    /// </summary>
    public static Vector3 Avoidance3PFlat(Vector3 pos, Vector3 velocity, GeneralBoidSO boidData) {
        return Avoidance3PFlat(
            pos, velocity, boidData.AvoidanceWhiskerAngle,
            boidData.AvoidanceMaxLookDist, boidData.AvoidanceMinIntensity, boidData.AvoidanceMaxIntensity, boidData.AvoidanceMaxSteeringForce
        );
    }
    
    /// <summary>
    /// Avoidance using two additional whiskers.<br/>
    /// Rotates the right and left whiskers based on a given rotation.
    /// </summary>
    public static Vector3 Avoidance3P3D(Vector3 pos, Vector3 velocity, Quaternion rotation, float angleDeg, float lookDist, float minAvoidIntensity, float maxAvoidIntensity, float maxSteeringForce) {
        float speed = velocity.magnitude;
        Vector3 forward = velocity / speed;
        // The below commented-out code may be faster by a little bit, but is not as cool as weird quaternion math
        // Vector3 right = (rotation * Vector3.right).normalized;
        // Vector3 velFactor = forward * Mathf.Cos(angleDeg * Mathf.Deg2Rad);
        // Vector3 rightFactor = right * Mathf.Sin(angleDeg * Mathf.Deg2Rad);
        Vector3 rightVect = Quaternion.AngleAxis(angleDeg, rotation * Vector3.up) * forward;
        Vector3 leftVect = Vector3.Reflect(-rightVect, forward); // Quaternion.AngleAxis(angleDeg, Vector3.up) * forward;
        return Vector3.ClampMagnitude(
            calcAvoid(pos, velocity, lookDist, minAvoidIntensity, maxAvoidIntensity) + // Center whisker
            // calcAvoid(pos, (velFactor + rightFactor) * speed, lookDist, minAvoidIntensity, maxAvoidIntensity) + // Right whisker
            // calcAvoid(pos, (velFactor - rightFactor) * speed, lookDist, minAvoidIntensity, maxAvoidIntensity), // Left whisker
            calcAvoid(pos, rightVect * speed, lookDist, minAvoidIntensity, maxAvoidIntensity) + // Right whisker
            calcAvoid(pos, leftVect * speed, lookDist, minAvoidIntensity, maxAvoidIntensity), // Left whisker
            maxSteeringForce
        );
    }
    
    /// <summary>
    /// Avoidance using two additional whiskers.<br/>
    /// Rotates the right and left whiskers based on a given rotation.
    /// </summary>
    public static Vector3 Avoidance3P3D(Vector3 pos, Vector3 velocity, Quaternion rotation, GeneralBoidSO boidData) {
        return Avoidance3P3D(
            pos, velocity, rotation, boidData.AvoidanceWhiskerAngle,
            boidData.AvoidanceMaxLookDist, boidData.AvoidanceMinIntensity, boidData.AvoidanceMaxIntensity, boidData.AvoidanceMaxSteeringForce
        );
    }
    
    /// <summary>
    /// 
    /// </summary>
    public static Vector3 Avoidance5P(Vector3 pos, Vector3 velocity, Quaternion rotation, float angleDeg, float lookDist, float minAvoidIntensity, float maxAvoidIntensity, float maxSteeringForce) {
        
        return Vector3.zero;
        // float speed = velocity.magnitude;
        // Vector3 forward = velocity / speed;
        // Vector3 right = -Vector3.Cross(forward, Vector3.up); // ??? Why tf is this backward
        // Vector3 velFactor = forward * Mathf.Cos(angleDeg * Mathf.Deg2Rad);
        // Vector3 rightFactor = right * Mathf.Sin(angleDeg * Mathf.Deg2Rad);
        // return Vector3.ClampMagnitude(
        //     calcAvoid(pos, velocity, lookDist, minAvoidIntensity, maxAvoidIntensity) + // Center whisker
        //     calcAvoid(pos, (velFactor + rightFactor) * speed, lookDist, minAvoidIntensity, maxAvoidIntensity) + // Right whisker
        //     calcAvoid(pos, (velFactor - rightFactor) * speed, lookDist, minAvoidIntensity, maxAvoidIntensity), // Left whisker
        //     maxSteeringForce
        // );
    }
    
    /// <summary>
    /// 
    /// </summary>
    public static Vector3 Avoidance5P(Vector3 pos, Vector3 velocity, Quaternion rotation, GeneralBoidSO boidData) {
        return Avoidance5P(
            pos, velocity, rotation, boidData.AvoidanceWhiskerAngle,
            boidData.AvoidanceMaxLookDist, boidData.AvoidanceMinIntensity, boidData.AvoidanceMaxIntensity, boidData.MaxSteeringForce
        );
    }
    
    static Vector3 calcAvoid(Vector3 pos, Vector3 velocity, float lookDist, float minAvoidIntensity, float maxAvoidIntensity) {
        Ray ray = new(pos, velocity);
        if (Physics.Raycast(ray: ray, maxDistance: lookDist, layerMask: AVOIDANCE_LAYER_MASK, hitInfo: out RaycastHit hit)) {
            Vector3 reflect = Vector3.Reflect(velocity, hit.normal);
#if UNITY_EDITOR && DEBUG_RAYS
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.black, Time.fixedDeltaTime, false); // raycast
            Debug.DrawRay(hit.point, hit.normal, Color.red, Time.fixedDeltaTime, false); // normal
            Debug.DrawRay(hit.point, reflect, Color.cyan, Time.fixedDeltaTime, false); // reflect
            Vector3 s = hit.normal * distIntensifier(hit.distance, lookDist, minAvoidIntensity, maxAvoidIntensity) + reflect;
            GameManager.D_DrawPoint(s, Color.green); // targ
            Debug.DrawRay(pos, s, Color.green, Time.fixedDeltaTime, false); // seek steering
#endif
            return hit.normal * distIntensifier(hit.distance, lookDist, minAvoidIntensity, maxAvoidIntensity) + reflect;
        }
#if UNITY_EDITOR && DEBUG_RAYS
         else {
            Debug.DrawRay(ray.origin, ray.direction * lookDist, Color.white, Time.fixedDeltaTime, false);
        }
#endif
        return Vector3.zero;
    }
    
    static float distIntensifier(float inDist, float lookDist, float minIntensity, float maxIntensity) {
        return maxIntensity + (minIntensity - maxIntensity) * inDist / lookDist; // linear, min minIntensity
        // return (1 - inDist / lookDist) * maxIntensity; // linear, min 0
        // return (inDist - maxIntensity * inDist + maxIntensity * lookDist) / lookDist; // linear, min 1
        // float xd = inDist / lookDist;
        // float a1 = maxIntensity - 1;
        // return a1 * xd * xd + 2 * -a1 * xd + maxIntensity;
    }
    
}