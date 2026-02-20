using System.Collections;
using UnityEngine;

public class UIPausePanel : UIPanel {
    
    bool isReturningToMain = false;
    
    
    
    public void OnButton_ReturnMain() {
        if (isReturningToMain) return;
        SREndlessMode sre = GameManagerOLD.Instance.currentSceneRunner as SREndlessMode;
        if (sre)
            sre.SaveGameNow();
        StartCoroutine(delayReturnToMenu());
    }
    
    IEnumerator delayReturnToMenu() {
        isReturningToMain = true;
        GameManagerOLD.Instance.MainCanvas.FadeToBlack();
        yield return new WaitForSecondsRealtime(UIMainCanvas.FADER_FADE_DURATION + 0.05f);
        GameManagerOLD.Instance.MainCanvas.FadeToClear();
        GameManagerOLD.Instance.currentSceneRunner.SwitchToScene("MainMenuScene");
    }
    
    public void OnButton_ResumeGame() {
        GameManagerOLD.Instance.ResumeGame();
    }
    
    public override void OnGameResumed() {
        // SetActive(false);
    }
    
    public override void OnGamePaused() {
        // SetActive(true);
    }
    
    public override void OnPlayerSpawned(PlayerCharacterCtrlr plr) {
        if (GameManagerOLD.Instance.gameIsPaused) {
            GameManagerOLD.Instance.ResumeGame();
        }
        // SetActive(false);
    }
    
    public override void OnPlayerDestroying(PlayerCharacterCtrlr plr) {}
    
    void OnEnable() {
        isReturningToMain = false;
        GameManagerOLD.Instance.Audio2D.SetUMastLPTo(true);
    }
    
}