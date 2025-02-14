using UnityEngine;

public class LaserEnemy : EnemyBase {
    
    [Header("Laser Enemy Config")]
    [Tooltip("In degrees/sec.")]
    public float CooldownRotSpeed = 30;
    [Tooltip("In degrees/sec.")]
    public float WindupRotSpeed = 20;
    [Tooltip("In degrees/sec.")]
    public float AttackRotSpeed = 10;
    [SerializeField]
    [Tooltip("Maximum distance (inclusive) from the Player in Cooldown phase. Outside of this distance, the Laser enemy will be AFK.")]
    float CooldownMaxDist = 50;
    [SerializeField]
    [Tooltip("Maximum distance (inclusive) from the Player for both Windup and Attack. Outside of this distance, the Laser enemy will switch to Cooldown.\n\nAlso determines max length of the laser beam.")]
    float WindupAndAttackMaxDist = 30;
    [SerializeField]
    [Tooltip("Duration for Laser to be in its Cooldown phase until it moves onto the Windup phase.")]
    float CooldownDuration = 2;
    [SerializeField]
    [Tooltip("Duration for Laser to be in its Windup phase until it moves onto the Attack phase.")]
    float WindupDuration = 3f;
    [SerializeField]
    [Tooltip("Duration for Laser to be in its Attack phase until it moves onto the Cooldown phase.")]
    float AttackDuration = 3f;
    [SerializeField]
    [Tooltip("Color of the laser when in the Windup state.")]
    Gradient LaserColorWindup;
    [SerializeField]
    [Tooltip("Color of the laser when in the Attack state.")]
    Gradient LaserColorAttack;
    [SerializeField]
    [Tooltip("Reference to the laser's line renderer.")]
    LineRenderer LaserLineRenderer;
    [SerializeField]
    [Tooltip("Endpoint of the laser whose children are game objects with particle effects at the laser's endpoint.")]
    Transform LaserEndpoint;
    
    LayerMask laserRayMaskWindup; // 1 = layer to be included in raycast
    LayerMask laserRayMaskAttack;
    float laserRaycastDist;
    ParticleSystem[] laserPointParticles;
    
    delegate void StateFunc();
    StateFunc currStateFunc;
    // Tracks the time the current state was entered. This same variable can be used for all the states
    float lastStateEnterTime = -1000;
    float currRotRate;
    
    
    
    protected override void Init() {
        DealDamageOnTouch = false;
        laserPointParticles = LaserEndpoint.GetComponentsInChildren<ParticleSystem>();
        setLaserRenderEnabled(false);
        setLaserPointVFXEnabled(false);
        laserRayMaskWindup = 1 << LayerMask.NameToLayer("Default");
        laserRayMaskAttack = laserRayMaskWindup | (1 << LayerMask.NameToLayer("Player"));
        enterStateCooldown();
    }
    
    // NOTE: Rotation is done in Update() and state functions (including raycasts and damage dealing) are in FixedUpdate()
    
    void Update() {
        if (GameManager.CurrentPlayer) {
            rotateTowardsPlayer(GameManager.CurrentPlayer.transform.position, currRotRate);
        }
    }

    protected override void onFixedUpdate() {
        base.onFixedUpdate();
        if (GameManager.CurrentPlayer) {
            currStateFunc();
        }
    }

    public override void TakeDamage(float amnt, EDamageType damageType) {
        if (damageType == EDamageType.Vacuum) {
            base.TakeDamage(amnt, damageType);
        } else if (damageType == EDamageType.Projectile) {
            onStunned();
        }
    }
    
    /*
    - Cooldown:
        - Look towards player (cldRotSpeed)
        - if dist < medDist
            if cooldownDuration seconds has passed
                - Enter Windup
        - else
            - reset timer cooldownDuration
    */
    void stateCooldown() {
        if (stateDistCheck(GameManager.CurrentPlayer.transform.position, CooldownMaxDist)) {
            if (Time.time - lastStateEnterTime > CooldownDuration) {
                enterStateWindup();
            }
        } else {
            lastStateEnterTime = Time.time;
        }
    }
    
