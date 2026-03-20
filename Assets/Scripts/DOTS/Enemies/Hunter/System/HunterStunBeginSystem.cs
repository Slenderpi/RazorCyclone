using Unity.Entities;
using Unity.Burst;
using UnityEngine;
using Unity.Physics;

[UpdateInGroup(typeof(EnemyHitPhysicsGroup))]
[UpdateAfter(typeof(HunterHitSystem))]
partial struct HunterStunBeginSystem : ISystem {
	
	//ComponentLookup<>

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		//state.RequireForUpdate<HunterEmpoweredStatics>();
		state.RequireForUpdate<HunterEmpowered>();
		
	}
	
	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		state.Dependency = new HunterStunBeginJob() {
			ElapsedTime = (float)SystemAPI.Time.ElapsedTime
		}.Schedule(state.Dependency);
	}

	[BurstCompile]
	partial struct HunterStunBeginJob : IJobEntity {
		public float ElapsedTime;

		[BurstCompile]
		public void Execute(
			in HunterEmpowered hunter,
			ref PhysicsGravityFactor gravityFactor,
			EnabledRefRW<HunterBoid> enrHunterBoid
		) {
			if (!hunter.IsStunned || !Util.IsNearZero(ElapsedTime - hunter.LastStunTime))
				return;
			gravityFactor.Value = 1f;
			enrHunterBoid.ValueRW = false;
			// TODO: Change hunter material, disable trail vfx (or maybe do this via gameObject?)
		}
	}
	
}