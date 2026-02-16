using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))] // Required
[UpdateAfter(typeof(TransformSystemGroup))]
[UpdateBefore(typeof(LateSimulationSystemGroup))] // Required
partial struct LifetimeExpireSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<LifetimeComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new LifetimeExpireJob() {
            ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
            dt = SystemAPI.Time.DeltaTime
        }.Schedule(state.Dependency);
    }

    [BurstCompile]
    partial struct LifetimeExpireJob : IJobEntity {
        public EntityCommandBuffer ecb;
        public float dt;

        [BurstCompile]
        public void Execute(ref LifetimeComponent lifetime, in Entity en) {
            if (lifetime.ShouldDieThisUpdate(dt)) {
                ecb.AddComponent<Disabled>(en);
                ecb.DestroyEntity(en);
            }
        }
    }

}
