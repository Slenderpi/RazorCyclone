using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateAfter(typeof(EnemyEndPhysicsGroup))]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(EnemyGeneralPostUpdateGroup))]
partial struct EnemyDeathSystem : ISystem {

    EntityQuery PlayerQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnemyDeathStatics>();
        state.RequireForUpdate<DeadEnemyTag>();
		PlayerQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<Player, PlayerResources>().Build(ref state);
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
  //      state.Dependency = new EnemyDeathJob() {
		//	ecb = SystemAPI
		//.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
		//.CreateCommandBuffer(state.WorldUnmanaged)
		//.AsParallelWriter(),
		//	DeathStatics = SystemAPI.GetSingleton<EnemyDeathStatics>(),
		//	PlrEntity = PlayerQuery.ToEntityArray(Allocator.Temp)[0],
		//	PlrResources = PlayerQuery.ToComponentDataArray<PlayerResources>(Allocator.Temp)[0]

		//}.ScheduleParallel(state.Dependency);

		state.Dependency = new EnemyDeathJob() {
			ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
						   .CreateCommandBuffer(state.WorldUnmanaged)
						   .AsParallelWriter(),
			DeathStatics = SystemAPI.GetSingleton<EnemyDeathStatics>(),
			PlrEntity = PlayerQuery.ToEntityArray(Allocator.Temp)[0],
			PlrResources = PlayerQuery.ToComponentDataArray<PlayerResources>(Allocator.Temp)[0],
			// TODO: fuel pickup prefab
		}.ScheduleParallel(state.Dependency);
    }

	[BurstCompile]
	public void OnDestroy() {
		PlayerQuery.Dispose();
	}

	[BurstCompile]
	partial struct EnemyDeathJob : IJobEntity {
		public EntityCommandBuffer.ParallelWriter ecb;
		public EnemyDeathStatics DeathStatics;
		public Entity PlrEntity;
		public PlayerResources PlrResources;
		public Entity FuelPickupEntity; // TODO

		[BurstCompile]
		public void Execute(
			[EntityIndexInQuery] int eiiq,
			in DeadEnemyTag tag,
			in LocalTransform transform,
			in Entity en
		) {
			if (tag.DeathSource == EEnemyDeathSource.Vacuum) {
				PlayerResources refilled = PlrResources;
				refilled.RefillFuel();
				ecb.SetComponent(eiiq, PlrEntity, refilled);
				Debug.Log("An enemy was killed by the Vacuum!");
			} else if (tag.DeathSource == EEnemyDeathSource.Cannon) {
				Debug.Log("An enemy was killed by the Cannon!");
				//SpawnFuelPickup(eiiq, transform); // TODO
			} else {
				Debug.LogWarning("An enemy was killed by some other death source...???");
			}

			SpawnDeathVfx(eiiq, transform);
		}

		[BurstCompile]
		void SpawnDeathVfx([EntityIndexInQuery] int eiiq, in LocalTransform transform) {
			Entity vfx = ecb.Instantiate(eiiq, DeathStatics.DeathVfx);
			ecb.SetComponent(eiiq, vfx, LocalTransform.FromPositionRotation(
				new() {
					x = transform.Position.x,
					y = transform.Position.y + 1f,
					z = transform.Position.z
				},
				quaternion.identity
			));
		}

		[BurstCompile]
		void SpawnFuelPickup([EntityIndexInQuery] int eiiq, in LocalTransform transform) {
			Entity vfx = ecb.Instantiate(eiiq, FuelPickupEntity);
			ecb.SetComponent(eiiq, vfx, LocalTransform.FromPositionRotation(
				new() {
					x = transform.Position.x,
					y = transform.Position.y + 1f,
					z = transform.Position.z
				},
				quaternion.identity
			));
		}
	}


		//[BurstCompile]
		//partial struct EnemyDeathJob : IJobEntity {
		//    public EntityCommandBuffer.ParallelWriter ecb;
		//    public EnemyDeathStatics DeathStatics;
		//    public Entity PlrEntity;
		//    public PlayerResources PlrResources;

		//    [BurstCompile]
		//    public void Execute(
		//        [EntityIndexInQuery] int entityInQueryIndex,
		//        in DeadEnemyTag tag,
		//        in LocalTransform transform,
		//        in Entity en
		//    ) {
		//        if (tag.DeathSource == EEnemyDeathSource.Vacuum) {
		//            PlayerResources refilled = PlrResources;
		//            refilled.RefillFuelFixedStep();
		//            ecb.SetComponent(entityInQueryIndex, PlrEntity, refilled);
		//        }
		//        if (tag.EnemyType == EEnemyType.CannonFodder) {
		//            switch (tag.DeathSource) {
		//                case EEnemyDeathSource.Vacuum:
		//                    //Debug.Log("Cannon fodder killed by vacuum!");
		//                    break;
		//                case EEnemyDeathSource.Cannon:
		//                    //Debug.Log("Cannon fodder killed by cannon!");
		//                    break;
		//            }
		//        }

		//        SpawnDeathVfx(entityInQueryIndex, transform);

		//        //ecb.AddComponent<Disabled>(entityInQueryIndex, en);
		//        ecb.DestroyEntity(entityInQueryIndex, en);
		//    }

		//    [BurstCompile]
		//    void SpawnDeathVfx([EntityIndexInQuery] int eiqi, in LocalTransform transform) {
		//        Entity vfx = ecb.Instantiate(eiqi, DeathStatics.DeathVfx);
		//        ecb.SetComponent(eiqi, vfx, LocalTransform.FromPositionRotation(
		//            new() {
		//                x = transform.Position.x,
		//                y = transform.Position.y + 1f,
		//                z = transform.Position.z
		//            },
		//            quaternion.identity
		//        ));
		//    }
		//}

	}

[UpdateInGroup(typeof(PresentationSystemGroup))]
partial struct EnemyDeathCleanupSystem : ISystem {
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnemyDeathStatics>();
        state.RequireForUpdate<DeadEnemyTag>();
    }

	[BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new EnemyDeathCleanupJob() {
            ecb = SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>()
                           .CreateCommandBuffer(state.WorldUnmanaged)
                           .AsParallelWriter()
        }.ScheduleParallel(state.Dependency);
	}

    [BurstCompile]
    partial struct EnemyDeathCleanupJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecb;

        [BurstCompile]
        public void Execute([EntityIndexInQuery] int eiiq, in DeadEnemyTag _, in Entity entity) {
            ecb.DestroyEntity(eiiq, entity);
		}
	}

}