using UnityEngine;

public class UITutorialPanel : UIPanel {
    
    [Header("Panel Groups")]
    [SerializeField]
    GameObject controlsPanel;
    [SerializeField]
    GameObject vacuumPanel;
    [SerializeField]
    GameObject cannonPanel;
    [SerializeField]
    GameObject killAllPanel;
    [SerializeField]
    GameObject finalPanel;
    
    
    
    public override void Init() {
        base.Init();
        SetAllPanelsInactive();
    }
    
    public void OnTutorialStateChanged(ETutorialState state) {
        SetAllPanelsInactive();
        switch (state) {
        case ETutorialState.NONE:
            SetAllPanelsInactive();
            break;
        case ETutorialState.IntroduceControls:
            doControlsIntro();
            break;
        case ETutorialState.IntroduceVacuum:
            doVacuumIntro();
            break;
        case ETutorialState.IntroduceCannon:
            doCannonIntro();
            break;
        case ETutorialState.KillTheWave:
            doKillAllIntro();
            break;
        case ETutorialState.FINISHED:
            doCongrats();
            break;
        }
    }
    
    void doControlsIntro() {
        controlsPanel.SetActive(true);
    }
    
    void doVacuumIntro() {
        vacuumPanel.SetActive(true);
    }
    
    void doCannonIntro() {
        cannonPanel.SetActive(true);
    }
    
    void doKillAllIntro() {
        killAllPanel.SetActive(true);
    }
    
    void doCongrats() {
        finalPanel.SetActive(true);
    }
    
    public void SetAllPanelsInactive() {
        controlsPanel.SetActive(false);
        vacuumPanel.SetActive(false);
        cannonPanel.SetActive(false);
        killAllPanel.SetActive(false);
        finalPanel.SetActive(false);
    }
    
}