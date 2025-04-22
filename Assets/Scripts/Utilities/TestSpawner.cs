using UnityEngine;

public class TestSpawner : MonoBehaviour {
    
    [SerializeField]
    bool instantiate = false;
    bool hasInst = false;
    [SerializeField]
    bool start = false;
    bool hasStrt = false;
    [SerializeField]
    EnemyBase enemy;
    
    [SerializeField]
    Transform Spawnpoint;
    
    EnemyBase spawnedEnemy;
    
    
    
    void Awake() {
        instantiate = false;
        start = false;
    }
    
    void Update() {
        if (!hasInst && instantiate) {
            hasInst = true;
            spawnedEnemy = Instantiate(enemy, Spawnpoint.position, Spawnpoint.rotation);
            spawnedEnemy.gameObject.SetActive(false);
        } else if (!hasStrt && start) {
            hasStrt = true;
            spawnedEnemy.gameObject.SetActive(true);
        }
    }
    
}