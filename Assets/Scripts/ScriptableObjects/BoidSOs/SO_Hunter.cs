using UnityEngine;

[CreateAssetMenu(fileName = "HunterBoidData", menuName = "ScriptableObjects/Boids/HunterBoid", order = 5)]
public class SO_Hunter : GeneralBoidSO {
    
    [Header("Hunter Boid parameters")]
    [Tooltip("Max steering velocity specifically for when running away.")]
    public float RunAwayMaxSteerVelocity = 10;
    [Tooltip("Max steering force specifically for when running away.")]
    public float RunAwayMaxSteerForce = 10;
    [Tooltip("After passing the player, how long should the Hunter flee for?")]
    public float RunAwayDuration = 0.5f;
    [Tooltip("The Hunter must have at least this much speed before checking if they've passed the player.")]
    public float RunAwayRequiredSpeed = 1f;
    [Tooltip("The Hunter must be within this distance to the player before checking if they've passed the player.")]
    public float RunAwayRequiredDist = 3;
    [Tooltip("If disabled, the Hunter will not include Wander.")]
    public bool IncludeWander = false;
    [Tooltip("When the Hunter is GREATER than this distance, enable wander.")]
    public float WanderTriggerDist = 7;
    [Tooltip("If the Hunter is within this distance (inclusive) to the player, obstacle avoidance will be disabled to make sure they actually hit the player.")]
    public float AvoidanceDisableDist = 5;
    
}