using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class HunterEnemy : EnemyBase
{
    public float moveSpeed = 5f;
    public bool isStunned = false;
    public float stunDuration = 5f;
    public bool shieldActive = true;

    public Material shieldActiveMaterial;
    public Material shieldInactiveMaterial;
    [SerializeField] MeshRenderer enemyRenderer;
    
    
    
    void Start() {
        Transform sphereTransform = transform.Find("Sphere");
        //enemyRenderer = sphereTransform.GetComponent<MeshRenderer>();
        shieldActive = true;
        UpdateMaterial();
    }

    void Update() {
        if (!isStunned) {
            // ChasePlayer();
        }
    }

    void ChasePlayer() {
        if (GameManager.CurrentPlayer != null) {
            // Debug.Log("chasing player");
            Vector3 direction = (GameManager.CurrentPlayer.transform.position - transform.position).normalized;
            transform.LookAt(GameManager.CurrentPlayer.transform.position);
            rb.velocity = direction * moveSpeed;
        }
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
            boid.enabled = false;
            shieldActive = false;
            UpdateMaterial();
            // rb.velocity = Vector3.down * 0.5f;
            rb.useGravity = true;
            StartCoroutine(StunRecovery());
        }
    }

    IEnumerator StunRecovery() {
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        boid.enabled = true;
        rb.useGravity = false;
        shieldActive = true;
        UpdateMaterial();
    }

    void UpdateMaterial() {
        if (shieldActive) {
            enemyRenderer.material = shieldActiveMaterial;
        } else {
            enemyRenderer.material = shieldInactiveMaterial;
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
