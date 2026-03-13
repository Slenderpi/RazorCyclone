using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PrePhysicsGroup))]
partial struct HunterBoidSystem : ISystem {
	
	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<HunterBasicStatics>();
		state.RequireForUpdate<HunterEmpoweredStatics>();
		state.RequireForUpdate<HunterBoid>();
		state.RequireForUpdate<Player>();
	}
	
	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		state.Dependency = new HunterBoidJob() {
			DeltaTime = SystemAPI.Time.DeltaTime,
			StaticsBasic = SystemAPI.GetSingleton<HunterBasicStatics>(),
			StaticsEmpowered = SystemAPI.GetSingleton<HunterEmpoweredStatics>(),
			pw = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld,
			PlayerPosition = SystemAPI.GetComponent<LocalToWorld>(SystemAPI.GetSingletonEntity<Player>()).Position
		}.ScheduleParallel(state.Dependency);
	}

	[BurstCompile]
	partial struct HunterBoidJob : IJobEntity {
		public float DeltaTime;
		public HunterBasicStatics StaticsBasic;
		public HunterEmpoweredStatics StaticsEmpowered;
		[ReadOnly] public PhysicsWorld pw;
		public float3 PlayerPosition;

		[BurstCompile]
		public void Execute(
			ref HunterBoid boid,
			ref PhysicsVelocity pv,
			ref LocalTransform trans,
			ref RandomGenerator randomGenerator,
			in PhysicsMass pm,
			in WavefrontReader wfReader,
			in Hunter hunterTag
		) {
			HunterSharedStatics hsStatics = hunterTag.Form == EEnemyForm.Basic ? StaticsBasic.Hunter : StaticsEmpowered.Hunter;
			GeneralBoidProperties gbProps = hunterTag.Form == EEnemyForm.Basic ? StaticsBasic.BoidProperties : StaticsEmpowered.BoidProperties;
			float3 newSteerForce = float3.zero;
			float3 vel = Util.IsNearZero(pv.Linear) ? new float3(0f, 0f, 0.01f) : pv.Linear;
			float3 toPlayer = PlayerPosition - trans.Position;
			float distCheck = math.lengthsq(toPlayer);
			boid.timeSinceLastWanderStep += DeltaTime;
			boid.timeSinceBeganFleeing += DeltaTime;

			if (boid.timeSinceBeganFleeing <= hsStatics.RunAwayDuration) {
				// Still in fleeing state, so keep fleeing
				newSteerForce = distCheck < 4f ?
					// Very close to player--use flee
					BoidUtil.Flee(
						trans.Position, PlayerPosition, vel, gbProps.MaxSteeringVelocity, gbProps.MaxSteeringForce
					) :
					// Farther away--use pathfinding
					BoidUtil.FleeDirection(
						new float3(wfReader.DescentDirection), vel, gbProps.MaxSteeringVelocity, gbProps.MaxSteeringForce
					);
			} else {
				// If greater than WanderTriggerDist, include wander
				if (distCheck > Util.pow2(hsStatics.WanderTriggerDist)) {
					// Wander
					if (boid.timeSinceLastWanderStep >= gbProps.WanderMinimumDelay) {
						boid.timeSinceLastWanderStep = 0;
						boid.wanderVector = BoidUtil.StepWanderVector2D(
							boid.wanderVector, gbProps.WanderChangeDist, gbProps.WanderLimitRadius, ref randomGenerator.rng
						);
					}
					newSteerForce = BoidUtil.SeekWander(
						trans.Position, vel, boid.wanderVector, gbProps.WanderLimitDist, gbProps.MaxSteeringVelocity, gbProps.MaxSteeringForce
					);
				}

				if (JustPassedPlayer(vel, toPlayer, distCheck, hsStatics, gbProps)) {
					// Hunter just passed player, so change to flee state
					boid.timeSinceBeganFleeing = 0f;
					newSteerForce += BoidUtil.Flee(
						trans.Position, PlayerPosition, vel, gbProps.MaxSteeringVelocity, gbProps.MaxSteeringForce
					);
				} else {
					// Hunter in chase state, so seek player
					//if (distCheck >= Util.pow2(hsStatics.WanderTriggerDist + 3f))
					//	Debug.Log("Using pathfinding!");
					if (distCheck < Util.pow2(hsStatics.WanderTriggerDist + 3f)) // TODO: add property for 'pathfinding trigger dist'
						// NOTE: this method currently does not account for when the player is outside of the PointCloud
						newSteerForce += BoidUtil.Seek(
							trans.Position, PlayerPosition, vel, gbProps.MaxSteeringVelocity, gbProps.MaxSteeringForce
						);
					else
						newSteerForce += BoidUtil.SeekDirection(new float3(wfReader.DescentDirection), vel, gbProps.MaxSteeringVelocity, gbProps.MaxSteeringForce);
				}
			}
			boid.steerForce = newSteerForce;
			pv.ApplyLinearImpulse(pm, boid.steerForce);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly bool JustPassedPlayer(in float3 vel, in float3 toPlayer, float distCheck, in HunterSharedStatics hsStatics, in GeneralBoidProperties gbProps) {
			return math.lengthsq(vel) > Util.pow2(hsStatics.RunAwayRequiredSpeed) &&
				distCheck <= Util.pow2(hsStatics.RunAwayRequiredDist) &&
				math.dot(toPlayer, vel) <= 0;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool HasLos(in float3 myPos) {
			return !pw.CastRay(new() {
				Start = PlayerPosition,
				End = myPos,
				//Filter = StaticsBasic.los
			});
		}
	}

}