using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(ProjectilePostUpdateGroup))]
partial struct PlayerCannonProjectileSystem : ISystem {

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        
    }

}
