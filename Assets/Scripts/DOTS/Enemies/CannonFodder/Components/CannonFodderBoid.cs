using Unity.Entities;
using Unity.Mathematics;

public struct CannonFodderBoid : IComponentData {
	public float3 steerForce;
	public float3 wanderVector;
	public float timeSinceLastWanderStep;

	// for debugging. Might delete
	public float3 lastWanderDelta;
}
