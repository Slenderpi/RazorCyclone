using Unity.Entities;
using Unity.Mathematics;

// might delete
//public struct GeneralBoid : IComponentData {
//	public float MaxSteeringVelocity;
//	public float MaxSteeringForce;
//	public float WanderLimitRadius;
//	public float WanderLimitDist;
//	public float WanderChangeDist;
//	public float MaxWanderForce;
//	public float WanderMinimumDelay;
//	public AvoidanceTestMode AvoidanceTestType;
//	public float AvoidanceMaxLookDist;
//	public float AvoidanceWhiskerAngle;
//	public float AvoidanceMinIntensity;
//	public float AvoidanceMaxIntensity;
//	public float AvoidanceMaxSteeringForce;
//	public bool AvoidInvisBoidWalls;

//	public float3 wanderPoint;
//	public float lastWanderStepTime;
//}

public struct GeneralBoidProperties {
	public float MaxSteeringVelocity;
	public float MaxSteeringForce;

	/// <summary>
	/// The length of wanderVector.
	/// 
	/// Wander works as follows:
	/// - Compute random wanderDelta vector, then set its length to WanderChangeDist
	/// - Take the previous computed wander vector, prevWanderVector, and add wanderDelta to it. This becomes the new wanderVector
	/// - Perform the above steps every WanderMinimumDelay seconds to compute the new WanderVector
	/// - Compute wanderVectorEnd, the starting point of the WanderVector, which is at norm(velocity) * WanderLimitDist
	/// - Every frame, Seek towards the position at Position + wanderVectorEnd + WanderVector
	/// </summary>
	public float WanderLimitRadius;

	/// <summary>
	/// The distance from the Boid's current position, in the direction its velocity. The wanderVector starts from this position.
	/// 
	/// Wander works as follows:
	/// - Compute random wanderDelta vector, then set its length to WanderChangeDist
	/// - Take the previous computed wander vector, prevWanderVector, and add wanderDelta to it. This becomes the new wanderVector
	/// - Perform the above steps every WanderMinimumDelay seconds to compute the new WanderVector
	/// - Compute wanderVectorEnd, the starting point of the WanderVector, which is at norm(velocity) * WanderLimitDist
	/// - Every frame, Seek towards the position at Position + wanderVectorEnd + WanderVector
	/// </summary>
	public float WanderLimitDist;

	/// <summary>
	/// The length of the wanderDelta vector.
	/// 
	/// Wander works as follows:
	/// - Compute random wanderDelta vector, then set its length to WanderChangeDist
	/// - Take the previous computed wander vector, prevWanderVector, and add wanderDelta to it. This becomes the new wanderVector
	/// - Perform the above steps every WanderMinimumDelay seconds to compute the new WanderVector
	/// - Compute wanderVectorEnd, the starting point of the WanderVector, which is at norm(velocity) * WanderLimitDist
	/// - Every frame, Seek towards the position at Position + wanderVectorEnd + WanderVector
	/// </summary>
	public float WanderChangeDist;

	// might discard
	public float MaxWanderForce;

	/// <summary>
	/// The delay (in seconds) for computing a new wanderVector.
	/// 
	/// Wander works as follows:
	/// - Compute random wanderDelta vector, then set its length to WanderChangeDist
	/// - Take the previous computed wander vector, prevWanderVector, and add wanderDelta to it. This becomes the new wanderVector
	/// - Perform the above steps every WanderMinimumDelay seconds to compute the new wanderVector
	/// - Compute wanderVectorEnd, the starting point of the wanderVector, which is at norm(velocity) * WanderLimitDist
	/// - Every frame, Seek towards the position at Position + wanderVectorEnd + wanderVector
	/// </summary>
	public float WanderMinimumDelay;

	public AvoidanceTestMode AvoidanceTestType;
	public float AvoidanceMaxLookDist;
	public float AvoidanceWhiskerAngle;
	public float AvoidanceMinIntensity;
	public float AvoidanceMaxIntensity;
	public float AvoidanceMaxSteeringForce;
	public bool AvoidInvisBoidWalls;
}