using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(EnemyLogicPostUpdateGroup))]
partial struct HunterRotationSystem : ISystem {

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<Hunter>();
		state.RequireForUpdate<HunterBoid>();
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		state.Dependency = new HunterRotationJob() {
			DeltaTime = SystemAPI.Time.DeltaTime
		}.ScheduleParallel(state.Dependency);
	}

	[BurstCompile]
	partial struct HunterRotationJob : IJobEntity {
		public float DeltaTime;

		[BurstCompile]
		public void Execute(
			ref LocalTransform trans,
			ref PhysicsMass pm,
			ref PhysicsVelocity pv,
			in HunterBoid boid
		) {
			pv.Angular = float3.zero;
			pm.InverseInertia = float3.zero;
			trans.Rotation = math.slerp(trans.Rotation, boid.currentDesiredRot, math.min(1f, DeltaTime * 5f));
		}
	}

}
