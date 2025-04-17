using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterCtrlr : MonoBehaviour {
    
    PlayerInputActions.PlayerActions inputActions;
    
    Vector3 desiredRotation = Vector3.zero;
    Vector3 weaponRelativeRot = Vector3.forward;
    Vector3 prevWeaponRelRot = Vector3.zero;
    [HideInInspector]
    public int currentBikeSpins = 0;
    int bikeSpinProgress = 0;
    Vector2 prevBikeFaceDir = Vector2.up; // For spinning the bike
    int ricochetBaseVal = 1;
    // int bikeRotSpinDir = 0; // For spinning the bike (determines CW or CCW)
    
    // Events
    public event Action<float, float> A_FuelAdded; // float changeAmnt, float fuelPerc
    public event Action<float, float, bool> A_FuelSpent; // float changeAmnt, float fuelPerc, bool spentAsHealth
    public event Action<float> A_PlayerTakenDamage; // float amount
    public event Action<float> A_PlayerHealed; // float amount
    public event Action A_PlayerDied;
    public event Action<int> A_SpinProgressed; // int progress
    public event Action<int> A_SpinProgressReset; // int progressBeforeReset
    public event Action<int> A_SpinCompleted; // int newSpinCount
    public event Action<int, int> A_SpinsSpent; // int prevSpinCount, int newSpinCount
    
    [HideInInspector]
    public float mouseSensitivity;
    
    Camera mainCamera;
    Camera rearCamera;
    
    // UI variables
    UIGamePanel _gamePanel;
    
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
    float CannonForce = 8;
    [SerializeField]
    float CannonBaseProjSpeed = 125;
    [SerializeField]
    [Tooltip("When a projectile is fired, its velocity will include the player's velocity by a factor.\nA value of 0 would mean player velocity has no effect on the projectile's veloctiy.\nA value of 0.5 would mean the projectile would add half of the player's velocity.")]
    float InheritedVelocityFactor = 0.7f;
    
    [Header("Fuel Settings")]
    [SerializeField]
    float MaxFuel = 100f;
    [SerializeField]
    float CannonFuelCost = 12;
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
    VacuumScript Vacuum;
    [SerializeField]
    [Tooltip("Transform indicating where the projectile will spawn from.")]
    Transform cannonProjSpawnTrans;
    [SerializeField]
    [Tooltip("Transform for spawning muzzle flash VFX.")]
    Transform cannonMuzzleTrans;
    [SerializeField]
    ProjectileBase projectilePrefab;
    [SerializeField]
    GameObject muzzleFlashEffect;
    GameObject currentMuzzleFlashEffect;
    [SerializeField]
    Transform rearCamPos;
    
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Vector2 lookDelta;
    float lookVertRot = 0;
    Vector3 aimPoint = Vector3.zero;
    float AimRayMaxDist = 1000f;
    int AimRayLayerMask;
    Quaternion rotBeforeInputUpdate = Quaternion.identity;
    Quaternion rotAfterInputUpdate = Quaternion.identity;
    float pivotRotF = 2.8f;
    float pivotRotZ = 0.81f;
    float pivotRotR = 0.7f;
    SecondOrderDynamicsF sodPivotRotAlpha;
    Lava lava;
    
    
    
    /** Variables for likely to be temporary features or for testing **/
    [Header("Temporary/testing")]
    public bool IsInvincible = false;
    public bool NoFuelCost = false;
    [SerializeField]
    float thirdPersonDist = 1.2f;
    bool isInThirdPerson = false;
    public bool mirrorModelEnabled = true;
    [SerializeField]
    GameObject rearMirrorModel;
    
    // NOTE: Adams stuff
    [HideInInspector]public bool spaceInput = true;
    [HideInInspector]public bool vacEnableddd = true;
    [HideInInspector]public bool cannonEnabled = true;
    
    
    
    void Awake() {
        inputActions = new PlayerInputActions().Player;
        _gamePanel = GameManager.Instance.MainCanvas.GamePanel;
        
        mouseSensitivity = GameManager.Instance.CurrentMouseSensitivity;
        
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        
        // Vacuum.SetActive(false);
        
        // 1 is for layers to include in raycast
        AimRayLayerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("EnemyHitbox"));
        
        AddFuel(MaxFuel);
        vacuumFuelCost = MaxFuel / VacuumFuelTime * Time.fixedDeltaTime;
        CurrentHealth = MaxHealth;
        
        sodPivotRotAlpha = new SecondOrderDynamicsF(pivotRotF, pivotRotZ, pivotRotR, 0);
        
        /** Temp stuff **/
        rearMirrorModel.SetActive(mirrorModelEnabled);
    }
    
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        currentMuzzleFlashEffect = Instantiate(muzzleFlashEffect, cannonMuzzleTrans);
        currentMuzzleFlashEffect.SetActive(false);
        
        lava = GameManager.Instance.currentSceneRunner.lava;
        rearCamera = GameManager.Instance.rearCamera;
        
        updateCameraTransform();
    }
    
    void Update() {
        // Rotate camera and character based on mouse input
        lookDelta = inputActions.Look.ReadValue<Vector2>() * mouseSensitivity;
        lookVertRot = Mathf.Clamp(lookVertRot - lookDelta.y, -90f, 90f);
        camtrans.localEulerAngles = new Vector3(lookVertRot, camtrans.localEulerAngles.y + lookDelta.x, 0);
        charModel.localEulerAngles = new Vector3(0, camtrans.localEulerAngles.y, 0);
        
        if (Time.deltaTime > 0) {
            charPivot.localRotation = Quaternion.LerpUnclamped(
                rotBeforeInputUpdate,
                rotAfterInputUpdate,
                sodPivotRotAlpha.Update(1, Time.deltaTime)
            );
        }
    }
    
    void FixedUpdate() {
        // if (isVacuumOn && vacEnableddd) {
        //     if (CurrentHealth <= 0) {
        //         isVacuumOn = false;
        //         vacuumHitbox.SetActive(false);
        //         // signifyOutOfFuel();
        //     } else {
        //         SpendFuel(vacuumFuelCost);
        //         rb.AddForce(charPivot.forward * (rb.velocity.magnitude <= VacuumForceNormalSpeed ? VacuumForceLowSpeed : VacuumForce), ForceMode.Acceleration);
        //     }
        // }
        if (isVacuumOn && vacEnableddd) {
            if (CurrentHealth > 0) {
                SpendFuel(vacuumFuelCost);
                rb.AddForce(charPivot.forward * (rb.velocity.magnitude <= VacuumForceNormalSpeed ? VacuumForceLowSpeed : VacuumForce), ForceMode.Acceleration);
            } else {
                isVacuumOn = false;
                Vacuum.DisableVacuum();
            }
        }
        handleHealthRegen();
        handleLavaCheck();
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
    
    // IEnumerator StartRefillFuelTimer() {
    //     yield return new WaitForSeconds(FuelRefillDelay);
    //     AddFuel(MaxFuel);
    // }
    
    public void TakeDamage(float amount, EDamageType damageType) {
        if (IsInvincible) return;
        
        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        
        if (CurrentHealth == 0) {
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
        setDesiredAndWepRelRots(v.x, desiredRotation.y, v.y);
        
        // Auto mirror
        if (mirrorModelEnabled) {
            updateMirrorActive();
        }

#if UNITY_EDITOR || KEEP_DEBUG
        /** Input overlay stuff **/
        _gamePanel.OnTurnInputChanged(v);
#endif
    }
    
    private void VertInputChanged(InputAction.CallbackContext context) {
        if(spaceInput){
            setDesiredAndWepRelRots(desiredRotation.x, context.ReadValue<float>(), desiredRotation.z);
            
#if UNITY_EDITOR || KEEP_DEBUG
            /** Input overlay stuff **/
            _gamePanel.OnVertInputChanged(context.ReadValue<float>());
#endif
        }
    }
    
    private void FireVacuumStarted(InputAction.CallbackContext context) {
        if (!vacEnableddd) return;
#if UNITY_EDITOR || KEEP_DEBUG
        _gamePanel.OnFireVacuum(true);
#endif
        if (CurrentHealth <= 0) {
            // signifyOutOfFuel();
            return;
        }
        isVacuumOn = true;
        Vacuum.EnableVacuum();
        
    }
    
    private void FireVacuumCanceled(InputAction.CallbackContext context) {
        isVacuumOn = false;
        Vacuum.DisableVacuum();
#if UNITY_EDITOR || KEEP_DEBUG
        _gamePanel.OnFireVacuum(false);
#endif
    }
    
    private void FireCannonStarted(InputAction.CallbackContext context) {
#if UNITY_EDITOR || KEEP_DEBUG
        _gamePanel.OnFireCannon(true);
#endif
        if (CurrentHealth <= 0) {
            // signifyOutOfFuel();
            return;
        }
        if(cannonEnabled){
            fireCannon();
        }
        
    }
    
    private void FireCannonCanceled(InputAction.CallbackContext context) {
#if UNITY_EDITOR || KEEP_DEBUG
        _gamePanel.OnFireCannon(false);
#endif
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
        if (mirrorModelEnabled) {
            Vector3 flatBackCamTrans = -camtrans.forward;
            if (flatBackCamTrans.x * flatBackCamTrans.x + flatBackCamTrans.z * flatBackCamTrans.z <= 0.00002f) {
                flatBackCamTrans -= camtrans.up * Mathf.Sign(flatBackCamTrans.y);
            }
            flatBackCamTrans.y = 0;
            rearCamera.transform.SetPositionAndRotation(rearCamPos.position, Quaternion.LookRotation(flatBackCamTrans));
        }
    }
    
    void setDesiredAndWepRelRots(float x, float y, float z) {
        prevWeaponRelRot = weaponRelativeRot;
        desiredRotation.x = x;
        desiredRotation.y = y;
        desiredRotation.z = z;
        if (desiredRotation.sqrMagnitude > 0.0001f) {
            weaponRelativeRot = desiredRotation.normalized;
            if (prevWeaponRelRot != weaponRelativeRot) {
                sodPivotRotAlpha.Reset(0);
                rotBeforeInputUpdate = charPivot.localRotation;
                rotAfterInputUpdate = Quaternion.LookRotation(weaponRelativeRot);
                checkBikeSpinning();
                // updateSpecialShotBonus();
                AudioPlayer2D.Instance.PlayClipSFX(AudioPlayer2D.EClipSFX.Plr_RotateWoosh);
            }
        } else {
            weaponRelativeRot = prevWeaponRelRot;
        }
    }
    
    void checkBikeSpinning() {
        // print("wrr: " + weaponRelativeRot);
        Vector2 currDir = new(weaponRelativeRot.x, weaponRelativeRot.z);
        if (Mathf.Abs(currDir.x) > 0.1f && Mathf.Abs(currDir.y) > 0.1f) {
            // print("DIAGONAL detected. currDir: " + currDir);
            return; // Ignore diagonal directions
        }
        currDir = currDir.normalized;
        if (prevBikeFaceDir.x == currDir.x && prevBikeFaceDir.y == currDir.y)
            // This case can occur when, for example, pressing WD, holding W and releasing D, then pressing D again
            return;
        // print("Passed validations. prevDir: " + prevBikeFaceDir + "; currDir: " + currDir);
        rotVec2ccw90(ref currDir);
        int dot = Mathf.RoundToInt(Vector2.Dot(prevBikeFaceDir, currDir)); // 1 is CW, -1 is CCW, 0 is 180 fail
        if (bikeSpinProgress == 0) { // RESET STATE
            // If currdir cw, go cw
            // else if ccw, go ccw
            // else that means it's 180 back. Reset spin progress if possible
            if (prevBikeFaceDir == Vector2.up) {
                if (dot != 0) {
                    // bikeRotSpinDir = dot;
                    // bikeSpinProgressed();
                    bikeSpinProgress = dot;
                    A_SpinProgressed?.Invoke(bikeSpinProgress);
                } else if (bikeSpinProgress > 0) {
                    bikeSpinProgressReset();
                }
            }
        } else {
            if (Math.Sign(bikeSpinProgress) == dot) { // Checks if the rotation matches spin direction
                bikeSpinProgressed();
            } else {
                bikeSpinProgressReset();
            }
        }
        prevBikeFaceDir.x = currDir.y;
        prevBikeFaceDir.y = -currDir.x;
    }
    
    void updateSpecialShotBonus() {
        int newRicBaseVal = 0;
        if (weaponRelativeRot.x == 0 && weaponRelativeRot.y == 0 && weaponRelativeRot.z == -1)
            newRicBaseVal = 0;
        else {
            // +1 if non-zero y
            newRicBaseVal += Mathf.Abs(weaponRelativeRot.y) > 0.1f ? 1 : 0;
            // +1 if non-zero x
            newRicBaseVal += Mathf.Abs(weaponRelativeRot.x) > 0.1f ? 1 : 0;;
            // +1 if 0 z
            if (Mathf.Abs(weaponRelativeRot.z) <= 0.001f)
                newRicBaseVal++;
        }
        if (newRicBaseVal != ricochetBaseVal) {
            ricochetBaseVal = newRicBaseVal;
            print("Base ricochet: " + ricochetBaseVal);
        }
    }
    
    /// <summary>
    /// Rotates a Vector2 90 degrees counter-clockwise. Pass the vector by reference:
    /// <code>rotVec2ccw90(ref myVector);</code>
    /// </summary>
    /// <param name="v"></param>
    void rotVec2ccw90(ref Vector2 v) {
        (v.x, v.y) = (-v.y, v.x);
    }
    
    void bikeSpinProgressReset() {
        A_SpinProgressReset?.Invoke(bikeSpinProgress);
        bikeSpinProgress = 0;
        // bikeRotSpinDir = 0;
        // Debug.LogError("Spin progress reset.");
    }
    
    void bikeSpinProgressComplete() {
        bikeSpinProgress = 0;
        // bikeRotSpinDir = 0;
        A_SpinCompleted?.Invoke(++currentBikeSpins);
        // Debug.LogWarning("Spin complete! Spins: " + currentBikeSpins);
    }
    
    void bikeSpinProgressed() {
        bikeSpinProgress += bikeSpinProgress > 0 ? 1 : -1;
        // print("Spin progressed to: " + bikeSpinProgress + "; Direction: " + (bikeRotSpinDir == 1 ? "clockwise" : "counter-clockwise"));
        if (bikeSpinProgress * bikeSpinProgress == 4 * 4)
            bikeSpinProgressComplete();
        else
            A_SpinProgressed?.Invoke(bikeSpinProgress);
        // if (++bikeSpinProgress == 4) {
        //     bikeSpinProgressComplete();
        // }
    }
    
    void fireCannon() {
        updateRayCastedAimPoint();
        rb.AddForce((charModel.rotation * weaponRelativeRot).normalized * CannonForce * 100000);
        Vector3 projVel = (aimPoint - cannonProjSpawnTrans.position).normalized * CannonBaseProjSpeed + rb.velocity * InheritedVelocityFactor;
        // if (ricochetBaseVal > 0) { // Only spend spins of base val is non-zero
        //     projectilePrefab.MaxRicochet = ricochetBaseVal * currentBikeSpins;
        //     print($"Ricochets: {projectilePrefab.MaxRicochet} ({ricochetBaseVal} * {currentBikeSpins})");
        //     A_SpinsSpent?.Invoke(currentBikeSpins, 1);
        //     currentBikeSpins = 1;
        // } else {
        //     projectilePrefab.MaxRicochet = 0;
        // }
        // NOTE: Ignoring special shot stuff since it's weird to figure out. Just going to have bike spins be the ricochet count
        projectilePrefab.MaxRicochet = currentBikeSpins;
        if (currentBikeSpins > 0) {
            A_SpinsSpent?.Invoke(currentBikeSpins, 0);
            currentBikeSpins = 0;
        }
        ProjectileBase proj = Instantiate(projectilePrefab, cannonProjSpawnTrans.position, Quaternion.LookRotation(projVel));
        proj.rb.velocity = projVel;
        SpendFuel(CannonFuelCost);
        GameManager.Instance.Audio2D.PlayClipSFX(AudioPlayer2D.EClipSFX.Weapon_CannonShot);
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
    
    void updateCrosshairPositions() {
        Vector3 rbVelocityCompensation = rb.velocity.sqrMagnitude > 0.001f ? rb.velocity * InheritedVelocityFactor / CannonBaseProjSpeed : Vector3.zero;
        // The vacuum does not account for the player's velocity
        Vector3 screenPointVacuum;
        if (!isInThirdPerson) {
            screenPointVacuum = mainCamera.WorldToScreenPoint(camtrans.position + charPivot.forward);
        } else {
            screenPointVacuum = mainCamera.WorldToScreenPoint(charModel.position + charPivot.forward * 1.5f);
        }
        // The cannon does account for the player's velocity
        Vector3 screenPointCannon;
        if (!isInThirdPerson) {
            screenPointCannon = mainCamera.WorldToScreenPoint(camtrans.position - charPivot.forward + rbVelocityCompensation);
        } else {
            updateRayCastedAimPoint();
            screenPointCannon = mainCamera.WorldToScreenPoint(
                aimPoint + rbVelocityCompensation * (aimPoint - mainCamera.transform.position).magnitude
            );
            screenPointCannon.z *= -1;
        }
        
        _gamePanel.UpdateCrosshairPositions(screenPointVacuum, screenPointCannon);
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
        currentMuzzleFlashEffect = Instantiate(muzzleFlashEffect, cannonMuzzleTrans);
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
         * OnDestroy() is still only called the frame right after.
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
            inputActions.Cannon.Enable();
            
            inputActions.TurnInputs.performed += TurnInputChanged;
            inputActions.TurnInputs.canceled += TurnInputChanged;
            inputActions.VertInputs.started += VertInputChanged;
            inputActions.VertInputs.canceled += VertInputChanged;
            
            inputActions.Vacuum.started += FireVacuumStarted;
            inputActions.Vacuum.canceled += FireVacuumCanceled;
            inputActions.Cannon.started += FireCannonStarted;
            inputActions.Cannon.canceled += FireCannonCanceled;
            
            
            /** Features not necessarily meant for final gameplay **/
            inputActions.SlowTime.Enable();
            inputActions.SlowTime.started += OnTimeSlowStarted;
            inputActions.SlowTime.canceled += OnTimeSlowCanceled;
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
            inputActions._AddRicCharges.Enable();
            inputActions._AddRicCharges.started += On_AddRicCharges;
        } else {
            inputActions.Look.Disable();
            inputActions.TurnInputs.Disable();
            inputActions.VertInputs.Disable();
            inputActions.Vacuum.Disable();
            inputActions.Cannon.Disable();
            
            inputActions.TurnInputs.performed -= TurnInputChanged;
            inputActions.TurnInputs.canceled -= TurnInputChanged;
            inputActions.VertInputs.started -= VertInputChanged;
            inputActions.VertInputs.canceled -= VertInputChanged;
            
            inputActions.Vacuum.started -= FireVacuumStarted;
            inputActions.Vacuum.canceled -= FireVacuumCanceled;
            inputActions.Cannon.started -= FireCannonStarted;
            inputActions.Cannon.canceled -= FireCannonCanceled;
            
            if (isVacuumOn) {
                isVacuumOn = false;
                Vacuum.DisableVacuum();
#if UNITY_EDITOR || KEEP_DEBUG
                _gamePanel.OnFireVacuum(false);
#endif
            }
            
            
            /** Features not necessarily meant for final gameplay **/
            inputActions.SlowTime.Disable();
            inputActions.SlowTime.started -= OnTimeSlowStarted;
            inputActions.SlowTime.canceled -= OnTimeSlowCanceled;
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
            inputActions._AddRicCharges.Disable();
            inputActions._AddRicCharges.started -= On_AddRicCharges;
        }
    }
    
    
    
    
    
    
    
    /*****  Probably temporary stuff  *****/
    
    void OnTimeSlowStarted(InputAction.CallbackContext context) {
        GameManager.Instance.SetPreferredTimeScale(0.1f);
    }
    
    void OnTimeSlowCanceled(InputAction.CallbackContext context) {
        GameManager.Instance.SetPreferredTimeScale(1);
    }
    
    void On_ToggleThirdPerson(InputAction.CallbackContext context) {
        isInThirdPerson = !isInThirdPerson;
    }
    
    void On_AddFuelKey(InputAction.CallbackContext context) {
        print("Fully refueling fuel (F cheat key).");
        AddFuel(MaxFuel);
    }
    
    void On_ToggleMirrorInput(InputAction.CallbackContext context) {
        mirrorModelEnabled = !mirrorModelEnabled;
        if (!mirrorModelEnabled) {
            rearMirrorModel.SetActive(false);
            rearCamera.gameObject.SetActive(false);
        } else {
            rearCamera.gameObject.SetActive(true);
            updateMirrorActive();
        }
    }
    
    void updateMirrorActive() {
        if (rearMirrorModel.activeSelf) {
            if (weaponRelativeRot.z < 0)
                rearMirrorModel.SetActive(false);
        } else {
            if (weaponRelativeRot.z >= 0)
                rearMirrorModel.SetActive(true);
        }
    }
    
    void On_TakeDamage(InputAction.CallbackContext context) {
        float DamageAmount = 10;
        print("Damaging player for " + DamageAmount + " damage.");
        TakeDamage(DamageAmount, EDamageType.Any);
    }
    
    void On_HealHealth(InputAction.CallbackContext context) {
        float HealAmount = 50;
        print("Healing player for " + HealAmount + " health.");
        HealHealth(HealAmount);
    }
    
    void On_AddRicCharges(InputAction.CallbackContext context) {
        int numCharges = 5;
        print($"Adding {numCharges} ricochet charges (from T key).");
        currentBikeSpins += numCharges;
        A_SpinCompleted?.Invoke(currentBikeSpins);
    }
    
}