using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class LaserEnemy : EnemyBase {
    
    [Header("Laser Enemy Config")]
    // public float MaxLaserRange = 30;
    // public float laserDuration = 3f;
    // public float pauseBeforeFiring = 0.3f;
    [Tooltip("TEMPORARY")]
    public float rotationSpeed = 2f;
    [Tooltip("NOT YET IMPLEMENTED")] // TODO
    public float LaserDamagePerSecond = 10f;
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
    LayerMask LaserRayMask;
    float laserRaycastDist;
    ParticleSystem[] laserPointParticles;
    
    delegate void StateFunc();
    StateFunc currStateFunc;
    
    
    
    protected override void Init() {
        // attackTimer = Time.time - AttackDuration; //
        DealDamageOnTouch = false;
        laserPointParticles = LaserEndpoint.GetComponentsInChildren<ParticleSystem>();
        setLaserVFXEnabled(false);
        LaserRayMask = ~(
            (1 << LayerMask.NameToLayer("Enemy")) |
            (1 << LayerMask.NameToLayer("Projectile")) |
            (1 << LayerMask.NameToLayer("Weapon")) |
            (1 << LayerMask.NameToLayer("Pickup"))
        );
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
        print("Entered Cooldown");
        lastStateEnterTime = Time.time;
        currStateFunc = stateCooldown;
        setLaserVFXEnabled(false);
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
        updateLaserLine();
        if (stateDistCheck(plr.transform.position, MaxDistWindupAndAttack)) {
            if (Time.time - lastStateEnterTime > WindupDuration) {
                enterStateAttack();
            }
        } else {
            enterStateCooldown();
        }
    }
    
    void enterStateWindup() {
        print("Entered Windup");
        lastStateEnterTime = Time.time;
        currStateFunc = stateWindup;
        // Setup for Windup
        LaserLineRenderer.colorGradient = LaserColorWindup;
        setLaserVFXEnabled(true);
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
        updateLaserLine();
        if (!stateDistCheck(plr.transform.position, MaxDistWindupAndAttack) || Time.time - lastStateEnterTime > AttackDuration) {
            enterStateCooldown();
            return;
        }
        // Attempt to damage player
    }
    
    void enterStateAttack() {
        print("Entered Attack");
        lastStateEnterTime = Time.time;
        currStateFunc = stateAttack;
        // Assume laser VFX already enabled so don't turn it on again
        LaserLineRenderer.colorGradient = LaserColorAttack;
    }
    
    bool stateDistCheck(Vector3 plrPos, float maxDist) {
        return (plrPos - transform.position).sqrMagnitude <= maxDist * maxDist;
    }

    // IEnumerator performAttack() {
    //     isAttacking = true;
        
    //     setLaserVFXEnabled(true);
    //     StartCoroutine(chargeLaser());
    //     yield return new WaitForSeconds(WindupDuration);
        
    //     setLaserVFXEnabled(false);
    //     yield return new WaitForSeconds(pauseBeforeFiring);
        
    //     isLaserActive = true;
    //     setLaserVFXEnabled(true);
    //     StartCoroutine(fireLaser());
    //     yield return new WaitForSeconds(laserDuration);
        
    //     isLaserActive = false;
    //     setLaserVFXEnabled(false);
    //     isAttacking = false;
    //     attackTimer = AttackDuration;
    // }
    
    // IEnumerator chargeLaser() {
    //     while (isAttacking && !isLaserActive) {
    //         rotateTowardsPlayer();
    //         updateLaserLine();
    //         yield return null;
    //     }
    // }
    
    void rotateTowardsPlayer() {
        PlayerCharacterCtrlr player = GameManager.CurrentPlayer;
        if (player != null) {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }
    
    void raycastLaser() {
        Ray ray = new(LaserLineRenderer.transform.position, LaserLineRenderer.transform.forward * MaxDistWindupAndAttack);
        Debug.DrawRay(ray.origin, ray.direction);
        LaserEndpoint.position = ray.origin + ray.direction * MaxDistWindupAndAttack;
        // Raycast() returns a bool
        if (Physics.Raycast(ray: ray, maxDistance: MaxDistWindupAndAttack, layerMask: LaserRayMask, hitInfo: out RaycastHit hit)) {
            laserRaycastDist = hit.distance;
        } else {
            laserRaycastDist = MaxDistWindupAndAttack;
        }
        LaserEndpoint.localPosition = new(0, 0, laserRaycastDist);
    }
    
    void updateLaserLine() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (plr) {
            raycastLaser();
            LaserLineRenderer.SetPosition(1, new(0, 0, laserRaycastDist));
        }
    }
    
    // IEnumerator fireLaser() {
    //     while (isLaserActive) {
    //         // Debug.Log("pow pow");
    //         yield return null;
    //     }
    // }
    
    void onStunned() {
        // TODO
        enterStateCooldown();
    }
    
    void setLaserVFXEnabled(bool newEnabled) {
        LaserLineRenderer.enabled = newEnabled;
        foreach (ParticleSystem p in laserPointParticles)
            p.gameObject.SetActive(newEnabled);
    }
    
}
