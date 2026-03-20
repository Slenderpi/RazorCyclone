using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class HunterAuthoring : MonoBehaviour {

	[SerializeField]
	[Tooltip("Determines if this Hunter is a Basic one or an Empowered one.\nDo not set multiple forms.")]
	EEnemyForm Form = EEnemyForm.Basic;

	[Tooltip("Radius of the Hunter's HurtboxCollider.")]
	public float HurtboxColliderRadius = 1f;

	[Tooltip("Whether or not the Hunter's HurtboxCollider should start enabled.")]
	public bool HurtboxStartsEnabled = true;

	public GameObject LightsMesh;

	// TEMP



	class Baker : Baker<HunterAuthoring> {
		public override void Bake(HunterAuthoring auth) {
			if (auth.Form != EEnemyForm.Basic && auth.Form != EEnemyForm.Empowered)
				Debug.LogError($"HunterAuthoring: an invalid Enemy Form type was provided for this Hunter.");
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			if (auth.Form == EEnemyForm.Basic)
				AddComponent(entity, new HunterBasic());
			else
				AddComponent(entity, new HunterEmpowered() {
					IsStunned = false,
					LastStunTime = -1000f,
					LightsEntity = GetEntity(auth.LightsMesh, TransformUsageFlags.Dynamic)
				});
			AddComponent(entity, new EnemyForm() {
				Form = auth.Form
			});
			AddComponent(entity, BoidUtil.BoidBuilder.HunterBoid());
			HurtboxCollider hc = new();
			hc.SetRadius(auth.HurtboxColliderRadius);
			AddComponent(entity, hc);
			if (!auth.HurtboxStartsEnabled)
				SetComponentEnabled<HurtboxCollider>(entity, false);

		}
	}
	
}

//[BurstCompile]
public struct HunterBasic : IComponentData {
	//public EEnemyForm Form;

	///// <summary>
	///// Represents the following:<br/>
	/////  - The form (Basic or Empowered).<br/>
	/////  - Whether or not this Hunter (Empowered) is stunned.<br/>
	///// Form is on the first two bits. Stun state is on the third.
	///// </summary>
	//uint _data;

	///// <summary>
	///// Set the Form of this Hunter.<br/>
	///// This function should be called when the Hunter is baked. It assumes the Hunter isn't stunned.
	///// </summary>
	///// <param name="form"></param>
	//[BurstCompile]
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public void SetForm(EEnemyForm form) {
	//    _data = 0u & (uint)form;
	//}

	//[BurstCompile]
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public void SetStunnedState(bool newStunned) {

	//}
}

[BurstCompile]
public struct HunterEmpowered : IComponentData {
	public bool IsStunned;
	public float LastStunTime;
	public Entity LightsEntity;
}

public struct EnemyForm : IComponentData {
	public EEnemyForm Form;
}