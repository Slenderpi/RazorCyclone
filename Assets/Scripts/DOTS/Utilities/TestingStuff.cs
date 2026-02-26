using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public class TestingStuff : MonoBehaviour {

    [Tooltip("TestingStuff spawns a cube of enemies.\nThis value determines which enemy to spawn.\nNote: an error will be printed if the selected value is currently unspawnable.")]
    public EEnemyType EnemyTypeToSpawn;
    [Tooltip("TestingStuff spawns a cube of enemies.\nThe position of this cube starts at this corner and expands in the positive global axes. The Y value of Corner1 is meant to be the Z position of the cube.")]
    public Vector2 Corner1 = new(-55, -9);
    [Min(0)]
    [Tooltip("Distance between each enemy when spawning them.")]
    public float SpawnOffset = 2;
    [Min(1)]
    [Tooltip("TestingStuff spawns a cube of enemies.\nThe number of enemies per side is CountPerSide, meaning it will spawn a total of CountPerSide ^ 3 enemies.\n\nExample: CountPerSide=4, Total=64.")]
    public int CountPerSide = 16;



	class Baker : Baker<TestingStuff> {
		public override void Bake(TestingStuff auth) {
			Entity entity = GetEntity(TransformUsageFlags.None);
            int cps = auth.CountPerSide;
            if (cps <= 0) {
                Debug.LogError($"TestingStuff: CountPerSide cannot be a non-positive value. Was provided: {cps}. Defaulting to CountPerSide=1");
                cps = 1;
            }
			AddComponent(entity, new TestingStuffComponent() {
				EnemyTypeToSpawn = auth.EnemyTypeToSpawn,
				SpawnStartCorner = new(auth.Corner1.x, 0, auth.Corner1.y),
                SpawnOffset = auth.SpawnOffset,
                CountPerSide = cps
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
}

partial struct TestingStuffSystem : ISystem {

    ComponentLookup<LocalTransform> TransformLookup;

	[BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<TestingStuffComponent>();
        state.RequireForUpdate<EntityBakerSingleton>();
        TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(false);
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.TryGetSingletonEntity<TestingStuffComponent>(out Entity en))
            return;
        if (!SystemAPI.TryGetSingleton(out EntityBakerSingleton ebs))
            return;
        TestingStuffComponent tsc = SystemAPI.GetComponent<TestingStuffComponent>(en);
        if (tsc.HasSpawned)
            return;
        tsc.HasSpawned = true;
        SystemAPI.SetComponent(en, tsc);
        TransformLookup.Update(ref state);
        int SpawnCount = Util.pow3(tsc.CountPerSide);
        if (!ebs.TryGetEntityPrefabById(tsc.EnemyTypeToSpawn, out Entity entityToSpawn))
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
