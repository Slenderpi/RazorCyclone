using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(PlayerPostUpdateGroup))]
[UpdateBefore(typeof(PlayerRotationSystem))]
partial struct PlayerExtraInputsSystem : ISystem {
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<Player>();
		state.RequireForUpdate<PlayerExtraInput>();
		state.RequireForUpdate<PlayerResources>();
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		EntityQuery eqPlayer = SystemAPI
			.QueryBuilder()
			.WithAll<PlayerExtraInput, PlayerResources, PlayerSpinfo>().Build();
		Entity playerEntity = eqPlayer.ToEntityArray(Allocator.Temp)[0];
		PlayerExtraInput input = eqPlayer.ToComponentDataArray<PlayerExtraInput>(Allocator.Temp)[0];
		PlayerResources resources = eqPlayer.ToComponentDataArray<PlayerResources>(Allocator.Temp)[0];
		PlayerSpinfo spinfo = eqPlayer.ToComponentDataArray<PlayerSpinfo>(Allocator.Temp)[0];

		// Handle input
		if (input.SlowTime) {
			// TODO
		}
		if (input.RefillFuel) {
			resources.RefillFuel();
		}
		if (input.AddRicochets) {
			spinfo.CurrentSpins += 5;
		}
		if (input.HealHealth) {
			resources.HealHealth(100f);
		}
		if (input.TakeDamage) {
			resources.TakeDamage(25f, (float)SystemAPI.Time.ElapsedTime);
		}

		// Reset extra input
		input.ResetPresses();

		// Update values
		SystemAPI.SetComponent(playerEntity, resources);
		SystemAPI.SetComponent(playerEntity, spinfo);
		SystemAPI.SetComponent(playerEntity, input);
	}

}
