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

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerCannonProjectile>();
        state.RequireForUpdate<PlayerCannonProjectileStatics>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        new CannonProjectileJob() {
            ecb = SystemAPI
                .GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged),
            pcps = SystemAPI.GetSingleton<PlayerCannonProjectileStatics>()
        }.Schedule();
    }

    [BurstCompile]
    partial struct CannonProjectileJob : IJobEntity {
        public EntityCommandBuffer ecb;
        public PlayerCannonProjectileStatics pcps;
        //public EntityManager em;

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

            // If hit an enemy, damage them
            /*
			if (em.HasComponent<Enemy>(proj.Hit.Entity)) {
                Enemy hitEnemy = em.GetComponentData<Enemy>(proj.Hit.Entity);
                // TODO
            }
            */
        }
    }

}
