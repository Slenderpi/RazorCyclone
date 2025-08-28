using Unity.Entities;
using UnityEngine;

public class CannonFodderAuthoring : MonoBehaviour {

	public SO_CannonFodder CannonFodderData;



	class Baker : Baker<CannonFodderAuthoring> {
        public override void Bake(CannonFodderAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new CannonFodder());
			AddComponent(entity, Util.BuildGeneralBoidComponentDataFromSO(authoring.CannonFodderData));
			AddComponent(entity, new CannonFodderBoid() {
				FleeTriggerDistance = authoring.CannonFodderData.FleeTriggerDistance
			});
		}
    }
    
}
