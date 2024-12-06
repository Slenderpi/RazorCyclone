using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonFodder : EnemyBase
{
    public float movementSpeed = 2f;
    public int fuelAmount = 50;
    public float changeDirectionTime = 3f; 

    private Rigidbody rb;
    private float lastChangeTime;
    private Vector3 targetDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastChangeTime = Time.time;
        ChooseRandomDirection();
    }

    void Update()
    {
        if (Time.time - lastChangeTime >= changeDirectionTime)
        {
            ChooseRandomDirection();
            lastChangeTime = Time.time;
        }

        rb.AddForce(targetDirection * movementSpeed, ForceMode.Force);

        if (IsNearLedge())
        {
            ChooseRandomDirection();
        }
    }

    void ChooseRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        targetDirection = new Vector3(Mathf.Cos(randomAngle), 0f, Mathf.Sin(randomAngle)).normalized;
    }

    // TODO: fix ledge detection
    bool IsNearLedge()
    {
        Vector3 direction = Vector3.down;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 1f))
        {
            if (!hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }

    // bounces off surfaces
    void OnCollisionEnter(Collision collision)
    {
        Vector3 bounceDirection = Vector3.Reflect(rb.velocity.normalized, collision.contacts[0].normal);
        rb.AddForce(bounceDirection * movementSpeed, ForceMode.Impulse);
    }

    public void TakeDamage(float amnt)
    {
        health -= amnt;
        if (health <= 0) 
        {
            GameManager.Instance.OnEnemyDied();
            // DropFuel(50);
            Destroy(gameObject);
            GameManager.CurrentPlayer.AddFuel(50.0f);
        }
    }

    public void DropFuel(float amnt)
    {
        for (int i = 0; i < amnt; i++)
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
