using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public class CannonFodderStaticsAuthoring : MonoBehaviour {

	public SO_CannonFodder CannonFodderSO;



	class Baker : Baker<CannonFodderStaticsAuthoring> {
		public override void Bake(CannonFodderStaticsAuthoring auth) {
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, BoidUtil.StaticsBuilder.CannonFodder(auth.CannonFodderSO));
		}
	}

}

public struct CannonFodderStatics : IComponentData {
	public GeneralBoidProperties BoidProperties;
	public float FleeTriggerDistance;
	public float FleeForce;
	public CollisionFilter LosFilterForFleeing;
}