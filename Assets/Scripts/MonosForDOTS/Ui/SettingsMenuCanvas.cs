using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Mathematics;
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
	GameObject ApplyScreenChangesButton;
	[SerializeField]
	TMP_Dropdown FullScreenModeDropdown;
	[SerializeField]
	TMP_Dropdown ResolutionDropdown;
	[SerializeField]
	TMP_Dropdown RefreshRateDropdown;
	[SerializeField]
	GameObject RefreshRateDisabler;
	[SerializeField]
	Toggle VSyncToggle;
	[SerializeField]
	Slider FpsLimitSlider;
	[SerializeField]
	TMP_InputField FpsLimitInputField;
	[SerializeField]
	GameObject FpsLimitDisabler;
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

	private void Start() {
		InitResolutionDropdown();
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
		MouseSenseInputField.SetTextWithoutNotify(Mathf.RoundToInt(val).ToString());
	}

	public void OnMouseSenseInputEndEdit(string val) {
		if (!float.TryParse(val, out float desiredMouseSense) || desiredMouseSense <= 0f || desiredMouseSense >= 10000f) {
			// Invalid values are ignored and the input field gets reset
			MouseSenseInputField.SetTextWithoutNotify(GameManager.GetGameSettings().ControlSettings.MouseSensitivity.ToString());
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

	public void OnFullScreenDropdownValueChanged(int index) {
		pendingChanges.ScreenSettings.FullScreenMode = FullScreenModeOptionToEnum(index);
		if (pendingChanges.ScreenSettings.FullScreenMode == FullScreenMode.ExclusiveFullScreen) {
			if (RefreshRateDisabler.activeSelf)
				RefreshRateDisabler.SetActive(false);
			UpdateRefreshRateOptions();
		} else if (!RefreshRateDisabler.activeSelf)
			RefreshRateDisabler.SetActive(true);
		OnScreenSettingsChanged();
	}

	public void OnResolutionDropdownValueChanged(int index) {
		pendingChanges.ScreenSettings.CurrentResolutionOptionChoice = index;
		UpdateRefreshRateOptions();
		OnScreenSettingsChanged();
	}

	public void OnRefreshRateDropdownValueChanged(int index) {
		SetPendingRefreshRateFromDropdown(index);
		Debug.Log($"Selected refresh rate: {pendingChanges.ScreenSettings.CurrentRefreshRate.value}");
		OnScreenSettingsChanged();
	}

	public void OnVSyncValueChanged(bool isOn) {
		pendingChanges.ScreenSettings.IsVsyncEnabled = isOn;
		FpsLimitDisabler.SetActive(isOn);
		OnScreenSettingsChanged();
	}

	public void OnFpsLimitSliderValueChanged(float val) {
		int fps = val >= FpsLimitSlider.maxValue ? -1 : (int)val;
		pendingChanges.ScreenSettings.FpsLimit = fps;
		FpsLimitInputField.SetTextWithoutNotify(FpsLimitToString(fps));
		OnScreenSettingsChanged();
	}

	public void OnFpsLimitInputValueChanged(string str) {
		if (str == string.Empty)
			return;
		if (!int.TryParse(str, out int _))
			// This handles the case where the user attempts to input '-'.
			FpsLimitInputField.SetTextWithoutNotify("");
	}

	public void OnFpsLimitInputEndEdit(string str) {
		if (int.TryParse(str, out int desiredFpsLimit)) {
			if (desiredFpsLimit < FpsLimitSlider.minValue)
				desiredFpsLimit = (int)FpsLimitSlider.minValue;
			else if (desiredFpsLimit >= FpsLimitSlider.maxValue)
				desiredFpsLimit = -1;
			pendingChanges.ScreenSettings.FpsLimit = desiredFpsLimit;
		}
		FpsLimitSlider.SetValueWithoutNotify(pendingChanges.ScreenSettings.FpsLimit != -1 ? pendingChanges.ScreenSettings.FpsLimit : FpsLimitSlider.maxValue);
		FpsLimitInputField.SetTextWithoutNotify(FpsLimitToString(pendingChanges.ScreenSettings.FpsLimit));
		OnScreenSettingsChanged();
	}

	public void OnFovSliderValueChanged(float val) {
		int fov = (int)val;
		FovInputField.SetTextWithoutNotify(fov.ToString());
		pendingChanges.ScreenSettings.FieldOfView = fov;
		OnScreenSettingsChanged();
	}

	public void OnFovInputValueChanged(string str) {
		if (str == string.Empty)
			return;
		if (!float.TryParse(str, out float _))
			// This handles the case where the user attempts to input '-'.
			FovInputField.SetTextWithoutNotify("");
	}

	public void OnFovInputEndEdit(string str) {
		if (int.TryParse(str, out int desiredFov)) {
			if (desiredFov < FovSlider.minValue)
				desiredFov = (int)FovSlider.minValue;
			else if (desiredFov > FovSlider.maxValue)
				desiredFov = (int)FovSlider.maxValue;
			pendingChanges.ScreenSettings.FieldOfView = desiredFov;
		}
		FovSlider.SetValueWithoutNotify(pendingChanges.ScreenSettings.FieldOfView);
		FovInputField.SetTextWithoutNotify(pendingChanges.ScreenSettings.FieldOfView.ToString());
		OnScreenSettingsChanged();
	}

	public void OnScreenApplyButtonClicked() {
		hasPendingChanges = false;
		ApplyScreenChangesButton.SetActive(false);
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
			{ // Discard changes for now
				DiscardPendingScreenChanges();
			}
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
				ApplyScreenChangesButton.SetActive(false);
				break;
			case ESettingsCategory.Quality:
				QualityCategory.SetActive(newEnabled);
				ButtonQuality.interactable = !newEnabled;
				ApplyQualityChangesButton.gameObject.SetActive(false);
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
				ApplyScreenChangesButton.SetActive(false);
			}
		} else {
			if (!hasPendingChanges) {
				hasPendingChanges = true;
				ApplyScreenChangesButton.SetActive(true);
			}
		}
	}

	void DiscardPendingScreenChanges() {
		hasPendingChanges = false;
		ApplyScreenChangesButton.SetActive(false);
		pendingChanges.ScreenSettings.SetFrom(GameManager.GetGameSettings().ScreenSettings);
		SetScreenSettingsTo(pendingChanges.ScreenSettings);
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
		MouseSenseInputField.SetTextWithoutNotify(csettings.MouseSensitivity.ToString());
		MouseSenseSlider.SetValueWithoutNotify(csettings.MouseSensitivity);

		/** SCREEN **/
		GameScreenSettings ssettings = GameManager.GetGameSettings().ScreenSettings;
		SetScreenSettingsTo(ssettings);

		/** QUALITY **/
		GameQualitySettings qsettings = GameManager.GetGameSettings().QualitySettings;


		/** AUDIO **/
		GameAudioSettings asettings = GameManager.GetGameSettings().AudioSettings;

	}

	void InitResolutionDropdown() {
		int2[] resOps = GameManager.Resolutions.GetAllResolutionsNoRefreshRate();
		List<string> optStrs = new(resOps.Length);
		for (int i = 0; i < resOps.Length; i++)
			optStrs.Add($"{resOps[i].x} x {resOps[i].y}");
		ResolutionDropdown.ClearOptions();
		ResolutionDropdown.AddOptions(optStrs);
	}

	void UpdateRefreshRateOptions() {
		ResolutionOptions resops = GameManager.Resolutions;
		GameScreenSettings ss = pendingChanges.ScreenSettings;
		RefreshRate rrBeforeDropdownUpdate = pendingChanges.ScreenSettings.CurrentRefreshRate;
		int rrdIndex = resops.GetRefreshRateOptionSameOrBetterTo(ss.CurrentRefreshRate, ss.CurrentResolutionOptionChoice);
		RefreshRateDropdown.ClearOptions();
		RefreshRateDropdown.AddOptions(resops.GetRefreshRatesOptionsForResolution(ss.CurrentResolutionOptionChoice));
		RefreshRateDropdown.SetValueWithoutNotify(rrdIndex);
		SetPendingRefreshRateFromDropdown(rrdIndex);
		if (!Util.equal(rrBeforeDropdownUpdate, pendingChanges.ScreenSettings.CurrentRefreshRate)) {
			Debug.Log($"[Settings-RefreshRate]: Resolution dropdown update caused the selected RefreshRate to change (was {rrBeforeDropdownUpdate.value}, now {pendingChanges.ScreenSettings.CurrentRefreshRate.value}).");
			OnScreenSettingsChanged();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void SetPendingRefreshRateFromDropdown(int dropdownIndex) {
		pendingChanges.ScreenSettings.CurrentRefreshRate = GameManager.Resolutions.GetRefreshRateFrom(
			pendingChanges.ScreenSettings.CurrentResolutionOptionChoice,
			dropdownIndex
		);
	}

	void SetScreenSettingsTo(GameScreenSettings settings) {
		FullScreenModeDropdown.SetValueWithoutNotify(FullScreenModeEnumToOption(settings.FullScreenMode));
		ResolutionDropdown.SetValueWithoutNotify(settings.CurrentResolutionOptionChoice);
		if (settings.FullScreenMode == FullScreenMode.ExclusiveFullScreen) {
			RefreshRateDisabler.SetActive(false);
			UpdateRefreshRateOptions();
		} else
			RefreshRateDisabler.SetActive(true);
		VSyncToggle.SetIsOnWithoutNotify(settings.IsVsyncEnabled);
		FpsLimitSlider.SetValueWithoutNotify(settings.FpsLimit != -1 ? settings.FpsLimit : FpsLimitSlider.maxValue);
		FpsLimitInputField.SetTextWithoutNotify(FpsLimitToString(settings.FpsLimit));
		FpsLimitDisabler.SetActive(settings.IsVsyncEnabled);
		FovSlider.SetValueWithoutNotify(settings.FieldOfView);
		FovInputField.SetTextWithoutNotify(settings.FieldOfView.ToString());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	string FpsLimitToString(int fpsLimit) {
		return fpsLimit == -1 ? "Unlimited" : fpsLimit.ToString();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	FullScreenMode FullScreenModeOptionToEnum(int option) {
		return option switch {
			0 => FullScreenMode.ExclusiveFullScreen,
			1 => FullScreenMode.FullScreenWindow,
			_ => FullScreenMode.Windowed,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	int FullScreenModeEnumToOption(FullScreenMode fsm) {
		return fsm switch {
			FullScreenMode.ExclusiveFullScreen => 0,
			FullScreenMode.FullScreenWindow => 1,
			_ => 2
		};
	}

}
