using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayerCharacterCtrlr : MonoBehaviour {
    
    PlayerInputActions.PlayerActions inputActions;
    
    Vector3 desiredRotation = Vector3.forward;
    Vector3 prevDesiredRotation = Vector3.forward;
    Vector3 weaponRelativeRot = Vector3.forward;
    
    // Events
    public event Action<float, float> A_FuelAdded; // float changeAmnt, float fuelPerc
    public event Action<float, float, bool> A_FuelSpent; // float changeAmnt, float fuelPerc, bool spentAsHealth
    public event Action<float> A_PlayerTakenDamage; // float amount
    public event Action<float> A_PlayerHealed; // float amount
    public event Action A_PlayerDied;
    
    [HideInInspector]
    public float mouseSensitivity;
    
    Camera mainCamera;
    Camera rearCamera;
    // RectTransform mirrorCrosshairRectTrans;
    
    // UI variables
    UIGamePanel _gamePanel;
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
    float FuelRefillDelay = 3;
    [SerializeField]
    float CanonFuelCost = 6f;
    [SerializeField]
    [Tooltip("The amount of seconds to spend 100 fuel")]
    float VacuumFuelTime = 8f; // The amount of seconds to spend 100 fuel
    float vacuumFuelCost; // Calculated based on VacuumFuelTime
    float currentFuel;
    
    [Header("Health Settings")]
    public float MaxHealth = 100f;
    [HideInInspector]
    public float CurrentHealth;
    [SerializeField]
    float HealthRegenPerSecond;
    [SerializeField]
    float HealthRegenDelay = 1;
    float lastDmgTimeForRegen = -1000;
    [SerializeField]
    [Tooltip("Multiplier for the cost of fuel when the player uses health as fuel")]
    float FuelHelathCostMultiplier = 1;
    
    [Header("References")]
    [SerializeField]
    Transform camtrans;
    [SerializeField]
    Transform charModel;
    [SerializeField]
    Transform charPivot;
    [SerializeField]
    GameObject vacuumHitbox;
    [SerializeField]
    Transform canonTip;
    [SerializeField]
    ProjectileBase projectilePrefab;
    [SerializeField]
    GameObject muzzleFlashEffect;
    GameObject currentMuzzleFlashEffect;
    [SerializeField]
    Transform rearCamPos;
    
    [HideInInspector]
    public Rigidbody rb;
    float lookVertRot = 0;
    Vector3 aimPoint = Vector3.zero;
    float AimRayMaxDist = 1000f;
    // float AimRayMinDist = 0f;
    int AimRayLayerMask;
    float desiredRotationUpdateTime = 0;
    Quaternion rotBeforeInputUpdate = Quaternion.identity;
    float pivotRotLerpPower = 4;
    float pivotRotLerpTime = 0.1f;
    Lava lava;
    
    
    
    /** Variables for likely to be temporary features or for testing **/
    [Header("Temporary/testing")]
    public bool IsInvincible = false;
    public bool NoFuelCost = false;
    [SerializeField]
    float thirdPersonDist = 1.2f;
    bool isInThirdPerson = false;
    [SerializeField]
    GameObject rearMirrorModel;
    bool mirrorModelEnabled = false;
    // Sprite[] crosshairSprites = new Sprite[200];
    // int crosshairIndex = 0;
    
    // NOTE: Adams stuff
    [HideInInspector]public bool spaceInput = true;
    [HideInInspector]public bool vacEnableddd = true;
    [HideInInspector]public bool cannonEnabled = true;

    void Awake() {
        // inputActions = GameManager.PInputActions.Player;
        inputActions = new PlayerInputActions().Player;
        _gamePanel = GameManager.Instance.MainCanvas.GamePanel;
        
        mouseSensitivity = GameManager.Instance.CurrentMouseSensitivity;
        
        mainCamera = Camera.main;
        rearCamera = GameManager.Instance.rearCamera;
        rb = GetComponent<Rigidbody>();

        vacuumHitbox.SetActive(false);
        
        // ~(layers to ignore)
        AimRayLayerMask = ~(
            (1 << LayerMask.NameToLayer("Player")) |
            (1 << LayerMask.NameToLayer("Projectile")) |
            (1 << LayerMask.NameToLayer("Weapon")) |
            (1 << LayerMask.NameToLayer("Pickup"))
        );
        
        AddFuel(MaxFuel);
        vacuumFuelCost = MaxFuel / VacuumFuelTime * Time.fixedDeltaTime;
        CurrentHealth = MaxHealth;
        
        // _pausePanel.SetActive(false);
        
        /** Temp stuff **/
        // crosshairSprites = Resources.LoadAll<Sprite>("White") ;
        rearMirrorModel.SetActive(mirrorModelEnabled);
    }
    
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        currentMuzzleFlashEffect = Instantiate(muzzleFlashEffect, canonTip);
        currentMuzzleFlashEffect.SetActive(false);
        
        lava = GameManager.Instance.currentSceneRunner.lava;
        
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
        if (isVacuumOn && vacEnableddd) {
            if (CurrentHealth <= 0) {
                isVacuumOn = false;
                vacuumHitbox.SetActive(false);
                // print("Not enough fuel (" + currentFuel + ") for vacuum (need " + vacuumFuelCost + ").");
                // signifyOutOfFuel();
            } else {
                SpendFuel(vacuumFuelCost);
                rb.AddForce(charPivot.forward * (rb.velocity.magnitude <= VacuumForceNormalSpeed ? VacuumForceLowSpeed : VacuumForce), ForceMode.Acceleration);
            }
        }
        handleHealthRegen();
        handleLavaCheck();
        _gamePanel.SetSpeedText(rb.velocity.magnitude);
    }
    
    void LateUpdate() {
        updateCameraTransform();
        updateCrosshairPositions();
    }

    public void AddFuel(float amount) {
        currentFuel = Mathf.Min(currentFuel + amount, MaxFuel);
        A_FuelAdded?.Invoke(amount, currentFuel / MaxFuel);
    }
    
    public void SpendFuel(float amount) {
        if (NoFuelCost) return;
        
        bool spentAsHealth = false;
        if (currentFuel > 0) {
            currentFuel -= amount;
            if (currentFuel < 0) {
                spentAsHealth = true;
                TakeDamage(-currentFuel * FuelHelathCostMultiplier, EDamageType.Any);
                currentFuel = 0;
            }
        } else {
            spentAsHealth = true;
            //StartCoroutine(StartRefillFuelTimer());
            TakeDamage(amount * FuelHelathCostMultiplier, EDamageType.Any);
        }
        A_FuelSpent?.Invoke(amount, currentFuel / MaxFuel, spentAsHealth);
    }
    
    IEnumerator StartRefillFuelTimer() {
        yield return new WaitForSeconds(FuelRefillDelay);
        AddFuel(MaxFuel);
    }
    
    public void TakeDamage(float amount, EDamageType damageType) {
        if (IsInvincible) return;
        
        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        
        // print("Player took " + amount + " damage. Health: " + currentHealth);
        if (CurrentHealth == 0) {
            // Debug.Log("player died womp womp");
            A_PlayerDied?.Invoke();
            gameObject.SetActive(false);
        }
        
        A_PlayerTakenDamage?.Invoke(amount);

        lastDmgTimeForRegen = Time.fixedTime;
    }
    
    public void HealHealth(float amount) {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        A_PlayerHealed?.Invoke(amount);
    }
    
    void signifyOutOfFuel() {
        GameManager.Instance.Audio2D.PlayClipSFX(AudioPlayer2D.EClipSFX.Plr_OutOfFuel);
        _gamePanel.OnOutOfFuel();
    }

    private void TurnInputChanged(InputAction.CallbackContext context) {
        Vector2 v = context.ReadValue<Vector2>();
        setDesiredRotation(v.x, desiredRotation.y, v.y);
        
        /** Input overlay stuff **/
        // A_TurnInputChanged?.Invoke(v);
        _gamePanel.OnTurnInputChanged(v);
        
        // print(context.control.name + " - pf: " + context.performed + " | st: " + context.started + " | ca: " + context.canceled);
    }

    private void VertInputChanged(InputAction.CallbackContext context) {
        if(spaceInput){
            setDesiredRotation(desiredRotation.x, context.ReadValue<float>(), desiredRotation.z);
            
            /** Input overlay stuff **/
            // A_VertInputChanged?.Invoke(context.ReadValue<float>());
            _gamePanel.OnVertInputChanged(context.ReadValue<float>());
        }
    }

    private void FireVacuumStarted(InputAction.CallbackContext context) {
        if (!vacEnableddd) return;
        
        _gamePanel.OnFireVacuum(true);
        
        if (CurrentHealth <= 0) {
            // print("Not enough fuel (" + currentFuel + ") for vacuum (need " + vacuumFuelCost + ").");
            // signifyOutOfFuel();
            return;
        }
        isVacuumOn = true;
        vacuumHitbox.SetActive(true);
        
    }

    private void FireVacuumCanceled(InputAction.CallbackContext context) {
        isVacuumOn = false;
        vacuumHitbox.SetActive(false);
        
        _gamePanel.OnFireVacuum(false);
    }

    private void FireCanonStarted(InputAction.CallbackContext context) {
        _gamePanel.OnFireCanon(true);
        
        if (CurrentHealth <= 0) {
            // print("Not enough fuel (" + currentFuel + ") for canon (need " + CanonFuelCost + ").");
            // signifyOutOfFuel();
            return;
        }
        // Time.timeScale = 0.15f;
        if(cannonEnabled){
            fireCanon();
        }
        
    }

    private void FireCanonCanceled(InputAction.CallbackContext context) {
        _gamePanel.OnFireCanon(false);
        
        // Time.timeScale = 1f;
        if (currentFuel <= 0) {
            // print("Not enough fuel (" + currentFuel + ") for canon (need " + CanonFuelCost + ").");
            // signifyOutOfFuel();
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
    }
    
    public void OnResumeGame() {
        SetPlayerControlsEnabled(true);
    }

    void updateCameraTransform() {
        if (!isInThirdPerson) {
            mainCamera.transform.position = camtrans.position;
        } else {
            mainCamera.transform.position = camtrans.position - camtrans.forward * thirdPersonDist;
        }
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
        if (desiredRotation.magnitude > 0.0001f) {
            prevDesiredRotation = desiredRotation;
            Vector3 prevWpRelRot = weaponRelativeRot;
            weaponRelativeRot = desiredRotation.normalized;
            if (prevWpRelRot != weaponRelativeRot) {
                AudioPlayer2D.Instance.PlayClipSFX(AudioPlayer2D.EClipSFX.Plr_RotateWoosh);
            }
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
        SpendFuel(CanonFuelCost);
        GameManager.Instance.Audio2D.PlayClipSFX(AudioPlayer2D.EClipSFX.Weapon_CanonShot);
        playMuzzleFlashEffect();
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
        // The vacuum does not account for the player's velocity
        Vector3 screenPointVacuum;
        if (!isInThirdPerson) {
            screenPointVacuum = mainCamera.WorldToScreenPoint(camtrans.position + charPivot.forward);
        } else {
            screenPointVacuum = mainCamera.WorldToScreenPoint(charModel.position + charPivot.forward * 1.5f);
        }
        // The canon does account for the player's velocity
        Vector3 screenPointCanon;
        if (!isInThirdPerson) {
            screenPointCanon = mainCamera.WorldToScreenPoint(camtrans.position + charPivot.forward + -rbVelocityCompensation);
        } else {
            updateRayCastedAimPoint();
            screenPointCanon = mainCamera.WorldToScreenPoint(
                aimPoint + rbVelocityCompensation * (aimPoint - mainCamera.transform.position).magnitude
            );
            screenPointCanon.z *= -1;
        }
        
        _gamePanel.UpdateCrosshairPositions(screenPointVacuum, screenPointCanon);
    }
    
    void handleHealthRegen() {
        if ((CurrentHealth < MaxHealth) && (Time.fixedTime - lastDmgTimeForRegen >= HealthRegenDelay)) {
            HealHealth(HealthRegenPerSecond * Time.fixedDeltaTime);
        }
    }
    
    void handleLavaCheck() {
        float offset = 0.7321717f;
        if (lava && transform.position.y - offset <= lava.currentHeight) {
            transform.position = new(transform.position.x, lava.currentHeight + offset + 0.1f, transform.position.z);
            Vector3 bouncedVel = Vector3.Reflect(rb.velocity, Vector3.up);
            bouncedVel.y = Mathf.Max(bouncedVel.y, lava.MinimumVerticalBounceSpeed);
            rb.velocity = bouncedVel;
            TakeDamage(lava.LavaDamage, EDamageType.Any);
        }
    }
    
    void playMuzzleFlashEffect() {
        currentMuzzleFlashEffect.SetActive(true);
        Destroy(currentMuzzleFlashEffect, 1);
        currentMuzzleFlashEffect = Instantiate(muzzleFlashEffect, canonTip);
        currentMuzzleFlashEffect.SetActive(false);
    }
    
    void OnEnable() {
        GameManager.A_GamePaused += OnPauseGame;
        GameManager.A_GameResumed += OnResumeGame;
        SetPlayerControlsEnabled(true);
    }

    void OnDisable() {
        GameManager.A_GamePaused -= OnPauseGame;
        GameManager.A_GameResumed -= OnResumeGame;
        SetPlayerControlsEnabled(false);
    }
    
    void OnDestroy() {
        /* A GameObject is only truly destroyed the frame after Destroy() is called on it.
         * OnDestroy() is called right before the object is destroyed. However, this means
         * OnDestroy() is still only caleld the frame right after.
         * If you want to do something at the moment the player (or any game object) gets
         * destroyed, do that code in OnDisable() instead.
         */
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
            
            
            /** Features not necessarily meant for final gameplay **/
            inputActions._ToggleTP.Enable();
            inputActions._ToggleTP.started += On_ToggleThirdPerson;
            inputActions._AddFuel.Enable();
            inputActions._AddFuel.started += On_AddFuelKey;
            inputActions._ToggleMirror.Enable();
            inputActions._ToggleMirror.started += On_ToggleMirrorInput;
            inputActions._TakeDamage.Enable();
            inputActions._TakeDamage.started += On_TakeDamage;
            inputActions._HealHealth.Enable();
            inputActions._HealHealth.started += On_HealHealth;
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
            inputActions._ToggleTP.Disable();
            inputActions._ToggleTP.started -= On_ToggleThirdPerson;
            inputActions._AddFuel.Disable();
            inputActions._AddFuel.started -= On_AddFuelKey;
            inputActions._ToggleMirror.Disable();
            inputActions._ToggleMirror.started -= On_ToggleMirrorInput;
            inputActions._TakeDamage.Disable();
            inputActions._TakeDamage.started -= On_TakeDamage;
            inputActions._HealHealth.Disable();
            inputActions._HealHealth.started -= On_HealHealth;
        }
    }







    /*****  Probably temporary stuff  *****/
    
    void On_ToggleThirdPerson(InputAction.CallbackContext context) {
        isInThirdPerson = !isInThirdPerson;
    }

    void On_AddFuelKey(InputAction.CallbackContext context) {
        print("Fully refueling fuel (F cheat key).");
        AddFuel(MaxFuel);
    }

    void On_ToggleMirrorInput(InputAction.CallbackContext context) {
        mirrorModelEnabled = !mirrorModelEnabled;
        rearMirrorModel.SetActive(mirrorModelEnabled);
    }
    
    void On_TakeDamage(InputAction.CallbackContext context) {
        float DamageAmount = 10;
        print("Damaging player for " + DamageAmount + " damage.");
        TakeDamage(DamageAmount, EDamageType.Any);
    }
    
    void On_HealHealth(InputAction.CallbackContext context) {
        float HealAmount = 10;
        print("Healing player for " + HealAmount + " health.");
        HealHealth(HealAmount);
    }
    
    // public void OnSinkBelowLava() {
    //     TakeDamage(GameManager.Instance.currentSceneRunner.lava.LavaDamage, EDamageType.Any);
    // }
    
}