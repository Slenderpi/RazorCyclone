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
                } else if (Time.time - en.lastVacuumHitTime >= pchar.VacuumSuckRate) {
                    // TODO: Check that the enemy isn't behind a wall
                    // en.lastVacuumHitTime = Time.time;
                    // en.TakeDamage(pchar.VacuumDamage);
                    if (en.health <= 0)
                        enemiesInRange.RemoveAt(i);
                    else
                        en.rb.AddForce((VacuumKillbox.position - en.transform.position).normalized * pchar.VacuumSuckForce);
                }
            }
        }
    }
    
    void onSuckboxEnter(Collider collider) {
        EnemyBase en = collider.GetComponentInParent<EnemyBase>();
        if (en) enemiesInRange.Add(en);
    }
    
    void onSuckboxExit(Collider collider) {
        EnemyBase en = collider.GetComponentInParent<EnemyBase>();
        if (en) enemiesInRange.Remove(en);
    }
    
    void onKillboxEnter(Collider collider) {
        if (collider.CompareTag("Enemy")) {
            EnemyBase en = collider.GetComponentInParent<EnemyBase>();
            en.TakeDamage(en.MaxHealth, EDamageType.Vacuum);
        }
    }
    
    void OnEnable() {
        SuckboxNotifier.A_TriggerEntered += onSuckboxEnter;
        SuckboxNotifier.A_TriggerExited += onSuckboxExit;
        KillboxNotifier.A_TriggerEntered += onKillboxEnter;
    }
    
    void OnDisable() {
        enemiesInRange.Clear();
        SuckboxNotifier.A_TriggerEntered -= onSuckboxEnter;
        SuckboxNotifier.A_TriggerExited -= onSuckboxExit;
        KillboxNotifier.A_TriggerEntered -= onKillboxEnter;
    }
    
}