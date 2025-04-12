using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDEBUGPanel : UIPanel {
    
    public static UIDEBUGPanel inst;
    
    public bool ShowPanel = false;
    
    [Header("References")]
    public TMP_Text ShowHideButtonText;
    public GameObject DebugPanelContainer;
    public TMP_Text SurvivalTimerText;
    public GameObject F1Hint;
    // public TMP_Text SetTimeScaleText;
    // public TMP_InputField FieldTimeScale;
    public TMP_Text TimeScaleTitleText;
    public TMP_InputField FieldTimeScaleCustom;
    public GameObject TogIndInputOverlay;
    public GameObject TogIndInvincibility;
    public GameObject TogIndInfFuel;
    public TMP_Text SpawnWaveButtonText;
    public TMP_InputField FieldWaveNum;
    
    SecondOrderDynamicsF sod;
    public Slider MasterSlider;
    public Slider SlaveSlider;
    [Range(0, 5)]
    public float f = 1;
    [Range(0, 1)]
    public float z = 1;
    [Range(-2, 2)]
    public float r = 1;
    
#if UNITY_EDITOR || KEEP_DEBUG
    WaveSpawnerManager wsm = null;
    int desiredWaveToSpawn = 1;
#endif
    
#if UNITY_EDITOR || KEEP_DEBUG
    
    void Start() {
#if !UNITY_EDITOR && KEEP_DEBUG
        ShowPanel = false;
#endif
#if !UNITY_EDITOR && !KEEP_DEBUG
        Destroy(gameObject);
#else
        DebugPanelContainer.SetActive(ShowPanel);
        ShowHideButtonText.text = (ShowPanel ? "HIDE" : "SHOW") + "\nDebug Panel";
        setWSM();
        GameManager.A_PlayerSpawned += (PlayerCharacterCtrlr plr) => {
            setWSM();
        };
        inst = this;
        
        sod = new SecondOrderDynamicsF(f, z, r, MasterSlider.value);
#endif
    }
    
    void Update() {
        sod.SetDynamics(f, z, r);
        if (Time.deltaTime > 0) SlaveSlider.value = sod.Update(MasterSlider.value, Time.deltaTime);
    }
    
    float lerp(float a, float b, float dt) {
        return Mathf.Lerp(a, b, Mathf.Min(1, dt / 0.1f));
    }
    
    void LateUpdate() {
        if (wsm) {
            SurvivalTimerText.text = "Time Survived: " + wsm.OwningEndlessMode._TimeSinceSceneStarted;
        }
    }
    
    public void OnButton_ToggleDebugPanel() {
        ShowPanel = !ShowPanel;
        DebugPanelContainer.SetActive(ShowPanel);
        ShowHideButtonText.text = (ShowPanel ? "HIDE" : "SHOW") + "\nDev Panel";
    }
    
    public void OnButton_ToggleInputOverlay() {
        UIGamePanel gp = GameManager.Instance.MainCanvas.GamePanel;
        gp.InputOverlay.SetActive(!gp.InputOverlay.activeSelf);
        TogIndInputOverlay.SetActive(gp.InputOverlay.activeSelf);
    }
    
    public void OnButton_ToggleInvincible() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return;
        plr.IsInvincible = !plr.IsInvincible;
        TogIndInvincibility.SetActive(plr.IsInvincible);
    }
    
    public void OnButton_ToggleInfFuel() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return;
        plr.NoFuelCost = !plr.NoFuelCost;
        TogIndInfFuel.SetActive(plr.NoFuelCost);
    }
    
    public void OnButton_SpawnNextWave() {
        // if (!wsm) {
        //     Debug.LogWarning("WARN: There is no wave spawner in the current scene. Cannot spawn a 'next wave'.");
        //     return;
        // }
        // if (wsm.CurrentPreloadedWaveNumber != -1) {
        //     wsm.ActivateWave();
        //     wsm.PreloadWave(wsm.CurrentPreloadedWaveNumber + 1);
        //     // Debug.Log($"DEBUG: Clicked debug button for force spawning next wave. Next wave ({wsm.CurrentPreloadedWaveNumber}) will spawn at time {wsm.currentWaveSpawnTime}.");
        //     Debug.Log($"DEBUG: Clicked debug button for force spawning next wave. Next wave number is ({wsm.CurrentPreloadedWaveNumber}).");
        // } else {
        //     Debug.Log("DEBUG: Clicked debug button for force spawning next wave but there is no preloaded wave left.");
        // }
    }
    
    public void OnButton_SpawnWaveNumber() {
        if (!wsm.activateWaveFinished) {
            Debug.LogWarning("Warning: a wave is currently being activated. To avoid issues, please wait until the current wave has finished activating.");
            return;
        }
        wsm.UnloadWave();
        wsm.PreloadWave(desiredWaveToSpawn);
        wsm.ActivateWave();
    }
    
    public void OnField_WaveNumberEndEdit(string num) {
        if (!wsm) {
            Debug.LogWarning("WARN: There is no wave spawner in the current scene. Cannot spawn a wave.");
            return;
        }
        setWaveNumToLoad(int.Parse(num));
    }
    
    public void OnField_TimeScaleCustomEndEdit(string scaleStr) {
        float scale = float.Parse(scaleStr);
        if (scale < 0) {
            scale = 0;
            FieldTimeScaleCustom.SetTextWithoutNotify("0");
        }
        On_SetTimeScale(scale);
    }
    
    void setWaveNumToLoad(int num) {
        if (num < 1)
            num = 1;
        else if (num > wsm.waveEntries.Length)
            num = wsm.waveEntries.Length;
        FieldWaveNum.SetTextWithoutNotify(num.ToString());
        desiredWaveToSpawn = num;
    }
    
    public void On_SetTimeScale(float scale) {
        string scalestr = scale.ToString("0.0");
        // SetTimeScaleText.text = "Set Time Scale (" + scalestr + ")";
        // FieldTimeScale.SetTextWithoutNotify(scalestr);
        TimeScaleTitleText.SetText($"Time Scale ({scalestr})");
        GameManager.Instance.SetPreferredTimeScale(scale);
    }
    
    void setWSM() {
        SREndlessMode sre = GameManager.Instance.currentSceneRunner as SREndlessMode;
        wsm = sre ? sre.WaveSpawnManager : null;
        initUI();
    }
    
    void initUI() {
        PlayerCharacterCtrlr plr = GameManager.CurrentPlayer;
        if (!plr) return;
        TogIndInputOverlay.SetActive(GameManager.Instance.MainCanvas.GamePanel.InputOverlay.activeSelf);
        TogIndInvincibility.SetActive(plr.IsInvincible);
        TogIndInfFuel.SetActive(plr.NoFuelCost);
        if (!wsm) return;
        SpawnWaveButtonText.text = "Spawn Wave (1-" + wsm.waveEntries.Length + "):";
    }
#endif
    
}