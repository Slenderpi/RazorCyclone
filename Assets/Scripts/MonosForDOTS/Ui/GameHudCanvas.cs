using Unity.Entities;
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

	const float BENT_BAR_MAX_FILL = 0.2f;
	// RAD = SLIDER_BG.Width / 2 * SLIDER_BG.Scale - SLIDER_LEVEL.Width / 2
	const float BENT_BAR_CENTERED_RADIUS = 115f * 1.927799f - 22.16969f / 2f;

	EntityManager entityManager;
	//Entity playerEntity;



	private void Awake() {
		Toggler.SetActive(false);
	}

	void Start() {
		entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		//EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<Player>());
		//playerEntity = query.GetSingletonEntity();
	}

	void Update() {
		if (!TryGetPlayer(out Entity playerEntity))
			return;
		PlayerResources resources = entityManager.GetComponentData<PlayerResources>(playerEntity);
		UpdateFuelUi(resources.Fuel);
		UpdateHealthUi(resources.Health);
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

	bool TryGetPlayer(out Entity playerEntity) {
		bool found = entityManager.CreateEntityQuery(ComponentType.ReadOnly<Player>()).TryGetSingletonEntity<Player>(out playerEntity);
		if (found) {
			if (!Toggler.activeSelf)
				Toggler.SetActive(true);
		} else {
			if (Toggler.activeSelf)
				Toggler.SetActive(false);
		}
		return found;
	}

}
