using Unity.Entities;
using UnityEngine;

public class FuelPickupAuthoring : MonoBehaviour {

    [SerializeField]
    GameObject PivotReference;
    
    
    
    class Baker : Baker<FuelPickupAuthoring> {
        public override void Bake(FuelPickupAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FuelPickup());
        }
    }
    
}

public struct FuelPickup : IComponentData {
    /// <summary>
    /// Fuel pickups spawn with an upward jump. This boolean determines if it's done it yet.<br/>
    /// The jump is applied by FuelPickupSystem.
    /// </summary>
    public bool hasJumped;
}
