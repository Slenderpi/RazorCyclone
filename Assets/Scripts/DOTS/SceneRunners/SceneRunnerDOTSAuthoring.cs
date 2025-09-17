using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SceneRunnerDOTSAuthoring : MonoBehaviour {

    public Transform PlayerSpawnTransform;
    
    
    
    class Baker : Baker<SceneRunnerDOTSAuthoring> {
        public override void Bake(SceneRunnerDOTSAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SceneRunnerDOTS(
                auth.PlayerSpawnTransform.position,
                auth.PlayerSpawnTransform.rotation
            ));
        }
    }
    
}



public struct SceneRunnerDOTS : IComponentData {

    public float3 PlayerSpawnPosition;
    public quaternion PlayerSpawnRotation;

    public SceneRunnerDOTS(in float3 playerSpawnPosition, in quaternion playerSpawnRotation) {
        PlayerSpawnPosition = playerSpawnPosition;
        PlayerSpawnRotation = playerSpawnRotation;
    }
    
}