using Unity.Entities;
using UnityEngine;

public class CannonFodderStaticsAuthoring : MonoBehaviour {

	public SO_CannonFodder CannonFodderSO;



	class Baker : Baker<CannonFodderStaticsAuthoring> {
		public override void Bake(CannonFodderStaticsAuthoring auth) {
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			//AddComponent(entity, new CannonFodderStatics());
			AddComponent(entity, BoidUtil.StaticsBuilder.CannonFodder(auth.CannonFodderSO));
		}
	}

}

//public struct CannonFodderStatics : IComponentData { }

public struct CannonFodderBoidStatics : IComponentData {
	/** COMMON **/
	public float MaxSteeringVelocity;
	public float MaxSteeringForce;
	public float WanderLimitRadius;
	public float WanderLimitDist;
	public float WanderChangeDist;
	public float MaxWanderForce;
	public float WanderMinimumDelay;
	public AvoidanceTestMode AvoidanceTestType;
	public float AvoidanceMaxLookDist;
	public float AvoidanceWhiskerAngle;
	public float AvoidanceMinIntensity;
	public float AvoidanceMaxIntensity;
	public float AvoidanceMaxSteeringForce;
	public bool AvoidInvisBoidWalls;

	/** BOID-SPECIFIC **/
	public float FleeTriggerDistance;
	public float FleeForce;
}