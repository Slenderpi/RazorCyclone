// UNCOMMENT THE LINE BELOW FOR DEBUGGING PROJECTILE RAYCASTS
// #define DEBUG_RAYS

using System.Collections;
using UnityEngine;

public class ProjectileBase : MonoBehaviour {
    
    [Header("Projectile Config")]
    public ProjectileSO ProjConfig;
    [Tooltip("Maximum number of times this projectile can ricochet.\nA value of 0 means NO ricochet.")]
    public int MaxRicochet = 1;
    int ricRemain;
    bool couldNotShowRicEffect = false; // Occurs when there is an extreme number of ricochets and a low framerate
    protected GameObject impeffect; // Pre-spawned impact effect
    protected GameObject[] riceffects;
    
    [Header("References")]
    [SerializeField]
    GameObject ProjectileModel;
    [SerializeField]
    TrailRenderer TrailRicochet;
    [SerializeField]
    TrailRenderer TrailNormal;
    
    [HideInInspector]
    public Rigidbody rb;
    EnemyBase enemyToIgnore = null; // Most recent enemy that was hit for ricocheting
    Collider colliderToIgnore = null; // Most recent non-enemy collider that was hit for ricocheting
    LayerMask layerMask;
    
    bool projectileDead = false;
    int hitCase = 0; // 0 nothing, 1 enemy, 2 non enemy
    protected RaycastHit closestHit;
    float closestHitSqrd = float.MaxValue;
    
