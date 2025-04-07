using UnityEngine;

[CreateAssetMenu(fileName = "CrabBasicEnemyData", menuName = "ScriptableObjects/Enemies/CrabBasicEnemy", order = 5)]
public class CrabBasicEnemySO : ScriptableObject {
    
    [Header("Crab Enemy Config")]
    [Tooltip("Prefab for the projectile the basic crab will fire.")]
    public EnemyProjectile CrabProjectilePrefab;
    [Tooltip("Speed of the projectile.")]
    public float ProjectileSpeed = 25f;
    [Tooltip("Max rotation speed for the body of the crab.")]
    public float MaxBodyRotPerSec = 45;
    
}