using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPausePanel : UIPanel {
    
    public void OnButton_ReturnMain() {
        GameManager.Instance.currentSceneRunner.SwitchToScene("MainMenuScene");
    }
    
    public void OnButton_ReloadLevel() {
        GameManager.Instance.SetPauseInputActionsEnabled(true);
        GameManager.Instance.currentSceneRunner.ReloadCurrentScene();
    }
    
    public void OnButton_ResetPlayer() {
        if (!GameManager.CurrentPlayer) return;
        GameManager.Instance.ResumeGame();
        GameManager.CurrentPlayer.TakeDamage(9999);
    }
    
    public void OnCloseGameButtonClicked() {
        Application.Quit();
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

}