using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Entities;
using UnityEngine;

public enum EMenu {
	None,
	MainMenu,
	Gameplay,
	Pause,
	Settings,
	Almanac
}

public class GameManager : MonoBehaviour {

	public static Action A_OnGameResumed;
	public static Action A_OnGamePaused;
	public static Action A_OnPlayerSpawned;

	//public static Action A_OnGameSettingsChanged;

	/// <summary>
	/// EMenu: newMenu
	/// </summary>
	public static Action<EMenu> A_OnMenuChanged;

	public static GameManager Singleton;

	bool _paused = false;
	public static bool IsPaused { get { return Singleton._paused; } private set { Singleton._paused = value; } }

	float _timeScale = 1f;
	/// <summary>
	/// Sets timeScale in a persistent manner, meaning it can be maintained before and after a pause, or set during a pause.<br/>
	/// <br/>
	/// Use this property to set Time.timeScale. Do not set Time.timeScale manually.
	/// </summary>
	public static float TimeScale { get { return Singleton._timeScale; } set { SetTimeScale(value); } }

	Entity _playerEntity;
	public static Entity PlayerEntity { get { return Singleton._playerEntity; } private set { Singleton._playerEntity = value; } }
	bool _isPlayerSpawned = false;
	public static bool IsPlayerSpawned { get { return Singleton._isPlayerSpawned; } private set { Singleton._isPlayerSpawned = value; } }

	EMenu _currentMenu;
	public static EMenu CurrentMenu { get { return Singleton._currentMenu; } private set { Singleton._currentMenu = value; } }

	static ResolutionOptions _resolutionOptions;
	public static ResolutionOptions Resolutions => _resolutionOptions;

	GameSettings _settings;

	EntityManager entityManager;



	private void Awake() {
		Singleton = this;
		_resolutionOptions = new();
		// TODO: Load settings
		_settings = GameSettings.Default;
		_settings.ScreenSettings.SetValuesBasedOnCurrentScreen(_resolutionOptions);
	}

	private void Start() {
		entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		StartCoroutine(WaitForEntityBakerSingleton());
	}

	IEnumerator WaitForEntityBakerSingleton() {
		while (!TryGetEntityBakerSingleton(out var _))
			yield return new WaitForEndOfFrame();
		TrySpawnPlayer();
	}

	public static void OnPauseKeyPressed() {
		if (IsPaused) {
			if (CurrentMenu == EMenu.Settings) {
				ChangeMenuTo(EMenu.Pause);
			} else {
				ResumeGame();
			}
		} else {
			PauseGame();
		}
	}

	/// <summary>
	/// Changes the game state to the main menu.
	/// </summary>
	public static void GoToMainMenu() {
		// TODO
		Debug.LogWarning("GameManager: Main menu not yet implemented.");
		ChangeMenuTo(EMenu.MainMenu);
	}

	public static void ResumeGame() {
		IsPaused = false;
		HideMouse();
		Time.timeScale = TimeScale;
		A_OnGameResumed?.Invoke();
		ChangeMenuTo(EMenu.Gameplay);
	}

	public static void PauseGame() {
		IsPaused = true;
		ShowMouse();
		Time.timeScale = 0f;
		A_OnGamePaused?.Invoke();
		ChangeMenuTo(EMenu.Pause);
	}

	public static bool TrySpawnPlayer() {
		if (TryGetPlayerEntity(out Singleton._playerEntity)) {
			Debug.LogWarning("An existing player was found. TrySpawnPlayer() cancelled.");
			HideMouse();
			return false;
		}
		if (!TryGetEntityBakerSingleton(out EntityBakerSingleton entityBakerSingleton)) {
			Debug.LogWarning("The EntityBakerSingleton could not be found.");
			return false;
		}
		Singleton._playerEntity = Singleton.entityManager.Instantiate(entityBakerSingleton.Player);
		IsPlayerSpawned = true;
		if (!IsPaused) {
			HideMouse();
		}
		Singleton.InitPlayer();
		A_OnPlayerSpawned?.Invoke();
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GameSettings GetGameSettings() {
		return Singleton._settings;
	}

	/// <summary>
	/// Update and apply new control settings.
	/// </summary>
	/// <param name="newSettings">New controls settings.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ChangeControlSettings(GameControlSettings newSettings) {
		Singleton.ApplyNewControlSettings(newSettings);
	}

