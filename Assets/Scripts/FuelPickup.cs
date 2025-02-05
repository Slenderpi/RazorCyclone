using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPickup : MonoBehaviour {
    
    [HideInInspector]
    public float FuelValue = 100;
    
    public float AnimPeakHeight = 0.05f;
    [Tooltip("Time it takes to animate from low to high to low.")]
    public float AnimCycleDuration = 2;
    [Tooltip("In deg per sec.")]
    public float SpinRate = 180;
    
    [Header("References")]
    [SerializeField]
    Transform ModelPivot;
    
    
    
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))  {
            PlayerCharacterCtrlr player = GameManager.CurrentPlayer;
            if (player != null)  {
                player.AddFuel(FuelValue);
                Destroy(gameObject);
            }
        }
    }
    
    void Update() {
        Vector3 newPos = ModelPivot.localPosition;
        float t = Time.time % AnimCycleDuration / AnimCycleDuration * 2;
        float x = t <= 1 ? t : 2 - t;
        newPos.y = Mathf.Lerp(-AnimPeakHeight, AnimPeakHeight, 3 * x * x - 2 * x * x * x);
        ModelPivot.localPosition = newPos;
        ModelPivot.Rotate(new(0, SpinRate * Time.deltaTime, 0));
    }
    
}
