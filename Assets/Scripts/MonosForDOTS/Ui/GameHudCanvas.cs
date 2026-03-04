using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class GameHudCanvas : MonoBehaviour {

	[SerializeField]
	GameObject Toggler;

	[Header("MomentumAffectable")]
	public Animator FuelOutlineAnimator; // TODO: use
	public Image FuelSliderFill;
	public RectTransform FuelSliderLevel;

	public Animator HealthFillAnimator; // TODO: use
	public Animator HealthOutlineAnimator; // TODO: use
	public Image HealthSliderFill;
	public RectTransform HealthSliderLevel;
	public RawImage HealthVignette;

	[Header("Crosshairs")]
	public RectTransform MainVacuumCrosshair;
	public RectTransform MainCannonCrosshair;

	[Header("Misc")]
	public RectTransform MomentumPanel;

	// TODO: Organize this stuff
	[Range(0.01f, 10)]
	public float swf = 2;
	[Range(0, 1)]
	public float swz = 0.6f;
	[Range(-2, 2)]
	public float swr = 2;
	[SerializeField]
	[Range(5, 25)]
	float maxLookDelta = 7;
	[Range(5, 50)]
	[SerializeField]
	float maxSwayDist = 30;
	[SerializeField]
	float swayYExaggerateFactor = 2f;
	[Range(0.01f, 10)]
	public float scf = 3;
	[Range(0, 1)]
	public float scz = 1;
	[Range(-2, 2)]
	public float scr = 1;
	[SerializeField]
	float AdditionalScale = 0.17f; // The scale will reach this amount at a speed of HighAdditionalScaleSpeed
	[SerializeField]
	float HighAdditionalScaleSpeed = 50; // The scale will reach AdditionalScale at this speed

	const float BENT_BAR_MAX_FILL = 0.2f;
	// RAD = SLIDER_BG.Width / 2 * SLIDER_BG.Scale - SLIDER_LEVEL.Width / 2
	const float BENT_BAR_CENTERED_RADIUS = 115f * 1.927799f - 22.16969f / 2f;

	SecondOrderDynamicsF sodLookX;
	SecondOrderDynamicsF sodLookY;
	SecondOrderDynamicsF sodSpeed;

	Camera mainCamera;

	EntityManager entityManager;
	EntityQuery eqPlayer;
	EntityQuery eqPlayerPivot;
	EntityQuery eqPlayerCannon;



	private void Awake() {
		Toggler.SetActive(false);
		sodLookX = new SecondOrderDynamicsF(swf, swz, swr, 0);
		sodLookY = new SecondOrderDynamicsF(swf, swz, swr, 0);
		sodSpeed = new SecondOrderDynamicsF(scf, scz, scr, 0);
	}

	void Start() {
		mainCamera = Camera.main;

		entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		eqPlayer = entityManager.CreateEntityQuery(
			ComponentType.ReadOnly<Player>(),
			ComponentType.ReadOnly<PlayerResources>(),
			ComponentType.ReadOnly<PlayerInput>(),
			ComponentType.ReadOnly<PhysicsVelocity>());
		eqPlayerPivot = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerPivot>(), ComponentType.ReadOnly<LocalTransform>());
		eqPlayerCannon = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerCannon>());
	}

	void LateUpdate() {
		if (eqPlayer.HasSingleton<Player>()) {
			if (!Toggler.activeSelf)
				Toggler.SetActive(true);
		} else {
			if (Toggler.activeSelf)
				Toggler.SetActive(false);
		}
		PhysicsVelocity pv = eqPlayer.GetSingleton<PhysicsVelocity>();
		HandleCrosshairUi(pv);
		HandleResourceUi(eqPlayer.GetSingleton<PlayerResources>());
		//HandleMomentumSway(eqPlayer.GetSingleton<PlayerInput>());
		//HandleMomentumSpeed(pv);
	}

	// TODO: Clean code
	private void HandleMomentumSway(in PlayerInput pinput) {
		Vector2 newPos = new Vector2(
			sodLookX.Update(Mathf.Clamp(-pinput.LookInputDelta.x / maxLookDelta, -1, 1), Time.deltaTime),
			sodLookY.Update(Mathf.Clamp(-pinput.LookInputDelta.y / maxLookDelta, -1, 1), Time.deltaTime) * swayYExaggerateFactor
		);
		MomentumPanel.anchoredPosition = newPos * maxSwayDist;
	}

	// TODO: Clean code
	private void HandleMomentumSpeed(in PhysicsVelocity pv) {
		Debug.Log($"Speed: {pv.Linear}");
		float newScale = Mathf.LerpUnclamped(1, 1 - AdditionalScale, sodSpeed.Update(math.length(pv.Linear), Time.deltaTime) / HighAdditionalScaleSpeed);
		MomentumPanel.localScale = new Vector3(newScale, newScale, 1);
	}

	private void HandleCrosshairUi(in PhysicsVelocity playerVelocity) {
		if (!eqPlayerCannon.TryGetSingleton(out PlayerCannon playerCannon))
			return;
		if (!eqPlayerPivot.TryGetSingleton(out LocalTransform pivotTrans))
			return;

		const float InheritedVelocityFactor = 1f; // TODO ? unsure if this feature from the past will be kept.

		float3 rbVelocityCompensation = !Util.IsNearZero(math.lengthsq(playerVelocity.Linear)) ? playerVelocity.Linear * InheritedVelocityFactor / playerCannon.ProjectileSpeed : float3.zero;
		float3 camPos = mainCamera.transform.position;
		float3 pivotForward = pivotTrans.Forward();
		// The vacuum does not account for the player's velocity
		Vector3 screenPointVacuum = mainCamera.WorldToScreenPoint(camPos + pivotForward);
		// The cannon does account for the player's velocity
		Vector3 screenPointCannon = mainCamera.WorldToScreenPoint(camPos - pivotForward + rbVelocityCompensation);

		if (screenPointVacuum.z > 0.01f) {
			if (!MainVacuumCrosshair.gameObject.activeSelf)
				MainVacuumCrosshair.gameObject.SetActive(true);
			MainVacuumCrosshair.position = screenPointVacuum;
		} else {
			if (MainVacuumCrosshair.gameObject.activeSelf)
				MainVacuumCrosshair.gameObject.SetActive(false);
		}
		if (screenPointCannon.z > 0.01f) {
			if (!MainCannonCrosshair.gameObject.activeSelf)
				MainCannonCrosshair.gameObject.SetActive(true);
			MainCannonCrosshair.position = screenPointCannon;
		} else {
			if (MainCannonCrosshair.gameObject.activeSelf)
				MainCannonCrosshair.gameObject.SetActive(false);
		}
	}

	private void HandleResourceUi(in PlayerResources resources) {
		UpdateFuelUi(resources.Fuel);
		UpdateHealthUi(resources.Health);
		EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
		using (NativeArray<DeadEnemyTag> tags = em.CreateEntityQuery(typeof(DeadEnemyTag)).ToComponentDataArray<DeadEnemyTag>(Allocator.Temp)) {
			int byVacuum = 0;
			int byCannon = 0;
			for (int i = 0; i < tags.Length; i++) {
				DeadEnemyTag tag = tags[i];
				if (tag.DeathSource == EEnemyDeathSource.Vacuum) {
					byVacuum++;
				} else if (tag.DeathSource == EEnemyDeathSource.Cannon) {
					byCannon++;
				}
			}
			//if (byVacuum > 0 || byCannon > 0)
			//	Debug.LogWarning($"Enemies killed! By vac: {byVacuum} | By can: {byCannon}");
		}
		if (resources.DidRefillFuelThisFrame()) {
			// TODO
		}
	}

	// TODO: current code is written for the sake of just getting something working. Improve it
	void UpdateFuelUi(float fuel) {
		float fill = fuel / 100f;
		//float fill = Mathf.Clamp(sodFuelFill.Update(currFuel, Time.deltaTime), 0, 1);
		FuelSliderFill.fillAmount = fill * BENT_BAR_MAX_FILL;
		if (fill >= 0.001 && fill <= 0.999) {
			if (!FuelSliderLevel.gameObject.activeSelf)
				FuelSliderLevel.gameObject.SetActive(true);
			float ang = (2 * fill - 1) * (180f * BENT_BAR_MAX_FILL);
			FuelSliderLevel.localPosition = new Vector3(
				BENT_BAR_CENTERED_RADIUS * Mathf.Cos(ang * Mathf.Deg2Rad),
				BENT_BAR_CENTERED_RADIUS * Mathf.Sin(ang * Mathf.Deg2Rad),
				0
			);
			FuelSliderLevel.localRotation = Quaternion.Euler(0, 0, ang);
		} else if (FuelSliderLevel.gameObject.activeSelf)
			FuelSliderLevel.gameObject.SetActive(false);
	}

	// TODO: current code is written for the sake of just getting something working. Improve it
	void UpdateHealthUi(float health) {
		float fill = health / 100f;
		//float fill = Mathf.Clamp(sodFuelFill.Update(currFuel, Time.deltaTime), 0, 1);
		HealthSliderFill.fillAmount = fill * BENT_BAR_MAX_FILL;
		if (fill >= 0.001 && fill <= 0.999) {
			if (!HealthSliderLevel.gameObject.activeSelf)
				HealthSliderLevel.gameObject.SetActive(true);
			float ang = 180 - (2 * fill - 1) * (180f * BENT_BAR_MAX_FILL);
			HealthSliderLevel.localPosition = new Vector3(
				BENT_BAR_CENTERED_RADIUS * Mathf.Cos(ang * Mathf.Deg2Rad),
				BENT_BAR_CENTERED_RADIUS * Mathf.Sin(ang * Mathf.Deg2Rad),
				0
			);
			HealthSliderLevel.localRotation = Quaternion.Euler(0, 0, ang);
		} else if (HealthSliderLevel.gameObject.activeSelf)
			HealthSliderLevel.gameObject.SetActive(false);

		HealthVignette.color = new Color(0.65f, 0, 0, 1 - fill);
	}

	//bool TryGetPlayer(out Entity playerEntity) {
	//	bool found = eqPlayer.TryGetSingletonEntity<Player>(out playerEntity);
	//	if (found) {
	//		if (!Toggler.activeSelf)
	//			Toggler.SetActive(true);
	//	} else {
	//		if (Toggler.activeSelf)
	//			Toggler.SetActive(false);
	//	}
	//	return found;
	//}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//bool TryGetPlayerPivotLocalTrans(out LocalTransform trans) {
	//	return eqPlayerPivot.TryGetSingleton(out trans);
	//}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//bool TryGetPlayerCannon(out PlayerCannon playerCannon) {
	//	return eqPlayerCannon.TryGetSingleton(out playerCannon);
	//}

	///// <summary>
	///// Internally calls TryGetPlayer().
	///// </summary>
	///// <param name="resources"></param>
	///// <returns></returns>
	//bool TryGetPlayerResources(out PlayerResources resources) {
	//	if (!TryGetPlayer(out Entity playerEntity)) {
	//		resources = new();
	//		return false;
	//	}
	//	resources = entityManager.GetComponentData<PlayerResources>(playerEntity);
	//	return true;
	//}

}
