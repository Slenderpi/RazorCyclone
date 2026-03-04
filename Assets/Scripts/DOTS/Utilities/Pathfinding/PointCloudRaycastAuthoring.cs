using Unity.Entities;
using UnityEngine;

public class PointCloudRaycastAuthoring : MonoBehaviour {

	class Baker : Baker<PointCloudRaycastAuthoring> {
        public override void Bake(PointCloudRaycastAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PointCloudRaycast());
        }
    }
}



/// <summary>
/// For entities that need to check LOS with the player, but want to check it by
/// performing a raycast in the point cloud.<br/>
/// The position of the owning entity is clamped and normalized to a voxel in the point cloud.<br/>
/// The owning component must have a LocalTransform representing their world position.
/// </summary>
public struct PointCloudRaycast : IComponentData, IEnableableComponent {
    public bool HasLos;
}