using Unity.Entities;
using UnityEngine;

public class FuelPickupPivotAuthoring : MonoBehaviour {
    
    //
    
    
    
    class Baker : Baker<FuelPickupPivotAuthoring> {
        public override void Bake(FuelPickupPivotAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FuelPickupPivot());
        }
    }
    
}

public struct FuelPickupPivot : IComponentData { }
