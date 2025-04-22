using System.Collections;
using UnityEngine;

public class UIPausePanel : UIPanel {
    
    bool isReturningToMain = false;
    
    
    
    public void OnButton_ReturnMain() {
        if (isReturningToMain) return;
        StartCoroutine(delayReturnToMenu());
    }
    
    IEnumerator delayReturnToMenu() {
        isReturningToMain = true;
        GameManager.Instance.MainCanvas.FadeToBlack();
        yield return new WaitForSecondsRealtime(UIMainCanvas.FADER_FADE_DURATION + 0.05f);
        GameManager.Instance.MainCanvas.FadeToClear();
        GameManager.Instance.currentSceneRunner.SwitchToScene("MainMenuScene");
    }
    
    public void OnButton_ResumeGame() {
        GameManager.Instance.ResumeGame();
    }
    
    public override void OnGameResumed() {
        // SetActive(false);
    }
    
    public override void OnGamePaused() {
        // SetActive(true);
    }
    
    public override void OnPlayerSpawned(PlayerCharacterCtrlr plr) {
        if (GameManager.Instance.gameIsPaused) {
            GameManager.Instance.ResumeGame();
        }
        // SetActive(false);
    }
    
    public override void OnPlayerDestroying(PlayerCharacterCtrlr plr) {}
    
    void OnEnable() {
        isReturningToMain = false;
        GameManager.Instance.Audio2D.SetUMastLPTo(true);
    }
    
}