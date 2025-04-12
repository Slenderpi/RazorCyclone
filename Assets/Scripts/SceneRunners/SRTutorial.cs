using System.Collections;
using UnityEngine;

public class SRTutorial : SceneRunner {
    
    public delegate void CallbackFunc();
    
    [Header("Enemy Spawners")]
    [SerializeField]
    GameObject vacuumSpawnGroup;
    TUT_EnemySpawner[] vacuumOnlySpawners;
    [SerializeField]
    GameObject canonSpawnGroup;
    TUT_EnemySpawner[] canonOnlySpawners;
    [SerializeField]
    GameObject killAllSpawnGroup;
    TUT_EnemySpawner[] killAllSpawners;
    
    [HideInInspector]
    public EDamageType requiredDamageType = EDamageType.Enemy; // Initialize to impossible type
    int enemiesRequiredThisState;
    int enemiesKilled;
    
    UITutorialPanel TutorialPanel;
    
    [SerializeField]
    [Tooltip("For use in testing the tutorial.")]
    ETutorialState StartingState = ETutorialState.IntroduceVacuum;
    ETutorialState currState = ETutorialState.NONE;
    
    
    
    public override void BeginScene() {
        TutorialPanel = GameManager.Instance.MainCanvas.TutorialPanel;
        getAndSetSpawnGroups();
        SpawnPlayer();
        StartCoroutine(delayedStartTutorial());
    }
    
    IEnumerator delayedStartTutorial() {
        GameManager.Instance.MainCanvas.FadeToClear();
        yield return new WaitForSecondsRealtime(UIMainCanvas.FADER_FADE_DURATION + 1);
#if UNITY_EDITOR
        GoToState(StartingState);
#else
        GoToState((ETutorialState)1); // Go to first state in build version
#endif
    }
    
    public void OnEnemyKilled(bool wasByCorrectType) {
        if (wasByCorrectType) {
            enemiesKilled++;
            print($"Correct damage type! Enemies killed: {enemiesKilled} / {enemiesRequiredThisState}");
            if (enemiesKilled < enemiesRequiredThisState) return;
            print(" -- Objective complete! --");
            OnKilledAllEnemies();
        } else {
            Debug.LogWarning("Wrong damage type, try again.");
        }
    }
    
    public void GoToState(ETutorialState nextState) {
        switch(nextState) {
        case ETutorialState.IntroduceVacuum:
            WaitAndCall(AnnounceVacuumIntro, 1);
            break;
        case ETutorialState.IntroduceCanon:
            WaitAndCall(AnnounceCanonIntro, 1);
            break;
        case ETutorialState.KillTheWave:
            WaitAndCall(AnnounceKillAllIntro, 1);
            break;
        case ETutorialState.FINISHED:
            WaitAndCall(AnnounceCongrats, 0.5f);
            break;
        }
        currState = nextState;
    }
    
    public void OnKilledAllEnemies() {
        switch (currState) {
        case ETutorialState.IntroduceVacuum:
            GoToState(ETutorialState.IntroduceCanon);
            break;
        case ETutorialState.IntroduceCanon:
            GoToState(ETutorialState.KillTheWave);
            break;
        case ETutorialState.KillTheWave:
            GoToState(ETutorialState.FINISHED);
            break;
        }
    }
    
    public void AnnounceVacuumIntro() {
        requiredDamageType = EDamageType.Vacuum;
        enemiesRequiredThisState = vacuumOnlySpawners.Length;
        enemiesKilled = 0;
        print($"OBJECTIVE: Kill {enemiesRequiredThisState} Bugs using the VACUUM");
        foreach (TUT_EnemySpawner es in vacuumOnlySpawners)
            es.SpawnEnemy();
    }
    
    public void AnnounceCanonIntro() {
        requiredDamageType = EDamageType.Projectile;
        enemiesRequiredThisState = canonOnlySpawners.Length;
        enemiesKilled = 0;
        print($"OBJECTIVE: Kill {enemiesRequiredThisState} Bugs using the CANON");
        foreach (TUT_EnemySpawner es in canonOnlySpawners)
            es.SpawnEnemy();
    }
    
    public void AnnounceKillAllIntro() {
        requiredDamageType = EDamageType.Any;
        enemiesRequiredThisState = killAllSpawners.Length;
        enemiesKilled = 0;
        print($"OBJECTIVE: Kill {enemiesRequiredThisState} Bugs using any weapon.");
        foreach (TUT_EnemySpawner es in killAllSpawners)
            es.SpawnEnemy();
    }
    
    public void AnnounceCongrats() {
        print($"Congratulations! You have completed the tutorial. Now loading the endless level (in 3 seconds).");
        WaitAndCall(goToEndless, 3);
    }
    
    void goToEndless() {
        SwitchToScene("True Endless");
    }
    
    void getAndSetSpawnGroups() {
        vacuumOnlySpawners = vacuumSpawnGroup.GetComponentsInChildren<TUT_EnemySpawner>();
        canonOnlySpawners = canonSpawnGroup.GetComponentsInChildren<TUT_EnemySpawner>();
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
    IntroduceVacuum,
    IntroduceCanon,
    KillTheWave,
    FINISHED
}