using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
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

	/// <summary>
	/// Computes a new wanderVector.
	/// </summary>
	/// <param name="prevWander">The wanderVector to iterate on.</param>
	/// <param name="wanderChangeDist">The length of wanderDelta that will be applied to "turn" the wanderVector.</param>
	/// <param name="rng">Random number generator.</param>
	/// <param name="wanderLimitDist">The length of wanderVector.</param>
	/// <returns>The stepped wanderVector, randomly iterated from prevWander.</returns>
	public static float3 StepWanderVector2D(in float3 prevWander, float wanderChangeDist, float wanderLimitDist, ref Unity.Mathematics.Random rng) {
		float3 newWanderDelta = new(rng.NextFloat(-1f, 1f), 0f, rng.NextFloat(-1f, 1f));
		newWanderDelta = (Util.IsNearZero(newWanderDelta) ? math.forward() : math.normalize(newWanderDelta)) * wanderChangeDist;
		return math.normalizesafe(prevWander + newWanderDelta) * wanderLimitDist;
	}

	/// <summary>
	/// Convenient method to apply Seek in the context of Wandering.
	/// </summary>
	/// <param name="position">Current position of the Boid.</param>
	/// <param name="velocity">Current velocity of the Boid.</param>
	/// <param name="wanderVector">The wanderVector of the Boid.</param>
	/// <param name="wanderLimitDist">Distance along velocity, used as the start position of wanderVector.</param>
	/// <param name="maxSteerVel">Max steering velocity.</param>
	/// <param name="maxSteerForce">Max Steering force.</param>
	/// <returns>The Seek steer force to use for the given Wander parameters.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 SeekWander(in float3 position, in float3 velocity, in float3 wanderVector, float wanderLimitDist, float maxSteerVel, float maxSteerForce) {
		return Seek(
			position,
			position + math.normalizesafe(velocity) * wanderLimitDist + wanderVector,
			velocity,
			maxSteerVel,
			maxSteerForce
		);
	}



	/// <summary>
	/// For initializing boid components.
	/// </summary>
	public static class BoidBuilder {

		public static CannonFodderBoid CannonFodder() {
			return new() {
				steerForce = float3.zero,
				wanderVector = math.forward(),
				timeSinceLastWanderStep = 0f
			};
		}

	}

	/// <summary>
	/// For initializing boid statics from scriptable objects.
	/// </summary>
	public static class StaticsBuilder {

		public static CannonFodderStatics CannonFodder(SO_CannonFodder so) {
			return new() {
				BoidProperties = GeneralBoid(so),
				FleeTriggerDistance = so.FleeTriggerDistance,
				FleeForce = so.FleeForce,
				LosFilterForFleeing = new() {
					BelongsTo = 1u << 8, // Projectile
					CollidesWith = 1u | (1u << 17) | (1u << 18), // Collide with environment only
					GroupIndex = 0
				}
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GeneralBoidProperties GeneralBoid(GeneralBoidSO so) {
			return new() {
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
				AvoidInvisBoidWalls = so.AvoidInvisBoidWalls
			};
		}

	}

}