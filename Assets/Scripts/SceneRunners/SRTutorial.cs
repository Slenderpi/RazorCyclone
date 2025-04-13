using System.Collections;
using UnityEngine;

public class SRTutorial : SceneRunner {
    
    public delegate void CallbackFunc();
    
    [Header("Enemy Spawners")]
    [SerializeField]
    GameObject vacuumSpawnGroup;
    TUT_EnemySpawner[] vacuumOnlySpawners;
    [SerializeField]
    GameObject cannonSpawnGroup;
    TUT_EnemySpawner[] cannonOnlySpawners;
    [SerializeField]
    GameObject killAllSpawnGroup;
    TUT_EnemySpawner[] killAllSpawners;
    
    [HideInInspector]
    public EDamageType requiredDamageType = EDamageType.Enemy; // Initialize to impossible type
    [HideInInspector]
    public int enemiesRequiredThisState;
    [HideInInspector]
    public int enemiesKilled;
    
    UITutorialPanel TutorialPanel;

    [SerializeField]
    [Tooltip("For use in testing the tutorial.")]
    ETutorialState StartingState = ETutorialState.IntroduceControls;
    ETutorialState currState = ETutorialState.NONE;
    
    
    
    public override void BeginScene() {
        TutorialPanel = GameManager.Instance.MainCanvas.TutorialPanel;
        TutorialPanel.srt = this;
        TutorialPanel.SetAllPanelsInactive();
        TutorialPanel.SetActive(true);
        getAndSetSpawnGroups();
        SpawnPlayer();
        StartCoroutine(delayedStartTutorial());
    }
    
    IEnumerator delayedStartTutorial() {
        GameManager.Instance.MainCanvas.FadeToClear();
        yield return new WaitForSecondsRealtime(UIMainCanvas.FADER_FADE_DURATION + 0.5f);
#if UNITY_EDITOR
        GoToState(StartingState);
#else
        GoToState((ETutorialState)1); // Go to first state in build version
#endif
    }
    
    public void OnEnemyKilled(bool wasByCorrectType) {
        if (wasByCorrectType) {
            enemiesKilled++;
            // print($"Correct damage type! Enemies killed: {enemiesKilled} / {enemiesRequiredThisState}");
            TutorialPanel.PlayerKilledEnemy(true);
            if (enemiesKilled < enemiesRequiredThisState) return;
            print(" -- Objective complete! --");
            WaitAndCall(OnKilledAllEnemies, 0.5f);
        } else {
            TutorialPanel.PlayerKilledEnemy(false);
            // Debug.LogWarning("Wrong damage type, try again.");
        }
    }
    
    public void GoToState(ETutorialState nextState) {
        switch(nextState) {
        case ETutorialState.NONE:
            break;
        case ETutorialState.IntroduceControls:
            StartCoroutine(AnnounceControlsIntro());
            break;
        case ETutorialState.IntroduceVacuum:
            // WaitAndCall(AnnounceVacuumIntro, 1);
            AnnounceVacuumIntro();
            break;
        case ETutorialState.IntroduceCannon:
            // WaitAndCall(AnnounceCannonIntro, 1);
            AnnounceCannonIntro();
            break;
        case ETutorialState.KillTheWave:
            // WaitAndCall(AnnounceKillAllIntro, 1);
            AnnounceKillAllIntro();
            break;
        case ETutorialState.FINISHED:
            StartCoroutine(onTutorialCompleted());
            break;
        }
        TutorialPanel.OnTutorialStateChanged(nextState);
        currState = nextState;
    }
    
    public void OnKilledAllEnemies() {
        switch (currState) {
        case ETutorialState.IntroduceVacuum:
            GoToState(ETutorialState.IntroduceCannon);
            break;
        case ETutorialState.IntroduceCannon:
            GoToState(ETutorialState.KillTheWave);
            break;
        case ETutorialState.KillTheWave:
            GoToState(ETutorialState.FINISHED);
            break;
        }
    }
    
    IEnumerator AnnounceControlsIntro() {
        yield return new WaitForSeconds(3);
        GoToState(ETutorialState.IntroduceVacuum);
    }
    
    public void AnnounceVacuumIntro() {
        // TutorialPanel.OnTutorialStateChanged(ETutorialState.IntroduceVacuum);
        requiredDamageType = EDamageType.Vacuum;
        enemiesRequiredThisState = vacuumOnlySpawners.Length;
        enemiesKilled = 0;
        print($"OBJECTIVE: Kill {enemiesRequiredThisState} Bugs using the VACUUM");
        foreach (TUT_EnemySpawner es in vacuumOnlySpawners)
            es.SpawnEnemy();
    }
    
    public void AnnounceCannonIntro() {
        // TutorialPanel.OnTutorialStateChanged(ETutorialState.IntroduceCannon);
        requiredDamageType = EDamageType.Projectile;
        enemiesRequiredThisState = cannonOnlySpawners.Length;
        enemiesKilled = 0;
        print($"OBJECTIVE: Kill {enemiesRequiredThisState} Bugs using the CANNON");
        foreach (TUT_EnemySpawner es in cannonOnlySpawners)
            es.SpawnEnemy();
    }
    
    public void AnnounceKillAllIntro() {
        // TutorialPanel.OnTutorialStateChanged(ETutorialState.KillTheWave);
        requiredDamageType = EDamageType.Any;
        enemiesRequiredThisState = killAllSpawners.Length;
        enemiesKilled = 0;
        print($"OBJECTIVE: Kill {enemiesRequiredThisState} Bugs using any weapon.");
        foreach (TUT_EnemySpawner es in killAllSpawners)
            es.SpawnEnemy();
    }
    
    IEnumerator onTutorialCompleted() {
        // TutorialPanel.OnTutorialStateChanged(ETutorialState.FINISHED);
        print($"Congratulations! You have completed the tutorial. Now loading the endless level (in 3 seconds).");
        yield return new WaitForSeconds(3);
        GameManager.Instance.MainCanvas.FadeToBlack();
        yield return new WaitForSecondsRealtime(UIMainCanvas.FADER_FADE_DURATION);
        TutorialPanel.SetActive(false);
        SwitchToScene("True Endless");
    }
    
    void getAndSetSpawnGroups() {
        vacuumOnlySpawners = vacuumSpawnGroup.GetComponentsInChildren<TUT_EnemySpawner>();
        cannonOnlySpawners = cannonSpawnGroup.GetComponentsInChildren<TUT_EnemySpawner>();
        killAllSpawners = killAllSpawnGroup.GetComponentsInChildren<TUT_EnemySpawner>();
    }
    
    /// <summary>
    /// A coroutine to let you wait for some seconds before calling a specific function.
    /// </summary>
    /// <param name="funcToCall">The function you want to call after the duration has passed.</param>
    /// <param name="duration">Duration in seconds to wait.</param>
    /// <returns></returns>
    public void WaitAndCall(CallbackFunc funcToCall, float duration) {
        StartCoroutine(waiter(funcToCall, duration));
    }
    
    IEnumerator waiter(CallbackFunc funcToCall, float duration) {
        yield return new WaitForSeconds(duration);
        funcToCall();
    }
    
}

public enum ETutorialState {
    NONE,
    IntroduceControls,
    IntroduceVacuum,
    IntroduceCannon,
    KillTheWave,
    FINISHED
}