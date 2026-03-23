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

	EntityQuery eqPlayer;
	EntityQuery eqWavefrontGoalTarget;
	EntityQuery eqPcc;
	
	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<HunterBasicStatics>();
		state.RequireForUpdate<HunterEmpoweredStatics>();
		state.RequireForUpdate<HunterBoid>();
		state.RequireForUpdate<Player>();
		state.RequireForUpdate<WavefrontGoalTarget>();
		state.RequireForUpdate<PointCloudConfig>();

		using var eqb = new EntityQueryBuilder(Allocator.Temp);
		eqPlayer = eqb.WithAll<Player, LocalToWorld>().Build(ref state);
		eqWavefrontGoalTarget = eqb.Reset().WithAll<WavefrontGoalTarget>().Build(ref state);
		eqPcc = eqb.Reset().WithAll<PointCloudConfig>().Build(ref state);
	}
	
	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		// The pcc is used for raycasting. We can sacrifice accuracy to save on time.
		// Hence, we can group the points into chunks of FACTORxFACTORxFACTOR.
		// Effeciency of this value depends on the size of a Point. A larger factor is better if Points are small,
		// but a larger factor on big Points would cause too much inaccuracy and may allow Hunters that clearly
		// don't have LOS to move like they do.
		const int CHUNK_FACTOR = 3;

		PointCloudConfig pccChunked = eqPcc.GetSingleton<PointCloudConfig>();
		pccChunked.numX /= CHUNK_FACTOR;
		pccChunked.numY /= CHUNK_FACTOR;
		pccChunked.numZ /= CHUNK_FACTOR;
		pccChunked.DistBetweenPoints *= CHUNK_FACTOR;

		var CachedLosChecks = new NativeArray<byte>(pccChunked.numX * pccChunked.numY * pccChunked.numZ, Allocator.TempJob);
		state.Dependency = new HunterBoidJob() {
			DeltaTime = SystemAPI.Time.DeltaTime,
			StaticsBasic = SystemAPI.GetSingleton<HunterBasicStatics>(),
			StaticsEmpowered = SystemAPI.GetSingleton<HunterEmpoweredStatics>(),
			cw = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld,
			PlayerPosition = eqPlayer.GetSingleton<LocalToWorld>().Position,
			IsPlayerInPointCloud = eqWavefrontGoalTarget.ToComponentDataArray<WavefrontGoalTarget>(Allocator.Temp)[0].IsInPointCloud,
			pccChunked = pccChunked,
			CachedLosChecks = CachedLosChecks
		}.ScheduleParallel(state.Dependency);
		state.Dependency = CachedLosChecks.Dispose(state.Dependency);
	}

	[BurstCompile]
	partial struct HunterBoidJob : IJobEntity {
		public float DeltaTime;
		[ReadOnly] public HunterBasicStatics StaticsBasic;
		[ReadOnly] public HunterEmpoweredStatics StaticsEmpowered;
		[ReadOnly] public CollisionWorld cw;
		public float3 PlayerPosition;
		public bool IsPlayerInPointCloud;
		[ReadOnly] public PointCloudConfig pccChunked;

		/// <summary>
		/// 0: Not checked<br/>
		/// 1: Has NO LOS<br/>
		/// _: Has LOS
		/// </summary>
		[NativeDisableParallelForRestriction]
		public NativeArray<byte> CachedLosChecks;

		[BurstCompile]
		public void Execute(
			ref HunterBoid boid,
			ref PhysicsVelocity pv,
			ref LocalTransform trans,
			ref RandomGenerator randomGenerator,
			in PhysicsMass pm,
			in WavefrontReader wfReader,
			in EnemyForm eform
		) {
			HunterBoidSharedStatics hsStatics = eform.Form == EEnemyForm.Basic ? StaticsBasic.HunterBoid : StaticsEmpowered.HunterBoid;
			GeneralBoidProperties gbProps = eform.Form == EEnemyForm.Basic ? StaticsBasic.BoidProperties : StaticsEmpowered.BoidProperties;
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
				if (distCheck > hsStatics.WanderTriggerDistSq) {
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
					if (ShouldFollowWavefront(trans.Position, distCheck, hsStatics))
						newSteerForce += BoidUtil.SeekDirection(new float3(wfReader.DescentDirection), vel, gbProps.MaxSteeringVelocity, gbProps.MaxSteeringForce);
					else
						newSteerForce += BoidUtil.Seek(
							trans.Position, PlayerPosition, vel, gbProps.MaxSteeringVelocity, gbProps.MaxSteeringForce
						);
				}
			}
			boid.steerForce = newSteerForce;
			pv.ApplyLinearImpulse(pm, boid.steerForce);

			// If steer is 0, don't change currentDesiredRot (i.e. maintain previous deired rotation)
			if (Util.IsNearZero(math.lengthsq(boid.steerForce)))
				return;
			// If velocity is near 0, use transform's current forward
			float3 forward = Util.IsNearZero(math.lengthsq(pv.Linear)) ? trans.Forward() : pv.Linear;
			boid.currentDesiredRot = BoidUtil.Rotator.Airplane(forward, boid.steerForce);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly bool JustPassedPlayer(in float3 vel, in float3 toPlayer, float distCheck, in HunterBoidSharedStatics hsStatics, in GeneralBoidProperties gbProps) {
			return math.lengthsq(vel) > hsStatics.RunAwayRequiredSpeedSq &&
				distCheck <= hsStatics.RunAwayRequiredDistSq &&
				math.dot(toPlayer, vel) <= 0;
		}

		/// <summary>
		/// Returns true if:
		/// - Player is inside of pointcloud
		///		- && Player is far
		///	- || (
		///		- Hunter inside PointCloud && has no LOS
		///	)
		/// </summary>
		/// <param name="myPos"></param>
		/// <param name="distCheck"></param>
		/// <param name="hsStatics"></param>
		/// <returns></returns>
		[BurstCompile]
		bool ShouldFollowWavefront(in float3 myPos, float distCheck, in HunterBoidSharedStatics hsStatics) {
			if (IsPlayerInPointCloud)
				return distCheck > hsStatics.PathfindTriggerDistSq;
			else {
				int3 point = pccChunked.PositionToPointUnclamped(myPos);
				if (!pccChunked.IsPointInBounds(point))
					return false;
				int index = pccChunked.PointToIndex(point);
				//{ // VISUALIZE LOS CHECK
				//	if (CachedLosChecks[index] != 0) {
				//		Util.D_DrawBox(pccChunked.PointToPosition(point), pccChunked.DistBetweenPoints, CachedLosChecks[index] == 1 ? Color.red : Color.green, depthTest: false);
				//	}
				//}
				// Nothing fancy in terms of race condition prevention. Testing shows it still saves a ton of time even with possible race conditions.
				if (CachedLosChecks[index] != 0)
					// Return cached result instead
					return CachedLosChecks[index] == 1;
				else {
					// Perform raycast, then cache result
					bool hasNoLos = cw.CastRay(new() {
						Start = PlayerPosition,
						End = myPos,
						Filter = hsStatics.LosFilterForChasing
					});
					CachedLosChecks[index] = hasNoLos ? (byte)1 : (byte)2;
					//{ // VISUALIZE LOS CHECK
					//	Util.D_DrawBox(pccChunked.PointToPosition(point), pccChunked.DistBetweenPoints, hasNoLos ? Color.red : Color.green, depthTest: false);
					//}
					return hasNoLos;
				}
			}
		}
	}

}