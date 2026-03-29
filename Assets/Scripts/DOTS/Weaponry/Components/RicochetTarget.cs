using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Component for Entities that are ricochet targetable.<br/>
/// <br/>
/// Entities that are RicochetTargets MUST have:<br/>
///  - LocalTransform<br/>
/// Entities that are RicocheTargets are allowed to have:<br/>
///  - PhysicsVelocity (set HasPhysicsVelocity as true if so)<br/>
/// <br/>
/// Certain values are updated via the RicochetTargetInformationSystem
/// </summary>
[BurstCompile]
public struct RicochetTarget : IComponentData, IEnableableComponent {
    /// <summary>
    /// Position of this RicochetTarget.
    /// </summary>
    public float3 LocalPosition;
    /// <summary>
    /// Velocity of this RicochetTarget.
    /// </summary>
    public float3 Velocity;

	/// <summary>
	/// Determines if this RicochetTarget is enabled. If not, PlayerCannonProjectile will not ricochet to this target.
	/// </summary>
	public bool Enabled { readonly get { return GetBit(1u) == 0; } set { SetBit(1u, !value); } }

	/// <summary>
	/// When true, RicochetTargetInformationSystem will update the Velocity property of this RicochetTarget.<br/>
	/// It will also expect this Entity to have a PhysicsVelocity.<br/>
	/// <br/>
	/// NOTE: A runtime error will occur if this value is true but the Entity does not have a PhysicsVelocity.
	/// </summary>
	public bool HasPhysicsVelocity { readonly get { return GetBit(2u) != 0; } set { SetBit(2u, value); } }

	/// <summary>
	/// Ricochet targeting will prefer targets with higher priorities. The Priority is a uint.<br/>
	/// The maximum value (inclusive) the Priority can be is 2^30 - 1, or 1,073,741,823, due to how Priority is stored internally.<br/>
	/// This limit is not enforced. You must ensure that you do not provide an invalid value.
	/// </summary>
	public uint Priority { readonly get { return _state >> 2; } set { _state = (_state & 3u) | (value << 2); } }

	/// <summary>
	/// Bit: Meaning<br/>
	/// 1: IsDisabled<br/>
	/// 2: HasPhysicsVelocity<br/>
	/// 4+: Priority
	/// </summary>
	uint _state;

	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void SetBit(uint mask, bool value) {
		if (value)
			_state |= mask;  // set bit
		else
			_state &= ~mask; // clear bit
	}

	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly uint GetBit(uint mask) {
		return _state & mask;
	}
}