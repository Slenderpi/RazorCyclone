using Unity.Entities;
using Unity.Burst;
using UnityEngine;

[UpdateInGroup(typeof(EnemyEndPhysicsGroup))]
partial struct HunterHitSystem : ISystem {
	
	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<Hunter>();
		state.RequireForUpdate<CannonTarget>();
		state.RequireForUpdate<VacuumTarget>();
	}
	
	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		state.Dependency = new HunterHitJob() {
			ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
						   .CreateCommandBuffer(state.WorldUnmanaged)
						   .AsParallelWriter(),
		}.ScheduleParallel(state.Dependency);
	}

	[BurstCompile]
	partial struct HunterHitJob : IJobEntity {
		public EntityCommandBuffer.ParallelWriter ecb;

		[BurstCompile]
		public void Execute(
			[EntityIndexInQuery] int eiiq,
			ref VacuumTarget vacTargetEvents,
			ref CannonTarget canTargetEvents,
			in Entity en,
			in Hunter hunterTag
		) {
			if (vacTargetEvents.TryConsumeKillEvent()) {
				ecb.AddComponent(eiiq, en, new DeadEnemyTag() {
					EnemyType = hunterTag.Form == EEnemyForm.Basic ? EEnemyType.HunterBasic : EEnemyType.Hunter,
					DeathSource = EEnemyDeathSource.Vacuum
				});
				return;
			}
			if (canTargetEvents.IsHit()) {
				ecb.AddComponent(eiiq, en, new DeadEnemyTag() {
					EnemyType = hunterTag.Form == EEnemyForm.Basic ? EEnemyType.HunterBasic : EEnemyType.Hunter,
					DeathSource = canTargetEvents.GetHitAsDeathSource()
				});
				canTargetEvents.ConsumeHitEvent();
			}
		}
	}

}