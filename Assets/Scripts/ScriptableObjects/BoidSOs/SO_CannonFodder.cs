using UnityEngine;

[CreateAssetMenu(fileName = "CannonFodderBoidData", menuName = "ScriptableObjects/Boids/CannonFodderBoid", order = 4)]
public class SO_CannonFodder : GeneralBoidSO {
    
    [Header("Cannon Fodder Boid parameters")]
    [Tooltip("If the distance to the player is <= this distance, flee.")]
    public float FleeTriggerDistance = 10;

    [Tooltip("The force the Cannon Fodder uses to flee from the player.")]
    public float FleeForce = 10f;
}