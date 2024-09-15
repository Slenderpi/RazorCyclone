using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerCharacterCtrlr : MonoBehaviour {
    
    PlayerInputActions.PlayerActions inputActions;
    
    Vector3 desiredRotation = Vector3.forward;
    Vector3 prevDesiredRotation = Vector3.forward;
    Vector3 weaponRelativeRot = Vector3.forward;
    
    [HideInInspector]
    public float mouseSensitivity;
    
    Camera mainCamera;
    Camera rearCamera;
    
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
    Transform rearCamPos;
    // RectTransform mirrorCrosshairRectTrans;
    
    // ui
    RectTransform _mainVacuumCrosshair;
    RectTransform _mainCanonCrosshair;
    Slider _fuelSlider;
    GameObject _gamePanel;
    GameObject _pausePanel;
    Image _keyImageW;
    Image _keyImageA;
    Image _keyImageS;
    Image _keyImageD;
    Image _keyImageSpace;
    Image _keyImageShift;
    TMP_Text _textKeyM1;
    TMP_Text _textKeyM2;
    [SerializeField]
    Image mirrorCrosshairImageComp;
    
    bool isVacuumOn;
    [Header("Weapon Settings")]
    [SerializeField]
    [Tooltip("The force the vacuum pulls the player")]
    float VacuumForce;
    [SerializeField]
    [Tooltip("The force of the vacuum pull when the player is at low speeds")]
    float VacuumForceLowSpeed;
    [SerializeField]
    [Tooltip("The max speed for VacuumForceLowSpeed to be used at")]
    float VacuumForceNormalSpeed = 8;
    [Tooltip("The force applied to rigidbodies caught in the vacuum's hitbox")]
    public float VacuumSuckForce = 8500f;
    public float VacuumSuckRate { private set; get; } = 0.1f; // per second
    public float VacuumDamage {private set; get; } = 7f;
    [SerializeField]
    float CanonForce;
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
    
    float lookVertRot = 0;
    Vector3 aimPoint = Vector3.zero;
    float AimRayMaxDist = 1000f;
    // float AimRayMinDist = 0f;
    int AimRayLayerMask;
    float desiredRotationUpdateTime = 0;
    Quaternion rotBeforeInputUpdate = Quaternion.identity;
    float pivotRotLerpPower = 4;
    float pivotRotLerpTime = 0.1f;
    
    
    /** Variables for likely to be temporary features **/
    [Header("Temporary/testing")]
    [SerializeField]
    GameObject rearMirrorModel;
    bool mirrorModelEnabled = true;
    Sprite[] crosshairSprites = new Sprite[200];
    int crosshairIndex = 0;
    
    
    
    void Awake() {
        inputActions = GameManager.PInputActions.Player;
        _fuelSlider = GameManager.Instance.FuelSlider;
        _gamePanel = GameManager.Instance.GamePanel;
        _pausePanel = GameManager.Instance.PausePanel;
        _mainVacuumCrosshair = GameManager.Instance.MainVacuumCrosshair;
        _mainCanonCrosshair = GameManager.Instance.MainCanonCrosshair;
        _keyImageW = GameManager.Instance.KeyImageW;
        _keyImageA = GameManager.Instance.KeyImageA;
        _keyImageS = GameManager.Instance.KeyImageS;
        _keyImageD = GameManager.Instance.KeyImageD;
        _keyImageSpace = GameManager.Instance.KeyImageSpace;
        _keyImageShift = GameManager.Instance.KeyImageShift;
        _textKeyM1 = GameManager.Instance.TextKeyM1;
        _textKeyM2 = GameManager.Instance.TextKeyM2;
        
        mouseSensitivity = GameManager.Instance.CurrentMouseSensitivity;
        
        mainCamera = Camera.main;
        rearCamera = GameManager.Instance.rearCamera;
        rb = GetComponent<Rigidbody>();

        vacuumHitbox.SetActive(false);
        
        AimRayLayerMask = ~(1 << LayerMask.NameToLayer("Player"));
        
        AddFuel(MaxFuel);
        vacuumFuelCost = MaxFuel / VacuumFuelTime * Time.fixedDeltaTime;
        currentHealth = MaxHealth;
        
        _pausePanel.SetActive(false);
        
        
        /** Temp stuff **/
        crosshairSprites =  Resources.LoadAll<Sprite>("White") ;
        rearMirrorModel.SetActive(mirrorModelEnabled);
    }
    
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        updateCameraTransform();
    }
    
    void Update() {
        // Rotate camera and character based on mouse input
        Vector2 lookDelta = inputActions.Look.ReadValue<Vector2>() * mouseSensitivity;
        lookVertRot = Mathf.Clamp(lookVertRot - lookDelta.y, -90f, 90f);
        camtrans.localEulerAngles = new Vector3(lookVertRot, camtrans.localEulerAngles.y + lookDelta.x, 0);
        charModel.localEulerAngles = new Vector3(0, camtrans.localEulerAngles.y, 0);
        
        interpRotPivot();
    }

    void FixedUpdate() {
        if (isVacuumOn) {
            if (currentFuel <= 0) {
                isVacuumOn = false;
                vacuumHitbox.SetActive(false);
                // print("Not enough fuel (" + currentFuel + ") for vacuum (need " + vacuumFuelCost + ").");
            } else {
                AddFuel(-vacuumFuelCost);
                rb.AddForce(charPivot.forward * (rb.velocity.magnitude <= VacuumForceNormalSpeed ? VacuumForceLowSpeed : VacuumForce), ForceMode.Acceleration);
            }
        }
    }
    
    void LateUpdate() {
        updateCameraTransform();
        updateCrosshairPositions();
    }

    public void AddFuel(float amount) {
        currentFuel = Mathf.Clamp(currentFuel + amount, 0, MaxFuel);
        _fuelSlider.value = currentFuel / MaxFuel;
    }

    private void TurnInputChanged(InputAction.CallbackContext context) {
        Vector2 v = context.ReadValue<Vector2>();
        setDesiredRotation(v.x, desiredRotation.y, v.y);
        
        /** Input overlay stuff **/
        if (v.y > 0.001f) { // Forward/backward
            _keyImageW.color = Color.white;
            _keyImageS.color = Color.gray;
        } else if (v.y < -0.001f) {
            _keyImageW.color = Color.gray;
            _keyImageS.color = Color.white;
        } else {
            _keyImageW.color = Color.gray;
            _keyImageS.color = Color.gray;
        }
        if (v.x > 0.001f) { // Right/left
            _keyImageD.color = Color.white;
            _keyImageA.color = Color.gray;
        } else if (v.x < -0.001f) {
            _keyImageD.color = Color.gray;
            _keyImageA.color = Color.white;
        } else {
            _keyImageD.color = Color.gray;
            _keyImageA.color = Color.gray;
        }
        
        // print(context.control.name + " - pf: " + context.performed + " | st: " + context.started + " | ca: " + context.canceled);
    }

    private void VertInputChanged(InputAction.CallbackContext context) {
        setDesiredRotation(desiredRotation.x, context.ReadValue<float>(), desiredRotation.z);
        
        /** Input overlay stuff **/
        float y = context.ReadValue<float>();
        if (y > 0.001f) { // Up/down
            _keyImageSpace.color = Color.white;
            _keyImageShift.color = Color.gray;
        } else if (y < -0.001f) {
            _keyImageSpace.color = Color.gray;
            _keyImageShift.color = Color.white;
        } else {
            _keyImageSpace.color = Color.gray;
            _keyImageShift.color = Color.gray;
        }
    }

    private void FireVacuumStarted(InputAction.CallbackContext context) {
        _textKeyM1.color = Color.white;
        
        if (currentFuel <= 0) {
            // print("Not enough fuel (" + currentFuel + ") for vacuum (need " + vacuumFuelCost + ").");
            return;
        }
        isVacuumOn = true;
        vacuumHitbox.SetActive(true);
    }

    private void FireVacuumCanceled(InputAction.CallbackContext context) {
        isVacuumOn = false;
        vacuumHitbox.SetActive(false);
        
        _textKeyM1.color = Color.gray;
    }

    private void FireCanonStarted(InputAction.CallbackContext context) {
        _textKeyM2.color = Color.white;
        
        if (currentFuel <= 0) {
            // print("Not enough fuel (" + currentFuel + ") for canon (need " + CanonFuelCost + ").");
            return;
        }
        // Time.timeScale = 0.15f;
        fireCanon();
    }

    private void FireCanonCanceled(InputAction.CallbackContext context) {
        _textKeyM2.color = Color.gray;
        
        // Time.timeScale = 1f;
        if (currentFuel <= 0) {
            // print("Not enough fuel (" + currentFuel + ") for canon (need " + CanonFuelCost + ").");
            return;
        }
        // fireCanon();
    }
    
    private void OnTimeSlowStarted(InputAction.CallbackContext context) {
        // frontIsVacuum = !frontIsVacuum;
        Time.timeScale = 0.1f;
    }
    
    private void OnTimeSlowCanceled(InputAction.CallbackContext context) {
        Time.timeScale = 1f;
    }

    public void OnPauseGame() {
        SetPlayerControlsEnabled(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void OnResumeGame() {
        SetPlayerControlsEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void updateCameraTransform() {
        mainCamera.transform.position = camtrans.position;
        mainCamera.transform.rotation = camtrans.rotation;
        rearCamera.transform.position = rearCamPos.position;
        rearCamera.transform.rotation = Quaternion.LookRotation(canonTip.forward, canonTip.up);
    }
    
    void setDesiredRotation(float x, float y, float z) {
        desiredRotationUpdateTime = Time.time;
        rotBeforeInputUpdate = charPivot.localRotation;
        
        desiredRotation.x = x;
        desiredRotation.y = y;
        desiredRotation.z = z;
        if (desiredRotation.magnitude > 0.00001) {
            prevDesiredRotation = desiredRotation;
            weaponRelativeRot = desiredRotation.normalized;
        } else {
            weaponRelativeRot = prevDesiredRotation.normalized;
        }
    }
    
    void fireCanon() {
        updateRayCastedAimPoint();
        rb.AddForce((charModel.rotation * weaponRelativeRot).normalized * CanonForce * 100000);
        ProjectileBase proj = Instantiate(projectilePrefab, canonTip.position, canonTip.rotation);
        proj.damage = CanonDamage;
        proj.GetComponent<Rigidbody>().AddForce((aimPoint - canonTip.position).normalized * CanonProjSpeed + rb.velocity, ForceMode.VelocityChange);
        AddFuel(-CanonFuelCost);
    }

    void updateRayCastedAimPoint() {
        Ray ray = new Ray(camtrans.position, charModel.rotation * -weaponRelativeRot);
        RaycastHit hit;
        aimPoint = ray.origin + ray.direction * AimRayMaxDist;
        if (Physics.Raycast(ray: ray, maxDistance: AimRayMaxDist, layerMask: AimRayLayerMask, hitInfo: out hit)) {
            aimPoint = hit.point;
        }
    }

    void interpRotPivot() {
        Quaternion rot = Quaternion.LookRotation(weaponRelativeRot);
        /* This alpha calculation forms a curve with a steep start (f'(0) > 0) and a flat end (f'(1) = 0), creating a snappy feel
         * To increase snappiness, increase pivotRotLerpPower. If lowered to a power of 1, the curve is linear. If below 1, it
         *     creates a sense of lag by delaying the rotation, so it's better to keep the power above 1.
         * To increase/decrease the speed of the animation, decrease/increase pivotRotLerpTime. This variable determines the
         *     amount of time the animation takes (a time of 0.6 means it takes of 0.6 seconds to make the rotation).
         * The equation for alpha is alpha = -(-alphaX + 1)^lerpPowewr + 1
         */
        float alphaX = Mathf.Min((Time.time - desiredRotationUpdateTime) / pivotRotLerpTime, 1f);
        float alpha = -Mathf.Pow(-alphaX + 1, pivotRotLerpPower) + 1f;
        charPivot.localRotation = Quaternion.Lerp(rotBeforeInputUpdate, rot, alpha);
    }
    
    void updateCrosshairPositions() {
        Vector3 rbVelocityCompensation = rb.velocity.magnitude > 0.001f ? rb.velocity / CanonProjSpeed : Vector3.zero;
        // This one does not account for the plyaer's velocity
        Vector3 screenPointVacuum = mainCamera.WorldToScreenPoint(camtrans.position + charPivot.forward);
        // This one accounts for the player's velocity
        Vector3 screenPointCanon = mainCamera.WorldToScreenPoint(camtrans.position + charPivot.forward + -rbVelocityCompensation);
        if (screenPointVacuum.z > 0.01f) {
            if (!_mainVacuumCrosshair.gameObject.activeSelf) _mainVacuumCrosshair.gameObject.SetActive(true);
            _mainVacuumCrosshair.position = screenPointVacuum;
        } else {
            if (_mainVacuumCrosshair.gameObject.activeSelf) _mainVacuumCrosshair.gameObject.SetActive(false);
        }
        if (screenPointCanon.z < 0.01f) {
            if (!_mainCanonCrosshair.gameObject.activeSelf) _mainCanonCrosshair.gameObject.SetActive(true);
            _mainCanonCrosshair.position = screenPointCanon;
        } else {
            if (_mainCanonCrosshair.gameObject.activeSelf) _mainCanonCrosshair.gameObject.SetActive(false);
        }
        
        // // Position mirror's crosshair
        // Vector3 screenPointCanonMirror = rearCamera.WorldToScreenPoint(rearCamPos.position + rearCamPos.forward + rbVelocityCompensation);
        // mirrorCrosshairRectTrans.position = screenPointCanonMirror;
    }
    
    void OnEnable() {
        SetPlayerControlsEnabled(true);
    }

    void OnDisable() {
        SetPlayerControlsEnabled(false);
    }

    public void SetPlayerControlsEnabled(bool newEnabled) {
        if (newEnabled) {
            inputActions.Look.Enable();
            inputActions.TurnInputs.Enable();
            inputActions.VertInputs.Enable();
            inputActions.Vacuum.Enable();
            inputActions.Canon.Enable();
            inputActions.SlowTime.Enable();
            
            inputActions.TurnInputs.performed += TurnInputChanged;
            inputActions.TurnInputs.canceled += TurnInputChanged;
            inputActions.VertInputs.started += VertInputChanged;
            inputActions.VertInputs.canceled += VertInputChanged;

            inputActions.Vacuum.started += FireVacuumStarted;
            inputActions.Vacuum.canceled += FireVacuumCanceled;
            inputActions.Canon.started += FireCanonStarted;
            inputActions.Canon.canceled += FireCanonCanceled;
            inputActions.SlowTime.started += OnTimeSlowStarted;
            inputActions.SlowTime.canceled += OnTimeSlowCanceled;
            
            
            /** Featurescessarily meant for final gameplay **/
            inputActions._AddFuel.Enable();
            inputActions._AddFuel.started += OnAddFuelKey;
            inputActions._CycleCrosshair.Enable();
            inputActions._CycleCrosshair.started += OnCycleCrosshairInput;
            inputActions._ToggleMirror.Enable();
            inputActions._ToggleMirror.started += OnToggleMirrorInput;
        } else {
            inputActions.Look.Disable();
            inputActions.TurnInputs.Disable();
            inputActions.VertInputs.Disable();
            inputActions.Vacuum.Disable();
            inputActions.Canon.Disable();
            inputActions.SlowTime.Disable();
            
            inputActions.TurnInputs.performed -= TurnInputChanged;
            inputActions.TurnInputs.canceled -= TurnInputChanged;
            inputActions.VertInputs.started -= VertInputChanged;
            inputActions.VertInputs.canceled -= VertInputChanged;
            
            inputActions.Vacuum.started -= FireVacuumStarted;
            inputActions.Vacuum.canceled -= FireVacuumCanceled;
            inputActions.Canon.started -= FireCanonStarted;
            inputActions.Canon.canceled -= FireCanonCanceled;
            inputActions.SlowTime.started -= OnTimeSlowStarted;
            inputActions.SlowTime.canceled -= OnTimeSlowCanceled;
            
            
            /** Features not necessarily meant for final gameplay **/
            inputActions._AddFuel.Disable();
            inputActions._AddFuel.started -= OnAddFuelKey;
            inputActions._CycleCrosshair.Disable();
            inputActions._CycleCrosshair.started -= OnCycleCrosshairInput;
            inputActions._ToggleMirror.Disable();
            inputActions._ToggleMirror.started -= OnToggleMirrorInput;
        }
    }
    
    
    
    
    
    
    
    /*****  Probably temporary stuff  *****/
    
    private void OnAddFuelKey(InputAction.CallbackContext context) {
        print("Fully refueling fuel.");
        AddFuel(MaxFuel);
    }

    void OnCycleCrosshairInput(InputAction.CallbackContext context) {
        if (context.ReadValue<float>() > 0) {
            if (++crosshairIndex >= 200)
                crosshairIndex = 0;
        } else {
            crosshairIndex--;
            if (crosshairIndex < 0)
                crosshairIndex = 199;
        }
        
        mirrorCrosshairImageComp.sprite = crosshairSprites[crosshairIndex];
        print("Current mirror crosshair: \"" + crosshairSprites[crosshairIndex].name + "\"");
    }

    private void OnToggleMirrorInput(InputAction.CallbackContext context) {
        mirrorModelEnabled = !mirrorModelEnabled;
        rearMirrorModel.SetActive(mirrorModelEnabled);
    }
    
}