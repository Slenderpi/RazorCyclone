using System;
using System.Runtime.CompilerServices;
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
	}

	public void OnBackButtonClicked() {
		GameManager.ChangeMenuTo(EMenu.Pause);
	}

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
