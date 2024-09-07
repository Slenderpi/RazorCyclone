using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance;
    public static CTRL_PlayerCharacter CurrentPlayer;
    
    [SerializeField]
    Transform spawnPoint;
    [SerializeField]
    CTRL_PlayerCharacter playerPrefab;
    [SerializeField]
    CTRL_Enemy enemyPrefab;
    [SerializeField]
    Transform enemySpawnPoint;
    
    bool hasSpawnedPlayer = false;
    
    void Awake() {
        if (!Instance)
            Instance = this;
    }
    
    void Start() {
        
    }
    
    void Update() {
        if (!hasSpawnedPlayer && Time.time > 0.1) {
            hasSpawnedPlayer = true;
            spawnPlayer();
        }
    }
    
    void spawnPlayer() {
        CurrentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<CTRL_PlayerCharacter>();
    }
    
    public void OnEnemyDied() {
        CurrentPlayer.AddFuel(100f);
        Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
    }
    
}
