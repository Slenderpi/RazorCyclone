using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// This component notifies subscribers of its trigger events
[RequireComponent(typeof(Collider))]
public class TriggerNotifier : MonoBehaviour {
    
    public delegate void TriggerEvent(Collider collider);
    public event TriggerEvent TriggerEnterEvent;
    public event TriggerEvent TriggerExitEvent;
    
    void OnTriggerEnter(Collider collider) {
        TriggerEnterEvent?.Invoke(collider);
    }
    
    void OnTriggerExit(Collider collider) {
        TriggerExitEvent?.Invoke(collider);
    }
    
}