using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EnemyHitPhysicsGroup))]
//[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
partial struct HurtboxColliderSystem : ISystem {
	
	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<HurtboxCollider>();
	}
	
	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		if (SystemAPI.TryGetSingletonEntity<Player>(out Entity playerEn))
			state.Dependency = new HurtboxColliderJob() {
				PlayerPosition = SystemAPI.GetComponent<LocalTransform>(playerEn).Position
			}.Schedule(state.Dependency);
		else
			state.Dependency = new HurtboxColliderNoPlayerJob().ScheduleParallel(state.Dependency);

		//float3 PlayerPosition = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<Player>()).Position;
		//foreach (var (hc, trans) in SystemAPI.Query<RefRW<HurtboxCollider>, RefRO<LocalTransform>>()) {
		//	hc.ValueRW.Update(Util.DistLeq(PlayerPosition, trans.ValueRO.Position, hc.ValueRO.Radius));
		//}

	}

	[BurstCompile]
	partial struct HurtboxColliderJob : IJobEntity {
		[ReadOnly]
		public float3 PlayerPosition;

		[BurstCompile]
		public void Execute(ref HurtboxCollider hc, [ReadOnly] in LocalTransform trans) {
			hc.Update(math.distancesq(PlayerPosition, trans.Position) <= hc._radiusSq);
		}
	}

	[BurstCompile]
	partial struct HurtboxColliderNoPlayerJob : IJobEntity {
		[BurstCompile]
		public void Execute(ref HurtboxCollider hc) {
			//hc.Update(false);
			hc.ForceSetStateTo(0);
		}
	}
	
}