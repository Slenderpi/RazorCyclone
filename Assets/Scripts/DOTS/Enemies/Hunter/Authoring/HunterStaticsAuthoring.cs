using Unity.Entities;
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

public struct HunterBasicStatics : IComponentData {
	public GeneralBoidProperties BoidProperties;
}

public struct HunterEmpoweredStatics : IComponentData {
	public GeneralBoidProperties BoidProperties;

}
