using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuCanvas : MonoBehaviour {

	enum ESettingsCategory {
		Controls,
		Screen,
		Quality,
		Audio
	}

	[SerializeField]
	Canvas canvasComp;

	[Header("Category containers")]
	[SerializeField]
	GameObject ControlsCategory;
	[SerializeField]
	GameObject ScreenCategory;
	[SerializeField]
	GameObject QualityCategory;
	[SerializeField]
	GameObject AudioCategory;

	[Header("Category buttons")]
	[SerializeField]
	Button ButtonControls;
	[SerializeField]
	Button ButtonScreen;
	[SerializeField]
	Button ButtonQuality;
	[SerializeField]
	Button ButtonAudio;

	[Header("Controls references")]
	[SerializeField]
	TMP_InputField MouseSenseInputField;
	[SerializeField]
	Slider MouseSenseSlider;

	[Header("Screen references")]
	[SerializeField]
	Button ApplyScreenChangesButton;
	[SerializeField]
	Toggle FullscreenToggle;
	[SerializeField]
	TMP_Dropdown ResolutionDropdown;
	[SerializeField]
	GameObject CustomResolutionDisabler;
	[SerializeField]
	Slider FovSlider;
	[SerializeField]
	TMP_InputField FovInputField;

	[Header("Quality references")]
	[SerializeField]
	Button ApplyQualityChangesButton;

	ESettingsCategory currentCategory = ESettingsCategory.Controls;

	bool hasPendingChanges = false;
	GameSettings pendingChanges;



	private void Awake() {
		canvasComp.enabled = false;

		ButtonControls.onClick.AddListener(() => ChangeCategoryTo(ESettingsCategory.Controls));
		ButtonScreen.onClick.AddListener(() => ChangeCategoryTo(ESettingsCategory.Screen));
		ButtonQuality.onClick.AddListener(() => ChangeCategoryTo(ESettingsCategory.Quality));
		ButtonAudio.onClick.AddListener(() => ChangeCategoryTo(ESettingsCategory.Audio));

		InitCategoryAndCategoryButtonStates();

		GameManager.A_OnMenuChanged += OnMenuChanged;
		//GameManager.A_OnPlayerSpawned += OnPlayerSpawned; // TODO: Make a better way to Init() the settings ui
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
		pendingChanges.ControlSettings.MouseSensitivity = val;
		GameManager.ChangeControlSettings(pendingChanges.ControlSettings);
		MouseSenseInputField.SetTextWithoutNotify($"{Mathf.RoundToInt(val)}");
	}

	public void OnMouseSenseInputEndEdit(string val) {
		if (!float.TryParse(val, out float desiredMouseSense) || desiredMouseSense <= 0f || desiredMouseSense >= 10000f) {
			// Invalid values are ignored and the input field gets reset
			MouseSenseInputField.SetTextWithoutNotify($"{GameManager.GetGameSettings().ControlSettings.MouseSensitivity}");
			return;
		}
		pendingChanges.ControlSettings.MouseSensitivity = desiredMouseSense;
		GameManager.ChangeControlSettings(pendingChanges.ControlSettings);
		MouseSenseSlider.SetValueWithoutNotify(Mathf.Clamp(desiredMouseSense, MouseSenseSlider.minValue, MouseSenseSlider.maxValue));
	}

	//======================================================================================//

	/****************************************************************************************/
	/*																						*/
	/*										SCREEN											*/
	/*																						*/
	/****************************************************************************************/

	public void OnFullscreenValueChanged(bool isOn) {
		pendingChanges.ScreenSettings.IsFullscreen = isOn;
		CustomResolutionDisabler.SetActive(isOn || !pendingChanges.ScreenSettings.IsUsingCustomResolution);
		OnScreenSettingsChanged();
	}

	public void OnResolutionDropdownValueChanged(int index) {
		if (index == 0) {
			pendingChanges.ScreenSettings.IsUsingCustomResolution = true;
			// TODO: Custom resolution input fields
			pendingChanges.ScreenSettings.ScreenResolution = new() {
				width = 1, // TODO
				height = 1, // TODO
				refreshRateRatio = pendingChanges.ScreenSettings.ScreenResolution.refreshRateRatio
			};
			CustomResolutionDisabler.SetActive(pendingChanges.ScreenSettings.IsFullscreen);
			Debug.Log("Selected resolution: CUSTOM");
		} else {
			pendingChanges.ScreenSettings.IsUsingCustomResolution = false;
			pendingChanges.ScreenSettings.ScreenResolution = new() {
				width = GameManager.Resolutions.GetAllResolutions()[index - 1].width,
				height = GameManager.Resolutions.GetAllResolutions()[index - 1].height,
				refreshRateRatio = pendingChanges.ScreenSettings.ScreenResolution.refreshRateRatio
			};
			CustomResolutionDisabler.SetActive(true);
			Debug.Log($"Selected resolution: {pendingChanges.ScreenSettings.ScreenResolution.width} x {pendingChanges.ScreenSettings.ScreenResolution.height}");
		}
		OnScreenSettingsChanged();
	}

	public void OnFovSliderValueChanged(float val) {
		val = Mathf.Round(val);
		Debug.Log($"Fov changed via slider to {val}");
		FovInputField.SetTextWithoutNotify($"{val}");
		pendingChanges.ScreenSettings.FieldOfView = val;
	}

	public void OnFovInputValueChanged(string str) {
		if (str == string.Empty)
			return;
		if (!float.TryParse(str, out float _))
			// This handles the case where the user attempts to input '-'.
			FovInputField.SetTextWithoutNotify("");
	}

	public void OnFovInputEndEdit(string str) {
		if (float.TryParse(str, out float desiredFov)) {
			if (desiredFov < FovSlider.minValue)
				desiredFov = FovSlider.minValue;
			else if (desiredFov > FovSlider.maxValue)
				desiredFov = FovSlider.maxValue;
			pendingChanges.ScreenSettings.FieldOfView = desiredFov;
		}
		FovSlider.SetValueWithoutNotify(pendingChanges.ScreenSettings.FieldOfView);
		FovInputField.SetTextWithoutNotify($"{pendingChanges.ScreenSettings.FieldOfView}");
	}

	public void OnScreenApplyButtonClicked() {
		hasPendingChanges = false;
		ApplyScreenChangesButton.interactable = false;
		GameManager.ChangeScreenSettings(pendingChanges.ScreenSettings);
	}

	//======================================================================================//

	/****************************************************************************************/
	/*																						*/
	/*									   QUALITY											*/
	/*																						*/
	/****************************************************************************************/



	public void OnQualityApplyButtonClicked() {
		hasPendingChanges = false;
		ApplyQualityChangesButton.interactable = false;
		GameManager.ChangeQualitySettings(pendingChanges.QualitySettings);
	}

	//======================================================================================//

	/****************************************************************************************/
	/*																						*/
	/*										AUDIO											*/
	/*																						*/
	/****************************************************************************************/



	//======================================================================================//

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ChangeCategoryTo(ESettingsCategory newCategory) {
		if (hasPendingChanges) {
			// TODO: ask to apply/discard changes first
			Debug.Log("There were pending settings changes. Pending confirmation not yet implemented, so these changes will be discarded.");
		}
		SetCategoryEnabled(currentCategory, false);
		currentCategory = newCategory;
		SetCategoryEnabled(currentCategory, true);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void OnMenuChanged(EMenu newMenu) {
		if (newMenu == EMenu.Settings) {
			canvasComp.enabled = true;
			hasPendingChanges = false;
			pendingChanges = GameManager.GetGameSettings().Clone();
			InitAllSettingsValues(); // TODO: put this in a better place so it's called fewer times
		} else
			canvasComp.enabled = false;
	}

	void SetCategoryEnabled(ESettingsCategory category, bool newEnabled) {
		switch (category) {
			case ESettingsCategory.Controls:
				ControlsCategory.SetActive(newEnabled);
				ButtonControls.interactable = !newEnabled;
				break;
			case ESettingsCategory.Screen:
				ScreenCategory.SetActive(newEnabled);
				ButtonScreen.interactable = !newEnabled;
				ApplyScreenChangesButton.interactable = false;
				break;
			case ESettingsCategory.Quality:
				QualityCategory.SetActive(newEnabled);
				ButtonQuality.interactable = !newEnabled;
				ApplyQualityChangesButton.interactable = false;
				break;
			case ESettingsCategory.Audio:
				AudioCategory.SetActive(newEnabled);
				ButtonAudio.interactable = !newEnabled;
				break;
		}
	}

	/// <summary>
	/// Checks if the pending settings are different from the current settings.<br/>
	/// If different, hasPendingChanges is set true and
	/// the apply button is enabled.<br/>
	/// If the same, hasPendingChanges is set false and the apply button is disabled.
	/// </summary>
	void OnScreenSettingsChanged() {
		if (pendingChanges.ScreenSettings == GameManager.GetGameSettings().ScreenSettings) {
			if (hasPendingChanges) {
				hasPendingChanges = false;
				ApplyScreenChangesButton.interactable = false;
			}
		} else {
			if (!hasPendingChanges) {
				hasPendingChanges = true;
				ApplyScreenChangesButton.interactable = true;
			}
		}
	}

	void InitCategoryAndCategoryButtonStates() {
		ButtonControls.interactable = true;
		ButtonScreen.interactable = true;
		ButtonQuality.interactable = true;
		ButtonAudio.interactable = true;
		ControlsCategory.SetActive(false);
		ScreenCategory.SetActive(false);
		QualityCategory.SetActive(false);
		AudioCategory.SetActive(false);
		SetCategoryEnabled(currentCategory, true);
	}

	void InitAllSettingsValues() {
		/** CONTROLS **/
		GameControlSettings csettings = GameManager.GetGameSettings().ControlSettings;
		MouseSenseInputField.SetTextWithoutNotify($"{Mathf.RoundToInt(csettings.MouseSensitivity)}");
		MouseSenseSlider.SetValueWithoutNotify(Mathf.Clamp(csettings.MouseSensitivity, MouseSenseSlider.minValue, MouseSenseSlider.maxValue));

		/** SCREEN **/
		GameScreenSettings ssettings = GameManager.GetGameSettings().ScreenSettings;
		FullscreenToggle.SetIsOnWithoutNotify(ssettings.IsFullscreen);
		{ // INIT RESOLUTION OPTIONS
			Resolution[] resOps = GameManager.Resolutions.GetAllResolutions();
			List<string> resOptStrs = new(resOps.Length + 1) { "Custom" };
			for (int i = 0; i < resOps.Length; i++)
				resOptStrs.Add($"{resOps[i].width} x {resOps[i].height}");
			ResolutionDropdown.ClearOptions();
			ResolutionDropdown.AddOptions(resOptStrs);
			int currentResOption = GameManager.Resolutions.IndexOf(ssettings.ScreenResolution);
			Debug.Log($"Current resolution ({ssettings.ScreenResolution.width} x {ssettings.ScreenResolution.height}) is " + (currentResOption == -1 ? "custom." : $"option {currentResOption + 1}."));
			// Set to either Custom or the found resolution option
			ResolutionDropdown.SetValueWithoutNotify(currentResOption == -1 ? 0 : currentResOption + 1);
		}
		CustomResolutionDisabler.SetActive(ssettings.IsFullscreen || !ssettings.IsUsingCustomResolution);
		FovSlider.SetValueWithoutNotify(ssettings.FieldOfView);
		FovInputField.SetTextWithoutNotify($"{ssettings}");

		/** QUALITY **/
		GameQualitySettings qsettings = GameManager.GetGameSettings().QualitySettings;
		
		/** AUDIO **/

	}

}
