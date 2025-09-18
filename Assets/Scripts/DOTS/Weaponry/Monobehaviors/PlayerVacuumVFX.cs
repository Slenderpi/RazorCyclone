using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerVacuumVFX : MonoBehaviour {

	public GameObject vacuumVFXPrefab;

	GameObject vacuumVFX;
	bool isPlayingVFX = false;
	ParticleSystem[] vacuumParticles;



	private void Awake() {
		vacuumVFX = Instantiate(vacuumVFXPrefab);
		vacuumVFX.SetActive(false);
		vacuumParticles = vacuumVFX.GetComponentsInChildren<ParticleSystem>();
		StopVacuumVFX();
	}

	private void LateUpdate() {
		EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
		EntityQuery eq = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerVacuum, LocalToWorld>().Build(em);
		NativeArray<PlayerVacuum> vacuumArr = eq.ToComponentDataArray<PlayerVacuum>(Allocator.Temp);
		if (vacuumArr.Length == 0) {
			// No vacuum in the world right now, so ensure the vfx object as a whole is disabled
			if (vacuumVFX.activeSelf)
				vacuumVFX.SetActive(false);
			return;
		} else if (!vacuumVFX.activeSelf)
			vacuumVFX.SetActive(true);

		// Play/stop vacuum vfx accordingly
		PlayerVacuum vacuum = vacuumArr[0];
		if (vacuum.VacuumEnabled) {
			if (!isPlayingVFX)
				PlayVacuumVFX();
		} else {
			if (isPlayingVFX)
				StopVacuumVFX();
		}

		UpdateVacuumVFXPositionRotation(eq);
	}

	void PlayVacuumVFX() {
		foreach (ParticleSystem p in vacuumParticles)
			p.Play();
		isPlayingVFX = true;
	}

	void StopVacuumVFX() {
		foreach (ParticleSystem p in vacuumParticles)
			p.Stop();
		isPlayingVFX = false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void UpdateVacuumVFXPositionRotation(in EntityQuery vacuumQuery) {
		LocalToWorld ltw = vacuumQuery.ToComponentDataArray<LocalToWorld>(Allocator.Temp)[0];
		vacuumVFX.transform.SetPositionAndRotation(ltw.Position, ltw.Rotation);
	}

}
