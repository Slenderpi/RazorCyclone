using UnityEngine;

[CreateAssetMenu(fileName = "CanonFodderBoidData", menuName = "ScriptableObjects/CanonFodderBoid", order = 4)]
public class SO_CanonFodder : GeneralBoidSO {
    
    [Header("Canon Fodder Boid parameters")]
    [Tooltip("If the distance to the player is <= this distance, flee.")]
    public float FleeTriggerDistance = 10;
    
}