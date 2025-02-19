using UnityEngine;

public class SRMainMenu : SceneRunner {
    
    public override void BeginScene() {
        GameManager.Instance.MainCanvas.SetCanvasState(UIMainCanvas.ECanvasState.MainMenu);
        // AudioPlayer2D.Instance.PlayClipMusic(AudioPlayer2D.EClipMusic.Test);
    }
    
    public void LoadLevel_EndlessMode() {
        SwitchToScene("True Endless");
    }
    
    public void LoadLevel_Tutorial() {
        SwitchToScene("TutroialAdam");
    }
    
}