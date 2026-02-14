using System.Linq;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PrePhysicsGroup))]
partial struct CannonFodderBoidSystem : ISystem {

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<CannonFodderStatics>();
		state.RequireForUpdate<CannonFodderBoid>();
		state.RequireForUpdate<Player>();
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		new CannonFodderBoidJob() {
			DeltaTime = SystemAPI.Time.DeltaTime,
			Statics = SystemAPI.GetSingleton<CannonFodderStatics>(),
			PlayerPosition = SystemAPI.GetComponent<LocalToWorld>(SystemAPI.GetSingletonEntity<Player>()).Position
		}.ScheduleParallel();
	}

	[BurstCompile]
	partial struct CannonFodderBoidJob : IJobEntity {
		public float DeltaTime;
		public CannonFodderStatics Statics;
		public float3 PlayerPosition;

		[BurstCompile]
		public void Execute(
			ref CannonFodderBoid cannonFodderBoid,
			ref PhysicsVelocity physicsVelocity,
			ref LocalTransform localTransform,
			ref RandomGenerator randomGenerator,
			in PhysicsMass physicsMass,
			in WavefrontReader wavefrontReader,
			in EnemyComponent enemyComp
		) {
			float3 vel = Util.IsNearZero(physicsVelocity.Linear) ? new float3(0f, 0f, 0.1f) : physicsVelocity.Linear;
			float distCheck = math.lengthsq(PlayerPosition - localTransform.Position);
			//{ // VISUALIZER FOR FLEEING
			//	if (distCheck <= Statics.FleeTriggerDistance * Statics.FleeTriggerDistance)
			//		Util.D_DrawArrowStartingAt(
			//			localTransform.Position,
			//			PlayerPosition - localTransform.Position,
			//			Statics.FleeTriggerDistance,
			//			enemyComp.HasLineOfSight ? Color.red : Color.magenta,
			//			DeltaTime
			//		);
			//}
			if (enemyComp.HasLineOfSight && distCheck <= Statics.FleeTriggerDistance * Statics.FleeTriggerDistance) {
				cannonFodderBoid.steerForce = math.normalizesafe(
					distCheck <= 4f * 4f ? // At short distances, just run from the player directly
					localTransform.Position - PlayerPosition :
					new float3(-wavefrontReader.DescentDirection.x, 0, -wavefrontReader.DescentDirection.z
				)) * Statics.FleeForce;
			} else {
				if (cannonFodderBoid.timeSinceLastWanderStep >= Statics.BoidProperties.WanderMinimumDelay) {
					cannonFodderBoid.timeSinceLastWanderStep = 0;
					//float3 newWanderDelta = new(randomGenerator.rng.NextFloat(-1f, 1f), 0f, randomGenerator.rng.NextFloat(-1f, 1f));
					//newWanderDelta = (Util.IsNearZero(newWanderDelta) ? math.forward() : math.normalize(newWanderDelta)) * Statics.BoidProperties.WanderChangeDist;
					//cannonFodderBoid.lastWanderDelta = newWanderDelta;
					//cannonFodderBoid.wanderVector = math.normalizesafe(cannonFodderBoid.wanderVector + newWanderDelta) * Statics.BoidProperties.WanderLimitRadius;
					cannonFodderBoid.wanderVector = BoidUtil.StepWanderVector2D(
						cannonFodderBoid.wanderVector,
						Statics.BoidProperties.WanderChangeDist,
						Statics.BoidProperties.WanderLimitRadius,
						ref randomGenerator.rng
					);
				}
				cannonFodderBoid.steerForce = BoidUtil.SeekWander(
					localTransform.Position,
					vel,
					cannonFodderBoid.wanderVector,
					Statics.BoidProperties.WanderLimitDist,
					Statics.BoidProperties.MaxSteeringVelocity,
					Statics.BoidProperties.MaxSteeringForce
				);
			}
			cannonFodderBoid.timeSinceLastWanderStep += DeltaTime;
			physicsVelocity.ApplyLinearImpulse(physicsMass, cannonFodderBoid.steerForce);
			physicsVelocity.Angular = float3.zero;
			localTransform.Rotation = math.slerp(
				localTransform.Rotation,
				quaternion.LookRotationSafe(cannonFodderBoid.steerForce, math.up()),
				DeltaTime
			);
			//{ // VISUALIZER FOR BOID VECTORS
			//  // Steering force visual
			//	Util.D_DrawArrowStartingAt(localTransform.Position, cannonFodderBoid.steerForce, math.length(cannonFodderBoid.steerForce) / 5f, Color.cyan, DeltaTime);
			//	float3 wanderVelEnd = localTransform.Position + math.normalizesafe(vel) * Statics.BoidProperties.WanderLimitDist;
			//	float3 wanderVectorEnd = wanderVelEnd + cannonFodderBoid.wanderVector;
			//	//float3 wanderDeltaEnd = wanderVectorEnd + cannonFodderBoid.lastWanderDelta;
			//	// Visual for velocity limited by WanderLimitDist
			//	Util.D_DrawArrowFromTo(localTransform.Position, wanderVelEnd, Color.yellow, DeltaTime);
			//	// Visual for wanderVector
			//	Util.D_DrawArrowFromTo(wanderVelEnd, wanderVectorEnd, new Color(1f, 165f / 255f, 0f), DeltaTime);
			//	//Util.D_DrawArrowFromTo(wanderVectorEnd, wanderDeltaEnd, Color.red, DeltaTime);
			//}
		}
	}

}