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
    
    
    
    protected override void Init() {
        if (IsEmpowered) {
            suckable.CanGetVacuumSucked = false;
            rb.drag = HunterConfig.ShieldDrag;
            SetEffectState(isStunned);
        } else {
            SetEffectState(true);
        }
    }
    
    protected override void OnTakeDamage(float amnt, EDamageType damageType) {
        if (IsEmpowered) {
            if (!isStunned) {
                if (damageType == EDamageType.Projectile) {
                    GetStunned();
                }
            } else {
                base.OnTakeDamage(amnt, damageType);
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
            StartCoroutine(StunRecovery());
        }
    }
    
    IEnumerator StunRecovery() {
        yield return new WaitForSeconds(HunterConfig.StunDuration);
        suckable.CanGetVacuumSucked = false;
        // CanGetVacuumKilled = false;
        boid.enabled = true;
        rb.useGravity = false;
        rb.drag = HunterConfig.ShieldDrag;
        isStunned = false;
        SetEffectState(false);
    }
    
    void SetEffectState(bool toUnshielded) {
        if (toUnshielded) {
            ModelMeshRenderer.material = HunterConfig.ShieldInactiveMaterial;
        } else {
            ModelMeshRenderer.material = HunterConfig.ShieldActiveMaterial;
        }
    }
    
}
