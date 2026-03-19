using Unity.Entities;
using Unity.Physics;
using UnityEngine;

/// <summary>
/// Handles authoring for BOTH Hunter forms.
/// </summary>
public class HunterStaticsAuthoring : MonoBehaviour {

	public SO_Hunter HunterBasicSO;
	public SO_Hunter HunterEmpoweredSO;



	class Baker : Baker<HunterStaticsAuthoring> {
		public override void Bake(HunterStaticsAuthoring auth) {
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, BoidUtil.StaticsBuilder.HunterBasic(auth.HunterBasicSO));
			AddComponent(entity, BoidUtil.StaticsBuilder.HunterEmpowered(auth.HunterEmpoweredSO));
		}
	}

}

public struct HunterSharedStatics {
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

public struct HunterBasicStatics : IComponentData {
	public GeneralBoidProperties BoidProperties;
	public HunterSharedStatics Hunter;
}

public struct HunterEmpoweredStatics : IComponentData {
	public GeneralBoidProperties BoidProperties;
	public HunterSharedStatics Hunter;

}
