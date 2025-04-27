using UnityEngine;

public class LaserEnemy : EnemyBase {
    
    [Header("Laser Enemy Config")]
    public LaserEnemySO LaserConfig;
    // [Tooltip("In degrees/sec.")]
    // public float CooldownRotSpeed = 30;
    // [Tooltip("In degrees/sec.")]
    // public float WindupRotSpeed = 20;
    // [Tooltip("In degrees/sec.")]
    // public float AttackRotSpeed = 10;
    // [SerializeField]
    // [Tooltip("Maximum distance (inclusive) from the Player in Cooldown phase. Outside of this distance, the Laser enemy will be AFK.")]
    // float CooldownMaxDist = 50;
    // [SerializeField]
    // [Tooltip("Maximum distance (inclusive) from the Player for both Windup and Attack. Outside of this distance, the Laser enemy will switch to Cooldown.\n\nAlso determines max length of the laser beam.")]
    // float WindupAndAttackMaxDist = 30;
    // [SerializeField]
    // [Tooltip("Duration for Laser to be in its Cooldown phase until it moves onto the Windup phase.")]
    // float CooldownDuration = 2;
    // [SerializeField]
    // [Tooltip("Duration for Laser to be in its Windup phase until it moves onto the Attack phase.")]
    // float WindupDuration = 3f;
    // [SerializeField]
    // [Tooltip("Duration for Laser to be in its Attack phase until it moves onto the Cooldown phase.")]
    // float AttackDuration = 3f;
    
    // [SerializeField]
    // [Tooltip("Duration the Laser enemy will be in its weak damage state with damage set to WeakDamagePerSecond.\nAfter this duration, its damage will increase to StrongDamagePerSecond.")]
    // float WeakDamageDuration = 5;
    // [SerializeField]
    // [Tooltip("Duration the Laser enemy will be stunned for after getting hit by a cannon shot. Once the stun is over, it will attack the player with weak damage.")]
    // float StunDuration = 3;
    // [SerializeField]
    // [Tooltip("Before the end of the stun, the Laser enemy will animate itself rotating towards the player. This value determines the length of the animation, but does not affect the stun duration.\nThis means that if StunDuration = 3 and ReArmDuration = 1, the re-arm phase will start 2 seconds into the stun.")]
    // float ReArmDuration = 1;
    // [SerializeField]
    // [Tooltip("Weak damage value.")]
    // float WeakDamagePerSecond = 10;
    // [SerializeField]
    // [Tooltip("Strong damave value.")]
    // float StrongDamagePerSecond = 30;
    
    // [SerializeField]
    // [Tooltip("Color of the laser when in the Windup state.")]
    // Gradient LaserColorWindup;
    // [SerializeField]
    // [Tooltip("Color of the laser when in the Attack state.")]
    // Gradient LaserColorAttack;
    
    [SerializeField]
    [Tooltip("Reference to the laser's line renderer.")]
    LineRenderer LaserLineRenderer;
    [SerializeField]
    [Tooltip("Endpoint of the laser whose children are game objects with particle effects at the laser's endpoint.")]
    Transform LaserEndpoint;
    
    // [SerializeField]
    // [Tooltip("Laser color when damage is weak.")]
    // Gradient WeakColor;
    // [SerializeField]
    // [Tooltip("Laser colro when damage is strong.")]
    // Gradient StrongColor;
    
    [SerializeField]
    [Tooltip("Reference to the transform the turret will revolve (yaw) around.")]
    Transform revolvePivot;
    [Tooltip("Reference to the transform the turret's barrel will rotate (pitch) around.")]
    [SerializeField]
    Transform barrelPivot;
    [SerializeField]
    Transform bodyPivot;
    
    [Header("Crab Audio")]
    [SerializeField]
    AudioSource StunAudio;
    
    // 0 weak | 1 strong | 2 stunned | 3 transition from stunned to weak | 4 no LOS
    float state = 3;
    float lastStateChangeTime;
    
