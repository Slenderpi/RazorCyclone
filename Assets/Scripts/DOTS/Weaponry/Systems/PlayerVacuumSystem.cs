using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;

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

		// Suck/kill enemies
		new VacuumHitDetectionJob() {
			Vacuum = vacuum,
			VacLocation = SystemAPI.GetComponent<LocalToWorld>(vacuumEntity).Position,
			VacDirection = input.aimDirection
		}.ScheduleParallel();
	}

#pragma warning disable IDE0251 // Make member 'readonly'
	[BurstCompile]
	void DisableVacuum(in Entity vacuumEntity, PlayerVacuum vacuum, ref SystemState state) {
		vacuum.VacuumEnabled = false;
		SystemAPI.SetComponent(vacuumEntity, vacuum);
	}

	[BurstCompile]
	partial struct VacuumHitDetectionJob : IJobEntity {
		public PlayerVacuum Vacuum;
		public float3 VacLocation;
		public float3 VacDirection; // Must be provided normalized

		[BurstCompile]
		public void Execute(
			ref VacuumTarget target,
			ref PhysicsVelocity pv,
			in PhysicsMass pm,
			in LocalTransform trans
		) {
			float3 toPlayer = VacLocation - trans.Position;
			float distsq = math.lengthsq(toPlayer);
			if (target.CanGetKilled && distsq <= Util.pow2(Vacuum.VacuumKillRadius + target.VacuumHitboxRadius)) {
				target.SetEventKilled();
				return;
			}
			// If VacuumKillRadius < VacuumSuckRadius (which should always be the case) then distsq will never be 0 here
			if (target.CanGetSucked && distsq <= Util.pow2(Vacuum.VacuumSuckRadius + target.VacuumHitboxRadius)) {
				target.SetEventSucked();
				float dist = math.sqrt(distsq);
				float3 normToPlayer = toPlayer / dist;
				if (IsInSuckAngle(normToPlayer)) {
					ApplySuckForce(ref pv, pm, Vacuum, toPlayer, dist);
				}
			}

		}

		[BurstCompile]
		bool IsInSuckAngle(in float3 normToPlayer) {
			/* Intent: check that v . w >= |v||w|cos(a)
			 * |v| = |w| = 1, so v . w >= cos(a)
			 * cos(a) = -VacuumSuckAngleCosined
			 * so, IsInSuckAngle() == return v . w <= VacuumSuckAngleCosined
			 * normToPlayer is "to player", hence why VacuumSuckAngleCosined is negative
			 */
			return math.dot(normToPlayer, VacDirection) <= Vacuum.VacuumSuckAngleCosined;
		}

		[BurstCompile]
		void ApplySuckForce(ref PhysicsVelocity pv, in PhysicsMass pm, in PlayerVacuum vac, in float3 toPlayer, float dist) {
			float3 impulse = toPlayer / dist * vac.VacuumSuckForce;
			pv.ApplyLinearImpulse(pm, impulse);
		}
	}
#pragma warning restore IDE0251 // Make member 'readonly'

}
