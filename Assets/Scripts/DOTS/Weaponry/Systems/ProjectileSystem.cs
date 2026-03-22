using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
partial struct ProjectileSystem : ISystem {

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<Projectile>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		state.Dependency = new ProjectileJob() {
			dt = SystemAPI.Time.DeltaTime,
			pw = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld
		}.Schedule(state.Dependency);
	}

	[BurstCompile]
	partial struct ProjectileJob : IJobEntity {
		public float dt;
		public PhysicsWorld pw;

		[BurstCompile]
		public void Execute(ref Projectile proj, in LocalTransform trans, in PhysicsVelocity pv) {
			if (proj.DidHitThisFrame) return;
			//RaycastHit hit;
			//if (proj.Radius == 0) {
			//	RaycastInput rayInput = new() {
			//		Start = trans.Position,
			//		End = trans.Position - pv.Linear,
			//		Filter = CollisionFilter.Default
			//	};
			//	proj.DidHitThisFrame = pw.CastRay(rayInput, out hit);
			//} else {
			float speed = math.length(pv.Linear);
			if (Util.IsNearZero(speed)) return;
			proj.DidHitThisFrame = pw.SphereCast(
				trans.Position,
				proj.Radius,
				pv.Linear / speed,
				speed * dt,
				out ColliderCastHit hit,
				proj.Filter
			);
			//}
			if (proj.DidHitThisFrame)
				proj.Hit = hit;
		}
	}

}
