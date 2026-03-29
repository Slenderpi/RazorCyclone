using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Processes every CannonProjectile that hit something this frame.
/// </summary>
[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(ProjectileSystem))]
partial struct PlayerCannonProjectileSystem : ISystem {

	private ComponentLookup<CannonTarget> CannonTargetLookup;
	EntityQuery eqRicTarget;
	CollisionFilter RicochetLosFilter;

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<PlayerCannonProjectile>();
		state.RequireForUpdate<PlayerCannonProjectileStatics>();

		CannonTargetLookup = state.GetComponentLookup<CannonTarget>(false);
		using var eqb = new EntityQueryBuilder(Allocator.Temp);
		eqRicTarget = eqb.WithAll<RicochetTarget>().Build(ref state);
		RicochetLosFilter = new() {
			BelongsTo = 1u << 8, // Projectile
			CollidesWith = 1u | (1u << 17) | (1u << 18), // Collide with environment only
			GroupIndex = 0
		};
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		CannonTargetLookup.Update(ref state);
		var arrRicTargets = eqRicTarget.ToComponentDataArray<RicochetTarget>(Allocator.TempJob);
		var arrRicTargetEntities = eqRicTarget.ToEntityArray(Allocator.TempJob);

		state.Dependency = new CannonProjectileJob() {
			ecb = SystemAPI
				.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
				.CreateCommandBuffer(state.WorldUnmanaged),
			pcps = SystemAPI.GetSingleton<PlayerCannonProjectileStatics>(),
			cw = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld,
			CannonTargetLookup = CannonTargetLookup,
			RicTargets = arrRicTargets,
			RicTargetEntities = arrRicTargetEntities,
			RicochetLosFilter = RicochetLosFilter
		}.Schedule(state.Dependency); // Not necessary to be parallel. Very unlikely for there to be enough cannon projectiles to warrant that
		state.Dependency = arrRicTargets.Dispose(state.Dependency);
		state.Dependency = arrRicTargetEntities.Dispose(state.Dependency);
	}

	[BurstCompile]
	partial struct CannonProjectileJob : IJobEntity {
		public EntityCommandBuffer ecb;
		public PlayerCannonProjectileStatics pcps;
		[ReadOnly] public CollisionWorld cw;
		public ComponentLookup<CannonTarget> CannonTargetLookup;
		[ReadOnly] public NativeArray<RicochetTarget> RicTargets;
		[ReadOnly] public NativeArray<Entity> RicTargetEntities;
		public CollisionFilter RicochetLosFilter;

		[BurstCompile]
		public void Execute(
			ref Projectile proj,
			ref PlayerCannonProjectile cannonProjectile,
			ref PhysicsVelocity pv,
			ref LocalTransform transform,
			in Entity en) {
			if (!proj.DidHitThisFrame) return;
			if (cannonProjectile.TryOnHitSameEntity(proj.Hit.Entity)) {
				proj.DidHitThisFrame = false;
				return;
			}

			// Position projectile to the hit location
			float3 vfxLookDir = math.normalize(pv.Linear);
			transform.Position = proj.Hit.Position + vfxLookDir * -0.1f;

			// If hit a cannon target, set its hit event and try to deal damage
			bool didHitCannonTarget = CannonTargetLookup.TryGetComponent(proj.Hit.Entity, out CannonTarget target);
			if (didHitCannonTarget) {
				target.SetEventHit(cannonProjectile.RemainingRicochets > 0);
				CannonTargetLookup[proj.Hit.Entity] = target;
			}

			// Determine specifics regarding ricochet/not ricochet
			Entity vfx;
			Util.UpForLookRotation(vfxLookDir, out float3 upVector);
			if (cannonProjectile.RemainingRicochets <= 0) {
				vfx = ecb.Instantiate(pcps.ImpactEffect);
				ecb.DestroyEntity(en);
				pv.Linear = float3.zero;
			} else {
				// Projectile will ricochet. Deduct ricochets, use ricochet vfx, and reflect velocity
				cannonProjectile.RemainingRicochets--;
				vfx = ecb.Instantiate(pcps.RicochetEffect);
				proj.DidHitThisFrame = false;
				pv.Linear = DetermineVelocityFromRicochet(didHitCannonTarget, cannonProjectile.GetLastEntityHit(), transform.Position, pv.Linear, proj);
			}

			// Position the vfx
			ecb.SetComponent(
				vfx,
				LocalTransform.FromPositionRotation(
					transform.Position,
					quaternion.LookRotation(vfxLookDir, upVector)
				)
			);
		}

		[BurstCompile]
		readonly float3 DetermineVelocityFromRicochet(bool didHitCannonTarget, Entity lastEntityHit, in float3 myPos, in float3 prevVelocity, in Projectile proj) {
			if (!didHitCannonTarget) {
				//Debug.Log($"Hit wall. Reflecting.");
				return ReflectedVelocity(prevVelocity, proj);
			} else {
				int smartTargetIndex = TryGetSmartTargetIndex(myPos, lastEntityHit);
				//{ // RICOCHET VISUALIZATION
				//	if (smartTargetIndex == -1)
				//		Debug.Log($"No compatible target found. Reflecting.");
				//	else {
				//		Debug.LogWarning($"Found target! Smart targeting!");
				//		const float DRAW_TIME = 10f;
				//		Color PREDICTION_COLOR = Color.red;
				//		Color NAIVE_COLOR = Color.green;
				//		float3 targPos = RicTargets[smartTargetIndex].LocalPosition;
				//		float3 predictedPos = Util.PredictPosition(myPos, targPos, prevVelocity, RicTargets[smartTargetIndex].Velocity);
				//		Util.D_DrawPoint(predictedPos, PREDICTION_COLOR, DRAW_TIME);
				//		Util.D_DrawArrowFromTo(myPos, predictedPos, PREDICTION_COLOR, DRAW_TIME);
				//		Util.D_DrawPoint(targPos, NAIVE_COLOR, DRAW_TIME);
				//		Util.D_DrawArrowFromTo(targPos, predictedPos, NAIVE_COLOR, DRAW_TIME);
				//	}
				//}
				if (smartTargetIndex == -1)
					return ReflectedVelocity(prevVelocity, proj);
				else {
					RicochetTarget rt = RicTargets[smartTargetIndex];
					return math.normalize((
						rt.HasPhysicsVelocity ?
						Util.PredictPosition(myPos, rt.LocalPosition, prevVelocity, rt.Velocity) :
						rt.LocalPosition
					) - myPos) * math.length(prevVelocity);
				}
			}
		}

		[BurstCompile]
		readonly int TryGetSmartTargetIndex(in float3 myPos, Entity lastEntityHit) {
			int len = RicTargets.Length;
			if (len == 0)
				return -1;
			int bestIndex = -1;
			float bestDistSq = float.MaxValue;
			uint bestPriority = 0;
			for (int i = 0; i < len; i++) {
				if (lastEntityHit == RicTargetEntities[i])
					continue;
				RicochetTarget currRt = RicTargets[i];
				uint currPriority = currRt.Priority;
				float currDistsq = math.distancesq(myPos, currRt.LocalPosition);
				// If priority is lower, skip
				// If priority is equal, check that currDist is better than bestDist. If not better, skip
				// When priority is greater, don't bother checking distance. We will just greedily take higher priority
				// By this point, priority is either equal or greater. Now check LOS, and skip if no LOS
				if (currPriority < bestPriority || (currPriority == bestPriority && currDistsq >= bestDistSq) || HasNoLos(myPos, currRt.LocalPosition))
					continue;
				bestIndex = i;
				bestDistSq = currDistsq;
				bestPriority = currPriority;
			}
			return bestIndex;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly bool HasNoLos(in float3 myPos, in float3 otherPos) {
			return cw.CastRay(new() {
				Start = myPos,
				End = otherPos,
				Filter = RicochetLosFilter
			});
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly float3 ReflectedVelocity(in float3 prevVelocity, in Projectile proj) {
			return math.reflect(prevVelocity, proj.Hit.SurfaceNormal);
		}
	}

}
