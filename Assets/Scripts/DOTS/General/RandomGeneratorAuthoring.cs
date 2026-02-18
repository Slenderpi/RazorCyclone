using Unity.Entities;
using UnityEngine;

public class RandomGeneratorAuthoring : MonoBehaviour {
    
    // 
    
    
    
    class Baker : Baker<RandomGeneratorAuthoring> {
        public override void Bake(RandomGeneratorAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RandomGenerator()); // (uint)System.DateTime.Now.Ticks
			AddComponent(entity, new RandomGeneratorSeedRequest());
        }
    }
    
}



public struct RandomGenerator : IComponentData {
    public Unity.Mathematics.Random rng;
}

public struct RandomGeneratorSeedRequest : IComponentData, IEnableableComponent { }