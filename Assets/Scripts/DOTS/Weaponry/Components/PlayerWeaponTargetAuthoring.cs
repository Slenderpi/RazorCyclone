using Unity.Entities;
using UnityEngine;

public class PlayerWeaponTargetAuthoring : MonoBehaviour {

    [Header("Vacuum target properties")]
    [Tooltip("Determines if this entity is a VacuumTarget")]
    public bool CanGetHitByVacuum = true;
    [Tooltip("Starting suck state")]
	public bool CanGetSuckedByVacuum = true;
    [Tooltip("Starting vacuum kill state")]
    public bool CanGetKilledByVacuum = true;
    [Tooltip("Radius of this target's imaginary hitbox. This allows the vacuum to hit the target easier.")]
    [Min(0)]
    public float VacuumHitboxRadius;

	[Header("Cannon target properties")]
	public bool CanGetHitByCannon = true;



	class Baker : Baker<PlayerWeaponTargetAuthoring> {
        public override void Bake(PlayerWeaponTargetAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            if (auth.CanGetHitByVacuum)
                AddComponent(entity, new VacuumTarget() {
                    CanGetSucked = auth.CanGetSuckedByVacuum,
                    CanGetKilled = auth.CanGetKilledByVacuum,
                    VacuumHitboxRadius = auth.VacuumHitboxRadius
                });
            if (auth.CanGetHitByCannon)
                AddComponent(entity, new CannonTarget());
        }
    }

}



public struct VacuumTarget : IComponentData {
    public bool CanGetSucked;
    public bool CanGetKilled;
    public float VacuumHitboxRadius;

    private bool isGettingSucked;
    private bool isGettingKilled;

    public void SetEventSucked() {
        isGettingSucked = true;
    }

    public void SetEventKilled() {
        isGettingKilled = true;
    }

    public bool IsSucked() {
        return isGettingSucked;
    }

    public bool IsKilled() {
        return isGettingKilled;
    }

    public void ResetEvents() {
        isGettingSucked = false;
        isGettingKilled = false;
    }
}



public struct CannonTarget : IComponentData {
    private bool isHitThisFrame;

    public void SetEventHit() {
        isHitThisFrame = true;
    }

    public bool IsHit() {
        return isHitThisFrame;
    }

    public void ResetEvents() {
        isHitThisFrame = false;
    }
}