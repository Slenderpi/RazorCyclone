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
    
    List<Suckable> suckablesInRange;
    readonly int defaultListSize = 7;
    
    void Awake() {
        suckablesInRange = new List<Suckable>(defaultListSize);
    }
    
    void FixedUpdate() {
        for (int i = suckablesInRange.Count - 1; i >= 0; i--) {
            Suckable s = suckablesInRange.ElementAt(i);
            if (!s) {
                suckablesInRange.RemoveAt(i);
                continue;
            }
            // TODO: Check that the object is in conical (or frustrum?) range and not behind a wall
            if (s.CanGetVacuumSucked)
                s.rb.AddForce((VacuumKillbox.position - s.transform.position).normalized * pchar.VacuumSuckForce);
        }
    }
    
    void onSuckboxEnter(Collider collider) {
        if (!collider.TryGetComponent(out Suckable s))
            s = collider.GetComponentInParent<Suckable>();
        if (s)
            suckablesInRange.Add(s);
    }
    
    void onSuckboxExit(Collider collider) {
        if (!collider.TryGetComponent(out Suckable s))
            s = collider.GetComponentInParent<Suckable>();
        if (s)
            suckablesInRange.Remove(s);
    }
    
    void onKillboxEnter(Collider collider) {
        if (collider.CompareTag("Enemy")) {
            if (!collider.TryGetComponent(out EnemyBase en)) {
                en = collider.GetComponentInParent<EnemyBase>();
            }
            if (en != null)
                en.TakeDamage(en.MaxHealth, EDamageType.Vacuum);
        }
    }
    
    void OnEnable() {
        SuckboxNotifier.A_TriggerEntered += onSuckboxEnter;
        SuckboxNotifier.A_TriggerExited += onSuckboxExit;
        KillboxNotifier.A_TriggerEntered += onKillboxEnter;
    }
    
    void OnDisable() {
        suckablesInRange.Clear();
        SuckboxNotifier.A_TriggerEntered -= onSuckboxEnter;
        SuckboxNotifier.A_TriggerExited -= onSuckboxExit;
        KillboxNotifier.A_TriggerEntered -= onKillboxEnter;
    }
    
}