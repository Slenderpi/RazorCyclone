using Unity.Entities;
using UnityEngine;

public class PivotAuthoring : MonoBehaviour {



    class Baker : Baker<PivotAuthoring> {
        public override void Bake(PivotAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerPivot());
        }
    }

}



public struct PlayerPivot : IComponentData { }