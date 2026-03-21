using UnityEngine;

public class PauseMenuCanvas : MonoBehaviour {

	[SerializeField]
	Canvas canvasComp;



	private void Awake() {
		canvasComp.enabled = false;
	}

	private void Start() {
		GameManager.A_OnMenuChanged += OnMenuChanged;
	}

	public void OnResumeButtonClicked() {
		GameManager.ResumeGame();
	}

	public void OnSettingsButtonClicked() {
		GameManager.ChangeMenuTo(EMenu.Settings);
	}

	public void OnAlmanacButtonClicked() {
		GameManager.ChangeMenuTo(EMenu.Almanac);
	}

	public void OnMainMenuButtonClicked() {
		GameManager.GoToMainMenu();
	}

	void OnMenuChanged(EMenu newMenu) {
		canvasComp.enabled = newMenu == EMenu.Pause;
	}

}