	/// <summary>
	/// Update and apply new screen settings.
	/// </summary>
	/// <param name="newSettings">New screen settings.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ChangeScreenSettings(GameScreenSettings newSettings) {
		Singleton.ApplyNewScreenSettings(newSettings);
	}
	
	/// <summary>
	/// Update and apply new quality settings.
	/// </summary>
	/// <param name="newSettings">New quality settings.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ChangeQualitySettings(GameQualitySettings newSettings) {
		Singleton.ApplyNewQualitySettings(newSettings);
	}
	
	/// <summary>
	/// Update and apply new audio settings.
	/// </summary>
	/// <param name="newSettings">New audio settings.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ChangeAudioSettings(GameAudioSettings newSettings) {
		Singleton.ApplyNewAudioSettings(newSettings);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryGetPlayerEntity(out Entity playerEntity) {
		return Singleton.entityManager.CreateEntityQuery(ComponentType.ReadOnly<Player>()).TryGetSingletonEntity<Player>(out playerEntity);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryGetEntityBakerSingleton(out EntityBakerSingleton entityBakerSingleton) {
		return Singleton.entityManager.CreateEntityQuery(ComponentType.ReadOnly<EntityBakerSingleton>()).TryGetSingleton(out entityBakerSingleton);
	}

	/// <summary>
	/// Changes the current menu. The event A_OnMenuChanged will be broadcasted with the new menu.
	/// </summary>
	/// <param name="newMenu">The menu to change to.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ChangeMenuTo(EMenu newMenu) {
		CurrentMenu = newMenu;
		A_OnMenuChanged?.Invoke(CurrentMenu);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void UpdatePlayerControlsSettings() {
		entityManager.SetComponentData(_playerEntity, new PlayerControlsSettings(_settings.ControlSettings.MouseSensitivity));
	}

	void ApplyNewControlSettings(GameControlSettings newSettings) {
		_settings.ControlSettings.SetFrom(newSettings);
		if (IsPlayerSpawned) {
			UpdatePlayerControlsSettings();
		}
	}

	void ApplyNewScreenSettings(GameScreenSettings newSettings) {
		_settings.ScreenSettings.SetFrom(newSettings);
		GameScreenSettings s = _settings.ScreenSettings;
		Resolution res = s.Resolution;
		Screen.SetResolution(
			res.width,
			res.height,
			s.IsFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed,
			res.refreshRateRatio
		);
		QualitySettings.vSyncCount = s.IsVsyncEnabled ? 1 : 0;
		Application.targetFrameRate = s.FpsLimit;
		{
			string strVsyncEnabled = QualitySettings.vSyncCount == 1 ? "enabled" : "disabled";
			Debug.Log(
				$"[ApplyNewScreenSettings]: Screen settings changed! Screen updated to:\n" +
				$"\t> Resolution: {Screen.currentResolution.width} x {Screen.currentResolution.height} (option {s.CurrentResolutionOptionChoice})\n" +
				$"\t> Refresh rate: {Screen.currentResolution.refreshRateRatio.value}\n" +
				$"\t> VSync: {strVsyncEnabled}\n" +
				$"\t> FPS limit: {Application.targetFrameRate}\n" +
				$"\t> FOV: TODO"
			);
		}
	}

	void ApplyNewQualitySettings(GameQualitySettings newSettings) {
		_settings.QualitySettings.SetFrom(newSettings);
	}

	void ApplyNewAudioSettings(GameAudioSettings newSettings) {
		_settings.AudioSettings.SetFrom(newSettings);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static void SetTimeScale(float timeScale) {
#if UNITY_EDITOR
		if (!Singleton) {
			Debug.LogWarning("GameManager::TimeScale: GameManager Singleton does not exist! Unable to set TimeScale");
			return;
		}
		if (timeScale <= 0) {
			Debug.LogError($"GameManager::TimeScale: Illegal attempt to set a non-positive TimeScale. Value provided: {timeScale}");
			return;
		}
#endif
		Singleton._timeScale = timeScale;
		if (!IsPaused)
			Time.timeScale = timeScale;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static void HideMouse() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static void ShowMouse() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	void InitPlayer() {
		Singleton.UpdatePlayerControlsSettings();
	}

}
