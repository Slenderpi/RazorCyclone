using Unity.Entities;
using UnityEngine;

public class PlayerCameraTransformAuthoring : MonoBehaviour {
    
    // 
    
    
    
    class Baker : Baker<PlayerCameraTransformAuthoring> {
        public override void Bake(PlayerCameraTransformAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerCameraTransform());
        }
    }
    
}



public struct PlayerCameraTransform : IComponentData { }