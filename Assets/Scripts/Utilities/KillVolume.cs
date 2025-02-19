using UnityEngine;

public class KillVolume : MonoBehaviour {
    
    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Player"))
            GameManager.CurrentPlayer.TakeDamage(9999999, EDamageType.Any);
        else
            Debug.LogWarning("KillVolume triggered with a non-player object somehow. Object name: '" + collider.gameObject.name + "'");
    }
    
}