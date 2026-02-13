using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour {

	class Baker : Baker<EnemyAuthoring> {
		public override void Bake(EnemyAuthoring auth) {
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			//AddComponent(entity, new CannonFodderStatics());
			AddComponent(entity, new EnemyComponent() {
				HasLineOfSight = false
			});
		}
	}
}

public struct EnemyComponent : IComponentData {
	/// <summary>
	/// If true, this Enemy has LOS with the player.
	/// </summary>
	public bool HasLineOfSight;
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
partial struct GeneralEnemySystem : ISystem {

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<Player>();
		state.RequireForUpdate<EnemyComponent>();
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		Entity playerEn = SystemAPI.GetSingletonEntity<Player>();
		new GeneralEnemyJob() {
			pw = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld,
			PlayerEntity = playerEn,
			PlayerPosition = SystemAPI.GetComponentRO<LocalToWorld>(playerEn).ValueRO.Position,
			RayFilter = new CollisionFilter {
				BelongsTo = 1u << 8, // Projectile
				CollidesWith = 1u | (1u << 17) | (1u << 18), // Collide with environment only
				GroupIndex = 0
			}
		}.Schedule();
	}

	// Check for LOS by casting player to enemy like a cannon projectile.
	// If the player can direct LOS shoot you, then you have LOS with the player.
	[BurstCompile]
	partial struct GeneralEnemyJob : IJobEntity {
		public PhysicsWorld pw;
		public Entity PlayerEntity;
		public float3 PlayerPosition;
		public CollisionFilter RayFilter;

		[BurstCompile]
		public void Execute(
			ref EnemyComponent enemy,
			in LocalToWorld ltw
		) {
			if (pw.CastRay(
				new() {
					Start = PlayerPosition,
					End = ltw.Position,
					Filter = RayFilter
				},
				out Unity.Physics.RaycastHit hit
				)) {
				enemy.HasLineOfSight = false;
			} else {
				enemy.HasLineOfSight = true;
			}
		}
	}
}