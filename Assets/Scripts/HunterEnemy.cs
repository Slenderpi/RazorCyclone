using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterEnemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject player;
    public bool isStunned = false;
    public float stunDuration = 5f;
    public bool shieldActive = true;

    public GameObject fuelPrefab;
    public int fuelAmount = 1;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        shieldActive = true;
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
        Vector3 direction = (player.transform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    public void GetStunned()
    {
        if (!isStunned)
        {
            isStunned = true;
            shieldActive = false;
            rb.velocity = Vector3.zero;
            StartCoroutine(StunRecovery());
        }
    }

    IEnumerator StunRecovery()
    {
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        shieldActive = true;
    }

    public bool IsVulnerable()
    {
        return !shieldActive;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            GetStunned();
        }

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
    }

    void DropFuel()
    {
        for (int i = 0; i < fuelAmount; i++)
        {
            GameObject fuel = Instantiate(fuelPrefab, transform.position, Quaternion.identity);
            Rigidbody fuelRb = fuel.GetComponent<Rigidbody>();
            if (fuelRb != null)
            {
                fuelRb.useGravity = true;
            }
        }
    }
}
