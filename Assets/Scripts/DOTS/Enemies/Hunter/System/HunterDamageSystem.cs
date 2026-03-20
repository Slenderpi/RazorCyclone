using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(EnemyDamagePhysicsGroup))]
partial struct HunterDamageSystem : ISystem {

	ComponentLookup<PlayerResources> luPlayerResources;
	EntityQuery eqPlayer;
	EntityQuery eqVacuum;
	EntityQuery eqStatics;

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<HunterBoid>();
		state.RequireForUpdate<HurtboxCollider>();
		state.RequireForUpdate<HunterBasicStatics>();
		state.RequireForUpdate<HunterEmpoweredStatics>();
		state.RequireForUpdate<Player>();

		luPlayerResources = SystemAPI.GetComponentLookup<PlayerResources>(isReadOnly: false);
		var eqb = new EntityQueryBuilder(Allocator.Temp);
		eqPlayer = eqb.WithAll<Player>().Build(ref state);
		eqVacuum = eqb.Reset().WithAll<PlayerVacuum>().Build(ref state);
		eqStatics = eqb.Reset().WithAll<HunterBasicStatics, HunterEmpoweredStatics>().Build(ref state);
	}
	
	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		// If the Player's Vacuum is on, contact-based damage is disabled.
		if (eqVacuum.GetSingleton<PlayerVacuum>().VacuumEnabled)
			return;
		Entity playerEntity = eqPlayer.GetSingletonEntity();
		luPlayerResources.Update(ref state);

		state.Dependency = new HunterBasicDamageJob() {
			ElapsedTime = (float)SystemAPI.Time.ElapsedTime,
			luPlayerResources = luPlayerResources,
			PlayerEntity = playerEntity,
			Damage = eqStatics.GetSingleton<HunterBasicStatics>().HunterGameplay.Damage
		}.Schedule(state.Dependency);
		state.Dependency = new HunterEmpoweredDamageJob() {
			ElapsedTime = (float)SystemAPI.Time.ElapsedTime,
			luPlayerResources = luPlayerResources,
			PlayerEntity = playerEntity,
			Damage = eqStatics.GetSingleton<HunterEmpoweredStatics>().HunterGameplay.Damage
		}.Schedule(state.Dependency);
	}

	[BurstCompile]
	partial struct HunterBasicDamageJob : IJobEntity {
		public float ElapsedTime;
		public ComponentLookup<PlayerResources> luPlayerResources;
		public Entity PlayerEntity;
		public float Damage;

		[BurstCompile]
		public void Execute(in HunterBasic hunterTag, in HurtboxCollider collider) {
			if (!collider.DidBeginTouchThisFrame())
				return;
			PlayerResources rsrcs = luPlayerResources[PlayerEntity];
			rsrcs.TakeDamage(Damage, ElapsedTime);
			luPlayerResources[PlayerEntity] = rsrcs;
		}
	}

	[BurstCompile]
	partial struct HunterEmpoweredDamageJob : IJobEntity {
		public float ElapsedTime;
		public ComponentLookup<PlayerResources> luPlayerResources;
		public Entity PlayerEntity;
		public float Damage;

		[BurstCompile]
		public void Execute(in HunterEmpowered hunterTag, in HurtboxCollider collider) {
			if (!collider.DidBeginTouchThisFrame() || hunterTag.IsStunned)
				return;
			PlayerResources rsrcs = luPlayerResources[PlayerEntity];
			rsrcs.TakeDamage(Damage, ElapsedTime);
			luPlayerResources[PlayerEntity] = rsrcs;
		}
	}

}