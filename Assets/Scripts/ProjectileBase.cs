// UNCOMMENT THE LINE BELOW FOR DEBUGGING PROJECTILE RAYCASTS
// #define DEBUG_RAYS

using System.Collections;
using UnityEngine;

public class ProjectileBase : MonoBehaviour {
    
    [Header("Projectile Config")]
    public ProjectileSO ProjConfig;
    // [Tooltip("The radius to use when doing multiple raycasts for collision detection.\n\nNOTE: if set to a value less than 0.1, this projectile will only check directly in front of it once.")]
    // public float ProjectileRadius = 0.2f;
    // [Tooltip("Maximum lifetime in seconds of this projectile to prevent projectiles that go into the void from living too long.")]
    // public float MaxLifetime = 10f;
    [Tooltip("Maximum number of times this projectile can ricochet.\nA value of 0 means NO ricochet.")]
    public int MaxRicochet = 1;
    int ricRemain;
    // [Tooltip("VFX for projectile impact.")]
    // public GameObject ImpactEffect;
    protected GameObject impeffect; // Pre-spawned impact effect
    // [Tooltip("VFX for ricochet.")]
    // public GameObject RicochetEffect;
    protected GameObject[] riceffects;
    
    [HideInInspector]
    public Rigidbody rb;
    EnemyBase enemyToIgnore = null; // Most recent enemy that was hit for ricocheting
    LayerMask layerMask;
    
    GameObject closestHit = null;
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
        poolVFX();
        Init();
    }
    
    void Start() {
        ricRemain = MaxRicochet;
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
            onHitSomething(closestHit);
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
    void onHitSomething(GameObject hitObject) {
        if (!hitObject.CompareTag("Player") &&
            !hitObject.CompareTag("Projectile") &&
            !hitObject.CompareTag("Pickup")) {
            if (hitObject.CompareTag("Enemy")) {
                if (!hitObject.transform.parent.parent.TryGetComponent(out EnemyBase enemy)) {
                    enemy = hitObject.GetComponentInParent<EnemyBase>();
                }
                if (enemy == enemyToIgnore) return;
                transform.position = closestHitPos;
                if (enemy.RicochetCanon && ricRemain > 0)
                    OnRicochetEnemy(enemy);
                else {
                    // if (!enemy.RicochetCanon) print("No ricochet for '" + enemy.gameObject.name + "'");
                    OnHitEnemy(enemy);
                    Destroy(gameObject);
                }
            } else {
                transform.position = closestHitPos;
                OnHitNonEnemy(hitObject);
                Destroy(gameObject);
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
    protected virtual void OnHitNonEnemy(GameObject other) {
        showImpactEffect();
    }
    
    protected virtual void OnRicochetEnemy(EnemyBase enemy) {
        ricRemain--;
        enemyToIgnore = enemy;
        closestHit = null;
        closestHitSqrd = float.MaxValue;
        EnemyBase closestEn = GameManager.Instance.currentSceneRunner.GetClosestEnemy(transform.position, enemyToIgnore);
        Vector3 ricVel = closestEn ?
                         (closestEn.TransformForRicochetToAimAt.position - transform.position).normalized * rb.velocity.magnitude :
                         Vector3.Reflect(rb.velocity, closestHitNorm);
        rb.velocity *= 0;
        StartCoroutine(ricochetVelNextFrame(ricVel));
        enemy.TakeDamage(100, EDamageType.Projectile);
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
        for (int i = 0; i < ricRemain; i++) {
            Destroy(riceffects[i]);
        }
    }
    
    void showImpactEffect() {
        impeffect.transform.SetPositionAndRotation(transform.position, transform.rotation);
        impeffect.SetActive(true);
    }
    
    void showRicochetEffect() {
        GameObject re = riceffects[ricRemain];
        re.transform.SetPositionAndRotation(transform.position, transform.rotation);
        re.SetActive(true);
    }
    
    IEnumerator ProjectileLifetime() {
        yield return new WaitForSeconds(ProjConfig.MaxLifetime);
        OnProjectileLifetimeExpired();
    }
    
    void poolVFX() {
        impeffect = Instantiate(ProjConfig.ImpactEffect);
        impeffect.SetActive(false);
        riceffects = new GameObject[MaxRicochet];
        for (int i = 0; i < MaxRicochet; i++) {
            riceffects[i] = Instantiate(ProjConfig.RicochetEffect);
            riceffects[i].SetActive(false);
        }
    }
    
}