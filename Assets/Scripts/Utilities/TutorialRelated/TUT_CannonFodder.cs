using UnityEngine;

[RequireComponent(typeof(TUT_EnemyDefeatedDetector))]
public class TUT_CannonFodder : CannonFodderEnemy {
    
    [SerializeField]
    TUT_EnemyDefeatedDetector edd;
    
    
    
    protected override void LateInit() {
        base.LateInit();
        ConsiderForRicochet = true;
    }
    
    protected override void OnDefeated(EDamageType damageType) {
        if (Dead) return;
        if (edd)
            edd.OnEnemyDefeated(damageType);
        base.OnDefeated(damageType);
    }
    
}