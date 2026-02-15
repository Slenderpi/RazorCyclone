using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(EnemyGeneralPostUpdateGroup))]
partial struct CannonFodderSystem : ISystem {
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<CannonFodder>();
        state.RequireForUpdate<CannonTarget>();
        state.RequireForUpdate<VacuumTarget>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        new CannonFodderJob().ScheduleParallel();
    }

    [BurstCompile]
    partial struct CannonFodderJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            in VacuumTarget vacTargetEvents,
            in CannonTarget canTargetEvents,
            in CannonFodder cf
        ) {
            if (vacTargetEvents.IsKilled()) {
                Debug.Log("Cannon fodder killed by vacuum!");
            }
            if (canTargetEvents.IsHit()) {
                Debug.Log("Cannon fodder killed by cannon!");
            }
        }
    }

}
