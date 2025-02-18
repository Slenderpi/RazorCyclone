using System.Collections;
using UnityEngine;

public class CentipedeMissile : EnemyBase {
    
    [Header("Missile Config")]
    [Tooltip("When a missile is spawned in, it will start off without tracking and just burst forward, because that looks cool. This is the 'arming' phase.\nThe missile is still able to collide during this phase.\n\nThis variable determines how long arming lasts.")]
    public float ArmingTime = 0.5f;
    [Tooltip("Arming-phase burst strength.")]
    public float ArmingBoost = 30;
    [Tooltip("Arming-phase drag.\nBy having high drag and high boost, the missile looks like it bursts out quickly, gets weighed down by gravity, and then turns on its boosters to track the player.")]
    public float ArmingDrag = 5;
    [Tooltip("Prefab of explosion to use when the missile explodes.")]
    public GameObject ExplosionEffectPrefab;
    GameObject pooledExplosion;
    
    float drag;
    bool hasTriggered = false;
    
    
    
    protected override void Init() {
        base.Init();
        ConsiderForRicochet = false;
        boid.enabled = false;
        drag = rb.drag;
        rb.drag = ArmingDrag;
        pooledExplosion = Instantiate(ExplosionEffectPrefab);
        pooledExplosion.SetActive(false);
    }
    
    protected override void LateInit() {
        base.LateInit();
        rb.AddForce(transform.forward * ArmingBoost, ForceMode.VelocityChange);
        StartCoroutine(armTimer());
    }
    
    void OnTriggerEnter(Collider collider) {
        if (hasTriggered) return;
        hasTriggered = true;
        if (collider.CompareTag("Player"))
            Attack();
        pooledExplosion.transform.position = transform.position;
        pooledExplosion.SetActive(true);
        pooledExplosion = null;
        gameObject.SetActive(false);
        Destroy(gameObject, 1);
    }
    
    protected override void OnDestroying() {
        base.OnDestroying();
        if (pooledExplosion) {
            pooledExplosion.transform.position = transform.position;
            pooledExplosion.SetActive(true);
            pooledExplosion = null;
        }
    }
    
    IEnumerator armTimer() {
        yield return new WaitForSeconds(ArmingTime);
        rb.useGravity = false;
        rb.drag = drag;
        boid.enabled = true;
    }
    
}