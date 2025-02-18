using System.Collections;
using UnityEngine;

public class ProjectileBase : MonoBehaviour {
    
    // [Tooltip("Damage of this projectile.")]
    [HideInInspector]
    public float damage;
    // public GameObject explosionEffect;
    // [Tooltip("The radius to use when doing multiple raycasts for collision detection.\n\nNOTE: if set to a value less than 0.1, this projectile will only check directly in front of it once.")]
    // public float ProjectileRadius = 0.1f;
    float ProjectileRadius = 0.2f;
    [Tooltip("Maximum lifetime in seconds of this projectile to prevent projectiles that go into the void from living too long.")]
    public float MaxLifetime = 10f;
    [Tooltip("Maximum number of times this projectile can ricochet. 0 means no ricochet.")]
    public int MaxRicochet = 1;
    int ricRemain;
    
    [HideInInspector]
    public Rigidbody rb;
    EnemyBase enemyToIgnore = null; // Most recent enemy that was hit for ricocheting
    LayerMask layerMask;
    
    GameObject closestHit = null;
    Vector3 closestHitPos;
    Vector3 closestHitNorm;
    float closestHitSqrd = float.MaxValue;
    
    
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
        layerMask = (1 << LayerMask.NameToLayer("Default")) |
                    (1 << LayerMask.NameToLayer("Enemy")) |
                    (1 << LayerMask.NameToLayer("EnemyWeapon"));
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
        GameManager.D_DrawPoint(transform.position, Color.magenta);
        float dist = rb.velocity.magnitude * Time.fixedDeltaTime;
        checkForCollision(Vector3.zero, dist);
        if (ProjectileRadius >= 0.1f) {
            checkForCollision(transform.right * ProjectileRadius, dist);
            checkForCollision(-transform.right * ProjectileRadius, dist);
            checkForCollision(transform.up * ProjectileRadius, dist);
            checkForCollision(-transform.up * ProjectileRadius, dist);
        }
        if (closestHit) {
            transform.position = closestHitPos;
            onHitSomething(closestHit);
        }
    }
    
    void checkForCollision(Vector3 offset, float dist) {
        Debug.DrawRay(transform.position + offset, rb.velocity * Time.fixedDeltaTime, Color.white, Time.fixedDeltaTime, false);
        if (Physics.Raycast(
            origin: transform.position + offset,
            direction: rb.velocity,
            maxDistance: dist,
            layerMask: layerMask,
            hitInfo: out RaycastHit hit)) {
            float hitDistSqrd = (hit.point - transform.position).sqrMagnitude;
            if (hitDistSqrd < closestHitSqrd) {
                if (!hit.collider.CompareTag("Enemy") || enemyToIgnore == null || hit.collider.GetComponent<EnemyBase>() != enemyToIgnore) {
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
                if (!hitObject.TryGetComponent(out EnemyBase enemy)) {
                    enemy = hitObject.GetComponentInParent<EnemyBase>();
                }
                if (enemy == enemyToIgnore) return;
                if (enemy.RicochetCanon && ricRemain > 0)
                    OnRicochetEnemy(enemy);
                else {
                    OnHitEnemy(enemy);
                    Destroy(gameObject);
                }
            } else {
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
        enemy.TakeDamage(damage, EDamageType.Projectile);
    }
    
    /// <summary>
    /// Called after this projectile has hit something that wasn't an enemy. By default,
    /// this method does nothing. After calling OnHitNonEnemy(), this projectile
    /// will be destroyed.
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnHitNonEnemy(GameObject other) {}
    
    protected virtual void OnRicochetEnemy(EnemyBase enemy) {
        ricRemain--;
        enemyToIgnore = enemy;
        closestHit = null;
        closestHitSqrd = float.MaxValue;
        EnemyBase closestEn = GameManager.Instance.currentSceneRunner.GetClosestEnemy(transform.position, enemyToIgnore);
        if (closestEn) {
            rb.velocity = (closestEn.TransformForRicochetToAimAt.position - transform.position).normalized * rb.velocity.magnitude;
        } else {
            rb.velocity = Vector3.Reflect(rb.velocity, closestHitNorm);
        }
        transform.rotation = Quaternion.LookRotation(rb.velocity);
        enemy.TakeDamage(damage, EDamageType.Projectile);
    }
    
    /// <summary>
    /// Called if the projectile lives up to its MaxLifetime without collision. By default,
    /// this method destroys the projectile.
    /// </summary>
    protected virtual void OnProjectileLifetimeExpired() {
        Destroy(gameObject);
    }
    
    IEnumerator ProjectileLifetime() {
        yield return new WaitForSeconds(MaxLifetime);
        OnProjectileLifetimeExpired();
    }
    
}