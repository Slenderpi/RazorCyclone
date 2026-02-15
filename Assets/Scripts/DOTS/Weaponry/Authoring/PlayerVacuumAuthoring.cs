using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerVacuumAuthoring : MonoBehaviour {

    public float VacuumPullForce = 26f;
    public float VacuumSuckForce = 8500f;
    public float VacuumFuelTime = 10f;

    public float VacuumSuckRadius = 7f;
    public float VacuumKillRadius = 0.6f;
    
    
    class Baker : Baker<PlayerVacuumAuthoring> {
        public override void Bake(PlayerVacuumAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            PlayerVacuum vacuum = new() {
                VacuumPullForce = auth.VacuumPullForce,
                VacuumSuckForce = auth.VacuumSuckForce,
                VacuumFuelTime = auth.VacuumFuelTime,

                VacuumSuckRadius = auth.VacuumSuckRadius,
                VacuumKillRadius = auth.VacuumKillRadius
            };
            vacuum.Init();
			AddComponent(entity, vacuum);
        }
    }
    
}



[BurstCompile]
public struct PlayerVacuum : IComponentData {
    public float VacuumPullForce;
    public float VacuumSuckForce;
    public float VacuumFuelTime;
    public float VacuumSuckRadius;
    public float VacuumKillRadius;

    public bool VacuumEnabled;

    float vacuumFuelRate;

    [BurstCompile]
    public void Init() {
        vacuumFuelRate = 100f / VacuumFuelTime;
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly float GetFuelRate() => vacuumFuelRate;
}