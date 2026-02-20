using UnityEngine;

public abstract class UIPanel : MonoBehaviour {
    
    protected bool hasInitialized = false;
    


    public virtual void Init() {
        if (hasInitialized) return;
        hasInitialized = true;
        GameManagerOLD.A_GamePaused += OnGamePaused;
        GameManagerOLD.A_GameResumed += OnGameResumed;
        GameManagerOLD.A_PlayerSpawned += OnPlayerSpawned;
        GameManagerOLD.A_PlayerDestroying += OnPlayerDestroying;
    }
    
    public virtual void OnGamePaused() {}
    
    public virtual void OnGameResumed() {}
    
    public virtual void OnPlayerSpawned(PlayerCharacterCtrlr plr) {}
    
    public virtual void OnPlayerDestroying(PlayerCharacterCtrlr plr) {}
    
    public void SetActive(bool newActive) {
        gameObject.SetActive(newActive);
    }
    
}