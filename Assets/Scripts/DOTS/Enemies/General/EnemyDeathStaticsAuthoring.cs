using Unity.Entities;
using UnityEngine;

public class EnemyDeathStaticsAuthoring : MonoBehaviour {

    public GameObject DeathVfx;

    class Baker : Baker<EnemyDeathStaticsAuthoring> {
        public override void Bake(EnemyDeathStaticsAuthoring auth) {
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyDeathStatics() {
                DeathVfx = GetEntity(auth.DeathVfx, TransformUsageFlags.Dynamic)
            });
		}
    }
}

public struct EnemyDeathStatics : IComponentData {
    public Entity DeathVfx;
}