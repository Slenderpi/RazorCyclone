using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class PlayerCameraManager : MonoBehaviour {

	// NOTE: Updating these values will require the FOV slider in the settings menu to have its min/max adjusted
	public static readonly int MIN_FOV = 60;
	public static readonly int MAX_FOV = 110;

	Camera c;
	SecondOrderDynamicsF sodFOV;

	[Header("References")]
	[SerializeField]
	Camera WeaponCamera;

	[Header("Parameters")]
	[SerializeField]
	[Range(0.5f, 7)]
	float f = 3;
	[SerializeField]
	[Range(0, 1)]
	float z = 1;
	[SerializeField]
	[Range(-1, 1)]
	float r = 0.2f;
	[SerializeField]
	[Range(0, 100)]
	float MaxAddFOV = 20;
	[SerializeField]
	[Range(10, 90)]
	float SpeedForMaxFOV = 40;

	float currFOV = 90;



	void Awake() {
		c = GetComponent<Camera>();
		sodFOV = new SecondOrderDynamicsF(f, z, r, 0);
		//if (GameManager.Instance)
		//	c.fieldOfView = GameManager.Instance.CurrentFOV;
	}

	void LateUpdate() {
		EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
		EntityQuery eq = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerCameraTransform, LocalToWorld>().Build(em);
		NativeArray<Entity> camtransEntities = eq.ToEntityArray(Allocator.Temp);
		NativeArray<LocalToWorld> transforms = eq.ToComponentDataArray<LocalToWorld>(Allocator.Temp);
		if (transforms.Length == 0)
			return;
		PhysicsVelocity playerVelocity = new EntityQueryBuilder(Allocator.Temp)
			.WithAll<Player, PhysicsVelocity>()
			.Build(em)
			.ToComponentDataArray<PhysicsVelocity>(Allocator.Temp)[0];
		LocalToWorld camtrans = transforms[0];
		c.transform.SetPositionAndRotation(camtrans.Position, camtrans.Rotation);
		if (Time.deltaTime > 0) {
#if UNITY_EDITOR
			sodFOV.SetDynamics(f, z, r);
#endif
			updateFOV(
				sodFOV.Update(
					Mathf.Lerp(
						0,
						MaxAddFOV,
						Unity.Mathematics.math.length(playerVelocity.Linear) / SpeedForMaxFOV),
					Time.deltaTime
				)
			);
		}
	}

	public void SetFOV(float fov) {
		currFOV = fov;
		updateFOV(sodFOV.GetNoUpdate());
	}

	void updateFOV(float addFOV) {
		c.fieldOfView = currFOV + addFOV;
		//WeaponCamera.fieldOfView = c.fieldOfView;
	}

	void OnDestroy() {
		//GameManager.Instance.GCam = null;
	}

}
