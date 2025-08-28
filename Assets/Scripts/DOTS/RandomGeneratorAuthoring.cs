using Unity.Entities;
using UnityEngine;

public class RandomGeneratorAuthoring : MonoBehaviour {
    
    // 
    
    
    
    class Baker : Baker<RandomGeneratorAuthoring> {
        public override void Bake(RandomGeneratorAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RandomGenerator() {
                rng = new Unity.Mathematics.Random(Util.GenerateSeed(authoring.transform))
            });
        }
    }
    
}



public struct RandomGenerator : IComponentData {
    public Unity.Mathematics.Random rng;
}