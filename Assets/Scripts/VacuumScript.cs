using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VacuumScript : MonoBehaviour {
    
    [SerializeField]
    PlayerCharacterCtrlr pchar;
    [SerializeField]
    TriggerNotifier SuckboxNotifier;
    [SerializeField]
    Transform VacuumKillbox;
    [SerializeField]
    TriggerNotifier KillboxNotifier;
    
    List<EnemyBase> enemiesInRange;
    int defaultListSize = 10;
    
    float VacuumPullForce = 8500f;
    
    void Awake() {
        enemiesInRange = new List<EnemyBase>(defaultListSize);
    }
    
    void FixedUpdate() {
        if (enemiesInRange.Count > 0) {
            for (int i = enemiesInRange.Count - 1; i >= 0; i--) {
                EnemyBase en = enemiesInRange.ElementAt(i);
                if (!en) {
                    enemiesInRange.RemoveAt(i);
                    continue;
                } else if (Time.time - en.lastVacuumHitTime >= pchar.VacuumDamageRate) {
                    // TODO: Check that the enemy isn't behind a wall
                    // en.lastVacuumHitTime = Time.time;
                    // en.TakeDamage(pchar.VacuumDamage);
                    if (en.health <= 0)
                        enemiesInRange.RemoveAt(i);
                    
                    en.GetComponent<Rigidbody>().AddForce((VacuumKillbox.position - en.transform.position).normalized * VacuumPullForce);
                }
            }
        }
    }
    
    // void OnTriggerEnter(Collider other) {
    //     if (!other.TryGetComponent(out EnemyBase enemyComp)) return;
    //     enemyComp.vacuumArrayIndex = enemiesInRange.Count;
    //     enemiesInRange.Add(enemyComp);
    // }
    
    // void OnTriggerExit(Collider other) {
    //     if (!other.TryGetComponent(out EnemyBase enemyComp)) return;
    //     enemiesInRange.RemoveAt(enemyComp.vacuumArrayIndex);
    //     enemyComp.vacuumArrayIndex = -1;
    // }
    
    void onSuckboxEnter(Collider collider) {
        if (!collider.TryGetComponent(out EnemyBase enemyComp)) return;
        enemyComp.vacuumArrayIndex = enemiesInRange.Count;
        enemiesInRange.Add(enemyComp);
        
    }
    
    void onSuckboxExit(Collider collider) {
        if (!collider.TryGetComponent(out EnemyBase enemyComp)) return;
        enemiesInRange.RemoveAt(enemyComp.vacuumArrayIndex);
        enemyComp.vacuumArrayIndex = -1;
    }
    
    void onKillboxEnter(Collider collider) {
        if (collider.tag == "Enemy") {
            EnemyBase en = collider.GetComponent<EnemyBase>();
            en.TakeDamage(en.MaxHealth);
        }
    }
    
    void OnEnable() {
        SuckboxNotifier.TriggerEnterEvent += onSuckboxEnter;
        SuckboxNotifier.TriggerExitEvent += onSuckboxExit;
        KillboxNotifier.TriggerEnterEvent += onKillboxEnter;
    }
    
    void OnDisable() {
        enemiesInRange.Clear();
        SuckboxNotifier.TriggerEnterEvent -= onSuckboxEnter;
        SuckboxNotifier.TriggerExitEvent -= onSuckboxExit;
        KillboxNotifier.TriggerEnterEvent -= onKillboxEnter;
    }
    
}