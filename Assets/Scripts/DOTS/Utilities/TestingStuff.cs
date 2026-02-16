using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public class TestingStuff : MonoBehaviour {

    public GameObject EnemyToSpawn;
    public Vector2 Corner1 = new(-55, -9);
    public float SpawnOffset = 2;
    public int CountPerSide = 16;



	class Baker : Baker<TestingStuff> {
		public override void Bake(TestingStuff auth) {
			Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new TestingStuffComponent() {
                EnemyToSpawn = GetEntity(auth.EnemyToSpawn, TransformUsageFlags.Dynamic),
                SpawnStartCorner = new(auth.Corner1.x, 0, auth.Corner1.y),
                SpawnOffset = auth.SpawnOffset,
                CountPerSide = auth.CountPerSide
            });
		}
	}

}

public struct TestingStuffComponent : IComponentData {
    public Entity EnemyToSpawn;
    public bool HasSpawned;
    public float3 SpawnStartCorner;
    public float SpawnOffset;
    public int CountPerSide;
}

partial struct TestingStuffSystem : ISystem {

    ComponentLookup<LocalTransform> TransformLookup;

	[BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<TestingStuffComponent>();
        TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(false);
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.TryGetSingletonEntity<TestingStuffComponent>(out Entity en))
            return;
        TestingStuffComponent tsc = SystemAPI.GetComponent<TestingStuffComponent>(en);
        if (tsc.HasSpawned)
            return;
        tsc.HasSpawned = true;
        SystemAPI.SetComponent(en, tsc);
        TransformLookup.Update(ref state);
        int SpawnCount = Util.pow3(tsc.CountPerSide);
		Debug.Log($"TestingStuff: Spawning {SpawnCount} entities!");
		NativeArray<Entity> entities = state.EntityManager.Instantiate(tsc.EnemyToSpawn, SpawnCount, Allocator.TempJob);
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
