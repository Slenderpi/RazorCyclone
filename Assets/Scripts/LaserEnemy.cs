using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : EnemyBase
{
    public LineRenderer laser;
    public float attackCooldown = 5f;
    public float windUpTime = 3f;
    public float laserDuration = 3f;
    public float pauseBeforeFiring = 0.3f;
    public float rotationSpeed = 2f;
    public float laserDamage = 10f;

    private bool isAttacking = false;
    private bool isLaserActive = false;
    private float attackTimer;

    void Start()
    {
        attackTimer = attackCooldown;
        laser.enabled = false;
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        if (!isAttacking && attackTimer <= 0f)
        {
            StartCoroutine(PerformAttack());
            attackTimer = attackCooldown;
        }

        if (isLaserActive)
        {
            RotateTowardsPlayer();
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;

        laser.enabled = true;
        StartCoroutine(ChargeLaser());
        yield return new WaitForSeconds(windUpTime);

        laser.enabled = false;
        yield return new WaitForSeconds(pauseBeforeFiring);

        isLaserActive = true;
        laser.enabled = true;
        StartCoroutine(FireLaser());
        yield return new WaitForSeconds(laserDuration);

        isLaserActive = false;
        laser.enabled = false;
        isAttacking = false;
        attackTimer = attackCooldown;
    }

    IEnumerator ChargeLaser()
    {
        while (isAttacking && !isLaserActive)
        {
            RotateTowardsPlayer();
            UpdateLaserLine();
            yield return null;
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void UpdateLaserLine()
    {
        laser.SetPosition(0, transform.position);
        laser.SetPosition(1, player.transform.position);
    }

    IEnumerator FireLaser()
    {
        while (isLaserActive)
        {
            Debug.Log("pow pow");
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vacuum") || other.CompareTag("Cannon"))
        {
            Debug.Log("successfullly hit laser enemy");
            DropFuel();
            Destroy(gameObject);
        }
    }

}
