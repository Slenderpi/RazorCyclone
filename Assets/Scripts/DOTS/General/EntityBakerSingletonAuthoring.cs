using Unity.Entities;
using UnityEngine;

public class EntityBakerSingletonAuthoring : MonoBehaviour {

	[SerializeField]
    [Tooltip("Prefab of the player character.")]
	GameObject PlayerPrefab;

	[SerializeField]
    [Tooltip("Prefab of the Cannon Fodder.")]
	GameObject CannonFodderPrefab;



	class Baker : Baker<EntityBakerSingletonAuthoring> {
        public override void Bake(EntityBakerSingletonAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntityBakerSingleton() {
                Player = GetEntity(auth.PlayerPrefab, TransformUsageFlags.Dynamic),
                CannonFodder = GetEntity(auth.CannonFodderPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
    
}

public struct EntityBakerSingleton : IComponentData {
    /// <summary>
    /// Player character prefab.
    /// </summary>
    public Entity Player;

    /// <summary>
    /// Cannon Fodder prefab.
    /// </summary>
    public Entity CannonFodder;

    /// <summary>
    /// Given an EEnemyType, get the corresponding entity prefab.
    /// </summary>
    /// <param name="enemyType"></param>
    /// <returns></returns>
    public Entity GetEntityPrefabById(EEnemyType enemyType) {
		return enemyType switch {
			EEnemyType.CannonFodder => CannonFodder,
			_ => Entity.Null,
		};
	}

	/// <summary>
	/// Given an EEnemyType, try to get the corresponding entity prefab.<br/>
    /// Prints an error if the provided EEnemyType does not have a spawn-ready DOTS reimplementation yet.
	/// </summary>
	/// <param name="enemyType"></param>
	/// <returns>False if the enemy type has not been given a DOTS implementation yet.</returns>
	public readonly bool TryGetEntityPrefabById(EEnemyType enemyType, out Entity entity) {
        switch (enemyType) {
            case EEnemyType.CannonFodder:
                entity = CannonFodder;
                return true;
		}
		Debug.LogError($"TestingStuff: the enemy type \"{enemyType}\" does not have a spawn-ready DOTS implementation yet and cannot be spawned.");
        entity = Entity.Null;
        return false;
    }
}
