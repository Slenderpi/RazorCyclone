using Unity.Entities;
using Unity.Burst;
using Unity.Collections;

[UpdateInGroup(typeof(EnemyDamagePhysicsGroup))]
partial struct HunterDamageSystem : ISystem {

	ComponentLookup<PlayerResources> luPlayerResources;
	EntityQuery eqPlayer;
	EntityQuery eqVacuum;
	EntityQuery eqStatics;

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<Hunter>();
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
		luPlayerResources.Update(ref state);

		state.Dependency = new HunterDamageJob() {
			ElapsedTime = (float)SystemAPI.Time.ElapsedTime,
			luPlayerResources = luPlayerResources,
			PlayerEntity = eqPlayer.GetSingletonEntity(),
			StaticsBasic = eqStatics.GetSingleton<HunterBasicStatics>(),
			StaticsEmpowered = eqStatics.GetSingleton<HunterEmpoweredStatics>()
		}.Schedule(state.Dependency);
	}

	[BurstCompile]
	partial struct HunterDamageJob : IJobEntity {
		public float ElapsedTime;
		public ComponentLookup<PlayerResources> luPlayerResources;
		public Entity PlayerEntity;
		public HunterBasicStatics StaticsBasic;
		public HunterEmpoweredStatics StaticsEmpowered;

		[BurstCompile]
		public void Execute(in Hunter hunterTag, in HurtboxCollider collider) {
			if (!collider.DidBeginTouchThisFrame())
				return;
			PlayerResources rsrcs = luPlayerResources[PlayerEntity];
			// TODO: Add damage to Hunter statics, then read from them here
			rsrcs.TakeDamage(hunterTag.Form == EEnemyForm.Basic ? 10f : 50f, ElapsedTime);
			luPlayerResources[PlayerEntity] = rsrcs;
		}
	}
	
}