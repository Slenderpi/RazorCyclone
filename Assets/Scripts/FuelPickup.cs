using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPickup : MonoBehaviour {
    
    [HideInInspector]
    public float FuelValue = 100;
    
    
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))  {
            PlayerCharacterCtrlr player = GameManager.CurrentPlayer;
            if (player != null)  {
                player.AddFuel(FuelValue);
                Destroy(gameObject);
            }
        }
    }
}
