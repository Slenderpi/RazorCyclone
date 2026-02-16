using Unity.Entities;
using UnityEngine;

public class LifetimeComponentAuthoring : MonoBehaviour {

    public float Lifetime = 5f;

    class Baker : Baker<LifetimeComponentAuthoring> {
		public override void Bake(LifetimeComponentAuthoring auth) {
            Entity en = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(en, new LifetimeComponent() { Lifetime = auth.Lifetime });
		}
    }
}

public struct LifetimeComponent : IComponentData {
    public float Lifetime;

    private float timeAlive;

    public bool ShouldDieThisUpdate(float dt) {
        timeAlive += dt;
        return timeAlive > Lifetime;
    }
}
