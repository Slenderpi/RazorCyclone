using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainCanvas : UIPanel {
    
    public static readonly float FADER_FADE_DURATION = 0.6f;
    
    public enum ECanvasState {
        None,
        MainMenu,
        Gameplay,
        Paused,
        Settings,
        DiedEndless,
        Almanac
    }
    
    [HideInInspector]
    public ECanvasState CurrentCanvasState;
    
    public event Action<ECanvasState> CanvasStateChanged;
    
    [Header("Panels")]
    public UIDEVPanel DebugPanel;
    public UIGamePanel GamePanel;
    public UIPausePanel PausePanel;
    public UISettingsPanel SettingsPanel;
    public UIMainMenuPanel MainMenuPanel;
    public UIDeathPanel DeathPanel;
    public UITutorialPanel TutorialPanel;
    public UIAlmanac AlmanacPanel;
    
    [Header("Other")]
    public RawImage Fader;
    
    
    
    public void OnButton_Settings() {
        SetCanvasState(ECanvasState.Settings);
    }
    
    public void OnButton_OpenAlmanac() {
        SetCanvasState(ECanvasState.Almanac);
    }
    
    public void OnButton_Back() {
        switch (CurrentCanvasState) {
        case ECanvasState.Paused:
            if (GameManager.Instance.gameIsPaused)
                GameManager.Instance.ResumeGame();
            break;
        case ECanvasState.Settings:
            DataPersistenceManager.Instance.SaveSettings();
            SetCanvasState(SceneManager.GetActiveScene().name == "MainMenuScene" ? ECanvasState.MainMenu : ECanvasState.Paused);
            break;
        }
    }
    
    public void OnButton_CloseAlmanac() {
        SetCanvasState(SceneManager.GetActiveScene().name == "MainMenuScene" ? ECanvasState.MainMenu : ECanvasState.Paused);
    }
    
    public void SetCanvasState(ECanvasState newState) {
        CurrentCanvasState = newState;
        setAllInactive();
        switch (newState) {
        case ECanvasState.None:
            break;
        case ECanvasState.MainMenu:
            MainMenuPanel.SetActive(true);
            break;
        case ECanvasState.Gameplay:
            GamePanel.SetActive(true);
            break;
        case ECanvasState.Paused:
            PausePanel.SetActive(true);
            break;
        case ECanvasState.Settings:
            SettingsPanel.SetActive(true);
            break;
        case ECanvasState.DiedEndless:
            DeathPanel.SetActive(true);
            break;
        case ECanvasState.Almanac:
            // TODO
            // AlmanacPanel.SetActive(true);
            break;
        }
        CanvasStateChanged?.Invoke(CurrentCanvasState);
    }
    
    public override void OnGamePaused() {
        SetCanvasState(ECanvasState.Paused);
    }
    
    public override void OnGameResumed() {
        SetCanvasState(ECanvasState.Gameplay);
    }
    
    public override void OnPlayerDestroying(PlayerCharacterCtrlr plr) {
        SetCanvasState(ECanvasState.None);
    }
    
    public override void OnPlayerSpawned(PlayerCharacterCtrlr plr) {
        SetCanvasState(ECanvasState.Gameplay);
    }
    
    public override void Init() {
        base.Init();
#if UNITY_EDITOR || KEEP_DEBUG
        DebugPanel.SetActive(true);
#endif
        Fader.gameObject.SetActive(false);
        TutorialPanel.SetActive(false);
        SetCanvasState(ECanvasState.None);
    }
    
    void setAllInactive() {
        GamePanel.SetActive(false);
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        DeathPanel.SetActive(false);
        // TODO
        // AlmanacPanel.SetActive(false);
    }
    
    public void FadeToBlack() {
        StartCoroutine(animateFader(0, 1));
    }
    
    public void FadeToClear() {
        StartCoroutine(animateFader(1, 0));
    }
    
    IEnumerator animateFader(float startAlpha, float endAlpha) {
        GameManager.Instance.SetPauseInputActionsEnabled(false);
        Color c = Fader.color;
        c.a = startAlpha;
        Fader.color = c;
        Fader.gameObject.SetActive(true);
        float time = 0;
        while (time < FADER_FADE_DURATION) {
            time += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, time / FADER_FADE_DURATION);
            Fader.color = c;
            yield return null;
        }
        c.a = endAlpha;
        Fader.color = c;
        if (endAlpha < 0.5f) { // Fading to clear
            Fader.gameObject.SetActive(false);
            GameManager.Instance.SetPauseInputActionsEnabled(true);
        }
    }
    
}