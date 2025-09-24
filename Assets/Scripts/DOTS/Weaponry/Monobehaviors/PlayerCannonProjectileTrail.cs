using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerCannonProjectileTrail : MonoBehaviour {

    [HideInInspector]
    public Entity AttachedEntity;

	[SerializeField]
	TrailRenderer TrailNormal;
	[SerializeField]
	TrailRenderer TrailRicochet;

	EntityManager em;

	bool isDisabled = false;



	private void Awake() {
		TrailNormal.emitting = false;
		TrailRicochet.emitting = false;
	}

	private void Start() {
		em = World.DefaultGameObjectInjectionWorld.EntityManager;
		if (HasComponent<PlayerCannonProjectile>() && GetCannonProjectile().RemainingRicochets == 0)
			TrailNormal.emitting = true;
		else
			TrailRicochet.emitting = true;
	}

	private void LateUpdate() {
		if (isDisabled)
			return;
		if (AttachedEntityStillExists()) {
			DestroyTrailAndDisable();
		} else {
			TrySetTrailToNormal();
			UpdateTrailTransform();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void TrySetTrailToNormal() {
		if (HasComponent<PlayerCannonProjectile>() && GetCannonProjectile().RemainingRicochets == 0) {
			TrailNormal.emitting = true;
			TrailRicochet.emitting = false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void UpdateTrailTransform() {
		if (HasComponent<LocalToWorld>()) {
			LocalToWorld refTrans = GetEntityTransform();
			transform.SetPositionAndRotation(refTrans.Position, refTrans.Rotation);
		}
	}

	void DestroyTrailAndDisable() {
		isDisabled = true;
		if (TrailNormal.emitting)
			TrailNormal.emitting = false;
		if (TrailRicochet.emitting)
			TrailRicochet.emitting = false;
		Destroy(gameObject, 1f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	LocalToWorld GetEntityTransform() {
		return em.GetComponentData<LocalToWorld>(AttachedEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	PlayerCannonProjectile GetCannonProjectile() {
		return em.GetComponentData<PlayerCannonProjectile>(AttachedEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool HasComponent<T>() where T : IComponentData {
		return em.HasComponent<T>(AttachedEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool AttachedEntityStillExists() {
		return AttachedEntity == Entity.Null || !em.Exists(AttachedEntity);
	}

}
