using System;
using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class GameHudCanvas : MonoBehaviour {

	[SerializeField]
	Canvas canvasComp;

	[Header("MomentumAffectable")]
	public RectTransform MomentumPanel;
	[Space(5)]
	public Animator FuelOutlineAnimator; // TODO: use
	public Image FuelSliderFill;
	public RectTransform FuelSliderLevel;
	[Space(5)]
	public Animator HealthFillAnimator; // TODO: use
	public Animator HealthOutlineAnimator; // TODO: use
	public Image HealthSliderFill;
	public RectTransform HealthSliderLevel;
	public RawImage HealthVignette;
	[Space(5)]
	public Image SpinCounterOutline;
	public TMP_Text SpinCounterText;
	[Space(5)]
	public Animator KillfeedImpactAnimator;
	public RectTransform KillfeedArea;
	float killFeedElemHeight;
	int currKillElem;
	public RectTransform[] KillfeedElements;
	Image[] killfeedEntryImages; // Groups of 4: card bg, wp icon, kill type icon, enemy icon
	Sprite[] killfeedSprites; // Weapon, Weapon, Weapon, KillType, KillType, Enemies...
	Animator[] killfeedCardAnimators;
	[SerializeField] Color killColorKF;
	[SerializeField] Color hitColorKF;

	[Header("Crosshairs")]
	public RectTransform MainVacuumCrosshair;
	public RectTransform MainCannonCrosshair;

	// TODO: Organize this stuff
	[Header("NEEDS ORGANIZING")]
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
	[Range(0.01f, 10)]
	public float fillf = 5;
	[Range(0, 1)]
	public float fillz = 1;
	[Range(0, 1)]
	public float fillr = 1;
	SecondOrderDynamics sodFuelFill;
	SecondOrderDynamics sodHealthFill;
	const float BENT_BAR_MAX_FILL = 0.2f;
	// RAD = SLIDER_BG.Width / 2 * SLIDER_BG.Scale - SLIDER_LEVEL.Width / 2
	const float BENT_BAR_CENTERED_RADIUS = 115f * 1.927799f - 22.16969f / 2f;

	SecondOrderDynamics sodLookX;
	SecondOrderDynamics sodLookY;
	SecondOrderDynamics sodSpeed;

	Camera mainCamera;

	EntityManager entityManager;
	EntityQuery eqPlayer;
	EntityQuery eqPlayerPivot;
	EntityQuery eqPlayerCannon;

	int lastSpinCount = 0;



	private void Awake() {
		sodLookX = new(swf, swz, swr, 0);
		sodLookY = new(swf, swz, swr, 0);
		sodSpeed = new(scf, scz, scr, 0);
		sodFuelFill = new(fillf, fillz, fillr, 1);
		sodHealthFill = new(fillf, fillz, fillr, 1);

		SpinCounterOutline.gameObject.SetActive(false);
		SpinCounterText.text = "0";

		InitKillfeed();

		canvasComp.enabled = false;
	}

	void Start() {
		GameManager.A_OnMenuChanged += OnMenuChanged;

		mainCamera = Camera.main;

		entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		eqPlayer = entityManager.CreateEntityQuery(
			ComponentType.ReadOnly<Player>(),
			ComponentType.ReadOnly<PlayerResources>(),
			ComponentType.ReadOnly<PlayerInput>(),
			ComponentType.ReadOnly<PlayerSpinfo>(),
			ComponentType.ReadOnly<PhysicsVelocity>());
		eqPlayerPivot = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerPivot>(), ComponentType.ReadOnly<LocalTransform>());
		eqPlayerCannon = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerCannon>());

		StartCoroutine(loadKFIconsAsync());
	}

	void LateUpdate() {
		if (GameManager.IsPaused)
			return;
		if (eqPlayer.HasSingleton<Player>()) {
			if (!canvasComp.enabled)
				canvasComp.enabled = true;
		} else {
			if (canvasComp.enabled)
				canvasComp.enabled = false;
			return;
		}
		PhysicsVelocity pv = eqPlayer.GetSingleton<PhysicsVelocity>();
		HandleCrosshairUi(pv);
		if (Time.deltaTime > 0) {
			HandleResourceUi(eqPlayer.GetSingleton<PlayerResources>());
			HandleMomentumSway(eqPlayer.GetSingleton<PlayerInput>());
			HandleMomentumSpeed(pv);
			HandleSpinUi();
			HandleKillfeedUi();
		}
	}

	private void HandleKillfeedUi() {
		EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
		using NativeArray<DeadEnemyTag> tags = em.CreateEntityQuery(typeof(DeadEnemyTag)).ToComponentDataArray<DeadEnemyTag>(Allocator.Temp);
		if (tags.Length > 0) {
			for (int i = 0; i < tags.Length; i++) {
				DeadEnemyTag tag = tags[i];
				OnPlayerDamagedEnemy(true, tag.DeathSource, tag.EnemyType);
			}
		}
		LerpKillfeedElemPosns();
	}

	void OnPlayerDamagedEnemy(bool wasKill, EEnemyDeathSource dtype, EEnemyType etype) {
		// TODO: Hitmarker UI
		//if (wasKill) {
		//	CannonHitmarkerAnim.SetTrigger("Hit");
		//} else {
		//	if (dtype == EDamageType.Vacuum) {
		//		VacuumHitmarkerAnim.SetTrigger("Kill");
		//	} else {
		//		CannonHitmarkerAnim.SetTrigger("Kill");
		//	}
		//}

		// Increment current card index
		currKillElem = (currKillElem + 1) % KillfeedElements.Length;

		// Position card directly at the bottom
		KillfeedElements[currKillElem].anchoredPosition = Vector2.zero;

		// Set color of card background
		killfeedEntryImages[currKillElem * 4].color = wasKill ? killColorKF : hitColorKF;

		// Set images for card icons
		killfeedEntryImages[currKillElem * 4 + 1].sprite = killfeedSprites[dtype switch {
			EEnemyDeathSource.Vacuum => 0,
			EEnemyDeathSource.Cannon => 1,
			EEnemyDeathSource.CannonRicochet => 2,
			_ => throw new Exception($"ERROR: Player damage type when damaging enemy is invalid. Type given: {dtype}.")
		}];
		// killfeedEntryImages[currKillElem * 4 + 2].texture = killfeedTextures[3];
		killfeedEntryImages[currKillElem * 4 + 3].sprite = killfeedSprites[etype < EEnemyType.COUNT ? (int)etype + 4 : 4];

		// Start animators
		killfeedCardAnimators[currKillElem].SetTrigger("Activate");
		KillfeedImpactAnimator.SetTrigger("Activate");
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
		UpdateFuelUi(Mathf.Clamp(sodFuelFill.Update(resources.Fuel / 100f, Time.deltaTime), 0, 1));
		UpdateHealthUi(Mathf.Clamp(sodHealthFill.Update(resources.Health / 100f, Time.deltaTime), 0, 1));
		if (resources.DidRefillFuelThisFrame()) {
			// TODO
		}
	}

	// TODO: Clean code
	private void HandleMomentumSway(in PlayerInput pinput) {
		Vector2 newPos = new(
			sodLookX.Update(Mathf.Clamp(-pinput.LookInputDelta.x / maxLookDelta, -1, 1), Time.deltaTime),
			sodLookY.Update(Mathf.Clamp(-pinput.LookInputDelta.y / maxLookDelta, -1, 1), Time.deltaTime) * swayYExaggerateFactor
		);
		MomentumPanel.anchoredPosition = newPos * maxSwayDist;
	}

	// TODO: Clean code
	private void HandleMomentumSpeed(in PhysicsVelocity pv) {
		float newScale = Mathf.LerpUnclamped(1, 1 - AdditionalScale, sodSpeed.Update(math.length(pv.Linear), Time.deltaTime) / HighAdditionalScaleSpeed);
		MomentumPanel.localScale = new Vector3(newScale, newScale, 1);
	}

	private void HandleSpinUi() {
		PlayerSpinfo spinfo = eqPlayer.GetSingleton<PlayerSpinfo>();
		if (lastSpinCount != spinfo.CurrentSpins) {
			if (spinfo.CurrentSpins == 0) {
				if (spinfo.WereSpinsSpentAsRicochet()) {
					// TODO: effect for spending spins
				} else {
					// TODO: effect for running out
				}
				SpinCounterOutline.gameObject.SetActive(false);
			} else {
				if (!SpinCounterOutline.gameObject.activeSelf)
					SpinCounterOutline.gameObject.SetActive(true);
				// Effect for gaining a spin (unless spins decay one by one instead of all at once, then will need to check)

				SpinCounterText.text = spinfo.CurrentSpins.ToString();
			}
			lastSpinCount = spinfo.CurrentSpins;
		}
	}

	// TODO: current code is written for the sake of just getting something working. Improve it
	void UpdateFuelUi(float fill) {
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
	void UpdateHealthUi(float fill) {
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

	void LerpKillfeedElemPosns() {
		int curr = (currKillElem - 1 + KillfeedElements.Length) % KillfeedElements.Length;
		Vector2 secondCardPos = Vector2.Lerp(
			KillfeedElements[curr].anchoredPosition,
			new(0, -killFeedElemHeight),
			0.1f
		);
		for (int i = 1; i < 4; i++) {
			curr = (currKillElem - i + KillfeedElements.Length) % KillfeedElements.Length;
			KillfeedElements[curr].anchoredPosition = secondCardPos;
			secondCardPos.y -= killFeedElemHeight;
		}
	}

	void OnMenuChanged(EMenu newMenu) {
		canvasComp.enabled = newMenu == EMenu.Gameplay;
	}

	/*************   --   INITIALIZATION AND LOADING   --   ****************/

	void InitKillfeed() {
		if (!KillfeedArea.gameObject.activeSelf)
			KillfeedArea.gameObject.SetActive(true);
		int numKFElems = KillfeedElements.Length;
		currKillElem = numKFElems - 1;
		killFeedElemHeight = KillfeedElements[0].rect.height;
		killfeedEntryImages = new Image[numKFElems * 4];
		killfeedCardAnimators = new Animator[numKFElems];
		for (int i = 0; i < numKFElems; i++) {
			// GetComponentsInChildren() is supposed to only look in children but for some reason this one includes the current gameObject
			Image[] elemImages = KillfeedElements[i].GetComponentsInChildren<Image>();
			for (int ri = 0; ri < 4; ri++)
				killfeedEntryImages[i * 4 + ri] = elemImages[ri];
			killfeedCardAnimators[i] = KillfeedElements[i].GetComponent<Animator>();
			if (killfeedCardAnimators[i].gameObject.activeSelf) killfeedCardAnimators[i].SetTrigger("Default");
		}
		if (!KillfeedImpactAnimator.gameObject.activeSelf)
			KillfeedImpactAnimator.gameObject.SetActive(true);
	}

	IEnumerator loadKFIconsAsync() {
		string[] iconPaths = {
            // Weapons
            "Killfeed Icons/Vacuum",
			"Killfeed Icons/Cannon",
			"Killfeed Icons/Ricochet",
            // Arrow
            "Killfeed Icons/S_Arrow",
            // Enemy icons
            "Killfeed Icons/Bug",
			"Killfeed Icons/Bird",
			"Killfeed Icons/Bird+",
			"Killfeed Icons/Crab",
			"Killfeed Icons/Crab+",
			"Killfeed Icons/Turtle",
			"Killfeed Icons/Centipede"
		};
		int numIcons = iconPaths.Length;
#if UNITY_EDITOR
		int successes = 0;
#endif
		killfeedSprites = new Sprite[numIcons];
		ResourceRequest rr;
		for (int i = 0; i < numIcons; i++) {
			rr = Resources.LoadAsync<Sprite>(iconPaths[i]);
			yield return rr;
			killfeedSprites[i] = rr.asset as Sprite;
#if UNITY_EDITOR
			if (killfeedSprites[i])
				successes++;
#endif
		}
#if UNITY_EDITOR
		if (successes != numIcons)
			Debug.LogWarning($"WARN: UIGamePanel failed to load all required Killfeed Icons (successfully loaded: {successes} / {numIcons}).");
#endif
		currKillElem = 0;
		foreach (Animator kfanim in killfeedCardAnimators)
			if (kfanim.gameObject.activeSelf)
				kfanim.SetTrigger("Default");
	}

	void unloadKFIcons() {
		if (killfeedSprites == null) return;
		for (int i = 0; i < killfeedSprites.Length; i++) {
			Resources.UnloadAsset(killfeedSprites[i]);
		}
	}

}
