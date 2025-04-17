using UnityEngine;

public class TUT_EnemyDefeatedDetector : MonoBehaviour {
    
    TUT_EnemySpawner owningSpawner;
    bool wasDefeated = false;
    
    
    
    public void SetOwningSpawner(TUT_EnemySpawner sp) {
        owningSpawner = sp;
    }
    
    public void OnEnemyDefeated(EDamageType dtype) {
        wasDefeated = true;
        SRTutorial srt = GameManager.Instance.currentSceneRunner as SRTutorial;
        if (!srt) return;
        bool isCorrectType = srt.requiredDamageType == EDamageType.Any || srt.requiredDamageType == dtype;
        if (isCorrectType) {
            srt.OnEnemyKilled(true);
        } else {
            srt.OnEnemyKilled(false);
            owningSpawner.DelaySpawnEnemy();
        }
    }
    
    void OnDestroy() {
        if (!wasDefeated && owningSpawner)
            owningSpawner.DelaySpawnEnemy();
    }

}