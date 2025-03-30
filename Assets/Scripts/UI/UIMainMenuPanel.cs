using UnityEngine;

public class UIMainMenuPanel : UIPanel {
    
    public void OnButton_PlayEndless() {
        SRMainMenu sr = (SRMainMenu)GameManager.Instance.currentSceneRunner;
        sr.LoadLevel_EndlessMode();
    }
    
    public void OnButton_Tutorial() {
        SRMainMenu sr = (SRMainMenu)GameManager.Instance.currentSceneRunner;
        sr.LoadLevel_Tutorial();
    }
    
    public void OnButton_CloseGame() {
        Application.Quit();
    }
    
}