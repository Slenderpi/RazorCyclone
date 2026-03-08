using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PlayerPostUpdateGroup))]
partial struct PlayerResourcesSystem : ISystem {

    EntityQuery eqPlayer;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<Player>();
        state.RequireForUpdate<PlayerResources>();

        using var eqb = new EntityQueryBuilder(Allocator.Temp);
        eqPlayer = eqb.WithAll<PlayerResources>().Build(ref state);
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		PlayerResources resources = eqPlayer.ToComponentDataArray<PlayerResources>(Allocator.Temp)[0];

        if (resources.CanRegenHealth((float)SystemAPI.Time.ElapsedTime))
            resources.RegenerateHealth(SystemAPI.Time.DeltaTime);

		SystemAPI.SetComponent(eqPlayer.ToEntityArray(Allocator.Temp)[0], resources);
    }

}

[UpdateInGroup(typeof(PresentationSystemGroup))]
partial struct PlayerResourcesFixedStepEventsResetSystem : ISystem {

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<PlayerResources>();
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
        foreach (var resources in SystemAPI.Query<RefRW<PlayerResources>>())
            resources.ValueRW.ResetEvents();
	}

}