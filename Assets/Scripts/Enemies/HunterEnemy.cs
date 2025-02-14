using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class HunterEnemy : EnemyBase {
    
    [Header("Hunter Config")]
    public float StunDuration = 5f;
    [SerializeField] float shieldDrag = 0.2f;
    [SerializeField] float stunDrag = 1f;
    bool isStunned = false;
    public Material shieldActiveMaterial;
    public Material shieldInactiveMaterial;
    [SerializeField] MeshRenderer ModelMeshRenderer;



    protected override void Init() {
        CanGetVacuumSucked = false;
        rb.drag = shieldDrag;
        SetEffectState();
    }

    public override void TakeDamage(float amnt, EDamageType damageType) {
        if (invincible) return;
        if (!isStunned) {
            if (damageType == EDamageType.Projectile) {
                GetStunned();
            }
        } else {
            base.TakeDamage(amnt, damageType);
        }
    }

    public void GetStunned() {
        if (!isStunned) {
            isStunned = true;
            DealDamageOnTouch = false;
            CanGetVacuumSucked = true;
            CanGetVacuumKilled = true;
            boid.enabled = false;
            rb.useGravity = true;
            rb.drag = stunDrag;
            SetEffectState();
            StartCoroutine(StunRecovery());
        }
    }

    IEnumerator StunRecovery() {
        yield return new WaitForSeconds(StunDuration);
        DealDamageOnTouch = true;
        CanGetVacuumSucked = false;
        CanGetVacuumKilled = false;
        boid.enabled = true;
        rb.useGravity = false;
        rb.drag = shieldDrag;
        isStunned = false;
        SetEffectState();
    }

    void SetEffectState() {
        if (isStunned) {
            ModelMeshRenderer.material = shieldInactiveMaterial;
        } else {
            ModelMeshRenderer.material = shieldActiveMaterial;
        }
    }
    
}
