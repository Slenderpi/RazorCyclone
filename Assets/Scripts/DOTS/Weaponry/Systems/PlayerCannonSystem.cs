using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct PlayerCannonSystem : ISystem {

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<Player>();
        state.RequireForUpdate<PlayerInput>();
        state.RequireForUpdate<PlayerCannon>();
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        EntityQuery eqPlayer = SystemAPI.QueryBuilder().WithAll<PlayerInput, PhysicsVelocity, PhysicsMass, LocalToWorld>().Build();
        PlayerInput input = eqPlayer.ToComponentDataArray<PlayerInput>(Allocator.Temp)[0];
		if (!input.FireCannon)
            return;
        PhysicsVelocity pv = eqPlayer.ToComponentDataArray<PhysicsVelocity>(Allocator.Temp)[0];
        PhysicsMass pm = eqPlayer.ToComponentDataArray<PhysicsMass>(Allocator.Temp)[0];
        Entity playerEntity = eqPlayer.ToEntityArray(Allocator.Temp)[0];
        LocalToWorld camtransWorld = SystemAPI
            .QueryBuilder()
            .WithAll<PlayerCameraTransform, LocalToWorld>()
            .Build()
            .ToComponentDataArray<LocalToWorld>(Allocator.Temp)[0];
        EntityQuery eqCannon = SystemAPI.QueryBuilder().WithAll<PlayerCannon, LocalToWorld>().Build();
        PlayerCannon cannon = eqCannon.ToComponentDataArray<PlayerCannon>(Allocator.Temp)[0];
        LocalToWorld muzzleTrans = eqCannon.ToComponentDataArray<LocalToWorld>(Allocator.Temp)[0];
        Entity cannonEntity = eqCannon.ToEntityArray(Allocator.Temp)[0];
		EntityManager em = state.EntityManager;
        //EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginVariableRateSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        // Apply velocity
        pm.InverseInertia = float3.zero;
        SystemAPI.SetComponent(playerEntity, pm);
        SystemAPI.SetComponent(playerEntity, new PhysicsVelocity {
            Linear = cannon.RecoilForce * input.aimDirection + pv.Linear
        });

        // Spawn and setup projectile
        Entity projectile = em.Instantiate(cannon.ProjectilePrefab);
        em.AddComponentData(
            projectile,
            LocalTransform.FromPositionRotation(
                camtransWorld.Position,
                camtransWorld.Rotation
            )
        );
        em.AddComponentData(
            projectile,
            new PhysicsVelocity {
                Linear = -input.aimDirection * cannon.ProjectileSpeed // TODO: Add player velocity
            }
        );

        // Spawn muzzle flash vfx
        em.AddComponentData(
            em.Instantiate(cannon.MuzzleFlashVFX),
            new Parent { Value = cannonEntity }
        );
    }

}
