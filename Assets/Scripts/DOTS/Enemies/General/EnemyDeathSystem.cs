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

		using var eqb = new EntityQueryBuilder(Allocator.Temp);
		PlayerQuery = eqb
			.WithAll<Player, PlayerResources>()
			.Build(ref state);
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		var plrArr = PlayerQuery.ToEntityArray(Allocator.TempJob);
		var rsrcsArr = PlayerQuery.ToComponentDataArray<PlayerResources>(Allocator.TempJob);

		state.Dependency = new EnemyDeathJob() {
			ecbBegPres = SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>()
								  .CreateCommandBuffer(state.WorldUnmanaged)
								  .AsParallelWriter(),
			ecbEndSim = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
								 .CreateCommandBuffer(state.WorldUnmanaged)
								 .AsParallelWriter(),
			DeathStatics = SystemAPI.GetSingleton<EnemyDeathStatics>(),
			PlrEntity = plrArr[0],
			PlrResources = rsrcsArr[0],
			FuelPickupEntity = SystemAPI.GetSingleton<EntityBakerSingleton>().FuelPickup
		}.ScheduleParallel(state.Dependency);

		state.Dependency = plrArr.Dispose(state.Dependency);
		state.Dependency = rsrcsArr.Dispose(state.Dependency);
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
					y = transform.Position.y,
					z = transform.Position.z
				},
				quaternion.identity
			);

			PlayerResources rscs = PlrResources;
			rscs.HealHealth(rscs.HealOnKillAmount);
			if (tag.DeathSource == EEnemyDeathSource.Vacuum) {
				rscs.RefillFuel();
			} else if (tag.DeathSource != EEnemyDeathSource.Lava) {
				SpawnPrefab(eiiq, FuelPickupEntity, effectSpawnTransform);
			}
			ecbEndSim.SetComponent(eiiq, PlrEntity, rscs);

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