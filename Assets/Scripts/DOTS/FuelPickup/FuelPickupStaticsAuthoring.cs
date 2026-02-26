using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FuelPickupStaticsAuthoring : MonoBehaviour {

	public float AnimPeakHeight = 0.05f;
	[Tooltip("Time it takes to animate from low to high to low.")]
	public float AnimCycleDuration = 2f;
    [Tooltip("Spin rate in degrees.")]
	public float SpinRate = 180f;
	[Tooltip(@"When a fuel pickup spawns, it will have a bit of a jump.
    - The Y-value determines the vertical velocity.
        The jump's Y will always be exactly this value.
    - The X-value determines the horizontal (x-z)
        velocity. The jump's horizontal velocity
        will lie on a circle formed by this value.")]
	public Vector2 SpawnJumpVelRange = new(1f, 2.5f);



	class Baker : Baker<FuelPickupStaticsAuthoring> {
        public override void Bake(FuelPickupStaticsAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FuelPickupStatics() {
                AnimPeakHeight = auth.AnimPeakHeight,
                AnimCycleDuration = auth.AnimCycleDuration,
                SpinRate = auth.SpinRate * Mathf.Deg2Rad,
                SpawnJumpVelRange = auth.SpawnJumpVelRange
            });
        }
    }
    
}

public struct FuelPickupStatics : IComponentData {
    public float AnimPeakHeight;
	public float AnimCycleDuration;
    /// <summary>
    /// In radians.
    /// </summary>
	public float SpinRate;

    /// <summary>
    /// Calculated value. 
    /// </summary>
    public float elapsedTimeMod;

    public float2 SpawnJumpVelRange;
}
