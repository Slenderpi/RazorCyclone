using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public static class BoidUtil {

	public readonly static Unity.Mathematics.Random rng = new Unity.Mathematics.Random(1);



	///////////////////////////////////////////////////// BOID STEERING

	public static float3 Seek(float3 pos, float3 targetPos, float3 velocity, float maxSteeringVelocity, float maxSteeringForce) {
		// Find desired velocity via |targ - pos| * maxSteerVel
		// Steer force is then desired - currVel, clamped by maxSteerForce
		float3 steeringForce = math.normalize(targetPos - pos) * maxSteeringVelocity - velocity;
		if (math.lengthsq(steeringForce) > maxSteeringForce * maxSteeringForce)
			steeringForce = math.normalize(steeringForce) * maxSteeringForce;
		return steeringForce;
	}

	public static float3 Wander(float3 pos, float3 velocity, float3 wanderPoint, float wanderLimitDist, float maxSteeringVelocity, float maxWanderForce) {
		return Seek(pos, pos + wanderLimitDist * math.normalize(velocity) + wanderPoint, velocity, maxSteeringVelocity, maxWanderForce);
	}

	public static float3 StepWanderPoint2D(float3 wanderPoint, float wanderLimitRadius, float wanderChangeDist, Unity.Mathematics.Random rng) {
		wanderPoint.x += (rng.NextFloat(0, 2) * 2 - 1) * wanderChangeDist;
		wanderPoint.z += (rng.NextFloat(0, 2) * 2 - 1) * wanderChangeDist;
		return wanderPoint * wanderLimitRadius / math.length(wanderPoint);
	}

	/// <summary>
	/// For initializing boid components.
	/// </summary>
	public static class BoidBuilder {

		public static CannonFodderBoid CannonFodder() {
			return new() {
				wanderPoint = float3.zero,
				timeSinceLastWanderStep = 0f,
				steerForce = float3.zero
			};
		}

	}

	/// <summary>
	/// For initializing boid statics from scriptable objects.
	/// </summary>
	public static class StaticsBuilder {

		public static CannonFodderBoidStatics CannonFodder(SO_CannonFodder so) {
			return new CannonFodderBoidStatics() {
				MaxSteeringVelocity = so.MaxSteeringVelocity,
				MaxSteeringForce = so.MaxSteeringForce,
				WanderLimitRadius = so.WanderLimitRadius,
				WanderLimitDist = so.WanderLimitDist,
				WanderChangeDist = so.WanderChangeDist,
				MaxWanderForce = so.MaxWanderForce,
				WanderMinimumDelay = so.WanderMinimumDelay,
				AvoidanceTestType = so.AvoidanceTestType,
				AvoidanceMaxLookDist = so.AvoidanceMaxLookDist,
				AvoidanceWhiskerAngle = so.AvoidanceWhiskerAngle,
				AvoidanceMinIntensity = so.AvoidanceMinIntensity,
				AvoidanceMaxIntensity = so.AvoidanceMaxIntensity,
				AvoidanceMaxSteeringForce = so.MaxSteeringForce,
				AvoidInvisBoidWalls = so.AvoidInvisBoidWalls,

				FleeTriggerDistance = so.FleeTriggerDistance,
				FleeForce = so.FleeForce
			};
		}

	}

}
