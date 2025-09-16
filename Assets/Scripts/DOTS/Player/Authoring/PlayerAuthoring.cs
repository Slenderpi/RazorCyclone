using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

/// <summary>
/// Handles authoring of all Player-related components for the Player's character prefab
/// </summary>
public class PlayerAuthoring : MonoBehaviour {

    public float MouseSensitivity = 0.11f;


    
    class Baker : Baker<PlayerAuthoring> {
        public override void Bake(PlayerAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Player());
            AddComponent(entity, new PlayerInput() {
                RotationInput = math.forward()
            });
            AddComponent(entity, new PlayerControlsSettings(
                auth.MouseSensitivity
            ));
            AddComponent(entity, new PlayerMovement());
            AddComponent(entity, new PlayerResources());
		}
    }
    
}



public struct Player : IComponentData { }

public struct PlayerMovement : IComponentData {
    public float VacuumForce;
    public float VacuumForceLowSpeed;
	// The max speed for VacuumForceLowSpeed to be used at
	public float VacuumForceNormalSpeed; // default 8
    public float CannonForce;

    public bool IsGrounded;
}

public struct PlayerResources : IComponentData {
    /// <summary>
    /// Max fuel is 100
    /// </summary>
    public float Fuel;
    /// <summary>
    /// Max health is 100
    /// </summary>
    public float Health;

    public PlayerResources(float maxAmount = 100) {
        Fuel = maxAmount;
        Health = maxAmount;
    }
}

public struct PlayerInput : IComponentData {
    public float2 LookInputDelta;
    public float3 RotationInput;
    public bool FireCannon;
    public bool EnableVacuum;

    public bool RefillFuel;
    public bool SlowTime;
    public bool AddRicochets;

    public float3 aimDirection;

	public PlayerInput(
        float2 lookInputDelta,
        float3 rotationInput,
        bool fireCannon,
        bool enableVacuum,
        bool refillFuel,
        bool slowTime,
        bool addRicochets) {
        LookInputDelta = lookInputDelta;
        RotationInput = rotationInput;
        FireCannon = fireCannon;
		EnableVacuum = enableVacuum;

        RefillFuel = refillFuel;
        SlowTime = slowTime;
        AddRicochets = addRicochets;

        aimDirection = default;
	}

    public void ResetPresses() {
        FireCannon = false;

        AddRicochets = false;
    }
}

public struct PlayerControlsSettings : IComponentData {
    public float MouseSensitivity;

    public PlayerControlsSettings(float mouseSensitivity) {
        MouseSensitivity = mouseSensitivity;
    }
}