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
		float3 pos = transform.position;
		float val = (
			pos.x +
			pos.y * -pos.y +
			pos.z * pos.z * pos.z
		);
		if (val < 0) val = 1 + -val * 3;
		if (val < 1) {
			float3 rot = math.EulerXYZ(transform.rotation);
			val = 1 + (
				47 * (math.abs(rot.x) + math.abs(pos.y) + 1) +
				11 * (math.abs(rot.y) + math.abs(pos.z) + 1) +
				71 * (math.abs(rot.z) + math.abs(pos.z) + 1
			));
		}
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



	///////////////////////////////////////////////////// DEBUGGING
	
	public static void D_DrawPoint(float3 position, Color c) {
		D_DrawPoint(position, c, 9999999f);
	}

	public static void D_DrawPoint(float3 position, Color c, float t) {
		D_DrawPoint(position, c, t, 0.15f, false);
	}

	public static void D_DrawPoint(float3 position, Color c, float t, float radius, bool depthTest) {
		Debug.DrawRay(position + math.forward() * radius, 2 * radius * math.back(), c, t, depthTest);
		Debug.DrawRay(position + math.right() * radius, 2 * radius * math.left(), c, t, depthTest);
	}

	public static void D_DrawBox(float3 centerPosition, float3 size, Color c) {
		D_DrawBox(centerPosition, size, c, 9999999f);
	}

	public static void D_DrawBox(float3 centerPosition, float3 size, Color c, float t) {
		D_DrawBox(centerPosition, size, c, 9999999f, true);
	}

	public static void D_DrawBox(float3 centerPosition, float3 size, Color c, float t, bool depthTest) {
		float3 topRightFront = centerPosition + size / 2f;
		float3 topLeftFront = topRightFront + math.left() * size.x;
		float3 topRightBack = topRightFront + math.back() * size.z;
		float3 topLeftBack = topLeftFront + math.back() * size.z;
		float3 botRightFront = topRightFront + math.down() * size.y;
		float3 botLeftFront = topLeftFront + math.down() * size.y;
		float3 botRightBack = topRightBack + math.down() * size.y;
		float3 botLeftBack = topLeftBack + math.down() * size.y;

		/* Top rect */
		Debug.DrawRay(topRightFront, topLeftFront - topRightFront, c, t, depthTest); // top front
		Debug.DrawRay(topRightFront, topRightBack - topRightFront, c, t, depthTest); // top right
		Debug.DrawRay(topLeftFront, topLeftBack - topLeftFront, c, t, depthTest); // top left
		Debug.DrawRay(topRightBack, topLeftBack - topRightBack, c, t, depthTest); // top back

		/* Bottom rect */
		Debug.DrawRay(botRightFront, botLeftFront - botRightFront, c, t, depthTest); // bot front
		Debug.DrawRay(botRightFront, botRightBack - botRightFront, c, t, depthTest); // bot right
		Debug.DrawRay(botLeftFront, botLeftBack - botLeftFront, c, t, depthTest); // bot left
		Debug.DrawRay(botRightBack, botLeftBack - botRightBack, c, t, depthTest); // bot back

		/* 4 Poles */
		Debug.DrawRay(topRightFront, botRightFront - topRightFront, c, t, depthTest); // front right
		Debug.DrawRay(topLeftFront, botLeftFront - topLeftFront, c, t, depthTest); // front left
		Debug.DrawRay(topRightBack, botRightBack - topRightBack, c, t, depthTest); // back right
		Debug.DrawRay(topLeftBack, botLeftBack - topLeftBack, c, t, depthTest); // back left
	}

}