using UnityEngine;

/// <summary>
/// Class for animations using second order dynamics for floats.<br/>
/// <br/>
/// From video "Giving Personality to Procedural Animations using Math":<br/>
/// https://www.youtube.com/watch?v=KPoeNZZ6H4s
/// </summary>
public class SecondOrderDynamicsF {
    
    float k1, k2, k3;
    float px; // Prevoius x
    float y, dy; // Current y, change in y
    
    public SecondOrderDynamicsF(float x0) {
        SetDynamics(1, 0, 0);
        Reset(x0);
    }
    
    public SecondOrderDynamicsF(float f, float z, float r, float x0) {
        SetDynamics(f, z, r);
        Reset(x0);
    }
    
    public float Update(float x, float dt) {
        float dx = (x - px) / dt;
        px = x;
        float k2Stable = Mathf.Max(k2, 1.1f * (dt * dt / 4 + dt * k1 / 2)); // Clamp k2 for stability
        y += dt * dy; // Integrate position by velocity
        dy += dt * (x + k3 * dx - y - k1 * dy) / k2Stable; // Integrate velocity by acceleration
        return y;
    }
    
    public void Reset(float x0) {
        px = x0;
        y = x0;
        dy = 0;
    }
    
    public void SetDynamics(float f, float z, float r) {
        SODUtil.SetDynamics(f, z, r, out k1, out k2, out k3);
        // Debug.Log($"f: {f}; z: {z}; r: {r}\n - k1 = {k1}\n - k2 = {k2}\n - k3 = {k3}");
    }
    
}

/// <summary>
/// Class for animations using second order dynamics for Vector3s.<br/>
/// <br/>
/// From video "Giving Personality to Procedural Animations using Math":<br/>
/// https://www.youtube.com/watch?v=KPoeNZZ6H4s
/// </summary>
public class SecondOrderDynamicsV3 {
    
    float k1, k2, k3;
    Vector3 px; // Prevoius x
    Vector3 y, dy; // Current y, change in y
    
    public SecondOrderDynamicsV3(Vector3 x0) {
        SetDynamics(0, 0, 0);
        px = x0;
        y = x0;
        dy = Vector3.zero;
    }
    
    public SecondOrderDynamicsV3(float f, float z, float r, Vector3 x0) {
        SetDynamics(f, z, r);
        px = x0;
        y = x0;
        dy = Vector3.zero;
    }
    
    public Vector3 Update(Vector3 x, float dt) {
        Vector3 dx = (x - px) / dt;
        px = x;
        float k2Stable = Mathf.Max(k2, 1.1f * (dt * dt / 4 + dt * k1 / 2)); // Clamp k2 for stability
        y += dt * dy; // Integrate position by velocity
        dy += dt * (x + k3 * dx - y - k1 * dy) / k2Stable; // Integrate velocity by acceleration
        return y;
    }
    
    public void SetDynamics(float f, float z, float r) {
        SODUtil.SetDynamics(f, z, r, out k1, out k2, out k3);
    }
    
}

public static class SODUtil {
    
    public static void SetDynamics(float f, float z, float r, out float k1, out float k2, out float k3) {
        float temp = 2 * Mathf.PI * f;
        k1 = z / (Mathf.PI * f);
        k2 = 1 / (temp * temp);
        k3 = r * z / temp;
    }
    
}