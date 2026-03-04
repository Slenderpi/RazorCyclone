using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerVacuumVFX : MonoBehaviour {

	public GameObject vacuumVFXPrefab;

	GameObject vacuumVFX;
	bool isPlayingVFX = false;
	ParticleSystem[] vacuumParticles;
	
	EntityQuery eqVacuum;



	private void Awake() {
		vacuumVFX = Instantiate(vacuumVFXPrefab);
		vacuumParticles = vacuumVFX.GetComponentsInChildren<ParticleSystem>();
		StopVacuumVFX();
		vacuumVFX.SetActive(false);

		eqVacuum = World.DefaultGameObjectInjectionWorld
						.EntityManager
						.CreateEntityQuery(
							ComponentType.ReadOnly<PlayerVacuum>(),
							ComponentType.ReadOnly<LocalToWorld>()
						);
	}

	private void LateUpdate() {
		if (!eqVacuum.TryGetSingleton(out PlayerVacuum pv)) {
			// No vacuum in the world right now, so ensure the vfx object as a whole is disabled
			if (vacuumVFX.activeSelf)
				vacuumVFX.SetActive(false);
			return;
		} else if (!vacuumVFX.activeSelf)
			vacuumVFX.SetActive(true);

		if (pv.VacuumEnabled) {
			if (!isPlayingVFX)
				PlayVacuumVFX();
		} else {
			if (isPlayingVFX)
				StopVacuumVFX();
		}

		// Update Vacuum vfx transform
		LocalToWorld ltw = eqVacuum.ToComponentDataArray<LocalToWorld>(Allocator.Temp)[0];
		vacuumVFX.transform.SetPositionAndRotation(ltw.Position, ltw.Rotation);
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
	
}
