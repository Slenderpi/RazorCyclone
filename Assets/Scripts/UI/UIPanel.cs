using UnityEngine;

public abstract class UIPanel : MonoBehaviour {
    
    public abstract void OnGamePaused();
    
    public abstract void OnGameResumed();
    
    public abstract void OnPlayerSpawned(PlayerCharacterCtrlr plr);
    
    public abstract void OnPlayerDestroying(PlayerCharacterCtrlr plr);
    
    public void SetActive(bool newActive) {
        gameObject.SetActive(newActive);
    }
    
    
}