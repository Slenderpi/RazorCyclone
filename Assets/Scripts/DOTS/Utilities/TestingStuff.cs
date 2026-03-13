using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public class TestingStuff : MonoBehaviour {

    [Header("Enemy spawning")]
    [Tooltip("TestingStuff spawns a cube of enemies.\nThis value determines which enemy to spawn.\nNote: an error will be printed if the selected value is currently unspawnable.")]
    public EEnemyType EnemyTypeToSpawn;
    [Tooltip("TestingStuff spawns a cube of enemies.\nThe position of this cube starts at this corner and expands in the positive global axes. The Y value of Corner1 is meant to be the Z position of the cube.")]
    public Vector2 Corner1 = new(-55, -9);
    [Min(0)]
    [Tooltip("Distance between each enemy when spawning them.")]
    public float SpawnOffset = 2;
    [Min(0)]
    [Tooltip("TestingStuff spawns a cube of enemies.\nThe number of enemies per side is CountPerSide, meaning it will spawn a total of CountPerSide ^ 3 enemies.\n\nExample: CountPerSide=4, Total=64.")]
    public int CountPerSide = 16;

    [Header("Fuel spawning")]
    [Tooltip("If true, TestingStuff will spawn a fuel pickup every FuelPickupSpawnDelay seconds.")]
    public bool ShouldSpawnFuelPickups = true;
    [Tooltip("Location to spawn the fuel pickup.")]
    public Vector3 FuelPickupSpawnLocation = new(0, 10, 16f);
    [Tooltip("The delay (in seconds) to spawn a fuel pickup.")]
    [Min(0)]
    public float FuelPickupSpawnDelay = 2f;



	class Baker : Baker<TestingStuff> {
		public override void Bake(TestingStuff auth) {
			Entity entity = GetEntity(TransformUsageFlags.None);
            int cps = auth.CountPerSide;
            if (cps < 0) {
                Debug.LogError($"TestingStuff: CountPerSide cannot be a negative value. Was provided: {cps}. Defaulting to CountPerSide=1");
                cps = 1;
            }
			AddComponent(entity, new TestingStuffComponent() {
				EnemyTypeToSpawn = auth.EnemyTypeToSpawn,
				SpawnStartCorner = new(auth.Corner1.x, 0, auth.Corner1.y),
                SpawnOffset = auth.SpawnOffset,
                CountPerSide = cps,

                ShouldSpawnFuelPickups = auth.ShouldSpawnFuelPickups,
                FuelPickupSpawnLocation = auth.FuelPickupSpawnLocation,
                FuelPickupSpawnDelay = auth.FuelPickupSpawnDelay
            });
		}
	}

}

public struct TestingStuffComponent : IComponentData {
    public EEnemyType EnemyTypeToSpawn;
	public bool HasSpawned;
    public float3 SpawnStartCorner;
    public float SpawnOffset;
    public int CountPerSide;

    public bool ShouldSpawnFuelPickups;
    public float3 FuelPickupSpawnLocation;
    public float FuelPickupSpawnDelay;
}

partial struct TestingStuffSystem : ISystem {

    ComponentLookup<LocalTransform> TransformLookup;
    float timeSinceLastFuelSpawn;

	[BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<TestingStuffComponent>();
        state.RequireForUpdate<EntityBakerSingleton>();
        TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(false);
        timeSinceLastFuelSpawn = float.MaxValue;
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        EntityBakerSingleton ebs = SystemAPI.GetSingleton<EntityBakerSingleton>();
        Entity tscEntity = SystemAPI.GetSingletonEntity<TestingStuffComponent>();
		TestingStuffComponent tsc = SystemAPI.GetComponent<TestingStuffComponent>(tscEntity);
		TransformLookup.Update(ref state);

		if (!tsc.HasSpawned) {
			tsc.HasSpawned = true;
			SystemAPI.SetComponent(tscEntity, tsc);
			if (tsc.CountPerSide == 0) {
                Debug.Log($"TestingStuff: the SpawnCount was set to 0. No enemies will be spawned.");
				return;
			}
			SpawnEnemiesToSpawn(ref state, ebs, tsc);
        }
        if (tsc.ShouldSpawnFuelPickups) {
            if (timeSinceLastFuelSpawn >= tsc.FuelPickupSpawnDelay) {
                SpawnFuelPickup(ref state, ebs, tsc.FuelPickupSpawnLocation);
                timeSinceLastFuelSpawn = 0;
            }
            timeSinceLastFuelSpawn += SystemAPI.Time.DeltaTime;
        }
    }

	[BurstCompile]
	void SpawnEnemiesToSpawn(ref SystemState state, in EntityBakerSingleton ebs, in TestingStuffComponent tsc) {
		int SpawnCount = Util.pow3(tsc.CountPerSide);
		if (!ebs.TryGetEnemyPrefabById(tsc.EnemyTypeToSpawn, out Entity entityToSpawn))
			return;
		NativeArray<Entity> entities = state.EntityManager.Instantiate(entityToSpawn, SpawnCount, Allocator.TempJob);
		Debug.Log($"TestingStuff: Spawning {SpawnCount} {Util.EEnemyTypeName(tsc.EnemyTypeToSpawn)}s!");
		TestStuffJob job = new() {
			Entities = entities,
			TransformLookup = TransformLookup,
			SpawnStartCorner = tsc.SpawnStartCorner,
			SpawnOffset = tsc.SpawnOffset,
			CountPerSide = tsc.CountPerSide
		};
		state.Dependency = job.Schedule(entities.Length, 64, state.Dependency);
		entities.Dispose(state.Dependency);
	}

    [BurstCompile]
	void SpawnFuelPickup(ref SystemState state, in EntityBakerSingleton ebs, in float3 spawnLocation) {
        Entity fuel = state.EntityManager.Instantiate(ebs.FuelPickup);
        state.EntityManager.SetComponentData(fuel, new LocalTransform() {
            Position = spawnLocation,
            Rotation = quaternion.identity,
            Scale = 1f
        });
	}

	[BurstCompile]
    partial struct TestStuffJob : IJobParallelFor {
        [ReadOnly]
        public NativeArray<Entity> Entities;
		[NativeDisableParallelForRestriction]
		public ComponentLookup<LocalTransform> TransformLookup;
		public float3 SpawnStartCorner;
		public float SpawnOffset;
        public int CountPerSide;

		[BurstCompile]
        public void Execute(int index) {
            Entity en = Entities[index];
            if (TransformLookup.HasComponent(en)) {
                LocalTransform trans = TransformLookup[en];
                trans.Position = new() {
					x = SpawnStartCorner.x + SpawnOffset * (index % CountPerSide),
					y = SpawnStartCorner.y + SpawnOffset * (index / Util.pow2(CountPerSide) % CountPerSide),
					z = SpawnStartCorner.z + SpawnOffset * (index / CountPerSide % CountPerSide)
				};
                TransformLookup[en] = trans;
            }
        }
    }

}
