using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterEnemy : EnemyBase
{
    public float movementForce = 5f;
    public float attackDamage = 20f;
    public int miniHunterAmount = 4;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        player = GameManager.CurrentPlayer;
        ChasePlayer();
    }

    void ChasePlayer()
    {
        if (player != null) {
            Debug.Log("chasing player");
            Vector3 direction = (player.transform.position - transform.position).normalized;
            rb.velocity = direction * movementForce;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            player.TakeDamage(attackDamage);

        if (other.CompareTag("Cannon"))
        {
            Debug.Log("hunter killed by cannon");
            SpawnMiniHunters();
            Destroy(gameObject);
        }
    }

    // TODO: write mini hunter enemy script
    void SpawnMiniHunters()
    {
        for (int i = 0; i < miniHunterAmount; i++)
        {
            GameObject miniHunter = Instantiate(miniHunterPrefab, transform.position, Quaternion.identity);
        }
    }
}
