using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PrePhysicsGroup))]
partial struct HunterStunRecoverySystem : ISystem {

	bool foundStatics;
	HunterEmpoweredStatics heStatics;

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<HunterEmpowered>();
		state.RequireForUpdate<HunterEmpoweredStatics>();
	}
	
	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		if (!foundStatics) {
			heStatics = SystemAPI.GetSingleton<HunterEmpoweredStatics>();
			foundStatics = true;
		}
		state.Dependency = new HunterEmpoweredStunRecoveryJob() {
			ElapsedTime = (float)SystemAPI.Time.ElapsedTime,
			StunDuration = heStatics.StunDuration,
			NormalRicPrio = heStatics.NormalRicochetPriority
		}.Schedule(state.Dependency);
	}

	[BurstCompile]
	[WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
	partial struct HunterEmpoweredStunRecoveryJob : IJobEntity {
		public float ElapsedTime;
		public float StunDuration;
		public uint NormalRicPrio;

		[BurstCompile]
		public void Execute(
			ref HunterEmpowered hunter,
			ref VacuumTarget vtarget,
			ref RicochetTarget rtarget,
			ref PhysicsGravityFactor gravityFactor,
			EnabledRefRW<HurtboxCollider> enrHurtboxCollider,
			EnabledRefRW<HunterBoid> enrHunterBoid
		) {
			if (!hunter.IsStunned || ElapsedTime - hunter.LastStunTime <= StunDuration)
				return;
			hunter.IsStunned = false;
			vtarget.CanGetSucked = false;
			gravityFactor.Value = 0f;
			enrHurtboxCollider.ValueRW = true;
			enrHunterBoid.ValueRW = true;
			rtarget.Priority = NormalRicPrio;
		}
	}
	
}