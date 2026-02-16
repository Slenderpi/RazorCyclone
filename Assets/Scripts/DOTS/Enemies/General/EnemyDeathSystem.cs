using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EnemyEndPhysicsGroup))]
partial struct EnemyDeathSystem : ISystem {

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<DeadEnemyTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.TryGetSingleton(out EnemyDeathStatics DeathStatics)) {
            Debug.LogWarning("EnemyDeathSystem tried to get EnemyDeathStatics but couldn't find it.");
            return;
        }
        state.Dependency = new EnemyDeathJob() {
			ecb = SystemAPI
				.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
				.CreateCommandBuffer(state.WorldUnmanaged)
				.AsParallelWriter(),
			DeathStatics = DeathStatics
		}.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    partial struct EnemyDeathJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecb;
        public EnemyDeathStatics DeathStatics;

        [BurstCompile]
        public void Execute(
            [EntityIndexInQuery] int entityInQueryIndex,
            in DeadEnemyTag tag,
            in LocalTransform transform,
            in Entity en
        ) {
            if (tag.EnemyType == EEnemyType.CannonFodder) {
                switch (tag.DeathSource) {
                    case EEnemyDeathSource.Vacuum:
                        //Debug.Log("Cannon fodder killed by vacuum!");
                        break;
                    case EEnemyDeathSource.Cannon:
                        //Debug.Log("Cannon fodder killed by cannon!");
                        break;
                }
            }

            SpawnDeathVfx(entityInQueryIndex, transform);

			//ecb.AddComponent<Disabled>(entityInQueryIndex, en);
			ecb.DestroyEntity(entityInQueryIndex, en);
		}

        [BurstCompile]
        void SpawnDeathVfx([EntityIndexInQuery] int eiqi, in LocalTransform transform) {
			Entity vfx = ecb.Instantiate(eiqi, DeathStatics.DeathVfx);
			ecb.SetComponent(eiqi, vfx, LocalTransform.FromPositionRotation(
				new() {
					x = transform.Position.x,
					y = transform.Position.y + 1f,
					z = transform.Position.z
				},
				quaternion.identity
			));
		}
    }

}
