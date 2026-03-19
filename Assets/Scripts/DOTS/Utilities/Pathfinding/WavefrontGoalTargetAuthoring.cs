using Unity.Entities;
using UnityEngine;

public class WavefrontGoalTargetAuthoring : MonoBehaviour {

	public bool DISABLE_THIS_TARGET = false;

	public float WavefrontUpdateDelay = 0.5f;
	public bool VisualizeWavefrontHeatmap = false;



	class Baker : Baker<WavefrontGoalTargetAuthoring> {
		public override void Bake(WavefrontGoalTargetAuthoring auth) {
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new WavefrontGoalTarget(
				auth.WavefrontUpdateDelay
#if UNITY_EDITOR
				, auth.VisualizeWavefrontHeatmap
#endif
			));
			if (auth.DISABLE_THIS_TARGET)
				SetComponentEnabled<WavefrontGoalTarget>(entity, false);
		}
	}
	
}



public struct WavefrontGoalTarget : IComponentData, IEnableableComponent {
	/// <summary>
	/// Determines how often Wavefront Propagation is performed.
	/// </summary>
	public float WavefrontUpdateDelay;

	/// <summary>
	/// Determines if the target is inside the PointCloud or not.
	/// </summary>
	public bool IsInPointCloud;

#if UNITY_EDITOR
	public bool VisualizeWavefrontHeatmap;
#endif

	public WavefrontGoalTarget(float wavefrontUpdateDelay) {
		WavefrontUpdateDelay = wavefrontUpdateDelay;
		IsInPointCloud = true;
#if UNITY_EDITOR
		VisualizeWavefrontHeatmap = false;
#endif
	}


#if UNITY_EDITOR
	public WavefrontGoalTarget(float wavefrontUpdateDelay, bool visualizeWavefrontHeatmap) {
		WavefrontUpdateDelay = wavefrontUpdateDelay;
		IsInPointCloud = true;
		VisualizeWavefrontHeatmap = visualizeWavefrontHeatmap;
	}
#endif
}