    // LayerMask laserRayMaskWindup; // 1 = layer to be included in raycast
    // LayerMask laserRayMaskAttack;
    // float laserRaycastDist;
    ParticleSystem[] laserPointParticles;
    LayerMask laserMask; // 1 = layer to be included in raycast
    bool isPlayerInLOS = false;
    Vector3 laserHitPos;
    float currDmg;
    
    // delegate void StateFunc();
    // StateFunc currStateFunc;
    // // Tracks the time the current state was entered. This same variable can be used for all the states
    // float lastStateEnterTime = -1000;
    // float currRotRate;
    
    
    
    protected override void Init() {
        laserPointParticles = LaserEndpoint.GetComponentsInChildren<ParticleSystem>();
        if (state > 1) {
            setLaserRenderEnabled(false);
            setLaserPointVFXEnabled(false);
        }
        // laserRayMaskWindup = 1 << LayerMask.NameToLayer("Default");
        // laserRayMaskAttack = laserRayMaskWindup | (1 << LayerMask.NameToLayer("Player"));
        // enterStateCooldown();
        currDmg = LaserConfig.WeakDamagePerSecond;
        LaserLineRenderer.colorGradient = LaserConfig.WeakColor;
        laserMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));
    }
    
    void Update() {
        if (GameManager.CurrentPlayer) {
            // rotateTowardsPlayer(GameManager.CurrentPlayer.transform.position, currRotRate);
            Vector3 toPlayer = GameManager.CurrentPlayer.transform.position - barrelPivot.position;
            switch (state) {
            case 0: // Weak damage
                pointAtPlayer(toPlayer);
                pointBodyAtPlayer(toPlayer);
                if (!LaserLineRenderer.enabled) {
                    if (fireLaser())
                        setLaserAll(true);
                }
                if (Time.time - lastStateChangeTime >= LaserConfig.WeakDamageDuration) {
                    state = 1;
                    lastStateChangeTime += LaserConfig.WeakDamageDuration;
                    currDmg = LaserConfig.StrongDamagePerSecond;
                    LaserLineRenderer.colorGradient = LaserConfig.StrongColor;
                }
                break;
            case 1: // Strong damage
                pointAtPlayer(toPlayer);
                pointBodyAtPlayer(toPlayer);
                break;
            case 2: // Stunned
                if (Time.time - lastStateChangeTime >= LaserConfig.StunDuration - LaserConfig.ReArmDuration) {
                    state = 3;
                    lastStateChangeTime += LaserConfig.StunDuration - LaserConfig.ReArmDuration;
                }
                break;
            case 3: // Re-arming
                if (Time.time - lastStateChangeTime >= LaserConfig.ReArmDuration) {
                    state = 0;
                    lastStateChangeTime += LaserConfig.ReArmDuration;
                    currDmg = LaserConfig.WeakDamagePerSecond;
                    LaserLineRenderer.colorGradient = LaserConfig.WeakColor;
                    pointAtPlayer(toPlayer);
                    pointBodyAtPlayer(toPlayer);
                } else {
                    // Slerp rot to player
                    slerpToPlayer();
                }
                break;
            case 4: // No LOS
                break;
            }
        }
    }
    
    protected override void onFixedUpdate() {
        base.onFixedUpdate();
        if (GameManager.CurrentPlayer) {
            // currStateFunc();
            if (state < 2) {
                if (fireLaser()) {
                    GameManager.CurrentPlayer.TakeDamage(currDmg * Time.fixedDeltaTime, EDamageType.Enemy);
                } else {
                    // Go to no LOS state
                    state = 4;
                    setLaserAll(false);
                    lastStateChangeTime = Time.fixedTime;
                }
            } else if (state == 4) {
                if (fireLaser()) {
                    // Go to re-arm state
                    state = 3;
                    lastStateChangeTime = Time.fixedTime;
                }
            }
        }
    }
    
    protected override void OnTakeDamage(float amnt, EDamageType damageType) {
        if (damageType == EDamageType.Vacuum) {
            base.OnTakeDamage(amnt, damageType);
        } else if (damageType == EDamageType.Projectile || damageType == EDamageType.ProjectileRicochet) {
            onStunned();
        }
    }
        
    // /*
    // - Cooldown:
    //     - Look towards player (cldRotSpeed)
    //     - if dist < medDist
    //         if cooldownDuration seconds has passed
    //             - Enter Windup
    //     - else
    //         - reset timer cooldownDuration
    // */
    // void stateCooldown() {
    //     if (stateDistCheck(GameManager.CurrentPlayer.transform.position, CooldownMaxDist)) {
    //         if (Time.time - lastStateEnterTime > CooldownDuration) {
    //             enterStateWindup();
    //         }
    //     } else {
    //         lastStateEnterTime = Time.time;
    //     }
    // }
    
    // void enterStateCooldown() {
    //     // print("Entered Cooldown");
    //     lastStateEnterTime = Time.time;
    //     currStateFunc = stateCooldown;
    //     currRotRate = CooldownRotSpeed;
    //     setLaserRenderEnabled(false);
    //     setLaserPointVFXEnabled(false);
    // }
    
    // /*
    // - Windup:
    //     - Look towards player (windupRotSpeed)
    //     - Render laser as white line
    //     - if dist > MaxLaserDist, go back to Cooldown
    //     - if windupDuration seconds passes, enter Attack
    // */
    // void stateWindup() {
    //     updateLaserLine(true);
    //     if (stateDistCheck(GameManager.CurrentPlayer.transform.position, WindupAndAttackMaxDist)) {
    //         if (Time.time - lastStateEnterTime > WindupDuration) {
    //             enterStateAttack();
    //         }
    //     } else {
    //         enterStateCooldown();
    //     }
    // }
    
    // void enterStateWindup() {
    //     // print("Entered Windup");
    //     lastStateEnterTime = Time.time;
    //     currStateFunc = stateWindup;
    //     LaserLineRenderer.colorGradient = LaserColorWindup;
    //     currRotRate = WindupRotSpeed;
    //     // Turn laser line on. Assume laser point already off
    //     setLaserRenderEnabled(true);
    // }
    
    // /*
    // - Attack:
    //     - Lasts attackDuration seconds. Returns to Cooldown when done
    //     - Look towards player (attackRotSpeed)
    //     - Laser is laser-colored
    //     - If laser hits player, deal continuous damage over time while hitting
    // */
    // void stateAttack() {
    //     PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
    //     if (!stateDistCheck(plr.transform.position, WindupAndAttackMaxDist) || Time.time - lastStateEnterTime > AttackDuration) {
    //         enterStateCooldown();
    //         return;
    //     }
    //     if (updateLaserLine(false))
    //         plr.TakeDamage(Damage * Time.deltaTime, EDamageType.Enemy); // ASSUMES DAMAGE IS DAMAGE PER SEC
    // }
    
    // void enterStateAttack() {
    //     // print("Entered Attack");
    //     lastStateEnterTime = Time.time;
    //     currStateFunc = stateAttack;
    //     LaserLineRenderer.colorGradient = LaserColorAttack;
    //     currRotRate = AttackRotSpeed;
    //     // Assume laser VFX already enabled so only turn on laser point VFX
    //     setLaserPointVFXEnabled(true);
    // }
    
    // bool stateDistCheck(Vector3 plrPos, float maxDist) {
    //     return (plrPos - transform.position).sqrMagnitude <= maxDist * maxDist;
    // }
    
    // void rotateTowardsPlayer(Vector3 plrPos, float rotationSpeed) {
    //     transform.rotation = Quaternion.RotateTowards(
    //         LaserLineRenderer.transform.rotation,
    //         Quaternion.LookRotation(plrPos - LaserLineRenderer.transform.position, Vector3.up),
    //         rotationSpeed * Time.deltaTime
    //     );
    // }
    
    // bool updateLaserLine(bool useWindupMask) {
    //     Ray ray = new(LaserLineRenderer.transform.position, LaserLineRenderer.transform.forward);
    //     bool laserHitPlayer = false;
    //     if (Physics.Raycast(
    //         ray: ray,
    //         maxDistance: WindupAndAttackMaxDist,
    //         layerMask: useWindupMask ? laserRayMaskWindup : laserRayMaskAttack,
    //         hitInfo: out RaycastHit hit)) {
    //         laserRaycastDist = hit.distance;
    //         if (!useWindupMask)
    //             laserHitPlayer = hit.collider.CompareTag("Player");
    //     } else {
    //         laserRaycastDist = WindupAndAttackMaxDist;
    //     }
    //     LaserEndpoint.localPosition = new(0, 0, laserRaycastDist);
    //     LaserLineRenderer.SetPosition(1, new(0, 0, laserRaycastDist));
    //     return laserHitPlayer;
    // }
    
    void OnEnable() {
        lastStateChangeTime = Time.time;
    }
    
    void pointAtPlayer(Vector3 toPlayer) {
        Vector3 flat = toPlayer;
        flat.y = 0;
        revolvePivot.rotation = Quaternion.LookRotation(flat);
        barrelPivot.rotation = Quaternion.LookRotation(toPlayer);
        LaserLineRenderer.SetPosition(0, LaserLineRenderer.transform.position);
        LaserEndpoint.position = isPlayerInLOS ? toPlayer.normalized * (toPlayer.magnitude - 0.5f) + barrelPivot.position : laserHitPos;
        LaserLineRenderer.SetPosition(1, LaserEndpoint.position);
    }
    
    void pointBodyAtPlayer(Vector3 toPlayer) {
        toPlayer.y = 0;
        bodyPivot.rotation = Quaternion.RotateTowards(
            bodyPivot.rotation,
            Quaternion.LookRotation(toPlayer),
            LaserConfig.MaxBodyRotPerSec * Time.deltaTime
        );
        Hitboxes.transform.rotation = bodyPivot.rotation;
    }
    
    void slerpToPlayer() {
        Vector3 toPlayer = GameManager.CurrentPlayer.transform.position - barrelPivot.position;
        float a = (Time.time - lastStateChangeTime) / LaserConfig.ReArmDuration;
        a = a * a * a * a * a;
        revolvePivot.rotation = Quaternion.Slerp(
            revolvePivot.rotation,
            Quaternion.LookRotation(new (toPlayer.x, 0, toPlayer.z)),
            a
        );
        // Project the direction onto the plane defined by revolvePivot's right vector
        Vector3 right = revolvePivot.right;
        Vector3 forward = Vector3.ProjectOnPlane(toPlayer.normalized, right).normalized;
        // Calculate the angle to rotate around revolvePivot's right vector
        float angle = Vector3.SignedAngle(revolvePivot.forward, forward, right);
        // Create the quaternion by rotating around revolvePivot's right axis
        Quaternion rotation = Quaternion.AngleAxis(angle, right) * revolvePivot.rotation;
        barrelPivot.rotation = Quaternion.Slerp(barrelPivot.rotation, rotation, a);
    }
    
    bool fireLaser() {
        Ray ray = new(barrelPivot.position, GameManager.CurrentPlayer.transform.position - barrelPivot.position);
        if (Physics.Raycast(ray: ray, maxDistance: 500, layerMask: laserMask, hitInfo: out RaycastHit hit)) {
            laserHitPos = hit.point;
            isPlayerInLOS = hit.collider.CompareTag("Player");
        } else
            isPlayerInLOS = false;
        return isPlayerInLOS;
    }
    
    void onStunned() {
        // enterStateCooldown();
        state = 2;
        lastStateChangeTime = Time.time;
        setLaserAll(false);
        StunAudio.Play();
    }
    
    void setLaserRenderEnabled(bool newEnabled) {
        LaserLineRenderer.enabled = newEnabled;
    }
    
    void setLaserPointVFXEnabled(bool newEnabled) {
        foreach (ParticleSystem p in laserPointParticles)
            p.gameObject.SetActive(newEnabled);
    }
    
    void setLaserAll(bool newEnabled) {
        setLaserRenderEnabled(newEnabled);
        setLaserPointVFXEnabled(newEnabled);
    }
    
}
