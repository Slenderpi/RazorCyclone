using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SRMainMenu : SceneRunner {
    
    public override void BeginScene() {
        GameManager.Instance.MainCanvas.SetCanvasState(UIMainCanvas.ECanvasState.MainMenu);
    }
    
    public void LoadLevel_EndlessMode() {
        SwitchToScene("EndlessLevel");
    }
    
    public void LoadLevel_Tutorial() {
        SwitchToScene("TutorialScene");
    }
    
}