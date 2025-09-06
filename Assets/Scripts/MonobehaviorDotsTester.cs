using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MonobehaviorDotsTester : MonoBehaviour {

    public static MonobehaviorDotsTester Instance;

	public bool MAKE_WAVEFRONT_GOAL_FOLLOW_PLAYER = false;



	private void Awake() {
		Instance = this;
	}

	private void Update() {
		if (MAKE_WAVEFRONT_GOAL_FOLLOW_PLAYER)
			PositionWavefrontGoalToPlayer();
	}

	private void PositionWavefrontGoalToPlayer() {
		if (GameManager.Instance == null || GameManager.CurrentPlayer == null)
			return;
		EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
		EntityQuery eq = new EntityQueryBuilder(Allocator.Temp).WithAll<WavefrontGoalTarget, LocalTransform>().Build(em);
		NativeArray<WavefrontGoalTarget> wavefrontGoals = eq.ToComponentDataArray<WavefrontGoalTarget>(Allocator.Temp);
		if (wavefrontGoals.Length == 0) {
			return;
		}
		NativeArray<LocalTransform> goalTransforms = eq.ToComponentDataArray<LocalTransform>(Allocator.Temp);
		LocalTransform tempTransform = goalTransforms[0];
		tempTransform.Position = GameManager.CurrentPlayer.transform.position;
		goalTransforms[0] = tempTransform;
		eq.CopyFromComponentDataArray(goalTransforms);
	}

}
