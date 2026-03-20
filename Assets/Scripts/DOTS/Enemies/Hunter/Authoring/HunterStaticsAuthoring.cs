using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

/// <summary>
/// Handles authoring for BOTH Hunter forms.
/// </summary>
public class HunterStaticsAuthoring : MonoBehaviour {

	public SO_Hunter HunterBasicSO;
	public SO_Hunter HunterEmpoweredSO;

	// TODO: Consider wrapping this data into a scriptable object
	public float HunterBasicDamage = 15f;
	public float HunterEmpoweredDamage = 30f;
	public float HunterEmpoweredStunDuration = 5f;

	public UnityEngine.Material HunterEmpoweredNormalMaterial;
	public UnityEngine.Material HunterEmpoweredStunnedMaterial;



	class Baker : Baker<HunterStaticsAuthoring> {
		public override void Bake(HunterStaticsAuthoring auth) {
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, BoidUtil.StaticsBuilder.HunterBasic(auth.HunterBasicSO, auth.HunterBasicDamage));
			// TODO: adjust StaticsBuilder structuring stuff
			var heStatics = BoidUtil.StaticsBuilder.HunterEmpowered(auth.HunterEmpoweredSO, auth.HunterEmpoweredDamage, auth.HunterEmpoweredStunDuration);
			//heStatics.NormalMaterialIndex = indexOf(auth.HunterEmpoweredNormalMaterial);
			AddComponent(entity, heStatics);
		}
	}

}

public struct HunterBoidSharedStatics {
	public float Damage;

	public float RunAwayMaxSteerVelocity;
	public float RunAwayMaxSteerForce;
	public float RunAwayDuration;
	public float RunAwayRequiredSpeedSq;
	public float RunAwayRequiredDistSq;
	public float WanderTriggerDistSq;

	/// <summary>
	/// If the Hunter's dist from the Player is > sqrt(PathfindTriggerDistSq) (and the Player is in the PointCloud), the Hunter
	/// will follow the Wavefront to reach the Player.<br/>
	/// Otherwise, the Hunter will perform Seek towards the Player's position.
	/// </summary>
	public float PathfindTriggerDistSq;

	public CollisionFilter LosFilterForChasing;
}

public struct HunterGameplaySharedStatics {
	public float Damage;
}

public struct HunterBasicStatics : IComponentData {
	public GeneralBoidProperties BoidProperties;
	public HunterBoidSharedStatics HunterBoid;
	public HunterGameplaySharedStatics HunterGameplay;
}

[BurstCompile]
public struct HunterEmpoweredStatics : IComponentData {
	public GeneralBoidProperties BoidProperties;
	public HunterBoidSharedStatics HunterBoid;
	public HunterGameplaySharedStatics HunterGameplay;

	public float StunDuration;
	public int NormalMaterialIndex;
	public int StunMaterialIndex;

	//[BurstCompile]
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public readonly bool IsStunned(in HunterEmpowered hunterEmpowered, float elapsedTime) {
	//	return elapsedTime - hunterEmpowered.LastStunTime <= StunDuration;
	//}
}
