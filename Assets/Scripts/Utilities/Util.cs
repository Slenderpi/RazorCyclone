using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class Util {

	public readonly static Unity.Mathematics.Random rng = new Unity.Mathematics.Random(1);



	readonly static GeneralBoid DEFAULT_GENERAL_BOID = new GeneralBoid() {
		MaxSteeringVelocity = 15,
		MaxSteeringForce = 10,
		WanderLimitRadius = 0.5f,
		WanderLimitDist = 0.5f,
		WanderChangeDist = 0.15f,
		MaxWanderForce = 5,
		WanderMinimumDelay = 0,
		AvoidanceTestType = AvoidanceTestMode.None,
		AvoidanceMaxLookDist = 4,
		AvoidanceWhiskerAngle = 30f,
		AvoidanceMinIntensity = 1,
		AvoidanceMaxIntensity = 10,
		AvoidanceMaxSteeringForce = 10,
		AvoidInvisBoidWalls = false
	};

	public static GeneralBoid BuildGeneralBoidComponentDataDefault(GeneralBoidSO so) {
		return DEFAULT_GENERAL_BOID;
	}

	public static GeneralBoid BuildGeneralBoidComponentDataFromSO(GeneralBoidSO so) {
		return new GeneralBoid() {
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

			wanderPoint = float3.zero,
			lastWanderStepTime = -1000f
		};
	}

	public static float3 StepWanderPoint2D(float3 prevWanderPoint, float lastWanderStepTime, float wanderMinimumDelay, float wanderLimitRadius, float wanderChangeDist) {
		return BoidSteerer.StepWanderPoint2D(prevWanderPoint, wanderLimitRadius, wanderChangeDist);
	}

	public static uint GenerateSeed(Transform transform) {
		// uh um random weird code go
		float val = 1 + (
			transform.position.x +
			transform.position.y * -transform.position.y +
			transform.position.z * transform.position.z * transform.position.z
		);
		if (val <= 0) val = -val * 3 + 1;
		return (uint)val;
	}



	///////////////////////////////////////////////////// BOIDS
	
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

}