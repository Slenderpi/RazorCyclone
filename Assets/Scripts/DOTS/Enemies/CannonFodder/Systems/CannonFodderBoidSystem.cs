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
		state.RequireForUpdate<CannonFodderBoidStatics>();
		state.RequireForUpdate<CannonFodderBoid>();
		state.RequireForUpdate<Player>();
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		new CannonFodderBoidJob() {
			DeltaTime = SystemAPI.Time.DeltaTime,
			BoidStatics = SystemAPI.GetSingleton<CannonFodderBoidStatics>(),
			PlayerPosition = SystemAPI.GetComponent<LocalToWorld>(SystemAPI.GetSingletonEntity<Player>()).Position
		}.ScheduleParallel();
	}

	[BurstCompile]
	partial struct CannonFodderBoidJob : IJobEntity {
		public float DeltaTime;
		public CannonFodderBoidStatics BoidStatics;
		public float3 PlayerPosition;

		[BurstCompile]
		public void Execute(
			ref CannonFodderBoid cannonFodderBoid,
			ref PhysicsVelocity physicsVelocity,
			ref LocalTransform localTransform,
			in PhysicsMass physicsMass,
			in WavefrontReader wavefrontReader,
			in EnemyComponent enemyComp,
			in RandomGenerator randomGenerator
		) {
			float3 vel = Util.IsNearZero(physicsVelocity.Linear) ? new float3(0f, 0f, 0.1f) : physicsVelocity.Linear;
			float distCheck = math.lengthsq(PlayerPosition - localTransform.Position);
			{
				if (distCheck <= BoidStatics.FleeTriggerDistance * BoidStatics.FleeTriggerDistance)
					Util.D_DrawArrowStartingAt(
						localTransform.Position,
						PlayerPosition - localTransform.Position,
						BoidStatics.FleeTriggerDistance,
						enemyComp.HasLineOfSight ? Color.red : Color.magenta,
						DeltaTime,
						false
					);
			}
			if (enemyComp.HasLineOfSight && distCheck <= BoidStatics.FleeTriggerDistance * BoidStatics.FleeTriggerDistance) {
				cannonFodderBoid.steerForce = math.normalizesafe(
					distCheck <= 4f * 4f ? // At short distances, just run from the player directly
					localTransform.Position - PlayerPosition :
					new float3(-wavefrontReader.DescentDirection.x, 0, -wavefrontReader.DescentDirection.z
				)) * BoidStatics.FleeForce;
			} else if (cannonFodderBoid.timeSinceLastWanderStep >= BoidStatics.WanderMinimumDelay) {
				cannonFodderBoid.timeSinceLastWanderStep -= BoidStatics.WanderMinimumDelay;
				cannonFodderBoid.wanderPoint = BoidUtil.StepWanderPoint2D(
					cannonFodderBoid.wanderPoint,
					BoidStatics.WanderLimitRadius,
					BoidStatics.WanderChangeDist,
					randomGenerator.rng
				);
				cannonFodderBoid.steerForce = BoidUtil.Wander(
					localTransform.Position,
					vel,
					cannonFodderBoid.wanderPoint,
					BoidStatics.WanderLimitDist,
					BoidStatics.MaxSteeringVelocity,
					BoidStatics.MaxSteeringForce
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
			{
				Util.D_DrawArrowStartingAt(localTransform.Position, cannonFodderBoid.steerForce, math.length(cannonFodderBoid.steerForce) / 5f, Color.cyan, DeltaTime, false);
				Util.D_DrawArrowStartingAt(localTransform.Position, vel, math.length(vel) / 5f, Color.green, DeltaTime, false);
			}
		}
	}

}