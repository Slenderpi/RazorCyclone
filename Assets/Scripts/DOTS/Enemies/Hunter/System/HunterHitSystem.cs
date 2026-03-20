using Unity.Entities;
using Unity.Burst;
using System.Runtime.CompilerServices;
using Unity.Physics;

[UpdateInGroup(typeof(EnemyHitPhysicsGroup))]
partial struct HunterHitSystem : ISystem {

	bool staticsFound;
	HunterEmpoweredStatics heStatics;
	
	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<HunterBoid>();
		state.RequireForUpdate<CannonTarget>();
		state.RequireForUpdate<VacuumTarget>();
		state.RequireForUpdate<HunterEmpoweredStatics>();
	}
	
	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		if (!staticsFound) {
			heStatics = SystemAPI.GetSingleton<HunterEmpoweredStatics>();
			staticsFound = true;
		}
		var ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
						   .CreateCommandBuffer(state.WorldUnmanaged)
						   .AsParallelWriter();
		state.Dependency = new HunterBasicHitJob() {
			ecb = ecb
		}.ScheduleParallel(state.Dependency);
		state.Dependency = new HunterEmpoweredHitJob() {
			ecb = ecb,
			ElapsedTime = (float)SystemAPI.Time.ElapsedTime,
			StunDuration = heStatics.StunDuration
		}.ScheduleParallel(state.Dependency);
	}

	[BurstCompile]
	[WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
	partial struct HunterBasicHitJob : IJobEntity {
		public EntityCommandBuffer.ParallelWriter ecb;

		[BurstCompile]
		public void Execute(
			[EntityIndexInQuery] int eiiq,
			ref VacuumTarget vtarget,
			ref CannonTarget ctarget,
			in Entity en,
			in HunterBasic _,
			EnabledRefRW<HurtboxCollider> enrHurtboxCollider
		) {
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
		}
	}

	[BurstCompile]
	[WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
	partial struct HunterEmpoweredHitJob : IJobEntity {
		public EntityCommandBuffer.ParallelWriter ecb;
		public float ElapsedTime;
		public float StunDuration;

		[BurstCompile]
		public void Execute(
			[EntityIndexInQuery] int eiiq,
			ref VacuumTarget vtarget,
			ref CannonTarget ctarget,
			in Entity en,
			ref HunterEmpowered hunterTag,
			EnabledRefRW<HurtboxCollider> enrHurtboxCollider
		) {
			if (vtarget.TryConsumeKillEvent()) {
				// If stunned, they should die now. If not stunned, do nothing.
				if (hunterTag.IsStunned) {
					ecb.AddComponent(eiiq, en, new DeadEnemyTag() {
						EnemyType = EEnemyType.HunterBasic,
						DeathSource = EEnemyDeathSource.Vacuum
					});
					enrHurtboxCollider.ValueRW = false;
					return;
				}
			}
			if (ctarget.IsHit()) {
				// If stunned, they should die now. If not stunned, they should get stunned.
				if (hunterTag.IsStunned) {
					ecb.AddComponent(eiiq, en, new DeadEnemyTag() {
						EnemyType = EEnemyType.HunterBasic,
						DeathSource = ctarget.GetHitAsDeathSource()
					});
				} else {
					hunterTag.IsStunned = true;
					hunterTag.LastStunTime = ElapsedTime;
					vtarget.CanGetSucked = true;
				}
				ctarget.ConsumeHitEvent();
				enrHurtboxCollider.ValueRW = false;
			}
		}

		//[BurstCompile]
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//readonly bool ShouldBeUnStunned(in HunterEmpowered hunter) {
		//	return ElapsedTime - hunter.LastStunTime > StunDuration;
		//}
	}

}