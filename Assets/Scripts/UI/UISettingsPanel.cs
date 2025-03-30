using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
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
    
    [Header("Controls References")]
    public TMP_Text MouseSenseLabel;
    public Slider MouseSenseSlider;
    
    [Header("Video References")]
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public Toggle vsyncToggle;
    public Slider fpsLimitSlider;
    public TMP_InputField fpsLimitInputField;
    public Slider fovSlider;
    public TMP_InputField fovInputField;
    public TMP_Dropdown shadowResolutionDropdown;
    
    [Header("Audio References")]
    public Slider masterVolSlider;
    public TMP_InputField masterVolInputField;
    public Slider sfxVolSlider;
    public TMP_InputField sfxVolInputField;
    public Slider musicVolSlider;
    public TMP_InputField musicVolInputField;
    
    Resolution[] resolutionOptions;
    
    
    
    /*****  VIDEO SETTINGS  *****/
    
    public void OnToggle_FullScreen(bool isOn) {
        Resolution currRes = Screen.currentResolution;
        Screen.SetResolution(currRes.width, currRes.height, isOn);
    }
    
    public void OnDropdown_Resolution(int option) {
        Resolution resop = resolutionOptions[option];
        print("Selected: " + resop.width + " x " + resop.height + " " + resop.refreshRate + " Hz");
        Screen.SetResolution(resop.width, resop.height, Screen.fullScreen, resop.refreshRate);
    }
    
    public void OnToggle_VSync(bool isOn) {
        QualitySettings.vSyncCount = isOn ? 1 : 0;
    }
    
    public void OnSlider_FPSLimit(float val) {
        setFrameLimit((int)val);
    }
    
    public void OnField_FPSLimit(string str) {
        if (int.TryParse(str, out int fps)) {
            setFrameLimit(fps);
        }
    }
    
    public void OnSlider_FOV(float val) {
        setFOV(Mathf.RoundToInt(val));
    }
    
    public void OnField_FOV(string str) {
        if (int.TryParse(str, out int fps)) {
            setFOV(fps);
        }
    }
    
    public void OnDropdown_ShadowResolution(int option) {
        switch (option) {
        case 0:
            QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
            break;
        case 1:
            QualitySettings.shadowResolution = ShadowResolution.High;
            break;
        case 2:
            QualitySettings.shadowResolution = ShadowResolution.Medium;
            break;
        case 3:
            QualitySettings.shadowResolution = ShadowResolution.Low;
            break;
        }
    }
    
    
    
    /*****  AUDIO SETTINGS  *****/
    
    public void OnSlider_VolMaster(float val) {
        setVolumeMaster(Mathf.RoundToInt(val));
    }
    
    public void OnField_VolMaster(string str) {
        setVolumeMaster(Math.Clamp(int.Parse(str), 0, 100));
    }
    
    public void OnSlider_VolSFX(float val) {
        setVolumeSFX(Mathf.RoundToInt(val));
    }
    
    public void OnField_VolSFX(string str) {
        setVolumeSFX(Math.Clamp(int.Parse(str), 0, 100));
    }
    
    public void OnSlider_VolMusic(float val) {
        setVolumeMusic(Mathf.RoundToInt(val));
    }
    
    public void OnField_VolMusic(string str) {
        setVolumeMusic(Math.Clamp(int.Parse(str), 0, 100));
    }
    
    
    
    /*****  CATEGORY BUTTONS  *****/
    
    public void OnButton_Controls() {
        SetSettingsCategory(EActiveCategory.Controls);
    }
    
    public void OnButton_Video() {
        SetSettingsCategory(EActiveCategory.Video);
    }
    
    public void OnButton_Audio() {
        SetSettingsCategory(EActiveCategory.Audio);
    }
    
    
    
    /*****  OTHER  *****/
    
    public void SetSettingsCategory(EActiveCategory newCategory) {
        setAllCategoriesInactive();
        setAllButtonsInteractable();
        switch (newCategory) {
        case EActiveCategory.Controls:
            ControlsButton.interactable = false;
            CategoryControls.SetActive(true);
            break;
        case EActiveCategory.Video:
            updateVideoOptions();
            VideoButton.interactable = false;
            CategoryVideo.SetActive(true);
            break;
        case EActiveCategory.Audio:
            updateAudioOptions();
            AudioButton.interactable = false;
            CategoryAudio.SetActive(true);
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
    
    void setResolutionOptions() {
        resolutionOptions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> optstrs = new List<string>();
        for (int i = 0; i < resolutionOptions.Length; i++) {
            Resolution res = resolutionOptions[i];
            optstrs.Add(res.width + " x " + res.height + " " + res.refreshRate + " Hz");
        }
        resolutionDropdown.AddOptions(optstrs);
    }
    
    void setFrameLimit(int val) {
        val = Math.Clamp(val, 20, 200);
        if (val == 200) {
            Application.targetFrameRate = -1;
            fpsLimitInputField.SetTextWithoutNotify("Unlimited");
            fpsLimitSlider.SetValueWithoutNotify(200);
        } else {
            Application.targetFrameRate = val;
            fpsLimitInputField.SetTextWithoutNotify(val.ToString());
            fpsLimitSlider.SetValueWithoutNotify(val);
        }
    }
    
    void setFOV(int val) {
        val = Math.Clamp(val, 40, 130);
        GameManager.Instance.CurrentFOV = val;
        fovInputField.SetTextWithoutNotify(val.ToString());
        fovSlider.SetValueWithoutNotify(val);
    }
    
    void setVolumeMaster(int val) {
        masterVolSlider.SetValueWithoutNotify(val);
        masterVolInputField.SetTextWithoutNotify(val.ToString());
        AudioPlayer2D.Instance.SetMasterVolume(val);
    }
    
    void setVolumeSFX(int val) {
        sfxVolSlider.SetValueWithoutNotify(val);
        sfxVolInputField.SetTextWithoutNotify(val.ToString());
        AudioPlayer2D.Instance.SetSFXVolume(val);
    }
    
    void setVolumeMusic(int val) {
        musicVolSlider.SetValueWithoutNotify(val);
        musicVolInputField.SetTextWithoutNotify(val.ToString());
        AudioPlayer2D.Instance.SetMusicVolume(val);
    }
    
    void updateVideoOptions() {
        fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
        setResolutionOptions();
        int resopi = 0;
        bool found = false;
        Resolution currRes = Screen.currentResolution;
        for (; resopi < resolutionOptions.Length; resopi++) {
            if (resolutionOptions[resopi].width == currRes.width &&
                resolutionOptions[resopi].height == currRes.height) {
                found = true;
                break;
            }
        }
        if (!found) print("RESOLUTION NOT FOUND.");
        resolutionDropdown.SetValueWithoutNotify(resopi);
        vsyncToggle.SetIsOnWithoutNotify(QualitySettings.vSyncCount > 0);
        setFrameLimit(Application.targetFrameRate <= 0 ? 200 : Application.targetFrameRate);
        float fov = Camera.main.fieldOfView;
        fovInputField.SetTextWithoutNotify(Mathf.RoundToInt(fov).ToString());
        fovSlider.SetValueWithoutNotify(fov);
        int shadresopi = 0;
        switch (QualitySettings.shadowResolution) {
        case ShadowResolution.Low:
            shadresopi = 3;
            break;
        case ShadowResolution.Medium:
            shadresopi = 2;
            break;
        case ShadowResolution.High:
            shadresopi = 1;
            break;
        case ShadowResolution.VeryHigh:
            shadresopi = 0;
            break;
        }
        shadowResolutionDropdown.SetValueWithoutNotify(shadresopi);
    }
    
    void updateAudioOptions() {
        AudioMixer mam = AudioPlayer2D.Instance.MainAudioMixer;
        float val;
        mam.GetFloat("volMaster", out val);
        setVolumeMaster(calcPowerVolume(val));
        mam.GetFloat("volSFX", out val);
        setVolumeSFX(calcPowerVolume(val));
        mam.GetFloat("volMusic", out val);
        setVolumeMusic(calcPowerVolume(val));
    }
    
    int calcPowerVolume(float volAsLog) {
        return Mathf.RoundToInt(100 * Mathf.Pow(10, volAsLog / 20f));
    }
    
}