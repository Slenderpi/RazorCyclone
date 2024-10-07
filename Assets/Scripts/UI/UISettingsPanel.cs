using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class UISettingsPanel : UIPanel {
    
    public enum EActiveCategory {
        Controls,
        Video,
        Audio
    }
    
    [Header("Settings Category References")]
    public GameObject CategoryControls;
    public GameObject CategoryVideo;
    public GameObject CategoryAudio;
    
    [Header("Category Button References")]
    public Button ControlsButton;
    public Button VideoButton;
    public Button AudioButton;
    
    [Header("Other UI References")]
    public TMP_Text MouseSenseLabel;
    public Slider MouseSenseSlider;
    
    
    
    public void OnButton_Controls() {
        SetSettingsCategory(EActiveCategory.Controls);
    }
    
    public void OnButton_Video() {
        SetSettingsCategory(EActiveCategory.Video);
    }
    
    public void OnButton_Audio() {
        SetSettingsCategory(EActiveCategory.Audio);
    }
    
    public void SetSettingsCategory(EActiveCategory newCategory) {
        setAllCategoriesInactive();
        setAllButtonsInteractable();
        switch (newCategory) {
        case EActiveCategory.Controls:
            CategoryControls.SetActive(true);
            ControlsButton.interactable = false;
            break;
        case EActiveCategory.Video:
            CategoryVideo.SetActive(true);
            VideoButton.interactable = false;
            break;
        case EActiveCategory.Audio:
            CategoryAudio.SetActive(true);
            AudioButton.interactable = false;
            break;
        }
    }
    
    public void SetMouseSenseText(float sens) {
        MouseSenseLabel.text = "Mouse Sensitivity: " + sens.ToString("0.00");
    }

    public override void Init() {
        base.Init();
        SetSettingsCategory(EActiveCategory.Controls);
    }

    void setAllCategoriesInactive() {
        CategoryControls.SetActive(false);
        CategoryVideo.SetActive(false);
        CategoryAudio.SetActive(false);
    }
    
    void setAllButtonsInteractable() {
        ControlsButton.interactable = true;
        VideoButton.interactable = true;
        AudioButton.interactable = true;
    }
    
}