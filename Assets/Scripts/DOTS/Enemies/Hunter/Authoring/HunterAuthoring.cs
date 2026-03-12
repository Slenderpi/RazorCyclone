using Unity.Entities;
using UnityEngine;

public class HunterAuthoring : MonoBehaviour {

    [SerializeField]
    [Tooltip("Determines if this Hunter is a Basic one or an Empowered one.")]
    EEnemyForm Form;
    
    
    
    class Baker : Baker<HunterAuthoring> {
        public override void Bake(HunterAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Hunter() {
                Form = auth.Form
            });
			AddComponent(entity, BoidUtil.BoidBuilder.HunterBoid());
        }
    }
    
}

public struct Hunter : IComponentData {
	public EEnemyForm Form;
}
