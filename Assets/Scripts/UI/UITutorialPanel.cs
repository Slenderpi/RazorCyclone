using TMPro;
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
    
    [Header("Other references")]
    [SerializeField]
    TMP_Text vacuumText;
    [SerializeField]
    TMP_Text cannonText;
    [SerializeField]
    TMP_Text killAllText;
    
    [HideInInspector]
    public SRTutorial srt;
    
    ETutorialState currState;
    
    
    
    public override void Init() {
        base.Init();
        SetAllPanelsInactive();
    }
    
    public void OnTutorialStateChanged(ETutorialState state) {
        switch (state) {
        case ETutorialState.NONE:
            SetAllPanelsInactive();
            break;
        case ETutorialState.IntroduceControls:
            doControlsIntro();
            break;
        case ETutorialState.IntroduceVacuum:
            SetAllPanelsInactive(true);
            doVacuumIntro();
            break;
        case ETutorialState.IntroduceCannon:
            SetAllPanelsInactive(true);
            doCannonIntro();
            break;
        case ETutorialState.KillTheWave:
            SetAllPanelsInactive(true);
            doKillAllIntro();
            break;
        case ETutorialState.FINISHED:
            SetAllPanelsInactive(true);
            doCongrats();
            break;
        }
        currState = state;
    }
    
    void doControlsIntro() {
        SetAllPanelsInactive(true);
    }
    
    void doVacuumIntro() {
        vacuumText.text = $"Kill Bugs with the Vacuum {srt.enemiesKilled} / {srt.enemiesRequiredThisState}";
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
    
    public void PlayerKilledEnemy(bool wasCorrectWeapon) {
        string s = $"{srt.enemiesKilled} / {srt.enemiesRequiredThisState}";
        switch (currState) {
        case ETutorialState.IntroduceVacuum:
            vacuumText.text = "Kill Bugs with the Vacuum " + s + (wasCorrectWeapon ? "" : "\nincorrect weapon");
            break;
        case ETutorialState.IntroduceCannon:
            cannonText.text = "Kill Bugs with the Cannon " + s + (wasCorrectWeapon ? "" : "\nincorrect weapon");
            break;
        case ETutorialState.KillTheWave:
            killAllText.text = "Kill Bugs with any weapon " + s;
            break;
        }
    }
    
    public void SetAllPanelsInactive(bool skipControls = false) {
        controlsPanel.SetActive(skipControls);
        vacuumPanel.SetActive(false);
        cannonPanel.SetActive(false);
        killAllPanel.SetActive(false);
        finalPanel.SetActive(false);
    }
    
}