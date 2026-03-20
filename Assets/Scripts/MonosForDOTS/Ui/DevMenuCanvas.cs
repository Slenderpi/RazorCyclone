using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public class DevMenuCanvas : MonoBehaviour {

	public GameObject DevMenuContainer;

	EEnemyType spawnEnemy_Type = EEnemyType.CannonFodder;
	int spawnEnemy_Count = 1;

	Camera mainCamera;

	EntityQuery eqEntityBakerSingleton;



	private void Awake() {
		mainCamera = Camera.main;
	}

	private void Start() {
		EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		eqEntityBakerSingleton = entityManager.CreateEntityQuery(ComponentType.ReadOnly<EntityBakerSingleton>());

		DevMenuContainer.SetActive(false);
	}

	public void OnToggleDebugMenu() {
		DevMenuContainer.SetActive(!DevMenuContainer.activeSelf);
	}

	public void OnSpawnEnemyTypeChoiceValueChanged(int option) {
		spawnEnemy_Type = (EEnemyType)option;
	}

	public void OnSpawnEnemyCountInputValueChanged(string val) {
		if (int.TryParse(val, out spawnEnemy_Count)) {
			if (spawnEnemy_Count <= 0) {
				Debug.LogError("DevMenuCanvas: You cannot spawn a non-positive number of enemies.");
			}
		}
	}

	public void OnSpawnEnemySpawnButtonClicked() {
		if (spawnEnemy_Count < 0) {
			Debug.LogError("DevMenuCanvas: Illegal attempt to spawn a negative number of enemies.");
		} else if (spawnEnemy_Count == 0) {
			Debug.LogWarning("DevMenuCanvas: Attempt to spawn 0 enemies. Make sure you have a valid number of enemies to spawn.");
			return;
		}
		Debug.Log($"DevMenuCanvas: Spawning {spawnEnemy_Count} instances of {spawnEnemy_Type.DisplayName()}");
		Vector3 spawnLocation = mainCamera.transform.position + mainCamera.transform.forward * 3f;
		EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
		NativeArray<Entity> entities = em.Instantiate(
			eqEntityBakerSingleton.GetSingleton<EntityBakerSingleton>().GetEnemyPrefabById(spawnEnemy_Type),
			spawnEnemy_Count,
			Allocator.Temp
		);
		for (int i = 0; i < spawnEnemy_Count; i++)
			em.SetComponentData<LocalTransform>(entities[i], new() {
				Position = new float3(spawnLocation.x, spawnLocation.y, spawnLocation.z),
				Rotation = quaternion.LookRotationSafe(mainCamera.transform.forward, mainCamera.transform.up),
				Scale = 1
			});
	}
	
}
