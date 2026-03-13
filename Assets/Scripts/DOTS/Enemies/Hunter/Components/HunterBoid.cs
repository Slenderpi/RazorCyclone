using Unity.Entities;
using Unity.Mathematics;

public struct HunterBoid : IComponentData {
	public float3 steerForce;
	public float3 wanderVector;
	public float timeSinceLastWanderStep;
	public float timeSinceBeganFleeing;
}
