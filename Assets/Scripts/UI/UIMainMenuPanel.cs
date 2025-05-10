using System.Collections;
using UnityEngine;

public class UIMainMenuPanel : UIPanel {
    
    bool buttonsEnabled = true;
    
    
    
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
        buttonsEnabled = true;
        GameManager.Instance.Audio2D.SetUMastLPTo(false);
        Time.timeScale = 1;
    }
    
    IEnumerator delayLoadScene(bool isEndless) {
        if (buttonsEnabled) {
            buttonsEnabled = false;
            GameManager.Instance.MainCanvas.FadeToBlack();
            yield return new WaitForSecondsRealtime(UIMainCanvas.FADER_FADE_DURATION);
            if (isEndless)
                ((SRMainMenu)GameManager.Instance.currentSceneRunner).LoadLevel_EndlessMode();
            else
                ((SRMainMenu)GameManager.Instance.currentSceneRunner).LoadLevel_Tutorial();
        }
    }
    
}