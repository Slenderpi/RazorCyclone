using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterEnemy : EnemyBase
{
    public float moveSpeed = 5f;
    public bool isStunned = false;
    public float stunDuration = 5f;
    public bool shieldActive = true;

    public Material shieldActiveMaterial;
    public Material shieldInactiveMaterial;
    private MeshRenderer enemyRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Transform sphereTransform = transform.Find("Sphere");
        enemyRenderer = sphereTransform.GetComponent<MeshRenderer>();
        shieldActive = true;
        UpdateMaterial();
    }

    void Update()
    {
        if (!isStunned)
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        if (player != null) {
            Debug.Log("chasing player");
            Vector3 direction = (player.transform.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
    }

    public void GetStunned()
    {
        if (!isStunned)
        {
            Debug.Log("enemy stunned");
            isStunned = true;
            shieldActive = false;
            UpdateMaterial();
            rb.velocity = Vector3.zero;
            StartCoroutine(StunRecovery());
        }
    }

    IEnumerator StunRecovery()
    {
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        shieldActive = true;
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        if (shieldActive)
        {
            Debug.Log("changing to white");
            enemyRenderer.material.color = Color.white;
        }
        else
        {            
            Debug.Log("changing to red");
            enemyRenderer.material.color = Color.red;
        }
    }

    public bool IsVulnerable()
    {
        return !shieldActive;
    }

    // testing purposes
    void OnTriggerEnter(Collider other)
    {
        // if (other.CompareTag("Projectile"))
        // {
            GetStunned();
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
    }
    
}