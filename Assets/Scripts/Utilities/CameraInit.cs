using UnityEngine;

public class CameraInit : MonoBehaviour {
    
    void Awake() {
        if (GameManager.Instance)
            GetComponent<Camera>().fieldOfView = GameManager.Instance.CurrentFOV;
    }
    
}