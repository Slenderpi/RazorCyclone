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
		PlayerQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<Player, PlayerResources>().Build(ref state);
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		state.Dependency = new EnemyDeathJob() {
			ecb = SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>()
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
				Debug.Log("EnemyDeathSystem: An enemy was killed by the Vacuum!");
			} else if (tag.DeathSource == EEnemyDeathSource.Cannon) {
				Debug.Log("EnemyDeathSystem: An enemy was killed by the Cannon!"); // After Update()
				//SpawnFuelPickup(eiiq, transform); // TODO
			}

			SpawnDeathVfx(eiiq, transform);

			ecb.DestroyEntity(eiiq, en);
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

}