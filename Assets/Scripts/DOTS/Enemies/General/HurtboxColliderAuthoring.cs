using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public class HurtboxColliderAuthoring : MonoBehaviour {

	[Tooltip("The Hurtbox is treated as a sphere, and this value is the radius of that sphere.\n\nThe HurtboxCollider allows an Entity to check if it's 'touching' the Player.")]
	public float HurtboxColliderRadius = 0.5f;

	[Tooltip("If false, the HurtboxCollider component will not start in an enabled state.")]
	public bool StartEnabled = true;



	class Baker : Baker<HurtboxColliderAuthoring> {
		public override void Bake(HurtboxColliderAuthoring auth) {
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			HurtboxCollider hc = new();
			hc.SetRadius(auth.HurtboxColliderRadius);
			AddComponent(entity, hc);
			SetComponentEnabled<HurtboxCollider>(entity, auth.StartEnabled);
		}
	}
	
}

/// <summary>
/// Functions as a hurtbox that allows an entity to damage the Player. The Hurtbox is treated as a sphere.<br/>
/// HurtboxCollider is not meant to be large in size. This means that LOS to the Player will not be checked, only distance.
/// </summary>
[BurstCompile]
public struct HurtboxCollider : IComponentData, IEnableableComponent {
	/// <summary>
	/// Squared radius of this hurtbox.
	/// </summary>
	public float _radiusSq { get; private set; }

	///// <summary>
	///// HurtboxCollider will apply this amount of damage to the player when contact begins with them this frame.
	///// </summary>
	//public float DamageOnContact;

	/// <summary>
	/// 0 = No contact<br/>
	/// 1 = Touched this frame<br/>
	/// 2 = Touch has not ended<br/>
	/// 3 = Touch just ended
	/// </summary>
	int _state;

	/// <summary>
	/// Updates the state of this HurtboxCollider.<br/>
	/// Also returns true iff contact just began this frame.
	/// </summary>
	/// <param name="isTouchingThisFrame"></param>
	/// <returns>True iff contact began this frame.</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Update(bool isTouchingThisFrame) {
		_state = _state switch {
			1 or 2 => isTouchingThisFrame ? 2 : 3,
			_ => isTouchingThisFrame ? 1 : 0,
		};
		//switch (_state) {
		//	case 1:
		//		// Was in contact begin ? Move onto continuous-contact state : Touch ended
		//	case 2:
		//		// Was in continuous-contact ? Stay in continuous-contact : Touch ended
		//		_state = isTouchingThisFrame ? 2 : 3;
		//		return false;
		//	case 3:
		//		// Was in end contact ? Contact begin : No contact
		//	default:
		//		// Has no contact ? Contact begin : Remain no contact
		//		if (isTouchingThisFrame) {
		//			_state = 1;
		//			return true;
		//		} else {
		//			_state = 0;
		//		}
		//		return false;
		//}
	}

	/// <summary>
	/// Determines if contact began this frame.<br/>
	/// If this Entity is maintaining contact with the Player (i.e. it was already touching the Player in the previous frame), will return false.
	/// </summary>
	/// <returns>True iff the state changed from no contact to contact.</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool DidBeginTouchThisFrame() {
		return _state == 1;
	}

	/// <summary>
	/// Determines if touch ended this frame.
	/// </summary>
	/// <returns></returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool DidEndTouchThisFrame() {
		return _state == 3;
	}

	/// <summary>
	/// Determines if there is contact this frame, irregardless of if it only just began.<br/>
	/// To check if contact only just began, use DidBeginTouchThisFrame().
	/// </summary>
	/// <returns></returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool IsTouchingThisFrame() {
		return _state == 2 || _state == 1;
	}

	/// <summary>
	/// 0 = No contact<br/>
	/// 1 = Touched this frame<br/>
	/// 2 = Touch has not ended<br/>
	/// 3 = Touch just ended
	/// </summary>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ForceSetStateTo(int state) {
		_state = state;
	}

	/// <summary>
	/// Sets the radius of this Hurtbox.<br/>
	/// Internally, Hurtbox only stores the square of the radius.
	/// </summary>
	/// <param name="radius"></param>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetRadius(float radius) {
		_radiusSq = Util.pow2(radius);
	}
}
