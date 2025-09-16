using Unity.Burst;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// For 'click' type inputs, such as the cannon.
/// </summary>
[UpdateInGroup(typeof(PresentationSystemGroup))]
partial struct PlayerInputResetSystem : ISystem {

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerInput>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		foreach (var input in SystemAPI.Query<RefRW<PlayerInput>>())
            input.ValueRW.ResetPresses();
	}

}