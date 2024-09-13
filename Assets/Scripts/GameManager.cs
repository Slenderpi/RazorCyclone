using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance;
    public static PlayerCharacterCtrlr CurrentPlayer;
    
    [SerializeField]
    Transform spawnPoint;
    [SerializeField]
    PlayerCharacterCtrlr playerPrefab;
    [SerializeField]
    EnemyBase enemyPrefab;
    [SerializeField]
    Transform enemySpawnPoint;
    public Camera rearCamera;
    
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
        CurrentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<PlayerCharacterCtrlr>();
    }
    
    public void OnEnemyDied() {
        CurrentPlayer.AddFuel(100f);
        //Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
    }
    
}
