using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This component notifies subscribers of its trigger events
[RequireComponent(typeof(Collider))]
public class TriggerNotifier : MonoBehaviour {
    
    /// <summary>
    /// Provides the Collider that triggered this notifier.
    /// </summary>
    public event Action<Collider> A_TriggerEntered;
    /// <summary>
    /// Provides the Collider that exited this trigger notifier.
    /// </summary>
    public event Action<Collider> A_TriggerExited;
    
    void OnTriggerEnter(Collider collider) {
        A_TriggerEntered?.Invoke(collider);
    }
    
    void OnTriggerExit(Collider collider) {
        A_TriggerExited?.Invoke(collider);
    }
    
}