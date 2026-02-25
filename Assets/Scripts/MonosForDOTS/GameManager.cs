using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static Action OnGameResumed;
	public static Action OnGamePaused;

    public static GameManager Singleton;

	bool _paused = false;
	public static bool IsPaused { get { return Singleton._paused; } private set { Singleton._paused = value; } }

	float _timeScale = 1f;
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
			return false;
		}
		if (!TryGetEntityBakerSingleton(out EntityBakerSingleton entityBakerSingleton)) {
			Debug.LogWarning("The EntityBakerSingleton could not be found.");
			return false;
		}
		Singleton.entityManager.Instantiate(entityBakerSingleton.Player);
		if (!IsPaused) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		return true;
	}

	private static bool TryGetPlayerEntity(out Entity playerEntity) {
		return Singleton.entityManager.CreateEntityQuery(ComponentType.ReadOnly<Player>()).TryGetSingletonEntity<Player>(out playerEntity);
	}

	public static bool TryGetEntityBakerSingleton(out EntityBakerSingleton entityBakerSingleton) {
		return Singleton.entityManager.CreateEntityQuery(ComponentType.ReadOnly<EntityBakerSingleton>()).TryGetSingleton(out entityBakerSingleton);
	}

	static void SetTimeScale(float timeScale) {
		Singleton._timeScale = timeScale;
		Time.timeScale = timeScale;
	}

	void ResumeGame() {
		_paused = false;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = _timeScale;
		OnGameResumed?.Invoke();
	}

	void PauseGame() {
		_paused = true;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Time.timeScale = 0f;
		OnGamePaused?.Invoke();
	}

}
