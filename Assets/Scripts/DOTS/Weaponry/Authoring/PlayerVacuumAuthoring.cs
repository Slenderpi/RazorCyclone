using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public class PlayerVacuumAuthoring : MonoBehaviour {

    [Header("Pulling (Player movement)")]
	[Tooltip("The force of the Vacuum pull when the player is at low speeds (i.e. speed <= PullForceThreshold)")]
	public float PullForceWhenSlow = 25f;
	[Tooltip("The force of the Vacuum pull when the player is at high speeds (i.e. speed > PullForceThreshold)")]
	public float PullForceWhenFast = 26f;
	[Tooltip("If the Player's speed is <= this value, PullForceWhenSlow is used. Otherwise, PullForceWhenFast is used.")]
	public float PullForceThreshold = 8;

    [Header("Fuel")]
	[Tooltip("How long a full tank of fuel lasts (in seconds) (not including huel) if only the Vacuum is used.")]
	public float VacuumFuelTime = 10f;

	[Header("Vacuum sucking/killing")]
    [Tooltip("The force applied to VacuumTargets to suck them towards the Vacuum.")]
	public float VacuumSuckForce = 8500f;
    [Tooltip("The radius at which the Vacuum can suck in VacuumTargets.\nSort of like a radius of a spherical hitbox.")]
    public float VacuumSuckRadius = 7f;
    [Tooltip("The distance at which a VacuumTarget that can be killed by the Vacuum will be killed at.\nSort of like a radius of a spherical hitbox.")]
    public float VacuumKillRadius = 0.6f;
    [Tooltip("Full wide angle of Vacuum sucking (i.e. max is 180). In degrees.")]
    [Min(10f)]
    public float VacuumSuckAngle = 90f;
    
    
    class Baker : Baker<PlayerVacuumAuthoring> {
        public override void Bake(PlayerVacuumAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            PlayerVacuum vacuum = new() {
                PullForceWhenSlow = auth.PullForceWhenSlow,
                PullForceWhenFast = auth.PullForceWhenFast,
                PullForceThresholdSq = Util.pow2(auth.PullForceThreshold),

				VacuumSuckForce = auth.VacuumSuckForce,
				VacuumSuckRadius = auth.VacuumSuckRadius,
                VacuumKillRadius = auth.VacuumKillRadius,
                VacuumSuckAngleCosined = -Mathf.Cos(auth.VacuumSuckAngle / 2f * Mathf.Deg2Rad)
            };
            vacuum.SetFuelRate(auth.VacuumFuelTime);
			AddComponent(entity, vacuum);
        }
    }
    
}



[BurstCompile]
public struct PlayerVacuum : IComponentData {
	/// <summary>
	/// Determines if the Vacuum is supposed to be on. This means pulling (Player movement), sucking, killing, VFX, and audio.
	/// </summary>
	public bool VacuumEnabled;

	/// <summary>
	/// The pull force to use when the Player's speed is <= PullForceThreshold.
	/// </summary>
	public float PullForceWhenSlow;
	/// <summary>
	/// The pull force to use when the Player's speed is > PullForceThreshold.
	/// </summary>
	public float PullForceWhenFast;
    /// <summary>
    /// If the Player's speed is <= this value, PullForceWhenSlow is used. Otherwise, PullForceWhenFast is used.<br/>
    /// The value is stored as squared.
    /// </summary>
	public float PullForceThresholdSq;

	/// <summary>
	/// The force applied to VacuumTargets to suck them towards the Vacuum.
	/// </summary>
	public float VacuumSuckForce;
	/// <summary>
	/// The radius at which the Vacuum can suck in VacuumTargets. Sort of like a radius of a spherical hitbox.
	/// </summary>
	public float VacuumSuckRadius;
	/// <summary>
	/// The distance at which a VacuumTarget that can be killed by the Vacuum will be killed at. Sort of like a radius of a spherical hitbox.
	/// </summary>
	public float VacuumKillRadius;
	/// <summary>
	/// The cosine of the angle that enemies need to be within (to the forward vector) to get sucked.
	/// </summary>
	public float VacuumSuckAngleCosined;

    float _fuelRate;

    [BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetFuelRate(float vacuumFuelTime) {
        _fuelRate = 100f / vacuumFuelTime;
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly float GetFuelRate() => _fuelRate;
}