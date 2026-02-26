using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct EnemyDeathSystem : ISystem {

    EntityQuery PlayerQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnemyDeathStatics>();
        state.RequireForUpdate<DeadEnemyTag>();
		state.RequireForUpdate<EntityBakerSingleton>();
		PlayerQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<Player, PlayerResources>().Build(ref state);
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		state.Dependency = new EnemyDeathJob() {
			ecbBegPres = SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>()
								  .CreateCommandBuffer(state.WorldUnmanaged)
								  .AsParallelWriter(),
			ecbEndSim = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
								 .CreateCommandBuffer(state.WorldUnmanaged)
								 .AsParallelWriter(),
			DeathStatics = SystemAPI.GetSingleton<EnemyDeathStatics>(),
			PlrEntity = PlayerQuery.ToEntityArray(Allocator.Temp)[0],
			PlrResources = PlayerQuery.ToComponentDataArray<PlayerResources>(Allocator.Temp)[0],
			FuelPickupEntity = SystemAPI.GetSingleton<EntityBakerSingleton>().FuelPickup
		}.ScheduleParallel(state.Dependency);
    }

	[BurstCompile]
	public void OnDestroy() {
		PlayerQuery.Dispose();
	}

	[BurstCompile]
	partial struct EnemyDeathJob : IJobEntity {
		public EntityCommandBuffer.ParallelWriter ecbBegPres;
		public EntityCommandBuffer.ParallelWriter ecbEndSim;
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
			LocalTransform effectSpawnTransform = LocalTransform.FromPositionRotation(
				new() {
					x = transform.Position.x,
					y = transform.Position.y + 1f,
					z = transform.Position.z
				},
				quaternion.identity
			);
			if (tag.DeathSource == EEnemyDeathSource.Vacuum) {
				PlayerResources rscs = PlrResources;
				rscs.RefillFuel();
				rscs.HealHealth(rscs.HealOnKillAmount);
				ecbEndSim.SetComponent(eiiq, PlrEntity, rscs);
			} else if (tag.DeathSource == EEnemyDeathSource.Cannon) {
				PlayerResources rscs = PlrResources;
				rscs.HealHealth(rscs.HealOnKillAmount);
				ecbEndSim.SetComponent(eiiq, PlrEntity, rscs);
				SpawnPrefab(eiiq, FuelPickupEntity, effectSpawnTransform);
			}

			SpawnPrefab(eiiq, DeathStatics.DeathVfx, effectSpawnTransform);

			ecbBegPres.DestroyEntity(eiiq, en);
		}

		[BurstCompile]
		void SpawnPrefab([EntityIndexInQuery] int eiiq, in Entity en, in LocalTransform spawnTrans) {
			Entity spawned = ecbEndSim.Instantiate(eiiq, en);
			ecbEndSim.SetComponent(eiiq, spawned, spawnTrans);
		}
	}

}