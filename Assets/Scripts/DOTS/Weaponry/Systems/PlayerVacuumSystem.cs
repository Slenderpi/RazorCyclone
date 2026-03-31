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

	EntityQuery eqPlayer;
	EntityQuery eqVacuum;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<Player>();
		state.RequireForUpdate<PlayerInput>();
		state.RequireForUpdate<PlayerVacuum>();
		state.RequireForUpdate<PlayerResources>();

		using var eqb = new EntityQueryBuilder(Allocator.Temp);
		eqPlayer = eqb.WithAll<PlayerInput, PhysicsVelocity, PhysicsMass, PlayerResources>().Build(ref state);
		eqVacuum = eqb.Reset().WithAllRW<PlayerVacuum>().WithAll<LocalToWorld>().Build(ref state);
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		PlayerInput input = eqPlayer.GetSingleton<PlayerInput>();
		PlayerResources resources = eqPlayer.GetSingleton<PlayerResources>();
		RefRW<PlayerVacuum> vacuum = eqVacuum.GetSingletonRW<PlayerVacuum>();

		// Determine the enabled/disabled state of the vacuum
		if (input.EnableVacuum) {
			if (!resources.CanSpendFuel()) {
				// If not enough fuel, disable vacuum if necessary
				if (vacuum.ValueRO.VacuumEnabled)
					vacuum.ValueRW.VacuumEnabled = false;
				return;
			} else if (!vacuum.ValueRO.VacuumEnabled) {
				// There's enough fuel. If the vacuum isn't already enabled, enable it
				vacuum.ValueRW.VacuumEnabled = true;
			}
		} else {
			if (vacuum.ValueRO.VacuumEnabled)
				vacuum.ValueRW.VacuumEnabled = false;
			return;
		}

		// Reaching this point means the vacuum is supposed to be enabled.
		// Get necessary components
		PhysicsMass pm = eqPlayer.GetSingleton<PhysicsMass>();
		PhysicsVelocity pv = eqPlayer.GetSingleton<PhysicsVelocity>();
		Entity playerEntity = eqPlayer.GetSingletonEntity();

		// Spend fuel
		resources.SpendFuel(vacuum.ValueRO.GetFuelRate() * SystemAPI.Time.DeltaTime, (float)SystemAPI.Time.ElapsedTime);
		SystemAPI.SetComponent(playerEntity, resources);

		// Pull the player by applying the vacuum's pull force
		pm.InverseInertia = float3.zero;
		SystemAPI.SetComponent(playerEntity, pm);
		SystemAPI.SetComponent(playerEntity, new PhysicsVelocity {
			Linear = SystemAPI.Time.DeltaTime * vacuum.ValueRO.VacuumPullForce * input.aimDirection + pv.Linear
		});

		// Suck/kill enemies
		state.Dependency = new VacuumHitDetectionJob() {
			Vacuum = vacuum.ValueRO,
			VacLocation = eqVacuum.GetSingleton<LocalToWorld>().Position,
			VacDirection = input.aimDirection
		}.ScheduleParallel(state.Dependency);
	}

#pragma warning disable IDE0251 // Make member 'readonly'
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
