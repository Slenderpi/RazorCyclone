using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuCanvas : MonoBehaviour {

	enum ESettingsCategory {
		Controls,
		Video,
		Audio
	}

	[SerializeField]
	Canvas canvasComp;

	[Header("Category containers")]
	public GameObject ControlsCategory;
	public GameObject VideoCategory;
	public GameObject AudioCategory;

	[Header("Category buttons")]
	public Button ButtonControls;
	public Button ButtonVideo;
	public Button ButtonAudio;

	[Header("Controls references")]
	public TMP_InputField MouseSenseInputField;
	public Slider MouseSenseSlider;

	ESettingsCategory currentCategory = ESettingsCategory.Controls;



	private void Awake() {
		canvasComp.enabled = false;

		ButtonControls.onClick.AddListener(() => ChangeCategoryTo(ESettingsCategory.Controls));
		ButtonVideo.onClick.AddListener(() => ChangeCategoryTo(ESettingsCategory.Video));
		ButtonAudio.onClick.AddListener(() => ChangeCategoryTo(ESettingsCategory.Audio));

		InitCategoryAndCategoryButtonStates();
	}

	private void Start() {
		GameManager.A_OnMenuChanged += OnMenuChanged;
		GameManager.A_OnPlayerSpawned += OnPlayerSpawned;
	}

	public void OnBackButtonClicked() {
		GameManager.ChangeMenuTo(EMenu.Pause);
	}

	/****************************************************************************************/
	/*																						*/
	/*									   CONTROLS											*/
	/*																						*/
	/****************************************************************************************/

	public void OnMouseSenseSliderValueChanged(float val) {
		GameManager.MouseSensitivity = val;
		MouseSenseInputField.SetTextWithoutNotify($"{Mathf.RoundToInt(val)}");
	}

	public void OnMouseSenseInputEndEdit(string val) {
		float desiredMouseSense = float.Parse(val);
		if (desiredMouseSense <= 0f || desiredMouseSense >= 10000f) {
			// Invalid values are ignored and the input field gets reset
			MouseSenseInputField.SetTextWithoutNotify($"{GameManager.MouseSensitivity}");
			return;
		}
		GameManager.MouseSensitivity = desiredMouseSense;
		MouseSenseSlider.SetValueWithoutNotify(Mathf.Clamp(desiredMouseSense, MouseSenseSlider.minValue, MouseSenseSlider.maxValue));
	}

	//======================================================================================//

	/****************************************************************************************/
	/*																						*/
	/*										VIDEO											*/
	/*																						*/
	/****************************************************************************************/



	//======================================================================================//

	/****************************************************************************************/
	/*																						*/
	/*										AUDIO											*/
	/*																						*/
	/****************************************************************************************/



	//======================================================================================//

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ChangeCategoryTo(ESettingsCategory newCategory) {
		SetCategoryEnabled(currentCategory, false);
		currentCategory = newCategory;
		SetCategoryEnabled(currentCategory, true);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void OnMenuChanged(EMenu newMenu) {
		canvasComp.enabled = newMenu == EMenu.Settings;
	}

	void SetCategoryEnabled(ESettingsCategory category, bool newEnabled) {
		switch (category) {
			case ESettingsCategory.Controls:
				ControlsCategory.SetActive(newEnabled);
				ButtonControls.interactable = !newEnabled;
				break;
			case ESettingsCategory.Video:
				VideoCategory.SetActive(newEnabled);
				ButtonVideo.interactable = !newEnabled;
				break;
			case ESettingsCategory.Audio:
				AudioCategory.SetActive(newEnabled);
				ButtonAudio.interactable = !newEnabled;
				break;
		}
	}

	void OnPlayerSpawned() {
		MouseSenseInputField.SetTextWithoutNotify($"{Mathf.RoundToInt(GameManager.MouseSensitivity)}");
		MouseSenseSlider.SetValueWithoutNotify(Mathf.Clamp(GameManager.MouseSensitivity, MouseSenseSlider.minValue, MouseSenseSlider.maxValue));
	}

	private void InitCategoryAndCategoryButtonStates() {
		ButtonControls.interactable = true;
		ButtonVideo.interactable = true;
		ButtonAudio.interactable = true;
		ControlsCategory.SetActive(false);
		VideoCategory.SetActive(false);
		AudioCategory.SetActive(false);
		SetCategoryEnabled(currentCategory, true);
	}

}
