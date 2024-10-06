using UnityEngine;

public abstract class UIPanel : MonoBehaviour {
    
    public abstract void OnGamePaused();
    
    public abstract void OnGameResumed();
    
    public abstract void OnPlayerSpawned(PlayerCharacterCtrlr plr);
    
    public abstract void OnPlayerDestroying(PlayerCharacterCtrlr plr);
    
    public void SetActive(bool newActive) {
        gameObject.SetActive(newActive);
    }
    
    void Awake() {
        GameManager.A_GamePaused += OnGamePaused;
        GameManager.A_GameResumed += OnGameResumed;
        GameManager.A_PlayerSpawned += OnPlayerSpawned;
        GameManager.A_PlayerDestroying += OnPlayerDestroying;
    }
    
}