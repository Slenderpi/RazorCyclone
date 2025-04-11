using UnityEngine;

[RequireComponent(typeof(TUT_EnemyDefeatedDetector))]
public class TUT_CanonFodder : CanonFodderEnemy {
    
    [SerializeField]
    TUT_EnemyDefeatedDetector edd;
    
    
    
    protected override void OnDefeated(EDamageType damageType) {
        if (Dead) return;
        edd.OnEnemyDefeated(damageType);
        base.OnDefeated(damageType);
    }
    
}