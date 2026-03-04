using Unity.Entities;
using UnityEngine;

public class PointCloudRaycastTesterAuthoring : MonoBehaviour {
    
    //
    
    
    
    class Baker : Baker<PointCloudRaycastTesterAuthoring> {
        public override void Bake(PointCloudRaycastTesterAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PointCloudRaycastTester());
            AddComponent(entity, new PointCloudRaycast());
        }
    }
    
}

public struct PointCloudRaycastTester : IComponentData { }
