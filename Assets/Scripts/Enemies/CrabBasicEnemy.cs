using UnityEngine;

public class CrabBasicEnemy : EnemyBase {
    
    [Header("Crab Basic Enemy Config")]
    public CrabBasicEnemySO CrabBasicConfig;
    [SerializeField]
    [Tooltip("Reference to the transform the turret will revolve (yaw) around.")]
    Transform revolvePivot;
    [SerializeField]
    [Tooltip("Reference to the transform the turret's barrel will rotate (pitch) around.")]
    Transform barrelPivot;
    [SerializeField]
    Transform bodyPivot;
    [SerializeField]
    [Tooltip("Reference to the transform the projectile will spawn from.")]
    Transform BarrelEndpoint;
    [Tooltip("Reference to the gameObject holding firing VFX.")]
    [SerializeField]
    GameObject FireGunVFX;
    ParticleSystem[] fireGunParticles;
    
    [Header("FOR TESTING")]
    [SerializeField]
    bool predictiveFiring = true;
    [SerializeField]
    float maxPredictTime = 0.1f;
    
    EnemyProjectile pooledProj;
    
    float randomAtkDly;
    
    
    
    protected override void Init() {
        FireGunVFX.SetActive(false);
        fireGunParticles = FireGunVFX.GetComponentsInChildren<ParticleSystem>();
        randomAtkDly = EnConfig.AttackDelay + Random.Range(-1f, 1f) * CrabBasicConfig.AttackTimeOffset;
    }
    
    protected override void LateInit() {
        base.LateInit();
        lastAttackTime = Time.fixedTime;
        poolProjectile();
    }

    void Update() {
        Vector3 toPlr = predictPlrPos() - Model.transform.position;
        Vector3 flat = new Vector3(toPlr.x, 0, toPlr.z);
        revolvePivot.rotation = Quaternion.LookRotation(flat);
        barrelPivot.rotation = Quaternion.LookRotation(toPlr);
        bodyPivot.rotation = Quaternion.RotateTowards(
            bodyPivot.rotation,
            Quaternion.LookRotation(flat),
            CrabBasicConfig.MaxBodyRotPerSec * Time.deltaTime
        );
        Hitboxes.transform.rotation = bodyPivot.rotation;
    }

    protected override void onFixedUpdate() {
        if (!GameManager.CurrentPlayer) return;
        if (Time.fixedTime - lastAttackTime >= EnConfig.AttackDelay) {
            lastAttackTime += randomAtkDly;
            Attack();
        }
        base.onFixedUpdate();
    }
    
    public override void Attack() {
        Vector3 toPlr = calcVToPlr();
        pooledProj.transform.SetPositionAndRotation(BarrelEndpoint.position, Quaternion.LookRotation(toPlr));
        pooledProj.rb.velocity = toPlr.normalized * CrabBasicConfig.ProjectileSpeed;
        pooledProj.gameObject.SetActive(true);
        if (FireGunVFX.activeSelf)
            foreach (ParticleSystem ps in fireGunParticles)
                ps.Play();
        else
            FireGunVFX.SetActive(true);
        poolProjectile();
    }
    
    Vector3 calcVToPlr() {
        return (predictiveFiring ? predictPlrPos() : GameManager.CurrentPlayer.transform.position) - BarrelEndpoint.position;
    }
    
    Vector3 predictPlrPos() {
        Vector3 ppos = GameManager.CurrentPlayer.transform.position;
        Vector3 pvel = GameManager.CurrentPlayer.rb.velocity;
        // Solve for t in the following: ppos + pspeed * t == cpos + cspeed * t
        float predictTime = (ppos - BarrelEndpoint.position).magnitude / Mathf.Abs(pvel.magnitude - CrabBasicConfig.ProjectileSpeed);
        return ppos + pvel * Mathf.Min(predictTime, maxPredictTime);
    }
    
    void poolProjectile() {
        pooledProj = Instantiate(CrabBasicConfig.CrabProjectilePrefab);
        pooledProj.gameObject.SetActive(false);
        pooledProj.Damage = EnConfig.Damage;
    }
    
    protected override void OnDestroying() {
        if (pooledProj)
            Destroy(pooledProj.gameObject);
        base.OnDestroying();
    }
    
}