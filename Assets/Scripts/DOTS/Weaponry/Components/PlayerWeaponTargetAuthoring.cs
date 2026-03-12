using Unity.Entities;
using UnityEngine;

public class PlayerWeaponTargetAuthoring : MonoBehaviour {

    [Header("Vacuum target properties")]
    [Tooltip("Determines if this entity is a VacuumTarget")]
    public bool IsVacuumTarget = true;
    [Tooltip("Starting suck state")]
	public bool CanGetSuckedByVacuum = true;
    [Tooltip("Starting vacuum kill state")]
    public bool CanGetKilledByVacuum = true;
    [Tooltip("Radius of this target's imaginary hitbox. This allows the vacuum to hit the target easier.")]
    [Min(0)]
    public float VacuumHitboxRadius;

	[Header("Cannon target properties")]
	public bool IsCannonTarget = true;



	class Baker : Baker<PlayerWeaponTargetAuthoring> {
        public override void Bake(PlayerWeaponTargetAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            if (auth.IsVacuumTarget)
                AddComponent(entity, new VacuumTarget() {
                    CanGetSucked = auth.CanGetSuckedByVacuum,
                    CanGetKilled = auth.CanGetKilledByVacuum,
                    VacuumHitboxRadius = auth.VacuumHitboxRadius
                });
            if (auth.IsCannonTarget)
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

    /// <summary>
    /// This function can be used to both check and consume the Sucked event.
    /// To only check the Sucked event, use IsSucked().
    /// </summary>
    /// <returns></returns>
	public bool TryConsumeSuckEvent() {
		if (isGettingSucked) {
			isGettingSucked = false;
			return true;
		}
		return false;
	}

	/// <summary>
	/// This function can be used to both check and consume the Killed event.
	/// To only check the Killed event, use IsKilled().
	/// </summary>
	/// <returns></returns>
	public bool TryConsumeKillEvent() {
		if (isGettingKilled) {
			isGettingKilled = false;
			return true;
		}
		return false;
	}

	public void ResetEvents() {
        isGettingSucked = false;
        isGettingKilled = false;
    }
}



public struct CannonTarget : IComponentData {
    /// <summary>
    /// 0 = Not hit.<br/>
    /// 1 = Hit, no ricochet.<br/>
    /// 2 = Hit, ricochet.
    /// </summary>
    private byte hitStatusThisFrame;

    public void SetEventHit(bool wasAsRicochet) {
        hitStatusThisFrame = wasAsRicochet ? (byte)2 : (byte)1;
    }

    /// <summary>
    /// Returns true if this target was hit by a Cannon Projectile, regardless of if by ricochet or not.<br/>
    /// </summary>
    /// <returns></returns>
    public readonly bool IsHit() {
        return hitStatusThisFrame > 0;
    }

    /// <summary>
    /// Returns true if this target was hit by a Cannon Projectile ricochet.
    /// </summary>
    /// <returns></returns>
    public readonly bool IsHitAsRicochet() {
        return hitStatusThisFrame == 2;
    }

    /// <summary>
    /// Convenience method for getting the hit status of this target as an EEnemyDeathSource enum.<br/>
    /// NOTE: This function must be used <em>after</em> checking if the target has actually been hit.
    /// </summary>
    /// <returns>EEnemyDeathSource.Cannon if hit by Cannon w/ NO ricochet,<br/>
    /// EEnemyDeathSource.CannonRicochet OTHERWISE.</returns>
    public readonly EEnemyDeathSource GetHitAsDeathSource() {
        return hitStatusThisFrame == 1 ? EEnemyDeathSource.Cannon : EEnemyDeathSource.CannonRicochet;
    }

	/// <summary>
	/// This function can be used to both check and consume the Hit event.
    /// This is regardless of if the hit is by ricochet or not.<br/>
	/// To only check the Hit event, use IsHit().<br/>
    /// To only check the Hit as ricochet event, use IsHitAsRicochet().
	/// </summary>
	/// <returns>Hit status. 0 for hit, 1 for hit no ric, 2 for hit as ric.</returns>
	public byte TryConsumeHitEvent() {
        byte ret = hitStatusThisFrame;
        if (hitStatusThisFrame > 0)
            hitStatusThisFrame = 0;
        return ret;
    }

    /// <summary>
    /// Consumes the hit event without checking if the hit event was set in the first place.<br/>
    /// Use TryConsumeHitEvent() instead if you want to only consume the event when it's actually set.
    /// </summary>
    public void ConsumeHitEvent() {
        hitStatusThisFrame = 0;
    }

    public void ResetEvents() {
        hitStatusThisFrame = 0;
    }
}