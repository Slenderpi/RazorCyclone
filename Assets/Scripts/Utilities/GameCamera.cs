using UnityEngine;

public class GameCamera : MonoBehaviour {
    
    Camera c;
    SecondOrderDynamicsF sodFOV;
    
    [Header("References")]
    [SerializeField]
    Camera WeaponCamera;
    
    [Header("Parameters")]
    [SerializeField]
    [Range(0.5f, 7)]
    float f = 3;
    [SerializeField]
    [Range(0, 1)]
    float z = 1;
    [SerializeField]
    [Range(-1, 1)]
    float r = 0.2f;
    [SerializeField]
    [Range(0, 100)]
    float MaxAddFOV = 20;
    [SerializeField]
    [Range(10, 90)]
    float SpeedForMaxFOV = 40;
    
    float currFOV = 90;
    
    
    
    void Awake() {
        c = GetComponent<Camera>();
        sodFOV = new SecondOrderDynamicsF(f, z, r, 0);
        if (GameManager.Instance)
            c.fieldOfView = GameManager.Instance.CurrentFOV;
    }
    
    void LateUpdate() {
        if (!GameManager.CurrentPlayer) return;
        if (Time.deltaTime > 0) {
#if UNITY_EDITOR
            sodFOV.SetDynamics(f, z, r);
#endif
            updateFOV(sodFOV.Update(Mathf.Lerp(0, MaxAddFOV, GameManager.CurrentPlayer.rb.velocity.magnitude / SpeedForMaxFOV), Time.deltaTime));
        }
    }
    
    public void SetFOV(float fov) {
        currFOV = fov;
        updateFOV(sodFOV.GetNoUpdate());
    }
    
    void updateFOV(float addFOV) {
        c.fieldOfView = currFOV + addFOV;
        WeaponCamera.fieldOfView = c.fieldOfView;
    }
    
    void OnDestroy() {
        GameManager.Instance.GCam = null;
    }
    
}