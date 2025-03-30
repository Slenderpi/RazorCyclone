using UnityEngine;

public class Suckable : MonoBehaviour {
    
    [Tooltip("Determines if this object allows vacuum forces to be applied on it.\n\nNote: certain enemies (e.g. Hunter) will set this value on their own, and do not need this to be touched.")]
    public bool CanGetVacuumSucked = true;
    
    [HideInInspector]
    public Rigidbody rb;
    
    
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    
}