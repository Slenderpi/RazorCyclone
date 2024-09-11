using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCharacterCtrlr : MonoBehaviour {
    
    PlayerInputActions.PlayerActions pInputActions;
    
    Vector3 desiredRotation = Vector3.zero;
    
    Camera mainCamera;
    [Header("References")]
    [SerializeField]
    Transform camtrans;
    [SerializeField]
    Transform charModel;
    [SerializeField]
    Transform charPivot;
    Rigidbody rb;
    [SerializeField]
    GameObject vacuumHitbox;
    [SerializeField]
    Transform canonTip;
    [SerializeField]
    ProjectileBase projectilePrefab;
    [SerializeField]
    Slider fuelSlider;
    
    bool isVacuumOn;
    [Header("Weapon Settings")]
    [SerializeField]
    float VacuumForce = 20f;
    public float VacuumSuckForce = 8500f;
    public float VacuumSuckRate { private set; get; } = 0.1f; // per second
    public float VacuumDamage {private set; get; } = 7f;
    [SerializeField]
    float CanonForce = 8f;
    [SerializeField]
    float CanonProjSpeed = 100f;
    [SerializeField]
    float CanonDamage = 30;
    
    [Header("Fuel Settings")]
    [SerializeField]
    float MaxFuel = 100f;
    [SerializeField]
    float CanonFuelCost = 6f;
    [SerializeField]
    [Tooltip("The amount of seconds to spend 100 fuel")]
    float VacuumFuelTime = 8f; // The amount of seconds to spend 100 fuel
    float vacuumFuelCost; // Calculated based on VacuumFuelTime
    float currentFuel;
    
    float MaxHealth = 100f;
    float currentHealth;
    
    // [SerializeField]
    [Header("Mouse sens")]
    [SerializeField]
    float mouseSensitivity = 0.11f;
    float lookVertRot = 0;
    
    void Awake() {
        pInputActions = new PlayerInputActions().Player;
        // lookInput = pInputActions.Look;
        // rotationInputs = pInputActions.RotationInputs;
        // vacuumInput = pInputActions.Vacuum;
        // canonInput = pInputActions.Canon;
        // timeSlowInput = pInputActions.SlowTime;
        
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        
        vacuumHitbox.SetActive(false);
        
        AddFuel(MaxFuel);
        vacuumFuelCost = MaxFuel / VacuumFuelTime * Time.fixedDeltaTime;
        currentHealth = MaxHealth;
    }
    
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        
        updateCameraTransform();
    }
    
    void Update() {
        // Rotate camera and character based on mouse input
        Vector2 lookDelta = pInputActions.Look.ReadValue<Vector2>() * mouseSensitivity;
        lookVertRot = Mathf.Clamp(lookVertRot - lookDelta.y, -90f, 90f);
        camtrans.localEulerAngles = new Vector3(lookVertRot, camtrans.localEulerAngles.y + lookDelta.x, 0);
        charModel.localEulerAngles = new Vector3(0, camtrans.localEulerAngles.y, 0);
        // if (desiredRotation.y == 0) {
        //     charModel.localEulerAngles = new Vector3(lookVertRot, camtrans.localEulerAngles.y + lookDelta.x, 0);
        // } else {
        //     charModel.localEulerAngles = new Vector3(0, camtrans.localEulerAngles.y + lookDelta.x, 0);
        // }
        
        // Rotate character pivot based on rotation inputs
        charPivot.localEulerAngles = Quaternion.LookRotation(desiredRotation.magnitude != 0 ? desiredRotation : Vector3.forward).eulerAngles;
    }
    
    void FixedUpdate() {
        // print(desiredRotation);
        if (isVacuumOn) {
            if (currentFuel <= 0) {
                isVacuumOn = false;
                vacuumHitbox.SetActive(false);
                // print("Not enough fuel (" + currentFuel + ") for vacuum (need " + vacuumFuelCost + ").");
            } else {
                AddFuel(-vacuumFuelCost);
                rb.AddForce(charPivot.forward * VacuumForce, ForceMode.Acceleration);
            }
        }
    }
    
    void LateUpdate() {
        updateCameraTransform();
    }
    
    public void AddFuel(float amount) {
        currentFuel = Mathf.Clamp(currentFuel + amount, 0, MaxFuel);
        fuelSlider.value = currentFuel / MaxFuel;
    }

    // private void RotateInputPerformed(InputAction.CallbackContext context) {
    //     desiredRotation = context.ReadValue<Vector3>();
    // }

    private void TurnInputChanged(InputAction.CallbackContext context) {
        Vector2 v = context.ReadValue<Vector2>();
        desiredRotation.x = v.x;
        desiredRotation.z = v.y;
        
        // if (desiredRotation.magnitude > 0)
        //     StartVacuum();
        // else
        //     StopVacuum();
    }

    private void VertInputChanged(InputAction.CallbackContext context) {
        desiredRotation.y = context.ReadValue<float>();
        
        // if (desiredRotation.magnitude > 0)
        //     StartVacuum();
        // else
        //     StopVacuum();
    }

    private void FireVacuumStarted(InputAction.CallbackContext context) {
        if (currentFuel <= 0) {
            // print("Not enough fuel (" + currentFuel + ") for vacuum (need " + vacuumFuelCost + ").");
            return;
        }
        isVacuumOn = true;
        vacuumHitbox.SetActive(true);
    }
    
    // void StartVacuum() {
    //     if (currentFuel <= 0) {
    //         // print("Not enough fuel (" + currentFuel + ") for vacuum (need " + vacuumFuelCost + ").");
    //         return;
    //     }
    //     isVacuumOn = true;
    //     vacuumHitbox.SetActive(true);
    // }
    
    // void StopVacuum() {
    //     isVacuumOn = false;
    //     vacuumHitbox.SetActive(false);
    // }

    private void FireVacuumCanceled(InputAction.CallbackContext context) {
        isVacuumOn = false;
        vacuumHitbox.SetActive(false);
    }

    private void FireCanonStarted(InputAction.CallbackContext context) {
        if (currentFuel <= 0) {
            // print("Not enough fuel (" + currentFuel + ") for canon (need " + CanonFuelCost + ").");
            return;
        }
        // Time.timeScale = 0.15f;
        fireCanon();
    }

    private void FireCanonCanceled(InputAction.CallbackContext context) {
        // Time.timeScale = 1f;
        if (currentFuel <= 0) {
            // print("Not enough fuel (" + currentFuel + ") for canon (need " + CanonFuelCost + ").");
            return;
        }
        // fireCanon();
    }
    
    void fireCanon() {
        rb.AddForce(charPivot.forward * CanonForce * 100000);
        // TODO maybe?: Switch fire direction of projectile to be based off a raycast from camera to ~10000 units forward
        //              Might be weird to figure out. Makes sense when shooting straight, but what about left/right/etc of camera?
        // Ray ray = new Ray(canonTip.position);
        ProjectileBase proj = Instantiate(projectilePrefab, canonTip.position, canonTip.rotation);
        proj.damage = CanonDamage;
        proj.GetComponent<Rigidbody>().AddForce(canonTip.forward * CanonProjSpeed + rb.velocity, ForceMode.VelocityChange);
        AddFuel(-CanonFuelCost);
    }
    
    private void OnTimeSlowStarted(InputAction.CallbackContext context) {
        // frontIsVacuum = !frontIsVacuum;
        Time.timeScale = 0.1f;
    }
    
    private void OnTimeSlowCanceled(InputAction.CallbackContext context) {
        Time.timeScale = 1f;
    }
    
    private void OnAddFuelKey(InputAction.CallbackContext context) {
        print("Fully refueling fuel.");
        AddFuel(MaxFuel);
    }

    void updateCameraTransform() {
        mainCamera.transform.position = camtrans.position;
        mainCamera.transform.rotation = camtrans.rotation;
    }
    
    void OnEnable() {
        pInputActions.Look.Enable();
        pInputActions.TurnInputs.Enable();
        pInputActions.VertInputs.Enable();
        // rotationInputs.Enable(); //
        pInputActions.Vacuum.Enable();
        pInputActions.Canon.Enable();
        pInputActions.SlowTime.Enable();
        
        pInputActions.TurnInputs.performed += TurnInputChanged;
        pInputActions.TurnInputs.canceled += TurnInputChanged;
        pInputActions.VertInputs.performed += VertInputChanged;
        pInputActions.VertInputs.canceled += VertInputChanged;
        
        // rotationInputs.performed += RotateInputPerformed;
        pInputActions.Vacuum.started += FireVacuumStarted;
        pInputActions.Vacuum.canceled += FireVacuumCanceled;
        pInputActions.Canon.started += FireCanonStarted;
        pInputActions.Canon.canceled += FireCanonCanceled;
        pInputActions.SlowTime.started += OnTimeSlowStarted;
        pInputActions.SlowTime.canceled += OnTimeSlowCanceled;
        
        // Y lock toggle feature is for testing and likely not for final gameplay
        pInputActions._AddFuel.Enable();
        pInputActions._AddFuel.started += OnAddFuelKey;
    }

    void OnDisable() {
        pInputActions.Look.Disable();
        pInputActions.TurnInputs.Disable();
        pInputActions.VertInputs.Disable();
        // rotationInputs.Disable();
        pInputActions.Vacuum.Disable();
        pInputActions.Canon.Disable();
        pInputActions.SlowTime.Disable();
        
        pInputActions.TurnInputs.performed -= TurnInputChanged;
        pInputActions.TurnInputs.canceled -= TurnInputChanged;
        pInputActions.VertInputs.performed -= VertInputChanged;
        pInputActions.VertInputs.canceled -= VertInputChanged;
        
        // rotationInputs.performed -= RotateInputPerformed;
        pInputActions.Vacuum.started -= FireVacuumStarted;
        pInputActions.Vacuum.canceled -= FireVacuumCanceled;
        pInputActions.Canon.started -= FireCanonStarted;
        pInputActions.Canon.canceled -= FireCanonCanceled;
        pInputActions.SlowTime.started -= OnTimeSlowStarted;
        pInputActions.SlowTime.canceled -= OnTimeSlowCanceled;
        
        // Y lock toggle feature is for testing and likely not for final gameplay
        pInputActions._AddFuel.Disable();
        pInputActions._AddFuel.started -= OnAddFuelKey;
    }
    
}