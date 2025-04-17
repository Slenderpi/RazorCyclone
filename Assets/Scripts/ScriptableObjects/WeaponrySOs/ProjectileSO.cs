using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "ScriptableObjects/Weaponry/Projectile", order = 1)]
public class ProjectileSO : ScriptableObject {
    
    [Header("Projectile Config")]
    [Tooltip("The radius to use when doing multiple raycasts for collision detection.\n\nNOTE: if set to a value less than 0.1, this projectile will only check directly in front of it once.")]
    public float ProjectileRadius = 0.5f;
    [Tooltip("Lifetime in seconds of this projectile scaling with the number of ricochets + 1. This is to prevent projectiles that go into the void from living too long.")]
    public float LifetimePerRicochetAdd1 = 3.2f;
    [Tooltip("High ricochet values can lead to high lifetimes. If that happens, the lifetime will instead be capped to this value.")]
    public float MaxLifetime = 15f;
    [Tooltip("VFX for projectile impact.")]
    public GameObject ImpactEffect;
    [Tooltip("VFX for ricochet.")]
    public GameObject RicochetEffect;
    
}