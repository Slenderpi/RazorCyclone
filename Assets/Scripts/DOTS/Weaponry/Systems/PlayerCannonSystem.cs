using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(PlayerPostUpdateGroup))]
[UpdateAfter(typeof(PlayerRotationSystem))]
[UpdateBefore(typeof(PlayerResourcesSystem))]
partial struct PlayerCannonSystem : ISystem {

    EntityQuery eqPlayer;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<Player>();
        state.RequireForUpdate<PlayerInput>();
        state.RequireForUpdate<PlayerCannon>();
        state.RequireForUpdate<PlayerResources>();

        using var eqb = new EntityQueryBuilder(Allocator.Temp);
        eqPlayer = eqb.WithAll<PlayerInput, PhysicsVelocity, PhysicsMass, LocalToWorld, PlayerResources, PlayerSpinfo>().Build(ref state);
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        using var pinputArr = eqPlayer.ToComponentDataArray<PlayerInput>(Allocator.Temp);
        PlayerInput input = pinputArr[0];
		if (!input.FireCannon)
            return;
        using var rsrcsArr = eqPlayer.ToComponentDataArray<PlayerResources>(Allocator.Temp);
		PlayerResources resources = rsrcsArr[0];
        if (!resources.CanSpendFuel())
            return;
        using var pvArr = eqPlayer.ToComponentDataArray<PhysicsVelocity>(Allocator.Temp);
        using var pmArr = eqPlayer.ToComponentDataArray<PhysicsMass>(Allocator.Temp);
        using var spinArr = eqPlayer.ToComponentDataArray<PlayerSpinfo>(Allocator.Temp);
        using var enArr = eqPlayer.ToEntityArray(Allocator.Temp);
		PhysicsVelocity pv = pvArr[0];
        PhysicsMass pm = pmArr[0];
		PlayerSpinfo spinfo = spinArr[0];
		Entity playerEntity = enArr[0];
        LocalToWorld camtransWorld = SystemAPI
            .QueryBuilder()
            .WithAll<PlayerCameraTransform, LocalToWorld>()
            .Build()
            .ToComponentDataArray<LocalToWorld>(Allocator.Temp)[0];
        EntityQuery eqCannon = SystemAPI.QueryBuilder().WithAll<PlayerCannon, LocalToWorld>().Build();
        PlayerCannon cannon = eqCannon.ToComponentDataArray<PlayerCannon>(Allocator.Temp)[0];
        Entity cannonEntity = eqCannon.ToEntityArray(Allocator.Temp)[0];
		EntityManager em = state.EntityManager;
        //EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginVariableRateSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        // Spend fuel
        resources.SpendFuel(cannon.FuelCost, (float)SystemAPI.Time.ElapsedTime);
        SystemAPI.SetComponent(playerEntity, resources);

        // Spawn and setup projectile
        Entity projectile = em.Instantiate(cannon.ProjectilePrefab);
        RefRW<PlayerCannonProjectile> pcp = SystemAPI.GetComponentRW<PlayerCannonProjectile>(projectile);
        //Debug.Log($"Spins: {spinfo.CurrentSpins} | Mult: {spinfo.CurrentRicochetMultiplier} | Total: {spinfo.CurrentSpins * spinfo.CurrentRicochetMultiplier}");
        pcp.ValueRW.SetMaxRicochets(spinfo.SpendSpinsAsRicochet());
        SystemAPI.SetComponent(playerEntity, spinfo);
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
                Linear = -input.aimDirection * cannon.ProjectileSpeed + pv.Linear
            }
        );

		// Apply velocity
		pm.InverseInertia = float3.zero;
		SystemAPI.SetComponent(playerEntity, pm);
		SystemAPI.SetComponent(playerEntity, new PhysicsVelocity {
			Linear = cannon.RecoilForce * input.aimDirection + pv.Linear
		});

		// Spawn muzzle flash vfx
		em.AddComponentData(
            em.Instantiate(cannon.MuzzleFlashVFX),
            new Parent { Value = cannonEntity }
        );
	}

}
