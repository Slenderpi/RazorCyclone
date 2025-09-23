using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public class PlayerCannonProjectileAuthoring : MonoBehaviour {

    public float ProjectileRadius = 0.5f;

    public GameObject ImpactVFX;
    public GameObject RicochetVFX;
    
    
    
    class Baker : Baker<PlayerCannonProjectileAuthoring> {
        public override void Bake(PlayerCannonProjectileAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerCannonProjectile());
			Projectile proj = new() { Radius = auth.ProjectileRadius };
			proj.SetFilter(
				(1u << LayerMask.NameToLayer("Default")) |
				(1u << LayerMask.NameToLayer("EnemyHitbox"))
			);
			AddComponent(entity, proj);
        }
    }
    
}



public struct PlayerCannonProjectile : IComponentData {
    public float MaxRicochet;
    public float RemainingRicochets;

    //public float LifetimePerRicochetAdd1;
    //public float MaxLifetime;

    Entity lastEntityHitFromRicochet;

    [BurstCompile]
    public void SetMaxRicochets(float maxRicochets) {
        MaxRicochet = maxRicochets;
        RemainingRicochets = maxRicochets;
    }

    /// <summary>
    /// Checks if this new hit entity is the same as before (due to ricochet).<br/>
    /// If it is, return true.<br/>
    /// Otherwise, update the new lastEntityHit and return false.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>True if the same entity was hit</returns>
    [BurstCompile]
    public bool TryOnHitSameEntity(Entity entity) {
        if (entity == lastEntityHitFromRicochet)
            return true;
        lastEntityHitFromRicochet = entity;
        return false;
    }

    //[Header("Projectile Config")]
    //[Tooltip("The radius to use when doing multiple raycasts for collision detection.\n\nNOTE: if set to a value less than 0.1, this projectile will only check directly in front of it once.")]
    //public float ProjectileRadius = 0.5f;
    //[Tooltip("Lifetime in seconds of this projectile scaling with the number of ricochets + 1. This is to prevent projectiles that go into the void from living too long.")]
    //public float LifetimePerRicochetAdd1 = 3.2f;
    //[Tooltip("High ricochet values can lead to high lifetimes. If that happens, the lifetime will instead be capped to this value.")]
    //public float MaxLifetime = 15f;
    //[Tooltip("VFX for projectile impact.")]
    //public GameObject ImpactEffect;
    //[Tooltip("VFX for ricochet.")]
    //public GameObject RicochetEffect;
}