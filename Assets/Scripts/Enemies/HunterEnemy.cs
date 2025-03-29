using System.Collections;
using UnityEngine;

public class HunterEnemy : EnemyBase {
    
    [Header("Hunter Config")]
    public HunterEnemySO HunterConfig;
    // public float StunDuration = 5f;
    // [SerializeField] float shieldDrag = 0.2f;
    // [SerializeField] float stunDrag = 1f;
    bool isStunned = false;
    // public Material shieldActiveMaterial;
    // public Material shieldInactiveMaterial;
    [SerializeField] MeshRenderer ModelMeshRenderer;
    
    [Header("Hunter Audio")]
    [SerializeField]
    AudioSource StunAudio;
    
    
    
    protected override void Init() {
        suckable.CanGetVacuumSucked = false;
        rb.drag = HunterConfig.ShieldDrag;
        SetEffectState(isStunned);
    }
    
    protected override void LateInit() {
        base.LateInit();
        AmbientAudio.Play();
    }
    
    protected override void OnTakeDamage(float amnt, EDamageType damageType) {
        if (!isStunned) {
            if (damageType == EDamageType.Projectile) {
                GetStunned();
                GameManager.Instance.OnEnemyTookDamage(this, damageType, false);
            }
        } else {
            base.OnTakeDamage(amnt, damageType);
        }
    }
    
    public override void Attack() {
        if (isStunned) return;
        base.Attack();
    }
    
    public void GetStunned() {
        if (!isStunned) {
            isStunned = true;
            suckable.CanGetVacuumSucked = true;
            // CanGetVacuumKilled = true;
            boid.enabled = false;
            rb.useGravity = true;
            rb.drag = HunterConfig.StunDrag;
            SetEffectState(true);
            SetAudioState(true);
            StartCoroutine(StunRecovery());
        }
    }
    
    IEnumerator StunRecovery() {
        yield return new WaitForSeconds(HunterConfig.StunDuration);
        if (!Dead) {
            suckable.CanGetVacuumSucked = false;
            // CanGetVacuumKilled = false;
            boid.enabled = true;
            rb.useGravity = false;
            rb.drag = HunterConfig.ShieldDrag;
            isStunned = false;
            SetEffectState(false);
            SetAudioState(false);
        }
    }
    
    void SetEffectState(bool toStunned) {
        if (toStunned) {
            ModelMeshRenderer.material = HunterConfig.ShieldInactiveMaterial;
        } else {
            ModelMeshRenderer.material = HunterConfig.ShieldActiveMaterial;
        }
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
