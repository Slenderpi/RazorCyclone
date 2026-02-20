using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour {
    
    public static PlayerInputReader Instance;

	public static Action<float3> A_RotationInputChanged; // Current rotation input vector
														 //public static Action<>

	public PlayerInputActions AllInputActions { get; private set; }
    public PlayerInputActions.PlayerActions PlayerActions { get; private set; }
	public PlayerInputActions.PauseMenuActions PauseActions { get; private set; }

	bool noInputFound = true;
	Entity playerEntity;



	private void Awake() {
		Instance = this;
		AllInputActions = new PlayerInputActions();
		PlayerActions = AllInputActions.Player;
		PauseActions = AllInputActions.PauseMenu;
		EnablePlayerActions();
	}

	private void Start() {
		GameManager.OnGameResumed += EnablePlayerActions;
		GameManager.OnGamePaused += DisablePlayerActions;
		EnablePauseMenuActions();
	}

	private void Update() {
		EntityManager em = GetEntityManager();
		EntityQuery eq = new EntityQueryBuilder(Allocator.Temp)
			.WithAll<PlayerInput>()
			.Build(em);
		NativeArray<PlayerInput> inputArr = eq.ToComponentDataArray<PlayerInput>(Allocator.Temp);
		NativeArray<Entity> entityArr = eq.ToEntityArray(Allocator.Temp);
		if (inputArr.Length == 0 || entityArr.Length == 0) {
			if (!noInputFound)
				noInputFound = true;
			return;
		}
		Entity entity = entityArr[0]; // There should only be one
		playerEntity = entity;
		noInputFound = false;
		ReadLookInput(inputArr[0], em);
	}

	public void EnablePlayerActions() {
		//PlayerActions.TurnInputs.performed += TurnInputChanged;
		//PlayerActions.TurnInputs.canceled += TurnInputChanged;
		//PlayerActions.VertInputs.started += VertInputChanged;
		//PlayerActions.VertInputs.canceled += VertInputChanged;
		PlayerActions.RotationInput.performed += RotationInputChanged;
		PlayerActions.RotationInput.canceled += RotationInputChanged;

		PlayerActions.Vacuum.started += FireVacuumStarted;
		PlayerActions.Vacuum.canceled += FireVacuumCanceled;
		PlayerActions.Cannon.started += FireCannonStarted;
		PlayerActions.Cannon.canceled += FireCannonCanceled;

		/** Features not necessarily meant for final gameplay **/
		PlayerActions.SlowTime.started += On_TimeSlowStarted;
		PlayerActions.SlowTime.canceled += On_TimeSlowCanceled;
		PlayerActions._AddFuel.started += On_AddFuelKey;
		PlayerActions._AddRicCharges.started += On_AddRicCharges;
		PlayerActions._HealHealth.started += On_HealHealth;
		PlayerActions._TakeDamage.started += On_TakeDamage;
		//PlayerActions._ToggleTP.started += On_ToggleThirdPerson;
		//PlayerActions._ToggleMirror.started += On_ToggleMirrorInput;

		PlayerActions.Enable();
	}

	public void DisablePlayerActions() {
		//PlayerActions.TurnInputs.performed -= TurnInputChanged;
		//PlayerActions.TurnInputs.canceled -= TurnInputChanged;
		//PlayerActions.VertInputs.started -= VertInputChanged;
		//PlayerActions.VertInputs.canceled -= VertInputChanged;
		PlayerActions.RotationInput.performed -= RotationInputChanged;
		PlayerActions.RotationInput.canceled -= RotationInputChanged;

		PlayerActions.Vacuum.started -= FireVacuumStarted;
		PlayerActions.Vacuum.canceled -= FireVacuumCanceled;
		PlayerActions.Cannon.started -= FireCannonStarted;
		PlayerActions.Cannon.canceled -= FireCannonCanceled;

		/** Features not necessarily meant for final gameplay **/
		PlayerActions.SlowTime.started -= On_TimeSlowStarted;
		PlayerActions.SlowTime.canceled -= On_TimeSlowCanceled;
		PlayerActions._AddFuel.started -= On_AddFuelKey;
		PlayerActions._AddRicCharges.started -= On_AddRicCharges;
		PlayerActions._HealHealth.started -= On_HealHealth;
		PlayerActions._TakeDamage.started -= On_TakeDamage;
		//PlayerActions._ToggleTP.started -= On_ToggleThirdPerson;
		//PlayerActions._ToggleMirror.started -= On_ToggleMirrorInput;

		PlayerActions.Disable();
	}

	public void EnablePauseMenuActions() {
		PauseActions.Escape.started += OnEscapePressed;
		PauseActions.Enable();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadLookInput(PlayerInput input, EntityManager em) {
		input.LookInputDelta = PlayerActions.Look.ReadValue<Vector2>() * GetPlayerComponent<PlayerControlsSettings>(em).MouseSensitivity * 0.5f;
		WritePlayerInput(input, em);
	}

	private void RotationInputChanged(InputAction.CallbackContext context) {
		if (noInputFound) return;
		EntityManager em = GetEntityManager();
		PlayerInput input = GetCurrentPlayerInput(em);
		float3 v = context.ReadValue<Vector3>();
		if (Util.IsNearZero(v))
			return;
		input.RotationInput = v;
		WritePlayerInput(input, em);
	}

	//private void TurnInputChanged(InputAction.CallbackContext context) {
	//	if (noInputFound) return;
	//	Vector2 v = context.ReadValue<Vector2>();
	//	EntityManager em = GetEntityManager();
	//	PlayerInput input = GetCurrentPlayerInput(em);
	//	input.RotationInput.x = v.x;
	//	input.RotationInput.z = v.y;
	//	if (Util.IsNearZero(input.RotationInput))
	//		return;
	//	WritePlayerInput(input, em);
	//}

	//private void VertInputChanged(InputAction.CallbackContext context) {
	//	if (noInputFound) return;
	//	EntityManager em = GetEntityManager();
	//	PlayerInput input = GetCurrentPlayerInput(em);
	//	input.RotationInput.y = context.ReadValue<float>();
	//	if (Util.IsNearZero(input.RotationInput))
	//		return;
	//	WritePlayerInput(input, em);
	//}

	private void FireCannonStarted(InputAction.CallbackContext context) {
		if (noInputFound) return;
		EntityManager em = GetEntityManager();
		PlayerInput input = GetCurrentPlayerInput(em);
		input.FireCannon = true;
		WritePlayerInput(input, em);
	}

	private void FireCannonCanceled(InputAction.CallbackContext context) {
		if (noInputFound) return;
	}

	private void FireVacuumStarted(InputAction.CallbackContext context) {
		if (noInputFound) return;
		EntityManager em = GetEntityManager();
		PlayerInput input = GetCurrentPlayerInput(em);
		input.EnableVacuum = true;
		WritePlayerInput(input, em);
	}

	private void FireVacuumCanceled(InputAction.CallbackContext context) {
		if (noInputFound) return;
		EntityManager em = GetEntityManager();
		PlayerInput input = GetCurrentPlayerInput(em);
		input.EnableVacuum = false;
		WritePlayerInput(input, em);
	}

	private void On_TimeSlowStarted(InputAction.CallbackContext context) {
		if (noInputFound) return;
		EntityManager em = GetEntityManager();
		PlayerExtraInput extraInput = GetCurrentPlayerExtraInput(em);
		extraInput.SlowTime = true;
		WritePlayerExtraInput(extraInput, em);

		GameManager.TimeScale = 0.1f;
	}

	private void On_TimeSlowCanceled(InputAction.CallbackContext context) {
		if (noInputFound) return;
		EntityManager em = GetEntityManager();
		PlayerExtraInput extraInput = GetCurrentPlayerExtraInput(em);
		extraInput.SlowTime = false;
		WritePlayerExtraInput(extraInput, em);

		GameManager.TimeScale = 1f;
	}

	private void On_AddFuelKey(InputAction.CallbackContext context) {
		if (noInputFound) return;
		EntityManager em = GetEntityManager();
		PlayerExtraInput extraInput = GetCurrentPlayerExtraInput(em);
		extraInput.RefillFuel = true;
		WritePlayerExtraInput(extraInput, em);
	}

	private void On_AddRicCharges(InputAction.CallbackContext context) {
		if (noInputFound) return;
		EntityManager em = GetEntityManager();
		PlayerExtraInput extraInput = GetCurrentPlayerExtraInput(em);
		extraInput.AddRicochets = true;
		WritePlayerExtraInput(extraInput, em);
	}

	private void On_HealHealth(InputAction.CallbackContext context) {
		if (noInputFound) return;
		EntityManager em = GetEntityManager();
		PlayerExtraInput extraInput = GetCurrentPlayerExtraInput(em);
		extraInput.HealHealth = true;
		WritePlayerExtraInput(extraInput, em);
	}

	private void On_TakeDamage(InputAction.CallbackContext context) {
		if (noInputFound) return;
		EntityManager em = GetEntityManager();
		PlayerExtraInput extraInput = GetCurrentPlayerExtraInput(em);
		extraInput.TakeDamage = true;
		WritePlayerExtraInput(extraInput, em);
	}

	private void OnEscapePressed(InputAction.CallbackContext context) {
		//if (noInputFound) return;
		GameManager.OnPauseKeyPressed();
	}



	/************************* HELPER FUNCTIONS *************************/



	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private EntityManager GetEntityManager() {
		return World.DefaultGameObjectInjectionWorld.EntityManager;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//private PlayerInput GetCurrentPlayerInput() {
	//	return GetCurrentPlayerInput(GetEntityManager());
	//}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private PlayerInput GetCurrentPlayerInput(EntityManager em) {
		return em.GetComponentData<PlayerInput>(playerEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private PlayerExtraInput GetCurrentPlayerExtraInput(EntityManager em) {
		return em.GetComponentData<PlayerExtraInput>(playerEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void WritePlayerInput(PlayerInput input, EntityManager em) {
		em.SetComponentData(playerEntity, input);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void WritePlayerExtraInput(PlayerExtraInput extraInput, EntityManager em) {
		em.SetComponentData(playerEntity, extraInput);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private T GetPlayerComponent<T>(EntityManager em) where T : unmanaged, IComponentData {
		return new EntityQueryBuilder(Allocator.Temp)
			.WithAll<T>()
			.Build(em)
			.ToComponentDataArray<T>(Allocator.Temp)[0];
	}

	//private T GetPlayerComponent<T>() where T : unmanaged, IComponentData {
	//	return GetPlayerComponent<T>(GetEntityManager());
	//}

	//private (T, Entity) GetPlayerComponentWithEntity<T>(EntityManager em) where T : unmanaged, IComponentData {
	//	EntityQuery eq = new EntityQueryBuilder(Allocator.Temp)
	//		.WithAll<T>()
	//		.Build(em);
	//	return (
	//		eq.ToComponentDataArray<T>(Allocator.Temp)[0],
	//		eq.ToEntityArray(Allocator.Temp)[0]
	//	);
	//}

	//private (T, Entity) GetPlayerComponentWithEntity<T>() where T : unmanaged, IComponentData {
	//	return GetPlayerComponentWithEntity<T>(GetEntityManager());
	//}

	//private (LocalTransform, Entity) GetCamtransWithEntity(EntityManager em) {
	//	EntityQuery eq = new EntityQueryBuilder(Allocator.Temp)
	//		.WithAll<PlayerCameraTransform, LocalTransform>()
	//		.Build(em);
	//	return (
	//		eq.ToComponentDataArray<LocalTransform>(Allocator.Temp)[0],
	//		eq.ToEntityArray(Allocator.Temp)[0]
	//	);
	//}

	//private (LocalTransform, Entity) GetCamtransWithEntity() {
	//	return GetCamtransWithEntity(GetEntityManager());
	//}

}
