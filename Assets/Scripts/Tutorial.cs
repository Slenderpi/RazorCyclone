using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tutorial : MonoBehaviour

{
    [SerializeField] PlayerCharacterCtrlr player;

    [SerializeField] bool canVac;
    [SerializeField] bool canCannon;
    [SerializeField] bool infFuel;
    [SerializeField] bool canSpace;

    private bool canTriggerr = true;

    void Start(){
        player = GameManager.CurrentPlayer;
    }

    void OnTriggerEnter(Collider other){
        if(canTriggerr){
            if(other.CompareTag("Player")){
                canTriggerr = false;
                player.spaceInput = canSpace;
                player.vacEnableddd = canVac;
                player.cannonEnabled = canCannon;
                player.NoFuelCost = infFuel;
            }
        }
    }
}
