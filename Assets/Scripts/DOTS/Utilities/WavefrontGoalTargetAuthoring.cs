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
                auth.WavefrontUpdateDelay,
                auth.VisualizeWavefrontHeatmap
            ));
            if (auth.DISABLE_THIS_TARGET)
                SetComponentEnabled<WavefrontGoalTarget>(entity, false);
        }
    }
    
}



public struct WavefrontGoalTarget : IComponentData, IEnableableComponent {
    public float WavefrontUpdateDelay;
    public bool VisualizeWavefrontHeatmap;

    public WavefrontGoalTarget(float wavefrontUpdateDelay, bool visualizeWavefrontHeatmap) {
        WavefrontUpdateDelay = wavefrontUpdateDelay;
        VisualizeWavefrontHeatmap = visualizeWavefrontHeatmap;
    }
}