    void enterStateCooldown() {
        // print("Entered Cooldown");
        lastStateEnterTime = Time.time;
        currStateFunc = stateCooldown;
        currRotRate = CooldownRotSpeed;
        setLaserRenderEnabled(false);
        setLaserPointVFXEnabled(false);
    }
    
    /*
    - Windup:
        - Look towards player (windupRotSpeed)
        - Render laser as white line
        - if dist > MaxLaserDist, go back to Cooldown
        - if windupDuration seconds passes, enter Attack
    */
    void stateWindup() {
        updateLaserLine(true);
        if (stateDistCheck(GameManager.CurrentPlayer.transform.position, WindupAndAttackMaxDist)) {
            if (Time.time - lastStateEnterTime > WindupDuration) {
                enterStateAttack();
            }
        } else {
            enterStateCooldown();
        }
    }
    
    void enterStateWindup() {
        // print("Entered Windup");
        lastStateEnterTime = Time.time;
        currStateFunc = stateWindup;
        LaserLineRenderer.colorGradient = LaserColorWindup;
        currRotRate = WindupRotSpeed;
        // Turn laser line on. Assume laser point already off
        setLaserRenderEnabled(true);
    }
    
    /*
    - Attack:
        - Lasts attackDuration seconds. Returns to Cooldown when done
        - Look towards player (attackRotSpeed)
        - Laser is laser-colored
        - If laser hits player, deal continuous damage over time while hitting
    */
    void stateAttack() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!stateDistCheck(plr.transform.position, WindupAndAttackMaxDist) || Time.time - lastStateEnterTime > AttackDuration) {
            enterStateCooldown();
            return;
        }
        if (updateLaserLine(false))
            plr.TakeDamage(Damage * Time.deltaTime, EDamageType.Enemy); // ASSUMES DAMAGE IS DAMAGE PER SEC
    }
    
    void enterStateAttack() {
        // print("Entered Attack");
        lastStateEnterTime = Time.time;
        currStateFunc = stateAttack;
        LaserLineRenderer.colorGradient = LaserColorAttack;
        currRotRate = AttackRotSpeed;
        // Assume laser VFX already enabled so only turn on laser point VFX
        setLaserPointVFXEnabled(true);
    }
    
    bool stateDistCheck(Vector3 plrPos, float maxDist) {
        return (plrPos - transform.position).sqrMagnitude <= maxDist * maxDist;
    }
    
    void rotateTowardsPlayer(Vector3 plrPos, float rotationSpeed) {
        transform.rotation = Quaternion.RotateTowards(
            LaserLineRenderer.transform.rotation,
            Quaternion.LookRotation(plrPos - LaserLineRenderer.transform.position, Vector3.up),
            rotationSpeed * Time.deltaTime
        );
    }
    
    bool updateLaserLine(bool useWindupMask) {
        // NOTE: Consider using mask with player no matter what, and that in windup mode if player is hit just render laser at max dist anyway
        // NOTE cont.: Could be useful if making a system to warn the player they're in LOS of laser
        Ray ray = new(LaserLineRenderer.transform.position, LaserLineRenderer.transform.forward);
        bool laserHitPlayer = false;
        if (Physics.Raycast(
            ray: ray,
            maxDistance: WindupAndAttackMaxDist,
            layerMask: useWindupMask ? laserRayMaskWindup : laserRayMaskAttack,
            hitInfo: out RaycastHit hit)) {
            laserRaycastDist = hit.distance;
            if (!useWindupMask)
                laserHitPlayer = hit.collider.CompareTag("Player");
        } else {
            laserRaycastDist = WindupAndAttackMaxDist;
        }
        LaserEndpoint.localPosition = new(0, 0, laserRaycastDist);
        LaserLineRenderer.SetPosition(1, new(0, 0, laserRaycastDist));
        return laserHitPlayer;
    }
    
    void onStunned() {
        enterStateCooldown();
    }
    
    void setLaserRenderEnabled(bool newEnabled) {
        LaserLineRenderer.enabled = newEnabled;
    }
    
    void setLaserPointVFXEnabled(bool newEnabled) {
        foreach (ParticleSystem p in laserPointParticles)
            p.gameObject.SetActive(newEnabled);
    }
    
}
