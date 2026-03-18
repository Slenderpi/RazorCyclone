using Unity.Entities;
using Unity.Burst;

[UpdateInGroup(typeof(EnemyHitPhysicsGroup))]
partial struct HunterHitSystem : ISystem {
	
	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<Hunter>();
		state.RequireForUpdate<CannonTarget>();
		state.RequireForUpdate<VacuumTarget>();
	}
	
	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		state.Dependency = new HunterHitJob() {
			ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
						   .CreateCommandBuffer(state.WorldUnmanaged)
						   .AsParallelWriter(),
		}.ScheduleParallel(state.Dependency);
	}

	[BurstCompile]
	[WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
	partial struct HunterHitJob : IJobEntity {
		public EntityCommandBuffer.ParallelWriter ecb;

		[BurstCompile]
		public void Execute(
			[EntityIndexInQuery] int eiiq,
			ref VacuumTarget vtarget,
			ref CannonTarget ctarget,
			in Entity en,
			in Hunter hunterTag,
			EnabledRefRW<HurtboxCollider> enrHurtboxCollider
		) {
			if (hunterTag.Form == EEnemyForm.Basic) {
				// This is a Basic Hunter
				if (vtarget.TryConsumeKillEvent()) {
					ecb.AddComponent(eiiq, en, new DeadEnemyTag() {
						EnemyType = EEnemyType.HunterBasic,
						DeathSource = EEnemyDeathSource.Vacuum
					});
					enrHurtboxCollider.ValueRW = false;
					return;
				}
				if (ctarget.IsHit()) {
					ecb.AddComponent(eiiq, en, new DeadEnemyTag() {
						EnemyType = EEnemyType.HunterBasic,
						DeathSource = ctarget.GetHitAsDeathSource()
					});
					ctarget.ConsumeHitEvent();
					enrHurtboxCollider.ValueRW = false;
				}
			} else {
				// This is an Empowered Hunter
				if (vtarget.TryConsumeKillEvent()) {
					// If stunned, they should die now. If not stunned, do nothing.
					ecb.AddComponent(eiiq, en, new DeadEnemyTag() {
						EnemyType = EEnemyType.HunterBasic,
						DeathSource = EEnemyDeathSource.Vacuum
					});
					enrHurtboxCollider.ValueRW = false;
					return;
				}
				if (ctarget.IsHit()) {
					ecb.AddComponent(eiiq, en, new DeadEnemyTag() {
						EnemyType = EEnemyType.HunterBasic,
						DeathSource = ctarget.GetHitAsDeathSource()
					});
					ctarget.ConsumeHitEvent();
					enrHurtboxCollider.ValueRW = false;
				}
			}
			if (vtarget.TryConsumeKillEvent()) {
				if (hunterTag.Form == EEnemyForm.Basic) {
					ecb.AddComponent(eiiq, en, new DeadEnemyTag() {
						EnemyType = EEnemyType.HunterBasic,
						DeathSource = EEnemyDeathSource.Vacuum
					});
					enrHurtboxCollider.ValueRW = false;
					return;
				} else {
					// TODO
				}
			}
			if (ctarget.IsHit()) {
				if (hunterTag.Form == EEnemyForm.Basic) {
					ecb.AddComponent(eiiq, en, new DeadEnemyTag() {
						EnemyType = EEnemyType.HunterBasic,
						DeathSource = ctarget.GetHitAsDeathSource()
					});
					ctarget.ConsumeHitEvent();
				} else {
					// If stunned, they should die now. If not stunned, stun them.
					// TODO
				}
				enrHurtboxCollider.ValueRW = false;
				return;
			}
		}
	}

}