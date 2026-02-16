using Unity.Burst;
using Unity.Entities;
using UnityEngine;

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
        new CannonFodderJob() {
			ecb = SystemAPI
				.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
				.CreateCommandBuffer(state.WorldUnmanaged)
                .AsParallelWriter(),
		}.ScheduleParallel();
    }

    [BurstCompile]
    partial struct CannonFodderJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecb;

        [BurstCompile]
        public void Execute(
			[EntityIndexInQuery] int entityInQueryIndex,
			ref VacuumTarget vacTargetEvents,
            in CannonTarget canTargetEvents,
            in CannonFodder cf,
            in Entity en
        ) {
            if (vacTargetEvents.TryConsumeKillEvent()) {
				DeadEnemyTag tag = new() {
                    EnemyType = EEnemyType.CannonFodder,
					DeathSource = EEnemyDeathSource.Vacuum
				};
				ecb.AddComponent(entityInQueryIndex, en, tag);
                return;
            }
            if (canTargetEvents.TryConsumeHitEvent()) {
				DeadEnemyTag tag = new() {
                    EnemyType = EEnemyType.CannonFodder,
					DeathSource = EEnemyDeathSource.Cannon
				};
				ecb.AddComponent(entityInQueryIndex, en, tag);
			}
        }
    }

}
