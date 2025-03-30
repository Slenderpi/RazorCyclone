using UnityEngine;

[CreateAssetMenu(fileName = "CentMissEnemyData", menuName = "ScriptableObjects/Enemies/CentipedeMissileEnemy", order = 10)]
public class CentMissileEnemySO : ScriptableObject {
    
    [Header("Missile Config")]
    [Tooltip("Determines the amount of seconds this missile can exist before it automatically explodes.")]
    public float MaximumMissileLifetime = 10;
    [Tooltip("When a missile is spawned in, it will start off without tracking and just burst forward, because that looks cool. This is the 'arming' phase.\nThe missile is still able to collide during this phase.\n\nThis variable determines how long arming lasts.")]
    public float ArmingTime = 0.5f;
    [Tooltip("Arming-phase burst strength.")]
    public float ArmingBoost = 50;
    [Tooltip("Arming-phase drag.\nBy having high drag and high boost, the missile looks like it bursts out quickly, gets weighed down by gravity, and then turns on its boosters to track the player.")]
    public float ArmingDrag = 10;
    [Tooltip("Prefab of explosion to use when the missile explodes.")]
    public GameObject ExplosionEffectPrefab;
    
}