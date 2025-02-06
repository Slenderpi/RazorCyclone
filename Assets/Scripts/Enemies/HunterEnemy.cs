using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class HunterEnemy : EnemyBase {
    
    [Header("Hunter Config")]
    public float StunDuration = 5f;
    bool isStunned = false;
    public Material shieldActiveMaterial;
    public Material shieldInactiveMaterial;
    [SerializeField] MeshRenderer ModelMeshRenderer;
    
    
    
    void Start() {
        CanGetVacuumSucked = false;
        SetEffectState();
    }

    public override void TakeDamage(float amnt, EDamageType damageType) {
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
            boid.enabled = false;
            SetEffectState();
            rb.useGravity = true;
            StartCoroutine(StunRecovery());
        }
    }

    IEnumerator StunRecovery() {
        yield return new WaitForSeconds(StunDuration);
        DealDamageOnTouch = true;
        CanGetVacuumSucked = false;
        boid.enabled = true;
        rb.useGravity = false;
        isStunned = false;
        SetEffectState();
    }

    void SetEffectState() {
        if (isStunned) {
            ModelMeshRenderer.material = shieldActiveMaterial;
        } else {
            ModelMeshRenderer.material = shieldInactiveMaterial;
        }
    }
    
}
