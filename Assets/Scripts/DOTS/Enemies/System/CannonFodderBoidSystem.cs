using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

partial struct CannonFodderBoidSystem : ISystem {

	[BurstCompile]
	public void OnCreate(ref SystemState state) {

	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		CannonFodderBoidJob job = new CannonFodderBoidJob() {
			DeltaTime = SystemAPI.Time.DeltaTime,
			ElapsedTime = (float)SystemAPI.Time.ElapsedTime
		};
		job.ScheduleParallel();

		/*foreach (var (
			physicsVelocity,
			localTransform,
			generalBoid,
			cannonFodderBoid,
			physicsMass,
			randomGenerator
		) in SystemAPI.Query<
			RefRW<PhysicsVelocity>,
			RefRW<LocalTransform>,
			RefRW<GeneralBoid>,
			RefRO<CannonFodderBoid>,
			RefRO<PhysicsMass>,
			RefRO<RandomGenerator>
		>()) {
			if (SystemAPI.Time.ElapsedTime - generalBoid.ValueRO.lastWanderStepTime <= generalBoid.ValueRO.WanderMinimumDelay)
				continue;
			generalBoid.ValueRW.lastWanderStepTime = (float)SystemAPI.Time.ElapsedTime;
			generalBoid.ValueRW.wanderPoint = BoidSteerer.StepWanderPoint2D(
				generalBoid.ValueRO.wanderPoint,
				generalBoid.ValueRO.WanderLimitRadius,
				generalBoid.ValueRO.WanderChangeDist,
				randomGenerator.ValueRO.rng
			);
			float3 steer = BoidSteerer.Wander(
				localTransform.ValueRO.Position,
				physicsVelocity.ValueRO.Linear,
				generalBoid.ValueRO.wanderPoint,
				generalBoid.ValueRO.WanderLimitDist,
				generalBoid.ValueRO.MaxSteeringVelocity,
				generalBoid.ValueRO.MaxSteeringForce
			);

			physicsVelocity.ValueRW.ApplyLinearImpulse(physicsMass.ValueRO, steer * 10f);
			physicsVelocity.ValueRW.Angular = float3.zero;

			localTransform.ValueRW.Rotation = quaternion.identity;
		}*/
	}

}

partial struct CannonFodderBoidJob : IJobEntity {
	public float DeltaTime;
	public float ElapsedTime;

	public void Execute(
		ref PhysicsVelocity physicsVelocity,
		ref LocalTransform localTransform,
		ref GeneralBoid generalBoid,
		in CannonFodderBoid cannonFodderBoid,
		in PhysicsMass physicsMass,
		in RandomGenerator randomGenerator
	) {
		if (ElapsedTime - generalBoid.lastWanderStepTime <= generalBoid.WanderMinimumDelay)
			return;
		generalBoid.lastWanderStepTime = ElapsedTime;
		generalBoid.wanderPoint = Util.StepWanderPoint2D(
			generalBoid.wanderPoint,
			generalBoid.WanderLimitRadius,
			generalBoid.WanderChangeDist,
			randomGenerator.rng
		);
		float3 steer = Util.Wander(
			localTransform.Position,
			physicsVelocity.Linear,
			generalBoid.wanderPoint,
			generalBoid.WanderLimitDist,
			generalBoid.MaxSteeringVelocity,
			generalBoid.MaxSteeringForce
		);

		physicsVelocity.ApplyLinearImpulse(physicsMass, steer * 10f);
		physicsVelocity.Angular = float3.zero;

		localTransform.Rotation = math.slerp(
			localTransform.Rotation,
			quaternion.LookRotation(steer, math.up()),
			DeltaTime * 10f
		);
	}
}