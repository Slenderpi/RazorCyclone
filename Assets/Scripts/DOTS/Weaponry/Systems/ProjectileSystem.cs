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
		new ProjectileJob() {
			dt = SystemAPI.Time.DeltaTime,
			pw = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld
		}.Schedule();
	}

	[BurstCompile]
	partial struct ProjectileJob : IJobEntity {
		public float dt;
		public PhysicsWorld pw;

		[BurstCompile]
		public void Execute(ref Projectile proj, in LocalToWorld ltw, in PhysicsVelocity pv) {
			if (proj.DidHitThisFrame) return;
			//RaycastHit hit;
			//if (proj.Radius == 0) {
			//	RaycastInput rayInput = new() {
			//		Start = ltw.Position,
			//		End = ltw.Position - pv.Linear,
			//		Filter = CollisionFilter.Default
			//	};
			//	proj.DidHitThisFrame = pw.CastRay(rayInput, out hit);
			//} else {
			float speed = math.length(pv.Linear);
			if (speed <= 0.00001f) return;
			proj.DidHitThisFrame = pw.SphereCast(
				ltw.Position,
				proj.Radius,
				pv.Linear / speed,
				speed * dt,
				out ColliderCastHit hit,
				proj.Filter
			);
			//}
			if (proj.DidHitThisFrame)
				proj.Hit = hit;
			//	proj.Hit = hit;
		}
	}

}
