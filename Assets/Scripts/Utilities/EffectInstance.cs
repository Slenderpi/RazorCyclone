using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInstance : MonoBehaviour {
    
    ParticleSystem ps;
    public float Lifetime = 5f;
    
    void Start() {
        Destroy(gameObject, Lifetime);
    }
    
}