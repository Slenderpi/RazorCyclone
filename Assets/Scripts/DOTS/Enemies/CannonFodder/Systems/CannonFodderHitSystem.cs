using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

[UpdateInGroup(typeof(EnemyEndPhysicsGroup))]
partial struct CannonFodderHitSystem : ISystem {
	
	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<CannonFodder>();
		state.RequireForUpdate<CannonTarget>();
		state.RequireForUpdate<VacuumTarget>();
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		state.Dependency = new CannonFodderJob() {
			ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
						   .CreateCommandBuffer(state.WorldUnmanaged)
						   .AsParallelWriter(),
		}.ScheduleParallel(state.Dependency);
	}

	[BurstCompile]
	partial struct CannonFodderJob : IJobEntity {
		public EntityCommandBuffer.ParallelWriter ecb;

		[BurstCompile]
		public void Execute(
			[EntityIndexInQuery] int eiiq,
			ref VacuumTarget vacTargetEvents,
			ref CannonTarget canTargetEvents,
			in Entity en,
			in CannonFodder _
		) {
			if (vacTargetEvents.TryConsumeKillEvent()) {
				ecb.AddComponent(eiiq, en, new DeadEnemyTag() {
					EnemyType = EEnemyType.CannonFodder,
					DeathSource = EEnemyDeathSource.Vacuum
				});
				return;
			}
			if (canTargetEvents.IsHit()) {
				ecb.AddComponent(eiiq, en, new DeadEnemyTag() {
					EnemyType = EEnemyType.CannonFodder,
					DeathSource = canTargetEvents.GetHitAsDeathSource()
				});
				canTargetEvents.ConsumeHitEvent();
			}
		}
	}

}
