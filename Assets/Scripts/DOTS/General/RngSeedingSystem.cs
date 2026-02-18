using Unity.Entities;
using Unity.Burst;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(BeginInitializationEntityCommandBufferSystem))]
partial struct RngSeedingSystem : ISystem {
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<RandomGeneratorSeedRequest>();
        state.RequireForUpdate<RandomGenerator>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		state.Dependency = new RngSeedRequestJob() {
            BaseSeed = (uint)(SystemAPI.Time.ElapsedTime * 1000f)
		}.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    partial struct RngSeedRequestJob : IJobEntity {
        public uint BaseSeed;

        [BurstCompile]
        public void Execute(
			[EntityIndexInQuery] int entityIndexInQuery,
			ref RandomGenerator rng,
            EnabledRefRW<RandomGeneratorSeedRequest> request,
            in Entity entity
        ) {
            // We mix the Entity's permanent Index with the current Time
            // The 0x9E3779B9 constant (related to the Golden Ratio) is a bit-mixer to scatter bits
            uint seed = (BaseSeed ^ (uint)entity.Index) * 0x9E3779B9;
			rng.rng = new Unity.Mathematics.Random(seed != 0 ? seed : 1);

			request.ValueRW = false;
        }
    }
    
}