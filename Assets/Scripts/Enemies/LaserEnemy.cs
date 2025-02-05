using UnityEngine;

public class LaserEnemy : EnemyBase {
    
    [Header("Laser Enemy Config")]
    // public float MaxLaserRange = 30;
    // public float laserDuration = 3f;
    // public float pauseBeforeFiring = 0.3f;
    [Tooltip("TEMPORARY")]
    public float rotationSpeed = 2f;
    // [Tooltip("NOT YET IMPLEMENTED")] // TODO
    // public float LaserDamagePerSecond = 10f;
    [SerializeField]
    [Tooltip("Maximum distance (inclusive) from the Player in Cooldown phase. Outside of this distance, the Laser enemy will be AFK.")]
    float MaxDistCooldown = 50;
    [SerializeField]
    [Tooltip("Maximum distance (inclusive) from the Player for both Windup and Attack. Outside of this distance, the Laser enemy will switch to Cooldown.\n\nAlso determines max length of the laser beam.")]
    float MaxDistWindupAndAttack = 30;
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
    // Tracks the time the current state was entered. This same variable can be used for all the states
    float lastStateEnterTime = -1000;
    
    [SerializeField]
    [Tooltip("Reference to the laser's line renderer.")]
    LineRenderer LaserLineRenderer;
    [SerializeField]
    [Tooltip("Endpoint of the laser whose children are game objects with particle effects at the laser's endpoint.")]
    Transform LaserEndpoint;
    // bool isAttacking = false;
    // bool isLaserActive = false;
    // float attackTimer; //
    LayerMask laserRayMask; // 1 = layer to be included in raycast
    LayerMask laserRayMaskWithoutPlr;
    float laserRaycastDist;
    ParticleSystem[] laserPointParticles;
    
    delegate void StateFunc();
    StateFunc currStateFunc;
    
    
    
    protected override void Init() {
        // attackTimer = Time.time - AttackDuration; //
        DealDamageOnTouch = false;
        laserPointParticles = LaserEndpoint.GetComponentsInChildren<ParticleSystem>();
        // setLaserVFXEnabled(false);
        setLaserRenderEnabled(false);
        setLaserPointVFXEnabled(false);
        laserRayMask = ~(
            (1 << LayerMask.NameToLayer("Enemy")) |
            (1 << LayerMask.NameToLayer("Projectile")) |
            (1 << LayerMask.NameToLayer("Weapon")) |
            (1 << LayerMask.NameToLayer("Pickup"))
        );
        laserRayMaskWithoutPlr = laserRayMask & ~(1 << LayerMask.NameToLayer("Player"));
        enterStateCooldown();
    }
    
    void FixedUpdate() {
        if (GameManager.CurrentPlayer)
            currStateFunc();
        
        // attackTimer -= Time.fixedDeltaTime;
        
        // if (!isAttacking && attackTimer <= 0f) {
        //     // Debug.Log("performing attack");
        //     StartCoroutine(performAttack());
        //     attackTimer = attackCooldown;
        // }
        
        // if (isLaserActive) {
        //     rotateTowardsPlayer();
        // }
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
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        // RotateToPlayer(stateRotSpeed)
        rotateTowardsPlayer();
        if (stateDistCheck(plr.transform.position, MaxDistCooldown)) {
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
        // setLaserVFXEnabled(false);
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
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        // RotateToPlayer(stateRotSpeed)
        rotateTowardsPlayer();
        updateLaserLine(false);
        if (stateDistCheck(plr.transform.position, MaxDistWindupAndAttack)) {
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
        // setLaserVFXEnabled(true);
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
        // RotateToPlayer(stateRotSpeed)
        rotateTowardsPlayer();
        if (updateLaserLine(true))
            GameManager.CurrentPlayer.TakeDamage(Damage * Time.fixedDeltaTime); // ASSUMES DAMAGE IS DAMAGE PER SEC
        if (!stateDistCheck(plr.transform.position, MaxDistWindupAndAttack) || Time.time - lastStateEnterTime > AttackDuration) {
            enterStateCooldown();
            return;
        }
    }
    
    void enterStateAttack() {
        // print("Entered Attack");
        lastStateEnterTime = Time.time;
        currStateFunc = stateAttack;
        LaserLineRenderer.colorGradient = LaserColorAttack;
        // Assume laser VFX already enabled so only turn on laser point VFX
        setLaserPointVFXEnabled(true);
    }
    
    bool stateDistCheck(Vector3 plrPos, float maxDist) {
        return (plrPos - transform.position).sqrMagnitude <= maxDist * maxDist;
    }
    
    void rotateTowardsPlayer() {
        PlayerCharacterCtrlr player = GameManager.CurrentPlayer;
        if (player != null) {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }
    
    bool updateLaserLine(bool includePlr) {
        Ray ray = new(LaserLineRenderer.transform.position, LaserLineRenderer.transform.forward * MaxDistWindupAndAttack);
        // LaserEndpoint.position = ray.origin + ray.direction * MaxDistWindupAndAttack;
        bool hitPlayer = false;
        if (Physics.Raycast(
            ray: ray,
            maxDistance: MaxDistWindupAndAttack,
            layerMask: includePlr ? laserRayMask : laserRayMaskWithoutPlr,
            hitInfo: out RaycastHit hit)) {
            laserRaycastDist = hit.distance;
            if (includePlr)
                hitPlayer = hit.collider.CompareTag("Player");
        } else {
            laserRaycastDist = MaxDistWindupAndAttack;
        }
        LaserEndpoint.localPosition = new(0, 0, laserRaycastDist);
        LaserLineRenderer.SetPosition(1, new(0, 0, laserRaycastDist));
        return hitPlayer;
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
