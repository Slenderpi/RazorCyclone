using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuPanel : UIPanel {
    
    [Header("Main Menu Buttons")]
    public Button PlayEndlessButton;
    public Button PlayTutorialButton;
    public Button SettingsButton;
    public Button QuitGameButton;
    
    
    
    public void OnButton_PlayEndless() {
        StartCoroutine(delayLoadScene(true));
    }
    
    public void OnButton_Tutorial() {
        StartCoroutine(delayLoadScene(false));
    }
    
    public void OnButton_CloseGame() {
        Application.Quit();
    }
    
    void OnEnable() {
        setButtonsEnabled(true);
        GameManager.Instance.Audio2D.SetUMastLPTo(false);
        Time.timeScale = 1;
    }
    
    void setButtonsEnabled(bool newEnabled) {
        PlayEndlessButton.interactable = newEnabled;
        PlayTutorialButton.interactable = newEnabled;
        SettingsButton.interactable = newEnabled;
        QuitGameButton.interactable = newEnabled;
    }
    
    IEnumerator delayLoadScene(bool isEndless) {
        setButtonsEnabled(false);
        GameManager.Instance.MainCanvas.FadeToBlack();
        yield return new WaitForSecondsRealtime(UIMainCanvas.FADER_FADE_DURATION);
        if (isEndless)
            ((SRMainMenu)GameManager.Instance.currentSceneRunner).LoadLevel_EndlessMode();
        else
            ((SRMainMenu)GameManager.Instance.currentSceneRunner).LoadLevel_Tutorial();
    }
    
}