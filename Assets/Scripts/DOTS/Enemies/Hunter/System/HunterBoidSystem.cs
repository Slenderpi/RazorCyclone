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
		new HunterBoidJob() {
			DeltaTime = SystemAPI.Time.DeltaTime,
			StaticsBasic = SystemAPI.GetSingleton<HunterBasicStatics>(),
			StaticsEmpowered = SystemAPI.GetSingleton<HunterEmpoweredStatics>(),
			pw = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld,
			PlayerPosition = SystemAPI.GetComponent<LocalToWorld>(SystemAPI.GetSingletonEntity<Player>()).Position
		}.ScheduleParallel();
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
			//ref RandomGenerator randomGenerator,
			in PhysicsMass pm,
			in WavefrontReader wfReader,
			in Hunter hunterTag
		) {
			float3 vel = Util.IsNearZero(pv.Linear) ? new float3(0f, 0f, 0.1f) : pv.Linear;
			float distCheck = math.lengthsq(PlayerPosition - trans.Position);
			pv.Linear = math.normalize(PlayerPosition - trans.Position) * 2f;
			//{
			//	//{ // VISUALIZER FOR FLEEING
			//	//	if (distCheck <= Statics.FleeTriggerDistance * Statics.FleeTriggerDistance)
			//	//		Util.D_DrawArrowStartingAt(
			//	//			localTransform.Position,
			//	//			PlayerPosition - localTransform.Position,
			//	//			Statics.FleeTriggerDistance,
			//	//			HasLos(localTransform.Position, Statics.LosFilterForFleeing) ? Color.red : Color.magenta,
			//	//			DeltaTime
			//	//		);
			//	//}
			//	if (distCheck <= Statics.FleeTriggerDistance * Statics.FleeTriggerDistance && HasLos(localTransform.Position, Statics.LosFilterForFleeing)) {
			//		boid.steerForce = math.normalizesafe(
			//			distCheck <= 4f * 4f ? // At short distances, just run from the player directly
			//			localTransform.Position - PlayerPosition :
			//			new float3(-wavefrontReader.DescentDirection.x, 0, -wavefrontReader.DescentDirection.z
			//		)) * Statics.FleeForce;
			//	} else {
			//		if (boid.timeSinceLastWanderStep >= Statics.BoidProperties.WanderMinimumDelay) {
			//			boid.timeSinceLastWanderStep = 0;
			//			//float3 newWanderDelta = new(randomGenerator.rng.NextFloat(-1f, 1f), 0f, randomGenerator.rng.NextFloat(-1f, 1f));
			//			//newWanderDelta = (Util.IsNearZero(newWanderDelta) ? math.forward() : math.normalize(newWanderDelta)) * Statics.BoidProperties.WanderChangeDist;
			//			//boid.lastWanderDelta = newWanderDelta;
			//			//boid.wanderVector = math.normalizesafe(cannonFodderBoid.wanderVector + newWanderDelta) * Statics.BoidProperties.WanderLimitRadius;
			//			boid.wanderVector = BoidUtil.StepWanderVector2D(
			//				boid.wanderVector,
			//				Statics.BoidProperties.WanderChangeDist,
			//				Statics.BoidProperties.WanderLimitRadius,
			//				ref randomGenerator.rng
			//			);
			//		}
			//		cannonFodderBoid.steerForce = BoidUtil.SeekWander(
			//			localTransform.Position,
			//			vel,
			//			cannonFodderBoid.wanderVector,
			//			Statics.BoidProperties.WanderLimitDist,
			//			Statics.BoidProperties.MaxSteeringVelocity,
			//			Statics.BoidProperties.MaxSteeringForce
			//		);
			//	}
			//	cannonFodderBoid.timeSinceLastWanderStep += DeltaTime;
			//	physicsVelocity.ApplyLinearImpulse(physicsMass, cannonFodderBoid.steerForce);
			//	physicsVelocity.Angular = float3.zero;
			//	localTransform.Rotation = math.slerp(
			//		localTransform.Rotation,
			//		quaternion.LookRotationSafe(cannonFodderBoid.steerForce, math.up()),
			//		DeltaTime
			//	);
			//	//{ // VISUALIZER FOR BOID VECTORS
			//	//  // Steering force visual
			//	//	Util.D_DrawArrowStartingAt(localTransform.Position, cannonFodderBoid.steerForce, math.length(cannonFodderBoid.steerForce) / 5f, Color.cyan, DeltaTime);
			//	//	float3 wanderVelEnd = localTransform.Position + math.normalizesafe(vel) * Statics.BoidProperties.WanderLimitDist;
			//	//	float3 wanderVectorEnd = wanderVelEnd + cannonFodderBoid.wanderVector;
			//	//	//float3 wanderDeltaEnd = wanderVectorEnd + cannonFodderBoid.lastWanderDelta;
			//	//	// Visual for velocity limited by WanderLimitDist
			//	//	Util.D_DrawArrowFromTo(localTransform.Position, wanderVelEnd, Color.yellow, DeltaTime);
			//	//	// Visual for wanderVector
			//	//	Util.D_DrawArrowFromTo(wanderVelEnd, wanderVectorEnd, new Color(1f, 165f / 255f, 0f), DeltaTime);
			//	//	//Util.D_DrawArrowFromTo(wanderVectorEnd, wanderDeltaEnd, Color.red, DeltaTime);
			//	//}
			//}
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool HasLos(in float3 myPos, in CollisionFilter losFilter) {
			return !pw.CastRay(new() {
				Start = PlayerPosition,
				End = myPos,
				Filter = losFilter
			});
		}
	}

}