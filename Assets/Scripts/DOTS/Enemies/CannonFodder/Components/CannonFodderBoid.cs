using Unity.Entities;
using Unity.Mathematics;

public struct CannonFodderBoid : IComponentData {
	public float3 wanderPoint;
	public float timeSinceLastWanderStep;
	public float3 steerForce;
}
