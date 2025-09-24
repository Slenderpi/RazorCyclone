using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Handles spawning of PlayerCannonProjectileTrails for PlayerCannonProjectiles.
/// </summary>
public class PlayerCannonProjectileTrailManager : MonoBehaviour {
	
	public static PlayerCannonProjectileTrailManager Instance;

	public PlayerCannonProjectileTrail PlayerCannonProjectileTrailPrefab;



	private void Awake() {
		Instance = this;
	}

	public void SpawnTrail(Entity entity, float3 position, quaternion rotation) {
		PlayerCannonProjectileTrail trail = Instantiate(PlayerCannonProjectileTrailPrefab);
		trail.AttachedEntity = entity;
		trail.transform.SetPositionAndRotation(position, rotation);
	}

}

/// <summary>
/// Detects PlayerCannonProjectiles that need a trail.
/// </summary>
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PlayerCannonProjectileSystem))]
public partial class PlayerCannonProjectileTrailAttachmentSystem : SystemBase {

	protected override void OnUpdate() {
		PlayerCannonProjectileTrailManager pcptManager = PlayerCannonProjectileTrailManager.Instance;
		if (!pcptManager)
			return;
		foreach (var (
			trans,
			entity
			) in SystemAPI
			.Query<LocalToWorld>()
			.WithAll<PlayerCannonProjectileTrailAttachment>()
			.WithEntityAccess()) {
			pcptManager.SpawnTrail(entity, trans.Position, trans.Rotation);
			SystemAPI.SetComponentEnabled<PlayerCannonProjectileTrailAttachment>(entity, false);
		}
	}

}