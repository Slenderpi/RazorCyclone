using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class EntityTimer : MonoBehaviour {

    public float Lifetime = 5;
    
    
    
    class Baker : Baker<EntityTimer> {
        public override void Bake(EntityTimer auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntityLifetime(auth.Lifetime));
        }
    }
    
}



public struct EntityLifetime : IComponentData {
    public float LifetimeRemaining;

    public EntityLifetime(float startLifetime) {
        LifetimeRemaining = startLifetime;
    }
}



[UpdateInGroup(typeof(LateSimulationSystemGroup))]
//[UpdateAfter(typeof(TransformSystemGroup))]
//[UpdateBefore(typeof(LateSimulationSystemGroup))]
public partial struct EntityTimerSystem : ISystem {

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EntityLifetime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		new EntityLifetimeJob {
			ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
			DeltaTime = SystemAPI.Time.DeltaTime
		}.Schedule();
	}

    [BurstCompile]
    partial struct EntityLifetimeJob : IJobEntity {
        public EntityCommandBuffer ecb;
        public float DeltaTime;

        public void Execute(in Entity entity, ref EntityLifetime entityLifetime) {
			entityLifetime.LifetimeRemaining -= DeltaTime;
            if (entityLifetime.LifetimeRemaining <= 0)
                ecb.DestroyEntity(entity);
        }
    }

}