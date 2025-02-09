using UnityEngine;

[CreateAssetMenu(fileName = "HunterBoidData", menuName = "ScriptableObjects/HunterBoid", order = 4)]
public class SO_Hunter : GeneralBoidSO {
    
    [Header("Hunter Boid parameters")]
    [Tooltip("If disabled, the Hunter will not include Wander.")]
    public bool IncludeWander = false;
    [Tooltip("When the Hunter is GREATER than this distance, enable wander.")]
    public float WanderTriggerDist = 7;
    [Tooltip("After passing the player, how long should the Hunter flee for?")]
    public float RunawayDuration = 0.5f;
    [Tooltip("The Hunter must have at least this much speed before checking if they've passed the player.")]
    public float RunawayRequiredSpeed = 1f;
    [Tooltip("The Hunter must be within this distance to the player before checking if they've passed the player.")]
    public float RunawayRequiredDist = 3;
    
}