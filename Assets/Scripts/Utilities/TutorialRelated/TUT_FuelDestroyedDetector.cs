using UnityEngine;

public class TUT_FuelDestroyedDetector : MonoBehaviour {
    
    [HideInInspector]
    public TUT_FuelSpawner owningSpawner;
    
    
    
    void OnDestroy() {
        if (owningSpawner)
            owningSpawner.DelaySpawnFuel();
    }
    
}