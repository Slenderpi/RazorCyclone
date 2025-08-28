using Unity.Entities;
using Unity.Mathematics;

public struct GeneralBoid : IComponentData {
	public float MaxSteeringVelocity;
	public float MaxSteeringForce;
	public float WanderLimitRadius;
	public float WanderLimitDist;
	public float WanderChangeDist;
	public float MaxWanderForce;
	public float WanderMinimumDelay;
	public AvoidanceTestMode AvoidanceTestType;
	public float AvoidanceMaxLookDist;
	public float AvoidanceWhiskerAngle;
	public float AvoidanceMinIntensity;
	public float AvoidanceMaxIntensity;
	public float AvoidanceMaxSteeringForce;
	public bool AvoidInvisBoidWalls;

	public float3 wanderPoint;
	public float lastWanderStepTime;
}