    WaitForFixedUpdate waitFixedUpdForRic;
    
    
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
        layerMask = (1 << LayerMask.NameToLayer("Default")) |
                    (1 << LayerMask.NameToLayer("EnemyHitbox"));
        waitFixedUpdForRic = new WaitForFixedUpdate();
        TrailNormal.emitting = false;
        TrailRicochet.emitting = false;
        StartCoroutine(poolVFX());
        Init();
    }
    
    void Start() {
        ricRemain = MaxRicochet;
        if (ricRemain > 0)
            TrailRicochet.emitting = true;
        else
            TrailNormal.emitting = true;
        StartCoroutine(ProjectileLifetime());
    }
    
    /// <summary>
    /// Called by Awake().
    /// </summary>
    protected virtual void Init() {}
    
    void FixedUpdate() {
        if (projectileDead) return;
        checkForCollisions();
    }
    
    void checkForCollisions() {
#if DEBUG_RAYS
        GameManager.D_DrawPoint(transform.position, Color.green);
#endif
        float dist = rb.velocity.magnitude * Time.fixedDeltaTime;
        hitCase = 0;
        closestHitSqrd = float.MaxValue;
        raycastCollision(Vector3.zero, dist);
        if (ProjConfig.ProjectileRadius >= 0.1f) {
            raycastCollision(transform.right * ProjConfig.ProjectileRadius, dist);
            raycastCollision(-transform.right * ProjConfig.ProjectileRadius, dist);
            raycastCollision(transform.up * ProjConfig.ProjectileRadius, dist);
            raycastCollision(-transform.up * ProjConfig.ProjectileRadius, dist);
        }
        if (hitCase > 0) {
            onHitSomething();
        }
    }
    
    void raycastCollision(Vector3 offset, float dist) {
#if DEBUG_RAYS
        Debug.DrawRay(transform.position + offset, rb.velocity * Time.fixedDeltaTime, Color.white, Time.fixedDeltaTime, false);
#endif
        if (Physics.Raycast(
            origin: transform.position + offset,
            direction: rb.velocity,
            maxDistance: dist,
            layerMask: layerMask,
            hitInfo: out RaycastHit hit)) {
            if (!hit.collider.CompareTag("Enemy")) {
                // We've hit a non-enemy. If a previous cast hit an enemy, return early
                if (hitCase == 1)
                    return; // A previous cast hit an enemy and this one didn't. Prioritize enemy
                else
                    hitCase = 2; // We've hit a non-enemy
            } else
                hitCase = 1; // This was the first raycast to hit an enemy
            // Check if the new hit is closer
            float hitDistSqrd = (hit.point - transform.position).sqrMagnitude;
            if (hitDistSqrd < closestHitSqrd) {
                closestHitSqrd = hitDistSqrd;
                closestHit = hit;
            }
        }
    }
    
    // This function allows for a projectile's hitbox to be either a collider or a trigger
    void onHitSomething() {
        Collider hitCollider = closestHit.collider;
        if (!hitCollider.CompareTag("Player") &&
            !hitCollider.CompareTag("Projectile") &&
            !hitCollider.CompareTag("Pickup")) {
            EnemyBase enemy = null;
            if (hitCase == 1) { // Hit an enemy
                enemy = hitCollider.transform.parent.parent.GetComponent<EnemyBase>();
                if (enemy == enemyToIgnore) return;
            } else {
                if (hitCollider == colliderToIgnore) return;
            }
            transform.position += closestHit.distance * rb.velocity.normalized;
            if (ricRemain > 0) {
                ricRemain--;
                if (enemy)
                    OnRicochetEnemy(enemy);
                else
                    OnRicochetNonEnemy();
            } else {
                if (enemy)
                    OnHitEnemy(enemy);
                else
                    OnHitNonEnemy();
                endProjectile();
            }
        }
    }
    
    /// <summary>
    /// Called after this projectile has hit an enemy. By default, this method
    /// calls TakeDamage() on the enemy.
    /// After calling OnHitEnemy(), the projectile will be destroyed.
    /// </summary>
    /// <param name="enemy">The enemy that the projectile hit.</param>
    protected virtual void OnHitEnemy(EnemyBase enemy) {
        enemy.TakeDamage(100, EDamageType.Projectile);
        showImpactEffect();
    }
    
    /// <summary>
    /// Called after this projectile has hit something that wasn't an enemy. By default,
    /// this method does nothing. After calling OnHitNonEnemy(), this projectile
    /// will be destroyed.
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnHitNonEnemy() {
        showImpactEffect();
    }
    
    protected virtual void OnRicochetEnemy(EnemyBase enemy) {
        enemyToIgnore = enemy;
        colliderToIgnore = null;
        EnemyBase closestEn = GameManager.Instance.currentSceneRunner.GetClosestEnemy(transform.position, enemyToIgnore);
        Vector3 ricVel;
#if DEBUG_RAYS
        if (closestEn && closestEn.rb) {
            Vector3 predPos = BoidSteerer.PredictPosition(transform.position, closestEn.TransformForRicochetToAimAt.position, rb.velocity, closestEn.rb.velocity);
            GameManager.D_DrawPoint(closestEn.TransformForRicochetToAimAt.position, Color.cyan, 1000);
            // GameManager.D_DrawPoint(predPos, Color.green, 1000);
            GameManager.D_DrawPoint(transform.position, Color.white, 1000);
            Debug.DrawRay(transform.position, predPos - transform.position, Color.magenta, 1000, false);
        }
#endif
        if (closestEn) {
            if (closestEn.rb) // If has enemy has an rb, use predictive aiming
                ricVel = (BoidSteerer.PredictPosition(
                            transform.position, closestEn.TransformForRicochetToAimAt.position, rb.velocity, closestEn.rb.velocity
                        ) - transform.position).normalized * rb.velocity.magnitude;
            else // Enemy has no rb, so use their current position
                ricVel = (closestEn.TransformForRicochetToAimAt.position - transform.position).normalized * rb.velocity.magnitude;
        } else // No valid enemy to ricochet to. Reflect physically
            ricVel = Vector3.Reflect(rb.velocity, closestHit.normal);
        rb.velocity *= 0;
        if (ricRemain == 0) {
            TrailRicochet.emitting = false;
            TrailNormal.emitting = true;
        }
        StartCoroutine(ricochetVelNextFrame(ricVel));
        enemy.TakeDamage(100, EDamageType.Projectile);
        showRicochetEffect();
    }
    
    protected void OnRicochetNonEnemy() {
        colliderToIgnore = closestHit.collider;
        enemyToIgnore = null;
        Vector3 ricVel = Vector3.Reflect(rb.velocity, closestHit.normal);
        rb.velocity *= 0;
        if (ricRemain == 0) {
            TrailRicochet.emitting = false;
            TrailNormal.emitting = true;
        }
        StartCoroutine(ricochetVelNextFrame(ricVel));
        showRicochetEffect();
    }
    
    IEnumerator ricochetVelNextFrame(Vector3 ricVel) {
        yield return waitFixedUpdForRic;
        rb.velocity = ricVel;
        transform.rotation = Quaternion.LookRotation(ricVel);
    }
    
    /// <summary>
    /// Called if the projectile lives up to its MaxLifetime without collision. By default,
    /// this method destroys the projectile.
    /// </summary>
    protected virtual void OnProjectileLifetimeExpired() {
        if (projectileDead) return;
        endProjectile();
    }
    
    void endProjectile() {
        projectileDead = true;
        rb.velocity *= 0;
        if (ricRemain > 0)
            TrailRicochet.emitting = false;
        else
            TrailNormal.emitting = false;
        ProjectileModel.SetActive(false);
        showImpactEffect();
        Destroy(gameObject, 4);
    }
    
    void OnDestroy() {
        if (couldNotShowRicEffect) {
            // Go through every single ric effect and try to delete it to assure complete deletion
            for (int i = 0; i < MaxRicochet; i++)
                if (riceffects[i])
                    Destroy(riceffects[i]);
        } else {
            for (int i = MaxRicochet - ricRemain; i < MaxRicochet; i++)
                Destroy(riceffects[i]);
        }
    }
    
    void showImpactEffect() {
        impeffect.transform.SetPositionAndRotation(transform.position, transform.rotation);
        impeffect.SetActive(true);
    }
    
    void showRicochetEffect() {
        GameObject re = riceffects[MaxRicochet - ricRemain - 1]; // ricRemain has already been reduced so treat as (ricRemain + 1)
        if (!re) {
            couldNotShowRicEffect = true;
            return;
        }
        re.transform.SetPositionAndRotation(transform.position, transform.rotation);
        re.SetActive(true);
    }
    
    IEnumerator ProjectileLifetime() {
        yield return new WaitForSeconds(Mathf.Min(ProjConfig.LifetimePerRicochetAdd1 * (MaxRicochet + 1), ProjConfig.MaxLifetime));
        OnProjectileLifetimeExpired();
    }
    
    IEnumerator poolVFX() {
        impeffect = Instantiate(ProjConfig.ImpactEffect);
        impeffect.SetActive(false);
        riceffects = new GameObject[MaxRicochet];
        int counter = 0;
        for (int i = 0; i < MaxRicochet; i++) {
            riceffects[i] = Instantiate(ProjConfig.RicochetEffect);
            riceffects[i].SetActive(false);
            if (++counter >= 10) {
                counter = 0;
                yield return null;
            }
        }
    }
    
}