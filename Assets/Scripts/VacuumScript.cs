using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VacuumScript : MonoBehaviour {
    
    [SerializeField]
    CTRL_PlayerCharacter pchar;
    
    List<CTRL_Enemy> enemiesToDamage;
    int defaultListSize = 10;
    
    void Awake() {
        enemiesToDamage = new List<CTRL_Enemy>(defaultListSize);
    }
    
    void FixedUpdate() {
        if (enemiesToDamage.Count > 0) {
            for (int i = enemiesToDamage.Count - 1; i >= 0; i--) {
                CTRL_Enemy en = enemiesToDamage.ElementAt(i);
                // TODO maybe?: apply force on enemy towards vacuum
                if (Time.time - en.lastVacuumHitTime >= pchar.VacuumDamageRate) {
                    // TODO: Check that the enemy isn't behind a wall
                    en.lastVacuumHitTime = Time.time;
                    en.TakeDamage(pchar.VacuumDamage);
                    if (en == null || en.health <= 0)
                        enemiesToDamage.RemoveAt(i);
                    // else print("Enemy did not die");
                }
            }
        }
    }
    
    void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out CTRL_Enemy enemyComp)) return;
        enemyComp.vacuumArrayIndex = enemiesToDamage.Count;
        enemiesToDamage.Add(enemyComp);
    }
    
    void OnTriggerExit(Collider other) {
        if (!other.TryGetComponent(out CTRL_Enemy enemyComp)) return;
        enemiesToDamage.RemoveAt(enemyComp.vacuumArrayIndex);
        enemyComp.vacuumArrayIndex = -1;
    }
    
    void OnDisable() {
        enemiesToDamage.Clear();
    }
    
}