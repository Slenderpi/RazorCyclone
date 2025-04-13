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
    TrailRenderer TrailRicochet;
    [SerializeField]
    TrailRenderer TrailNormal;
    
    [HideInInspector]
    public Rigidbody rb;
    EnemyBase enemyToIgnore = null; // Most recent enemy that was hit for ricocheting
    Collider colliderToIgnore = null; // Most recent non-enemy collider that was hit for ricocheting
    LayerMask layerMask;
    
    GameObject closestHit = null;
    Collider closestHitCollider = null;
    Vector3 closestHitPos;
    Vector3 closestHitNorm;
    float closestHitSqrd = float.MaxValue;
    
    WaitForFixedUpdate waitFixedUpdForRic;
    
    
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
        layerMask = (1 << LayerMask.NameToLayer("Default")) |
                    (1 << LayerMask.NameToLayer("EnemyHitbox")); // |
                    // (1 << LayerMask.NameToLayer("EnemyWeapon"));
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
        checkForCollisions();
    }
    
    void checkForCollisions() {
#if DEBUG_RAYS
        GameManager.D_DrawPoint(transform.position, Color.green);
#endif
        float dist = rb.velocity.magnitude * Time.fixedDeltaTime;
        raycastCollision(Vector3.zero, dist);
        if (ProjConfig.ProjectileRadius >= 0.1f) {
            raycastCollision(transform.right * ProjConfig.ProjectileRadius, dist);
            raycastCollision(-transform.right * ProjConfig.ProjectileRadius, dist);
            raycastCollision(transform.up * ProjConfig.ProjectileRadius, dist);
            raycastCollision(-transform.up * ProjConfig.ProjectileRadius, dist);
        }
        if (closestHit) {
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
            float hitDistSqrd = (hit.point - transform.position).sqrMagnitude;
            if (hitDistSqrd < closestHitSqrd) {
                bool canCheck = !hit.collider.CompareTag("Enemy");
                if (!canCheck) {
                    // if (!hit.collider.TryGetComponent(out EnemyBase en)) {
                    //     en = hit.collider.GetComponentInParent<EnemyBase>();
                    // }
                    // canCheck = en != enemyToIgnore;
                    canCheck = enemyToIgnore != hit.collider.transform.parent.parent.GetComponent<EnemyBase>();
                }
                if (canCheck) {
                    closestHitSqrd = hitDistSqrd;
                    closestHit = hit.collider.gameObject;
                    closestHitCollider = hit.collider;
                    closestHitPos = hit.point;
                    closestHitNorm = hit.normal;
                }
            }
        }
    }
    
    // void OnCollisionEnter(Collision collision) {
    //     onHitSomething(collision.gameObject);
    // }
    
    // This function allows for a projectile's hitbox to be either a collider or a trigger
    void onHitSomething() {
        if (!closestHit.CompareTag("Player") &&
            !closestHit.CompareTag("Projectile") &&
            !closestHit.CompareTag("Pickup")) {
            EnemyBase enemy = null;
            if (closestHit.CompareTag("Enemy")) {
                enemy = closestHit.transform.parent.parent.GetComponent<EnemyBase>();
                if (enemy) {
                    // Check if we hit the ignored enemy
                    if (enemy == enemyToIgnore) return;
                } else if (closestHitCollider) {
                    // Check if we hit the ignored collider
                    if (closestHitCollider == colliderToIgnore) return;
                } else {
                    Debug.LogError("Projectile hit... nothing?");
                }
            }
            transform.position = closestHitPos;
            if (ricRemain > 0) {
                ricRemain--;
                if (enemy)
                    OnRicochetEnemy(enemy);
                else
                    OnRicochetNonEnemy(closestHitCollider);
                closestHit = null;
                closestHitSqrd = float.MaxValue;
            } else {
                if (enemy)
                    OnHitEnemy(enemy);
                else
                    OnHitNonEnemy(closestHit);
                Destroy(gameObject);
            }
            
            // if (hitObject.CompareTag("Enemy")) {
            //     if (!hitObject.transform.parent.parent.TryGetComponent(out EnemyBase enemy)) {
            //         enemy = hitObject.GetComponentInParent<EnemyBase>();
            //     }
            //     if (enemy == enemyToIgnore) return;
            //     transform.position = closestHitPos;
            //     if (enemy.RicochetCannon && ricRemain > 0)
            //         OnRicochetEnemy(enemy);
            //     else {
            //         // if (!enemy.RicochetCannon) print("No ricochet for '" + enemy.gameObject.name + "'");
            //         OnHitEnemy(enemy);
            //         Destroy(gameObject);
            //     }
            // } else {
            //     transform.position = closestHitPos;
            //     OnHitNonEnemy(hitObject);
            //     Destroy(gameObject);
            // }
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
    protected virtual void OnHitNonEnemy(GameObject other) {
        showImpactEffect();
    }
    
    protected virtual void OnRicochetEnemy(EnemyBase enemy) {
        enemyToIgnore = enemy;
        colliderToIgnore = null;
        EnemyBase closestEn = GameManager.Instance.currentSceneRunner.GetClosestEnemy(transform.position, enemyToIgnore);
        Vector3 ricVel;
        if (closestEn) {
            if (closestEn.rb) // If has enemy has an rb, use predictive aiming
                ricVel = (BoidSteerer.PredictPosition(
                            transform.position, closestEn.TransformForRicochetToAimAt.position, rb.velocity, closestEn.rb.velocity
                        ) - transform.position).normalized * rb.velocity.magnitude;
            else // Enemy has no rb, so use their current position
                ricVel = (closestEn.TransformForRicochetToAimAt.position - transform.position).normalized * rb.velocity.magnitude;
        } else // No valid enemy to ricochet to. Reflect physically
            ricVel = Vector3.Reflect(rb.velocity, closestHitNorm);
        rb.velocity *= 0;
        if (ricRemain == 0) {
            TrailRicochet.emitting = false;
            TrailNormal.emitting = true;
        }
        StartCoroutine(ricochetVelNextFrame(ricVel));
        enemy.TakeDamage(100, EDamageType.Projectile);
        showRicochetEffect();
    }
    
    protected void OnRicochetNonEnemy(Collider collider) {
        colliderToIgnore = closestHitCollider;
        enemyToIgnore = null;
        Vector3 ricVel = Vector3.Reflect(rb.velocity, closestHitNorm);
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
        showImpactEffect();
        Destroy(gameObject);
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