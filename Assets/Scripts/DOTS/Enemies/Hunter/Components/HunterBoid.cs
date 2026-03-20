using Unity.Entities;
using Unity.Mathematics;

public struct HunterBoid : IComponentData, IEnableableComponent {
	public float3 steerForce;
	public float3 wanderVector;
	/// <summary>
	/// The rotation the Hunter should have based on its forward and steerForce.<br/>
	/// Computed by HunterBoidSystem, used by HunterRotationSystem.
	/// </summary>
	public quaternion currentDesiredRot;
	public float timeSinceLastWanderStep;
	public float timeSinceBeganFleeing;
}
