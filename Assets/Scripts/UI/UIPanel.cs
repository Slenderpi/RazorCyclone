using UnityEngine;

public abstract class UIPanel : MonoBehaviour {
    
    protected bool hasInitialized = false;
    
    public virtual void Init() {
        if (hasInitialized) return;
        hasInitialized = true;
        GameManager.A_GamePaused += OnGamePaused;
        GameManager.A_GameResumed += OnGameResumed;
        GameManager.A_PlayerSpawned += OnPlayerSpawned;
        GameManager.A_PlayerDestroying += OnPlayerDestroying;
    }
    
    public virtual void OnGamePaused() {}
    
    public virtual void OnGameResumed() {}
    
    public virtual void OnPlayerSpawned(PlayerCharacterCtrlr plr) {}
    
    public virtual void OnPlayerDestroying(PlayerCharacterCtrlr plr) {}
    
    public void SetActive(bool newActive) {
        gameObject.SetActive(newActive);
    }
    
}