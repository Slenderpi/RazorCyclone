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

	EMenu _currentMenu;
	public static EMenu CurrentMenu { get { return Singleton._currentMenu; } private set { Singleton._currentMenu = value; } }

	EntityManager entityManager;



	private void Awake() {
		Singleton = this;
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
			ResumeGame();
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
		if (TryGetPlayerEntity(out Entity _)) {
			Debug.LogWarning("An existing player was found. TrySpawnPlayer() cancelled.");
			HideMouse();
			return false;
		}
		if (!TryGetEntityBakerSingleton(out EntityBakerSingleton entityBakerSingleton)) {
			Debug.LogWarning("The EntityBakerSingleton could not be found.");
			return false;
		}
		Singleton.entityManager.Instantiate(entityBakerSingleton.Player);
		if (!IsPaused) {
			HideMouse();
		}
		return true;
	}

	private static bool TryGetPlayerEntity(out Entity playerEntity) {
		return Singleton.entityManager.CreateEntityQuery(ComponentType.ReadOnly<Player>()).TryGetSingletonEntity<Player>(out playerEntity);
	}

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

}
