using Unity.Entities;
using UnityEngine;

public class PlayerCannonAuthoring : MonoBehaviour {

    public float RecoilForce = 8f;
    public float ProjectileSpeed = 125f;

    public float FuelCost = 12f;

    public GameObject CannonProjectilePrefab;
    public GameObject MuzzleFlashVFXPrefab;
    
    
    
    class Baker : Baker<PlayerCannonAuthoring> {
        public override void Bake(PlayerCannonAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerCannon(
                auth.RecoilForce,
                auth.ProjectileSpeed,
                auth.FuelCost,
                GetEntity(auth.CannonProjectilePrefab, TransformUsageFlags.Dynamic),
                GetEntity(auth.MuzzleFlashVFXPrefab, TransformUsageFlags.Dynamic)
            ));
        }
    }
    
}



public struct PlayerCannon : IComponentData {
    public float RecoilForce;
    public float ProjectileSpeed;

    public float FuelCost;

    public Entity ProjectilePrefab;
    public Entity MuzzleFlashVFX;

    public PlayerCannon(
        float recoilForce,
        float projectileSpeed,
        float fuelCost,
        Entity projectilePrefab,
        Entity muzzleFlashVFX) {
        RecoilForce = recoilForce;
        ProjectileSpeed = projectileSpeed;
        FuelCost = fuelCost;
        ProjectilePrefab = projectilePrefab;
        MuzzleFlashVFX = muzzleFlashVFX;
    }
}