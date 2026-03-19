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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 Seek(in float3 pos, in float3 targetPos, in float3 velocity, float maxSteeringVelocity, float maxSteeringForce) {
		// Find desired velocity via |targ - pos| * maxSteerVel
		// Steer force is then desired - currVel, clamped by maxSteerForce
		return SeekDirectionNormalized(math.normalize(targetPos - pos), velocity, maxSteeringVelocity, maxSteeringForce);
	}

	/// <summary>
	/// Seek, but instead of seeking a position, this seeks a deired velocity.<br/>
	/// The provied direction does not need to be normalized.
	/// </summary>
	/// <param name="desiredDir"></param>
	/// <param name="velocity"></param>
	/// <param name="maxSteeringVelocity"></param>
	/// <param name="maxSteeringForce"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 SeekDirection(in float3 desiredDir, in float3 velocity, float maxSteeringVelocity, float maxSteeringForce) {
		return SeekDirectionNormalized(math.normalize(desiredDir), velocity, maxSteeringVelocity, maxSteeringForce);
	}

	/// <summary>
	/// Seek, but instead of seeking a position, this seeks a deired velocity.<br/>
	/// The provied direction must already be normalized.
	/// </summary>
	/// <param name="desiredDir"></param>
	/// <param name="velocity"></param>
	/// <param name="maxSteeringVelocity"></param>
	/// <param name="maxSteeringForce"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 SeekDirectionNormalized(in float3 desiredDirNormalized, in float3 velocity, float maxSteeringVelocity, float maxSteeringForce) {
		return ClampSteeringForce(desiredDirNormalized * maxSteeringVelocity - velocity, maxSteeringForce);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static float3 ClampSteeringForce(in float3 steeringForce, float maxSteeringForce) {
		return math.lengthsq(steeringForce) > Util.pow2(maxSteeringForce) ? math.normalize(steeringForce) * maxSteeringForce : steeringForce;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 Flee(in float3 pos, in float3 targetPos, in float3 velocity, float maxSteeringVelocity, float maxSteeringForce) {
		return -Seek(pos, targetPos, velocity, maxSteeringVelocity, maxSteeringForce);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 FleeDirection(in float3 desiredDir, in float3 velocity, float maxSteeringVelocity, float maxSteeringForce) {
		return FleeDirectionNormalized(math.normalize(desiredDir), velocity, maxSteeringVelocity, maxSteeringForce);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 FleeDirectionNormalized(in float3 desiredDirNormalized, in float3 velocity, float maxSteeringVelocity, float maxSteeringForce) {
		return -SeekDirectionNormalized(desiredDirNormalized, velocity, maxSteeringVelocity, maxSteeringForce);
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

	[BurstCompile]
	public static class Rotator {
		public static quaternion Airplane(in float3 forward, in float3 steer) {
			// Find cos(theta) where theta is the angle between forward and steer strictly in the x-z plane
			// cos(th) = f . s / (|f||s|) == f . s / sqrt(f.sqrMag * s.sqrMag)
			float c = 1 - (forward.x * steer.x + forward.z * steer.z) / math.sqrt(
				(forward.x * forward.x + forward.z * forward.z) *
				(steer.x * steer.x + steer.z * steer.z)
			);
			float3 right = math.normalize(math.cross(forward, math.up()));
			// To check leftness/rightness, use dot product of steer and right
			return quaternion.LookRotation(forward, Util.nlerp(math.up(), math.dot(steer, right) > 0 ? right : -right, c));
		}
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

		public static HunterBoid HunterBoid() {
			return new() {
				steerForce = float3.zero,
				wanderVector = math.forward(),
				timeSinceLastWanderStep = 0f,
				timeSinceBeganFleeing = 1000f // Don't start at a fleeing state
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

		public static HunterBasicStatics HunterBasic(SO_Hunter so) {
			// TODO
			return new() {
				BoidProperties = GeneralBoid(so),
				Hunter = HunterShared(so)
			};
		}

		public static HunterEmpoweredStatics HunterEmpowered(SO_Hunter so) {
			// TODO
			return new() {
				BoidProperties = GeneralBoid(so),
				Hunter = HunterShared(so)
			};
		}

		static HunterSharedStatics HunterShared(SO_Hunter so) {
			return new() {
				RunAwayMaxSteerVelocity = so.RunAwayMaxSteerVelocity,
				RunAwayMaxSteerForce = so.RunAwayMaxSteerForce,
				RunAwayDuration = so.RunAwayDuration,
				RunAwayRequiredSpeedSq = Util.pow2(so.RunAwayRequiredSpeed),
				RunAwayRequiredDistSq = Util.pow2(so.RunAwayRequiredDist),
				WanderTriggerDistSq = Util.pow2(so.WanderTriggerDist),
				PathfindTriggerDistSq = Util.pow2(so.PathfindTriggerDist),

				LosFilterForChasing = new() {
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