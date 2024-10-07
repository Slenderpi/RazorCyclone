using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainCanvas : UIPanel {
    
    public enum ECanvasState {
        None,
        MainMenu,
        Gameplay,
        Paused,
        Settings
    }
    
    public ECanvasState CurrentCanvasState;
    
    public event Action<ECanvasState> CanvasStateChanged;
    
    public UIGamePanel GamePanel;
    public UIPausePanel PausePanel;
    public UISettingsPanel SettingsPanel;
    public UIMainMenuPanel MainMenuPanel;
    
    
    
    public void OnButton_Settings() {
        SetCanvasState(ECanvasState.Settings);
    }
    
    public void OnButton_Back() {
        switch (CurrentCanvasState) {
            case ECanvasState.Paused:
                if (GameManager.Instance.gameIsPaused)
                    GameManager.Instance.ResumeGame();
                break;
            case ECanvasState.Settings:
                if (GameManager.Instance.gameIsPaused) {
                    SetCanvasState(ECanvasState.Paused);
                } else {
                    SetCanvasState(ECanvasState.MainMenu);
                }
                break;
        }
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
        SetCanvasState(ECanvasState.None); // TODO: Death screen
    }

    public override void OnPlayerSpawned(PlayerCharacterCtrlr plr) {
        SetCanvasState(ECanvasState.Gameplay);
    }
    
    public override void Init() {
        base.Init();
        SetCanvasState(ECanvasState.None);
    }
    
    void setAllInactive() {
        GamePanel.SetActive(false);
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
    }
    
}