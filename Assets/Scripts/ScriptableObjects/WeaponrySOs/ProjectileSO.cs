using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "ScriptableObjects/Weaponry/Projectile", order = 1)]
public class ProjectileSO : ScriptableObject {
    
    [Header("Projectile Config")]
    [Tooltip("The radius to use when doing multiple raycasts for collision detection.\n\nNOTE: if set to a value less than 0.1, this projectile will only check directly in front of it once.")]
    public float ProjectileRadius = 0.5f;
    [Tooltip("Maximum lifetime in seconds of this projectile to prevent projectiles that go into the void from living too long.")]
    public float MaxLifetime = 10f;
    [Tooltip("VFX for projectile impact.")]
    public GameObject ImpactEffect;
    [Tooltip("VFX for ricochet.")]
    public GameObject RicochetEffect;
    
}