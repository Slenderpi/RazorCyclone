using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

[UpdateInGroup(typeof(PlayerPostUpdateGroup))]
[UpdateBefore(typeof(PlayerResourcesSystem))]
partial struct PlayerVacuumSystem : ISystem {

    [BurstCompile]
    public readonly void OnCreate(ref SystemState state) {
		state.RequireForUpdate<Player>();
		state.RequireForUpdate<PlayerInput>();
		state.RequireForUpdate<PlayerVacuum>();
		state.RequireForUpdate<PlayerResources>();
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		EntityQuery eqPlayer = SystemAPI
			.QueryBuilder()
			.WithAll<PlayerInput, PhysicsVelocity, PhysicsMass, PlayerResources>().Build();
		PlayerInput input = eqPlayer.ToComponentDataArray<PlayerInput>(Allocator.Temp)[0];
		PlayerResources resources = eqPlayer.ToComponentDataArray<PlayerResources>(Allocator.Temp)[0];
		EntityQuery eqVacuum = SystemAPI
			.QueryBuilder()
			.WithAll<PlayerVacuum>()
			.Build();
		PlayerVacuum vacuum = eqVacuum.ToComponentDataArray<PlayerVacuum>(Allocator.Temp)[0];
		Entity vacuumEntity = eqVacuum.ToEntityArray(Allocator.Temp)[0];

		// Determine the enabled/disabled state of the vacuum
		if (input.EnableVacuum) {
			if (!resources.CanSpendFuel()) {
				// If not enough fuel, disable vacuum if necessary
				if (vacuum.VacuumEnabled)
					DisableVacuum(vacuumEntity, vacuum, ref state);
				return;
			} else if (!vacuum.VacuumEnabled) {
				// There's enough fuel. If the vacuum isn't already enabled, enable it
				vacuum.VacuumEnabled = true;
				SystemAPI.SetComponent(vacuumEntity, vacuum);
			}
		} else {
			if (!vacuum.VacuumEnabled)
				return;
			DisableVacuum(vacuumEntity, vacuum, ref state);
		}

		// Reaching this point means the vacuum is supposed to be enabled.
		// Get necessary components
		PhysicsMass pm = eqPlayer.ToComponentDataArray<PhysicsMass>(Allocator.Temp)[0];
		PhysicsVelocity pv = eqPlayer.ToComponentDataArray<PhysicsVelocity>(Allocator.Temp)[0];
		Entity playerEntity = eqPlayer.ToEntityArray(Allocator.Temp)[0];

		// Spend fuel
		resources.SpendFuel(vacuum.GetFuelRate() * SystemAPI.Time.DeltaTime, (float)SystemAPI.Time.ElapsedTime);
		SystemAPI.SetComponent(playerEntity, resources);

		// Pull the player by applying the vacuum's pull force
		pm.InverseInertia = float3.zero;
		SystemAPI.SetComponent(playerEntity, pm);
		SystemAPI.SetComponent(playerEntity, new PhysicsVelocity {
			Linear = vacuum.VacuumPullForce * input.aimDirection + pv.Linear
		});
	}

	[BurstCompile]
#pragma warning disable IDE0251 // Make member 'readonly'
	void DisableVacuum(in Entity vacuumEntity, PlayerVacuum vacuum, ref SystemState state) {
		vacuum.VacuumEnabled = false;
		SystemAPI.SetComponent(vacuumEntity, vacuum);
	}
#pragma warning restore IDE0251 // Make member 'readonly'

}
