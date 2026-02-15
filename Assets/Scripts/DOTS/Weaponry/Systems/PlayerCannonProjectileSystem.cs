using Unity.Burst;
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

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerCannonProjectile>();
        state.RequireForUpdate<PlayerCannonProjectileStatics>();
        CannonTargetLookup = state.GetComponentLookup<CannonTarget>(false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        CannonTargetLookup.Update(ref state);
        new CannonProjectileJob() {
            ecb = SystemAPI
                .GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged),
            pcps = SystemAPI.GetSingleton<PlayerCannonProjectileStatics>(),
            CannonTargetLookup = CannonTargetLookup
        }.Schedule(); // Not necessary to be parallel. Very unlikely for there to be enough cannon projectiles to warrant that
    }

    [BurstCompile]
    partial struct CannonProjectileJob : IJobEntity {
        public EntityCommandBuffer ecb;
        public PlayerCannonProjectileStatics pcps;
		public ComponentLookup<CannonTarget> CannonTargetLookup;

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
                pv.Linear = math.reflect(pv.Linear, proj.Hit.SurfaceNormal);
				proj.DidHitThisFrame = false;
			}

            // Position the vfx
			ecb.SetComponent(
				vfx,
				LocalTransform.FromPositionRotation(
					transform.Position,
					quaternion.LookRotation(vfxLookDir, upVector)
				)
			);

            // If hit a cannon target, set its hit event and try to deal damage
            if (CannonTargetLookup.HasComponent(proj.Hit.Entity)) {
                CannonTarget target = CannonTargetLookup[proj.Hit.Entity];
                target.SetEventHit();
                CannonTargetLookup[proj.Hit.Entity] = target;
            }
        }
    }

}
