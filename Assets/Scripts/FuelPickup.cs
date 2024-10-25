using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            PlayerCharacterCtrlr player = GameManager.CurrentPlayer;
            if (player != null) 
            {
                player.AddFuel(1);
                Destroy(gameObject);
            }
        }
    }
}
