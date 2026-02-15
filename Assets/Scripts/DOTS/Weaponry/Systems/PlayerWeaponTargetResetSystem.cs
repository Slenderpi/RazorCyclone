using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(PresentationSystemGroup))]
partial struct VacuumTargetResetSystem : ISystem {

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<VacuumTarget>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        new VacuumTargetResetJob().ScheduleParallel();
    }

    [BurstCompile]
    partial struct VacuumTargetResetJob : IJobEntity {
        [BurstCompile]
        public void Execute(ref VacuumTarget target) {
            target.ResetEvents();
        }
    }

}



[UpdateInGroup(typeof(PresentationSystemGroup))]
partial struct CannonTargetResetSystem : ISystem {

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<CannonTarget>();
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		new CannonTargetResetJob().ScheduleParallel();
	}

	[BurstCompile]
	partial struct CannonTargetResetJob : IJobEntity {
		[BurstCompile]
		public void Execute(ref CannonTarget target) {
			target.ResetEvents();
		}
	}

}