using Unity.Entities;
using UnityEngine;

public class PlayerCannonProjectileStaticsAuthoring : MonoBehaviour {

	public GameObject ImpactVFX;
	public GameObject RicochetVFX;



	class Baker : Baker<PlayerCannonProjectileStaticsAuthoring> {
        public override void Bake(PlayerCannonProjectileStaticsAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerCannonProjectileStatics() {
				ImpactEffect = GetEntity(auth.ImpactVFX, TransformUsageFlags.Dynamic),
				RicochetEffect = GetEntity(auth.RicochetVFX, TransformUsageFlags.Dynamic)
			});
		}
    }
    
}



public struct PlayerCannonProjectileStatics : IComponentData {
	public Entity ImpactEffect;
	public Entity RicochetEffect;
}