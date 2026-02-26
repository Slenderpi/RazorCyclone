using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PlayerPostUpdateGroup))]
partial struct FuelPickupSystem : ISystem {

	EntityQuery PlayerQuery;
    NativeReference<Unity.Mathematics.Random> rng;

	[BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<FuelPickupStatics>();
        state.RequireForUpdate<FuelPickup>();
        state.RequireForUpdate<FuelPickupPivot>();

		PlayerQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<Player, PlayerResources, LocalToWorld>().Build(ref state);

		uint seed = (uint)(SystemAPI.Time.ElapsedTime * 1000f) * 0x9E3779B9;
		rng = new NativeReference<Unity.Mathematics.Random>(
			new Unity.Mathematics.Random(seed != 0 ? seed : 1),
			Allocator.Persistent
		);
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        FuelPickupStatics statics = SystemAPI.GetSingleton<FuelPickupStatics>();
        new FuelPickupJob() {
            PlayerEntity = PlayerQuery.ToEntityArray(Allocator.Temp)[0],
            PlayerResources = PlayerQuery.ToComponentDataArray<PlayerResources>(Allocator.Temp)[0],
            PlayerPosition = PlayerQuery.ToComponentDataArray<LocalToWorld>(Allocator.Temp)[0].Position,
            ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
            Statics = statics,
            PlayerHitboxRadius = 0.5f, // Based on the player prefab's sphere collider
			rng = rng
		}.Schedule();
        new FuelPickupPivotJob() {
            et = (float)SystemAPI.Time.ElapsedTime,
            dt = SystemAPI.Time.DeltaTime,
            Statics = statics
        }.Schedule();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        //PlayerQuery.Dispose();
        if (rng.IsCreated)
            rng.Dispose();
    }

    [BurstCompile]
    partial struct FuelPickupJob : IJobEntity {
        public Entity PlayerEntity;
        public PlayerResources PlayerResources;
        public float3 PlayerPosition;
		public EntityCommandBuffer ecb;
		public FuelPickupStatics Statics;
        public float PlayerHitboxRadius;
		public NativeReference<Unity.Mathematics.Random> rng;

		[BurstCompile]
        public void Execute(
            ref FuelPickup fuel,
            ref VacuumTarget vacTarget,
            ref PhysicsVelocity pv,
            ref PhysicsMass pm,
            in LocalTransform trans,
            in Entity en
        ) {
            if (!fuel.hasJumped) {
                fuel.hasJumped = true;
                float angle = GetRandomAngle();
				pv.Linear = new(
					Statics.SpawnJumpVelRange.x * math.cos(angle),
					Statics.SpawnJumpVelRange.y,
					Statics.SpawnJumpVelRange.x * math.sin(angle)
				);
			}
            pm.InverseInertia = float3.zero;
            if (math.lengthsq(PlayerPosition - trans.Position) <= Util.pow2(vacTarget.VacuumHitboxRadius + PlayerHitboxRadius)) {
                PlayerResources.RefillFuel();
                ecb.SetComponent(PlayerEntity, PlayerResources);
                ecb.DestroyEntity(en);
            }
        }

        float GetRandomAngle() {
			Unity.Mathematics.Random rand = rng.Value;
			float angle = rand.NextFloat(0, math.PI2);
			rng.Value = rand;
            return angle;
		}
    }

    [BurstCompile]
    partial struct FuelPickupPivotJob : IJobEntity {
        public float et;
        public float dt;
        public FuelPickupStatics Statics;

        [BurstCompile]
        public void Execute(
            ref LocalTransform trans,
            in FuelPickupPivot _
        ) {
			float t = et % Statics.AnimCycleDuration / Statics.AnimCycleDuration * 2;
			float x = t <= 1 ? t : 2 - t;
			trans.Position = new(trans.Position.x, math.lerp(-Statics.AnimPeakHeight, Statics.AnimPeakHeight, 3 * Util.pow2(x) - 2 * Util.pow3(x)), trans.Position.z);
            trans.Rotation = math.mul(trans.Rotation, quaternion.Euler(0, Statics.SpinRate * dt, 0));
		}
    }

}