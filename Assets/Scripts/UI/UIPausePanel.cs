using System.Collections;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPausePanel : UIPanel {
    
    public TMP_Text MouseSenseLabel;
    public Slider MouseSenseSlider;
    
    public void SetMouseSenseText(float sens) {
        MouseSenseLabel.text = "Mouse Sensitivity: " + sens.ToString("0.00");
    }
    
    public void OnChangeSceneButtonClicked() {
        GameManager.Instance.TestSceneChange();
    }
    
    public void OnResetPlayerCharacterClicked() {
        GameManager.Instance.DestroyPlayer();
        GameManager.Instance.SpawnPlayer();
    }
    
    public void OnCloseGameButtonClicked() {
        Application.Quit();
    }

    public override void OnGameResumed() {
        SetActive(false);
    }

    public override void OnGamePaused() {
        SetActive(true);
    }

    public override void OnPlayerSpawned(PlayerCharacterCtrlr plr) {
        if (GameManager.Instance.gameIsPaused) {
            GameManager.Instance.ResumeGame();
        }
        SetActive(false);
    }

    public override void OnPlayerDestroying(PlayerCharacterCtrlr plr) {}

}