using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Handles camtrans and pivot rotation based on their respective look and rotation inputs.
/// </summary>
[UpdateInGroup(typeof(PlayerPostUpdateGroup))]
[UpdateBefore(typeof(PlayerResourcesSystem))]
partial struct PlayerRotationSystem : ISystem {

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<Player>();
        state.RequireForUpdate<PlayerInput>();
        state.RequireForUpdate<PlayerPivot>();
        state.RequireForUpdate<PlayerCameraTransform>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        // Let's just assume there's only one player entity
        EntityQuery eqPlayer = SystemAPI.QueryBuilder().WithAll<Player, PlayerInput, PlayerControlsSettings>().Build();
        PlayerInput input = eqPlayer.ToComponentDataArray<PlayerInput>(Allocator.Temp)[0];
        PlayerControlsSettings settings = eqPlayer.ToComponentDataArray<PlayerControlsSettings>(Allocator.Temp)[0];

        float lookYLimit = math.radians(89f);

        float3 camtransRotEuler = default;
        foreach (var camtrans in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<PlayerCameraTransform>()) {
            // Rotate camtrans by look input
            float2 lookDelta = input.LookInputDelta * settings.MouseSensitivity;
            camtransRotEuler = math.Euler(camtrans.ValueRO.Rotation);
            camtransRotEuler = new(
                math.clamp(camtransRotEuler.x - lookDelta.y, -lookYLimit, lookYLimit),
                camtransRotEuler.y + lookDelta.x,
                0
            );
            camtrans.ValueRW.Rotation = quaternion.Euler(camtransRotEuler);
        }

        foreach (var pivotTrans in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<PlayerPivot>()) {
            float cos = math.cos(camtransRotEuler.y);
            float sin = math.sin(camtransRotEuler.y);
            float inX = input.RotationInput.x;
            float inY = input.RotationInput.y;
            float inZ = input.RotationInput.z;

			quaternion pivotRot;
            if (inX != 0 || inZ != 0) {
                // Rotation input is not purely up/down
                input.aimDirection = new(
                    inX * cos + inZ * sin,
                    inY,
                    inZ * cos - inX * sin
                );
                pivotRot = quaternion.LookRotation(input.aimDirection, math.up());
			} else {
                input.aimDirection = input.RotationInput;
                pivotRot = quaternion.LookRotation(
                    input.aimDirection,
                    inY >= 0 ?
                        new(-sin, 0, -cos) : // Facing up
                        new(sin, 0, cos) // Facing down
                );
			}
			if (math.abs(math.lengthsq(input.aimDirection) - 1) > 0.0000001f)
				input.aimDirection = math.normalize(input.aimDirection);

			pivotTrans.ValueRW.Rotation = math.slerp(
                pivotTrans.ValueRO.Rotation,
				pivotRot,
                math.min(1f, SystemAPI.Time.DeltaTime * 24f)
            );
            SystemAPI.SetComponent(eqPlayer.ToEntityArray(Allocator.Temp)[0], input);
		}
    }

}
