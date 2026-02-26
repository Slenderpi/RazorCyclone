using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Entities;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static Action OnGameResumed;
	public static Action OnGamePaused;

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
			Singleton.ResumeGame();
		} else {
			Singleton.PauseGame();
		}
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

	void ResumeGame() {
		_paused = false;
		HideMouse();
		Time.timeScale = _timeScale;
		OnGameResumed?.Invoke();
	}

	void PauseGame() {
		_paused = true;
		ShowMouse();
		Time.timeScale = 0f;
		OnGamePaused?.Invoke();
	}

}
