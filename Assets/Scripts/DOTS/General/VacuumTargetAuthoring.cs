using Unity.Entities;
using UnityEngine;

public class VacuumTargetAuthoring : MonoBehaviour {

    public bool CanGetSucked = true;
    public bool CanGetKilled = true;
    public float VacuumHitboxRadius = 0.5f;

    
    
    class Baker : Baker<VacuumTargetAuthoring> {
        public override void Bake(VacuumTargetAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new VacuumTarget() {
                CanGetSucked = auth.CanGetSucked,
                CanGetKilled = auth.CanGetKilled,
                VacuumHitboxRadius = auth.VacuumHitboxRadius
			});
        }
    }
    
}
