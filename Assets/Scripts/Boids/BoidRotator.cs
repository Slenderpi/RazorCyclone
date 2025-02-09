using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for Boid rotation functions.
/// </summary>
public class BoidRotator {
    
    public static Quaternion YawOnly(Vector3 forward) {
        forward.y = 0;
        return Quaternion.LookRotation(forward, Vector3.up);
    }
    
    public static Quaternion YawAndPitch(Vector3 forward) {
        return Quaternion.LookRotation(forward, Vector3.up);
    }
    
    public static Quaternion YawAndBank(Vector3 forward, Vector3 steer) {
        forward.y = 0;
        return Airplane(forward, steer);
    }
    
    public static Quaternion Airplane(Vector3 forward, Vector3 steer) {
        // Find cos(theta) where theta is the angle between forward and steer strictly in the x-z plane
        // cos(th) = f . s / (|f||s|) == f . s / sqrt(f.sqrMag * s.sqrMag)
        float c = 1 - (forward.x * steer.x + forward.z * steer.z) / Mathf.Sqrt(
            (forward.x * forward.x + forward.z * forward.z) *
            (steer.x * steer.x + steer.z * steer.z)
        );
        Vector3 right = Vector3.Cross(forward, Vector3.up).normalized;
        // To check leftness/rightness, use dot product of steer and right
        Vector3 up;
        if (Vector3.Dot(steer, right) > 0) {
            up = Vector3.Slerp(Vector3.up, right, c);
        } else {
            up = Vector3.Slerp(Vector3.up, -right, c);
        }
        return Quaternion.LookRotation(forward, up);
    }
    
}