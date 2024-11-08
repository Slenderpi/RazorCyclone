using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionNotifier : MonoBehaviour {
    
    /// <summary>
    /// Provides Collision information on collision enter.
    /// </summary>
    public event Action<Collision> A_CollisionEntered;
    /// <summary>
    /// Provides Collision information on collision exit.
    /// </summary>
    public event Action<Collision> A_CollisionExited;
    
    void OnCollisionEnter(Collision collision) {
        A_CollisionEntered?.Invoke(collision);
    }
    
    void OnCollisionExit(Collision collision) {
        A_CollisionExited?.Invoke(collision);
    }
    
}