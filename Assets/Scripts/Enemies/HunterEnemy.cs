using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Animations;

public class HunterEnemy : EnemyBase {
    
    [Header("Hunter Config")]
    public float StunDuration = 5f;
    [SerializeField] float shieldDrag = 0.2f;
    [SerializeField] float stunDrag = 1f;
    [HideInInspector]
    public bool isStunned = false;
    public bool shieldActive = true;
    public Material shieldActiveMaterial;
    public Material shieldInactiveMaterial;
    [SerializeField] MeshRenderer ModelMeshRenderer;
    
    
    
    void Start() {
        shieldActive = true;
        CanGetVacuumSucked = false;
        rb.drag = shieldDrag;
        UpdateMaterial();
    }

    public override void TakeDamage(float amnt, EDamageType damageType) {
        if (shieldActive) {
            if (damageType == EDamageType.Projectile) {
                GetStunned();
            }
        } else {
            /* Take damage since the shield is down. The base TakeDamage() function
             * already handles taking health damage.
             */
            base.TakeDamage(amnt, damageType);
        }
    }

    public void GetStunned() {
        if (!isStunned) {
            // Debug.Log("enemy stunned");
            isStunned = true;
            DealDamageOnTouch = false;
            CanGetVacuumSucked = true;
            boid.enabled = false;
            shieldActive = false;
            UpdateMaterial();
            // rb.velocity = Vector3.down * 0.5f;
            rb.useGravity = true;
            rb.drag = stunDrag;
            StartCoroutine(StunRecovery());
        }
    }

    IEnumerator StunRecovery() {
        yield return new WaitForSeconds(StunDuration);
        isStunned = false;
        DealDamageOnTouch = true;
        CanGetVacuumSucked = false;
        boid.enabled = true;
        rb.useGravity = false;
        rb.drag = shieldDrag;
        shieldActive = true;
        UpdateMaterial();
    }

    void UpdateMaterial() {
        if (shieldActive) {
            ModelMeshRenderer.material = shieldActiveMaterial;
        } else {
            ModelMeshRenderer.material = shieldInactiveMaterial;
        }
    }

    public bool IsVulnerable() {
        return !shieldActive;
    }

    // testing purposes
    // void OnTriggerEnter(Collider other)
    // {
        // if (other.CompareTag("Projectile"))
        // {
            // GetStunned();
        // }

        /*
        if (other.CompareTag("Vacuum") && IsVulnerable())
        {
            Debug.Log("hunter got vaccuuuuumed up");
            DropFuel();
            Destroy(gameObject);
        }

        if (other.CompareTag("Cannon") && IsVulnerable())
        {
            Debug.Log("hunter got hit by cannon boom boom");
            DropFuel();
            Destroy(gameObject);
        }
        */
    // }
    
}
