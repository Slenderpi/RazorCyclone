using UnityEngine;

public class LavaEnemy : WeakpointedEnemy {
    
    // [Header("Lava Enemy Configuration")]
    
    Lava lava;
    
    
    
    void Start() {
        lava = GameManager.Instance.currentSceneRunner.lava;
        lava.OnLavaEnemySpawned();
    }

    protected override void OnDefeated(EDamageType damageType) {
        lava.OnLavaEnemyDefeated();
        base.OnDefeated(damageType);
    }

    protected override void Init() {
        base.Init();
    }

}