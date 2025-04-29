using UnityEngine;

public class LavaEnemy : WeakpointedEnemy {
    
    [Header("Lava Enemy Configuration")]
    public LavaEnemySO LavaConfig;
    public AudioSource StunAudio;
    
    // WaitForSeconds exposeWaiter;
    LavaWeakpoint weakpoint;
    bool isArmored = true;
    float lastExposeTime = -1000f;
    
    
    
    protected override void Init() {
        base.Init();
        if (MaxHealth > 1)
            Debug.LogWarning(">> Lava Enemy has more than one weakpoint!");
        weakpoint = weakpoints[0] as LavaWeakpoint;
        // exposeWaiter = new WaitForSeconds(LavaConfig.WeakpointExposeDuration);
    }
    
    protected override void LateInit() {
        base.LateInit();
        lava.OnLavaEnemySpawned();
    }
    
    protected override void onFixedUpdate() {
        if (transform.position.y < lava.currentHeight) {
            if (!rb.constraints.HasFlag(RigidbodyConstraints.FreezePositionY))
                rb.constraints |= RigidbodyConstraints.FreezePositionY;
            transform.position = new(transform.position.x, lava.currentHeight, transform.position.z);
        }
        if (!isArmored && Time.fixedTime - lastExposeTime > LavaConfig.WeakpointExposeDuration) {
            ReArmor();
        }
    }
    
    protected override void OnDefeated(EDamageType damageType) {
        if (Dead) return;
        lava.OnLavaEnemyDefeated();
        // Lava enemy does not drop its own fuel cell. Instead, its weakpoint(s) will.
        if (StunAudio.isPlaying)
            StunAudio.Stop();
        ShowDeath();
    }
    
    protected override void OnTakeDamage(float amnt, EDamageType damageType) {
        if (damageType == EDamageType.Projectile || damageType == EDamageType.ProjectileRicochet) {
            OnShotByCannon();
            GameManager.Instance.OnEnemyTookDamage(this, damageType, false);
        }
    }
    
    void OnShotByCannon() {
        lastExposeTime = Time.fixedTime;
        if (!isArmored)
            return;
        isArmored = false;
        ConsiderForRicochet = false;
        weakpoint.BeginExpose();
        boid.enabled = false;
        SetAudioState(true);
    }
    
    void ReArmor() {
        isArmored = true;
        ConsiderForRicochet = true;
        weakpoint.BeginHide();
        boid.enabled = true;
        SetAudioState(false);
    }
    
    void SetAudioState(bool toStunned) {
        if (toStunned) {
            AmbientAudio.Stop();
            StunAudio.Play();
        } else {
            AmbientAudio.Play();
        }
    }
    
}