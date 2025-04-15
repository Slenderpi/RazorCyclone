using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UITutorialPanel : UIPanel {
    
    [Header("Tutorial UI References")]
    [SerializeField]
    GameObject demoUI;
    [SerializeField]
    VideoPlayer vidPlayer;
    [SerializeField]
    TMP_Text tutorialText;
    [SerializeField]
    Button confirmButton;
    [SerializeField]
    GameObject taskUI;
    [SerializeField]
    TMP_Text TaskText;
    [SerializeField]
    TMP_Text TaskProgressText;
    [SerializeField]
    TMP_Text IncorrectWeaponText;
    [SerializeField]
    Animator IncorrectWeaponAnimator;
    [SerializeField]
    GameObject DeathFuelUI;
    [SerializeField]
    GameObject controlsPanel;
    
    VideoClip prevVideoClip;
    VideoClip nextVideoClip;
    
    [HideInInspector]
    public SRTutorial srt;
    
    
    
    public override void Init() {
        base.Init();
    }
    
    public void OnBeginScene() {
        SetUIInactive();
#if UNITY_EDITOR
        nextVideoClip = srt.StartingState switch {
            ETutorialState.VacuumMovement1 => Resources.Load<VideoClip>("TutorialVideos/vacuum_movement_1"),
            ETutorialState.VacuumMovement2 => Resources.Load<VideoClip>("TutorialVideos/vacuum_movement_2"),
            ETutorialState.CannonMovement1 => Resources.Load<VideoClip>("TutorialVideos/cannon_movement_1"),
            ETutorialState.CannonMovement2 => Resources.Load<VideoClip>("TutorialVideos/cannon_movement_2"),
            ETutorialState.VacuumKill => Resources.Load<VideoClip>("TutorialVideos/vacuum_kill_1"),
            ETutorialState.CannonKill1 => Resources.Load<VideoClip>("TutorialVideos/cannon_kill_1"),
            ETutorialState.CannonKill2 => Resources.Load<VideoClip>("TutorialVideos/cannon_kill_2"),
            _ => Resources.Load<VideoClip>("TutorialVideos/vacuum_movement_1")
        };
#else
        nextVideoClip = Resources.Load<VideoClip>("TutorialVideos/vacuum_movement_1");
#endif
        TaskProgressText.text = "";
    }
    
    public void OnButton_ConfirmVideo() {
        hideDemoUI();
        srt.OnDemoDoneShowing();
    }
    
    public void OnButton_Respawn() {
        DeathFuelUI.SetActive(false);
        srt.respawnPlayer();
    }
    
    public void ShowDeathFuelUI() {
        DeathFuelUI.SetActive(true);
    }
    
    public void VacuumMovement1() {
        prevVideoClip = nextVideoClip;
        vidPlayer.clip = prevVideoClip;
        tutorialText.text = "Use LMB to use your Vacuum\nUse WASD to rotate your bike";
        StartCoroutine(waitEnableConfirmButton(vidPlayer.length));
        showDemoUI();
        vidPlayer.Play();
        StartCoroutine(loadNextVideoClipAsync("TutorialVideos/vacuum_movement_2"));
    }
    
    public void VacuumMovement2() {
        prevVideoClip = nextVideoClip;
        vidPlayer.clip = prevVideoClip;
        tutorialText.text = "Use Space/Shift to rotate your bike vertically";
        StartCoroutine(waitEnableConfirmButton(vidPlayer.length));
        showDemoUI();
        vidPlayer.Play();
        StartCoroutine(loadNextVideoClipAsync("TutorialVideos/cannon_movement_1"));
    }
    
    public void CannonMovement1() {
        prevVideoClip = nextVideoClip;
        vidPlayer.clip = prevVideoClip;
        tutorialText.text = "RMB to use your Cannon\nThe Cannon aims BEHIND the bike";
        StartCoroutine(waitEnableConfirmButton(vidPlayer.length));
        showDemoUI();
        vidPlayer.Play();
        StartCoroutine(loadNextVideoClipAsync("TutorialVideos/cannon_movement_2"));
    }
    
    public void CannonMovement2() {
        prevVideoClip = nextVideoClip;
        vidPlayer.clip = prevVideoClip;
        tutorialText.text = "Use Space/Shift with the Cannon\nThis is how you jump";
        StartCoroutine(waitEnableConfirmButton(vidPlayer.length));
        showDemoUI();
        vidPlayer.Play();
        StartCoroutine(loadNextVideoClipAsync("TutorialVideos/vacuum_kill_1"));
    }
    
    public void VacuumKill() {
        prevVideoClip = nextVideoClip;
        vidPlayer.clip = prevVideoClip;
        tutorialText.text = "The Vacuum pulls and kills enemies";
        StartCoroutine(waitEnableConfirmButton(vidPlayer.length));
        showDemoUI();
        vidPlayer.Play();
        StartCoroutine(loadNextVideoClipAsync("TutorialVideos/cannon_kill_1"));
    }
    
    public void CannonKill1() {
        prevVideoClip = nextVideoClip;
        vidPlayer.clip = prevVideoClip;
        tutorialText.text = "The Cannon shoots enemies at range\nIt shoots behind you. Use S to bring it forward";
        StartCoroutine(waitEnableConfirmButton(vidPlayer.length));
        showDemoUI();
        vidPlayer.Play();
        StartCoroutine(loadNextVideoClipAsync("TutorialVideos/cannon_kill_2"));
    }
    
    public void CannonKill2() {
        prevVideoClip = nextVideoClip;
        vidPlayer.clip = prevVideoClip;
        tutorialText.text = "Use S + Space to shoot downward";
        StartCoroutine(waitEnableConfirmButton(vidPlayer.length));
        showDemoUI();
        vidPlayer.Play();
    }
    
    IEnumerator loadNextVideoClipAsync(string path) {
        ResourceRequest rr = Resources.LoadAsync<VideoClip>(path);
        yield return rr;
        nextVideoClip = rr.asset as VideoClip;
    }
    
    public void VacuumMovement1_Task() {
        TaskText.text = "Move using the Vacuum (LMB) and WASD";
    }
    
    public void VacuumMovement2_Task() {
        TaskText.text = "Move using the Vacuum (LMB) and Space";
    }
    
    public void CannonMovement1_Task() {
        TaskText.text = "Move using the Cannon (RMB) and WASD";
    }
    
    public void CannonMovement2_Task() {
        TaskText.text = "Move using the Cannon (RMB) and Space";
    }
    
    public void VacuumKill_Task() {
        TaskText.text = "Kill the harmless Bugs with the Vacuum (LMB)";
        TaskProgressText.text = $"0 / {srt.enemiesRequiredThisState}";
        IncorrectWeaponText.text = "Incorrect weapon!\nUse the Vacuum";
        controlsPanel.SetActive(true);
    }
    
    public void CannonKill1_Task() {
        TaskText.text = "Kill the Bugs with the back-facing Cannon (RMB)";
        TaskProgressText.text = $"0 / {srt.enemiesRequiredThisState}";
        IncorrectWeaponText.text = "Incorrect weapon!\nUse the Cannon";
        controlsPanel.SetActive(true);
    }
    
    public void CannonKill2_Task() {
        TaskText.text = "Kill the Bugs with the Cannon (RMB)";
        TaskProgressText.text = $"0 / {srt.enemiesRequiredThisState}";
        IncorrectWeaponText.text = "Incorrect weapon!\nUse the Cannon";
        controlsPanel.SetActive(true);
    }
    
    public void PracticeKill_Task() {
        TaskText.text = "Kill all Bugs with any weapon";
        TaskProgressText.text = $"0 / {srt.enemiesRequiredThisState}";
        controlsPanel.SetActive(true);
    }
    
    public void PlayerKilledEnemy(bool wasCorrectWeapon, ETutorialState currState) {
        string s = $"{srt.enemiesKilled} / {srt.enemiesRequiredThisState}";
        if (!wasCorrectWeapon)
            IncorrectWeaponAnimator.SetTrigger("Warn");
        TaskProgressText.text = currState switch {
            ETutorialState.VacuumKill => s,
            ETutorialState.CannonKill1 => s,
            ETutorialState.CannonKill2 => s,
            ETutorialState.PracticeKill => s,
            _ => "",
        };
    }
    
    public void CongradulatePlayer() {
        TaskText.text = "Congratulations";
        TaskProgressText.text = "";
    }
    
    public void SetUIInactive() {
        controlsPanel.SetActive(false);
        taskUI.SetActive(false);
        demoUI.SetActive(false);
        DeathFuelUI.SetActive(false);
    }
    
    void showDemoUI() {
        GameManager.Instance.SetPauseInputActionsEnabled(false);
        GameManager.CurrentPlayer.SetPlayerControlsEnabled(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameManager.Instance.gameTimeScale = Time.timeScale;
        Time.timeScale = 0;
        demoUI.SetActive(true);
        taskUI.SetActive(false);
        controlsPanel.SetActive(false);
        confirmButton.interactable = false;
    }
    
    void hideDemoUI() {
        GameManager.Instance.SetPauseInputActionsEnabled(true);
        GameManager.CurrentPlayer.SetPlayerControlsEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = GameManager.Instance.gameTimeScale;
        demoUI.SetActive(false);
        taskUI.SetActive(true);
        if (prevVideoClip)
            Resources.UnloadAsset(prevVideoClip);
    }
    
    IEnumerator waitEnableConfirmButton(double videoTimeSeconds) {
        yield return new WaitForSecondsRealtime((float)videoTimeSeconds);
        confirmButton.interactable = true;
    }
    
}