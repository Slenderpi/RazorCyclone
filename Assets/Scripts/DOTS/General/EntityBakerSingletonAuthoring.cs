using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class EntityBakerSingletonAuthoring : MonoBehaviour {

	[SerializeField]
    [Tooltip("Prefab of the player character.")]
	GameObject PlayerPrefab;

	[SerializeField]
	[Tooltip("Prefab of the fuel pickup.")]
	GameObject FuelPickupPrefab;

	[SerializeField]
	[Tooltip("Prefab of the Cannon Fodder.")]
	GameObject CannonFodderPrefab;

	[SerializeField]
	[Tooltip("Prefab of the Hunter Basic.")]
	GameObject HunterBasicPrefab;

	[SerializeField]
	[Tooltip("Prefab of the Hunter Empowered.")]
	GameObject HunterEmpoweredPrefab;

	[SerializeField]
	[Tooltip("Prefab of the Crab Basic.")]
	GameObject CrabBasicPrefab;

	[SerializeField]
	[Tooltip("Prefab of the Crab Empowered.")]
	GameObject CrabEmpoweredPrefab;

	[SerializeField]
	[Tooltip("Prefab of the Turtle.")]
	GameObject TurtlePrefab;



	class Baker : Baker<EntityBakerSingletonAuthoring> {
        public override void Bake(EntityBakerSingletonAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntityBakerSingleton() {
                Player = GetEntity(auth.PlayerPrefab, TransformUsageFlags.Dynamic),
                FuelPickup = GetEntity(auth.FuelPickupPrefab, TransformUsageFlags.Dynamic),
                CannonFodder = GetEntity(auth.CannonFodderPrefab, TransformUsageFlags.Dynamic),
				HunterBasic = GetEntity(auth.HunterBasicPrefab, TransformUsageFlags.Dynamic),
				//HunterEmpowered = GetEntity(auth.HunterEmpoweredPrefab, TransformUsageFlags.Dynamic),
				//CrabBasic = GetEntity(auth.CrabBasicPrefab, TransformUsageFlags.Dynamic),
				//CrabEmpowered = GetEntity(auth.CrabEmpoweredPrefab, TransformUsageFlags.Dynamic),
				//Turtle = GetEntity(auth.TurtlePrefab, TransformUsageFlags.Dynamic),
				//Centipede = GetEntity(auth., TransformUsageFlags.Dynamic)
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
    /// Fuel pickup prefab.
    /// </summary>
    public Entity FuelPickup;

    /// <summary>
    /// Cannon Fodder prefab.
    /// </summary>
    public Entity CannonFodder;

	/// <summary>
	/// Hunter Basic prefab.
	/// </summary>
    public Entity HunterBasic; // TODO

    public Entity HunterEmpowered; // TODO

    public Entity CrabBasic; // TODO

    public Entity CrabEmpowered; // TODO

	public Entity Turtle; // TODO

	public Entity Centipede; // TODO

	/// <summary>
	/// Given an EEnemyType, get the corresponding entity prefab.
	/// </summary>
	/// <param name="enemyType"></param>
	/// <returns></returns>
	public readonly Entity GetEnemyPrefabById(EEnemyType enemyType) {
		return enemyType switch {
			EEnemyType.CannonFodder => CannonFodder,
            EEnemyType.HunterBasic => HunterBasic,
            //EEnemyType.Hunter => HunterEmpowered, // TODO
			//EEnemyType.CrabBasic => CrabBasic, // TODO
			//EEnemyType.Crab => CrabEmpowered, // TODO
			//EEnemyType.Turtle => Turtle, // TODO
			//EEnemyType.Centipede => Centipede, // TODO
			_ => Entity.Null,
		};
	}

	/// <summary>
	/// Given an EEnemyType, try to get the corresponding entity prefab.<br/>
    /// Prints an error if the provided EEnemyType does not have a spawn-ready DOTS reimplementation yet.
	/// </summary>
	/// <param name="enemyType"></param>
	/// <returns>False if the enemy type has not been given a DOTS implementation yet.</returns>
	public readonly bool TryGetEnemyPrefabById(EEnemyType enemyType, out Entity entity) {
        switch (enemyType) {
			case EEnemyType.CannonFodder:
				entity = CannonFodder;
				return true;
			case EEnemyType.HunterBasic:
				entity = HunterBasic;
				return true;
			case EEnemyType.Hunter: // TODO
			case EEnemyType.CrabBasic: // TODO
			case EEnemyType.Crab: // TODO
			case EEnemyType.Turtle: // TODO
			case EEnemyType.Centipede: // TODO
			default:
				Debug.LogError($"TestingStuff: the enemy type \"{Util.EEnemyTypeName(enemyType)}\" does not have a spawn-ready DOTS implementation yet and cannot be spawned.");
				entity = Entity.Null;
				return false;
		}
    }
}
