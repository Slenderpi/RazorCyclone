using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Handles authoring of all Player-related components for the Player's character prefab
/// </summary>
public class PlayerAuthoring : MonoBehaviour {

    [Header("Fuel")]
    public float HuelFactor = 2f;
    public float MinHuelRequired = 5f;

    [Header("Health")]
    public float HealthRegenRate = 15f;
    public float HealthRegenDelay = 1f;
    public float HealOnKillAmount = 100f;

    [Header("Spinning")]
    public float SpinHoldDuration = 3f;


    
    class Baker : Baker<PlayerAuthoring> {
        public override void Bake(PlayerAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Player());
            AddComponent(entity, new PlayerInput() {
                RotationInput = math.forward()
            });
            AddComponent(entity, new PlayerExtraInput());
            AddComponent(entity, new PlayerControlsSettings(40f)); // Will get set by GameManager on spawn
            //AddComponent(entity, new PlayerMovement());
			PlayerResources resources = new() {
                HealthRegenRate = auth.HealthRegenRate,
                HealthRegenDelay = auth.HealthRegenDelay,
                HealOnKillAmount = auth.HealOnKillAmount,
                HuelFactor = auth.HuelFactor,
                MinHuelRequired = auth.MinHuelRequired
            };
            resources.Init();
			AddComponent(entity, resources);
			AddComponent(entity, new PlayerSpinfo {
                SpinHoldDuration = auth.SpinHoldDuration,
                CurrentRicochetMultiplier = 1 // TEMPORARY
            });
		}
    }
    
}



public struct Player : IComponentData { }

//public struct PlayerMovement : IComponentData {
//    public float VacuumForce;
//    public float VacuumForceLowSpeed;
//	// The max speed for VacuumForceLowSpeed to be used at
//	public float VacuumForceNormalSpeed; // default 8
//    public float CannonForce;

//    public bool IsGrounded;
//}

[BurstCompile]
public struct PlayerResources : IComponentData {
    /// <summary>
    /// Max fuel is 100.
    /// </summary>
    public float Fuel;
    /// <summary>
    /// Max health is 100.
    /// </summary>
    public float Health;
    /// <summary>
    /// Amount per second to regenerate health.
    /// </summary>
    public float HealthRegenRate;
    /// <summary>
    /// This value determines the amount of time required since taking damage to begin health regen.
    /// </summary>
    public float HealthRegenDelay;
    /// <summary>
    /// Amount of health to heal on kill.
    /// </summary>
    public float HealOnKillAmount;
    /// <summary>
    /// Cost factor when spending huel (health as fuel).<br/>
    /// Example: for a factor of 2, the use of 20 fuel ends up costing 20 * 2 = 40 health.
    /// </summary>
    public float HuelFactor;
    /// <summary>
    /// Minimum huel (health as fuel) required to have in order to spend fuel.
    /// </summary>
    public float MinHuelRequired;

    float lastDamageTime;
    bool didRefillFuelThisFrame;



	[BurstCompile]
    public void Init() {
        Fuel = 100f;
        Health = 100f;
        lastDamageTime = -99999f;
	}

	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool CanRegenHealth(float time) {
		return Health < 100f && time - lastDamageTime >= HealthRegenDelay && Health > 0;
	}

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RegenerateHealth(float deltaTime) {
        HealHealth(HealthRegenRate * deltaTime);
    }

	[BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool CanSpendFuel() {
        return Fuel > 0 || Health > MinHuelRequired;
    }

    [BurstCompile]
    public void SpendFuel(float amount, float time) {
        if (Fuel > 0) {
            Fuel -= amount;
            // If there's overflow, spend it as huel
            if (Fuel < 0) {
                SpendHuel(-Fuel, time);
                Fuel = 0;
            }
        } else
            SpendHuel(amount, time);
    }
	
    [BurstCompile]
	public void RefillFuel() {
		Fuel = 100f;
		didRefillFuelThisFrame = true;
	}

	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool DidRefillFuelThisFrame() {
		return didRefillFuelThisFrame;
	}

	[BurstCompile]
	public void ResetEvents() {
		didRefillFuelThisFrame = false;
	}

	[BurstCompile]
    public void TakeDamage(float amount, float time) {
        lastDamageTime = time;
        Health -= amount;
        if (Health < 0f)
            Health = 0f;
    }

    [BurstCompile]
    public void HealHealth(float amount) {
        Health += amount;
        if (Health > 100f)
            Health = 100f;
    }

    [BurstCompile]
    private void SpendHuel(float amount, float time) {
		lastDamageTime = time;
		Health -= amount * HuelFactor;
        if (Health < 0) {
            // Prevent accidental suicide
            Health = 1f;
        }
    }

}

[BurstCompile]
public struct PlayerSpinfo : IComponentData {
    public int CurrentSpins;
	public float SpinHoldDuration;
    public int CurrentRicochetMultiplier;

    public float2 prevSpinDir;
    public int spinProgress;

    // If true, then the last time CurrentSpins got set to 0, it was because of a cannon shot
    // If false, then was because timeout
    bool spinsWereSpentAsRicochet;

    public int SpendSpinsAsRicochet() {
        spinsWereSpentAsRicochet = true;
        int rics = CurrentSpins * CurrentRicochetMultiplier;
        CurrentSpins = 0;
        return rics;
    }

    /// <summary>
    /// When CurrentSpins gets set to 0, use this method to determine if the cause was from spending them 
    /// in a Cannon shot or from running out of time.
    /// </summary>
    /// <returns>True if spent by Cannon.</returns>
    public readonly bool WereSpinsSpentAsRicochet() {
        return spinsWereSpentAsRicochet;
    }

    [BurstCompile]
    public void ProcessAimDirection(float3 aimDir) {
        // TODO: update spinProgress, maybe update CurrentSpins, maybe update CurrentRicochetMultiplier, etc.
    }

    // TODO: process spin timeout
}

public struct PlayerInput : IComponentData {
    /// <summary>
    /// The distance the mouse moved this frame. Does NOT include mouse sensitivity scaling.
    /// </summary>
    public float2 LookInputDelta;
    public float3 RotationInput;
    public bool FireCannon;
    public bool EnableVacuum;

    public float3 aimDirection;

    public void ResetPresses() {
        FireCannon = false;
    }
}

public struct PlayerExtraInput : IComponentData {
	public bool SlowTime;
	public bool RefillFuel;
	public bool AddRicochets;
    public bool HealHealth;
    public bool TakeDamage;

    public void ResetPresses() {
        RefillFuel = false;
        AddRicochets = false;
        HealHealth = false;
        TakeDamage = false;
    }
}

public struct PlayerControlsSettings : IComponentData {
    /// <summary>
    /// THIS VALUE MUST BE SET BY CALLING SetMouseSensitivity(). DO NOT SET DIRECTLY.<br/>
    /// <br/>
    /// Mouse sensitivity. This value is not the Player's exact desired mouse sensitivity. It is instead the quotient of the Player's
    /// value divided by MOUSE_SENSE_FACTOR.
    /// </summary>
    public float MouseSensitivity;
    /// <summary>
    /// The MouseSensitivity's true value is the quotient of the Player's desired mouse sensitivity value divided by this factor.
    /// </summary>
    public const float MOUSE_SENSE_FACTOR = 15000f;

    public PlayerControlsSettings(float mouseSensitivity) {
        MouseSensitivity = mouseSensitivity / MOUSE_SENSE_FACTOR;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetMouseSensitivity(float newMouseSense) {
		MouseSensitivity = newMouseSense / MOUSE_SENSE_FACTOR;
	}
}