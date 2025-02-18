using UnityEngine;

public class EffectInstance : MonoBehaviour {
    
    public float Lifetime = 5f;
    
    void Start() {
        Destroy(gameObject, Lifetime);
    }
    
}