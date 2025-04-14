using UnityEngine;

[CreateAssetMenu(fileName = "GeneralEnemyData", menuName = "ScriptableObjects/Enemies/GeneralEnemy", order = 1)]
public class GeneralEnemySO : ScriptableObject {
    
    [Header("Consistent")]
    public FuelPickup FuelPickupPrefab;
    public GameObject EnemyDeathVFX;
    
    [Header("Enemy Configuration")]
    public float Damage = 10;
    [Tooltip("A cooldown for attacking the player")]
    public float AttackDelay = 1;
    public bool DealDamageOnTouch = true;
    // [Tooltip("Determines if this enemy can be killed when touched by the vacuum's killbox.\n\nNote: certain enemies (e.g. Hunter) will, under special cases, ignore this value.")]
    // public readonly bool CanGetVacuumKilled = true;
    [Tooltip("If enabled, this enemy will call its OnSubmerged() method when it detects that it is below lava.")]
    public bool AffectedByLava = true;
    [Tooltip("This enemy will be affected by lava if its y position + HeightOffset is below the lava. This is to allow objects to sink lower before actually being counted as submerged.")]
    public float LavaSubmergeOffset = 1;
    [Tooltip("When an enemy is defeated, they will remain in the world for a duration so that death effects can be shown. This value determines how long that lasts until the the enemy gameObject gets destroyed.")]
    public float DestroyDelay = 5;
    
}