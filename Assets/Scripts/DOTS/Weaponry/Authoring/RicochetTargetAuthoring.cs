using Unity.Entities;
using UnityEngine;

public class RicochetTargetAuthoring : MonoBehaviour {

	public static readonly uint MAXIMUM_PRIORITY = (1u << 30) - 1;

	[Tooltip("If false, this RicochetTarget will start in a disabled state.")]
	public bool StartEnabled = true;
	[Tooltip("The priority value for this target. Ricochets will prioritize higher priority-valued targets and ignore lower priority targets.\n\nNOTE: The maximum value (inclusive) is 2^30 - 1, or 1,073,741,823.")]
	public uint RicochetPriority = 1000u;



	class Baker : Baker<RicochetTargetAuthoring> {
		public override void Bake(RicochetTargetAuthoring auth) {
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			RicochetTarget ricTarg = new();
			if (!auth.StartEnabled) {
				ricTarg.Enabled = false;
			}
			if (auth.TryGetComponent(out Rigidbody _)) {
				ricTarg.HasPhysicsVelocity = true;
			}
			if (auth.RicochetPriority > MAXIMUM_PRIORITY) {
				Debug.LogError($"{auth.gameObject.name}.RicochetTargetAuthoring: The RicochetPriority value {auth.RicochetPriority} is beyond the maximum allowed priority. The maximum (inclusive) value you can have is {MAXIMUM_PRIORITY}. Defaulting to 1000.");
				ricTarg.Priority = 1000u;
			} else
				ricTarg.Priority = auth.RicochetPriority;
			AddComponent(entity, ricTarg);
			if (!ricTarg.Enabled)
				SetComponentEnabled<RicochetTarget>(entity, false);
		}
	}

}
