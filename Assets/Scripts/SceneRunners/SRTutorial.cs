using System.Collections;
using UnityEngine;

public class SRTutorial : SceneRunner {
    
    public delegate void CallbackFunc();
    
    [Header("Triggers")]
    [SerializeField]
    TriggerNotifier vacuumMovement1Finished;
    [SerializeField]
    TriggerNotifier vacuumMovement2Finished;
    [SerializeField]
    TriggerNotifier cannonMovement1Finished;
    [SerializeField]
    TriggerNotifier cannonMovement2Finished;
    
    [Header("Player Spawnpoints")]
    [SerializeField]
    Transform spawnForVacMove1;
    [SerializeField]
    Transform spawnForVacMove2;
    [SerializeField]
    Transform spawnForCanMove1;
    [SerializeField]
    Transform spawnForCanMove2;
    [SerializeField]
    Transform spawnForKillPractice;
    
    [Header("Enemy Spawners")]
    [SerializeField]
    GameObject vacuumSpawnGroup;
    TUT_EnemySpawner[] vacuumOnlySpawners;
    [SerializeField]
    GameObject cannonSpawnGroup1;
    TUT_EnemySpawner[] cannonOnlySpawners1;
    [SerializeField]
    GameObject cannonSpawnGroup2;
    TUT_EnemySpawner[] cannonOnlySpawners2;
    [SerializeField]
    GameObject canRicSpawnGroup;
    TUT_EnemySpawner[] canRicSpawners;
    [SerializeField]
    GameObject practiceKillSpawnGroup;
    TUT_EnemySpawner[] practiceKillSpawners;
    
    [HideInInspector]
    public EDamageType requiredDamageType = EDamageType.Enemy; // Initialize to impossible type
    [HideInInspector]
    public int enemiesRequiredThisState;
    [HideInInspector]
    public int enemiesKilled;
    
    UITutorialPanel TutorialPanel;
    
    [Header("For Testing")]
    [Tooltip("For use in testing the tutorial.")]
    public ETutorialState StartingState = ETutorialState.VacuumMovement1;
    ETutorialState currState = ETutorialState.NONE;
    
    
    
    public override void BeginScene() {
        TutorialPanel = GameManager.Instance.MainCanvas.TutorialPanel;
        TutorialPanel.srt = this;
        getAndSetSpawnGroups();
        setupTriggers();
#if UNITY_EDITOR
        playerSpawnPoint = StartingState switch {
            ETutorialState.VacuumMovement1 => spawnForVacMove1,
            ETutorialState.VacuumMovement2 => spawnForVacMove2,
            ETutorialState.CannonMovement1 => spawnForCanMove1,
            ETutorialState.CannonMovement2 => spawnForCanMove2,
            _ => spawnForKillPractice
        };
        if (StartingState >= ETutorialState.VacuumKill)
            cannonMovement2Finished.gameObject.SetActive(false);
#else
        playerSpawnPoint = spawnForVacMove1;
#endif
        TutorialPanel.OnBeginScene();
        TutorialPanel.SetActive(true);
        SpawnPlayerAndConnect();
        StartCoroutine(delayedStartTutorial());
    }

    protected override void OnPlayerDied() {
        GameManager.Instance.SetPauseInputActionsEnabled(false);
        TutorialPanel.ShowDeathFuelUI();
        // GameManager.Instance.DestroyPlayer();
    }
    
    public void respawnPlayer() {
        GameManager.Instance.SetPauseInputActionsEnabled(true);
        GameManager.Instance.SpawnPlayer();
    }
    
    IEnumerator delayedStartTutorial() {
        GameManager.Instance.MainCanvas.FadeToClear();
        yield return new WaitForSecondsRealtime(UIMainCanvas.FADER_FADE_DURATION + 0.1f);
#if UNITY_EDITOR
        GoToState(StartingState);
#else
        GoToState((ETutorialState)1); // Go to first state in build version
#endif
    }
     
    public void OnEnemyKilled(bool wasByCorrectType) {
        if (wasByCorrectType) {
            enemiesKilled++;
            TutorialPanel.PlayerKilledEnemy(true, currState);
            if (enemiesKilled < enemiesRequiredThisState) return;
            WaitAndCall(OnKilledAllEnemies, 1.5f);
        } else {
            TutorialPanel.PlayerKilledEnemy(false, currState);
        }
    }
    
    public void GoToState(ETutorialState nextState) {
        switch(nextState) {
        case ETutorialState.NONE:
            break;
        case ETutorialState.VacuumMovement1:
            TutorialPanel.VacuumMovement1();
            break;
        case ETutorialState.VacuumMovement2:
            TutorialPanel.VacuumMovement2();
            break;
        case ETutorialState.CannonMovement1:
            TutorialPanel.CannonMovement1();
            break;
        case ETutorialState.CannonMovement2:
            TutorialPanel.CannonMovement2();
            break;
        case ETutorialState.VacuumKill:
            TutorialPanel.VacuumKill();
            break;
        case ETutorialState.CannonKill1:
            TutorialPanel.CannonKill1();
            break;
        case ETutorialState.CannonKill2:
            TutorialPanel.CannonKill2();
            break;
        case ETutorialState.CannonRicochet:
            TutorialPanel.CannonRico();
            break;
        case ETutorialState.PracticeKill:
            spawnEnemies_PracticeKill();
            TutorialPanel.PracticeKill_Task();
            break;
        case ETutorialState.FINISHED:
            TutorialPanel.CongradulatePlayer();
            StartCoroutine(onTutorialCompleted());
            break;
        }
        currState = nextState;
    }
    
    public void OnDemoDoneShowing() {
        switch(currState) {
        case ETutorialState.NONE:
            break;
        case ETutorialState.VacuumMovement1:
            TutorialPanel.VacuumMovement1_Task();
            break;
        case ETutorialState.VacuumMovement2:
            TutorialPanel.VacuumMovement2_Task();
            break;
        case ETutorialState.CannonMovement1:
            TutorialPanel.CannonMovement1_Task();
            break;
        case ETutorialState.CannonMovement2:
            TutorialPanel.CannonMovement2_Task();
            break;
        case ETutorialState.VacuumKill:
            spawnEnemies_VacuumKill();
            TutorialPanel.VacuumKill_Task();
            break;
        case ETutorialState.CannonKill1:
            spawnEnemies_CannonKill1();
            TutorialPanel.CannonKill1_Task();
            break;
        case ETutorialState.CannonKill2:
            spawnEnemies_CannonKill2();
            TutorialPanel.CannonKill2_Task();
            break;
        case ETutorialState.CannonRicochet:
            spawnEnemies_CannonRico();
            TutorialPanel.CannonRico_Task();
            break;
        case ETutorialState.FINISHED:
            StartCoroutine(onTutorialCompleted());
            break;
        }
    }
    
    void spawnEnemies_VacuumKill() {
        requiredDamageType = EDamageType.Vacuum;
        enemiesRequiredThisState = vacuumOnlySpawners.Length;
        enemiesKilled = 0;
        foreach (TUT_EnemySpawner es in vacuumOnlySpawners)
            es.SpawnEnemy();
    }
    
    void spawnEnemies_CannonKill1() {
        requiredDamageType = EDamageType.Projectile;
        enemiesRequiredThisState = cannonOnlySpawners1.Length;
        enemiesKilled = 0;
        foreach (TUT_EnemySpawner es in cannonOnlySpawners1)
            es.SpawnEnemy();
    }
    
    void spawnEnemies_CannonKill2() {
        requiredDamageType = EDamageType.Projectile;
        enemiesRequiredThisState = cannonOnlySpawners2.Length;
        enemiesKilled = 0;
        foreach (TUT_EnemySpawner es in cannonOnlySpawners2)
            es.SpawnEnemy();
    }
    
    void spawnEnemies_CannonRico() {
        requiredDamageType = EDamageType.Projectile;
        enemiesRequiredThisState = canRicSpawners.Length;
        enemiesKilled = 0;
        foreach (TUT_EnemySpawner es in canRicSpawners)
            es.SpawnEnemy();
    }
    
    void spawnEnemies_PracticeKill() {
        requiredDamageType = EDamageType.Any;
        enemiesRequiredThisState = practiceKillSpawners.Length;
        enemiesKilled = 0;
        foreach (TUT_EnemySpawner es in practiceKillSpawners)
            es.SpawnEnemy();
    }
    
    public void OnKilledAllEnemies() {
        switch (currState) {
        case ETutorialState.VacuumKill:
            GoToState(ETutorialState.CannonKill1);
            break;
        case ETutorialState.CannonKill1:
            GoToState(ETutorialState.CannonKill2);
            break;
        case ETutorialState.CannonKill2:
            GoToState(ETutorialState.CannonRicochet);
            break;
        case ETutorialState.CannonRicochet:
            GoToState(ETutorialState.PracticeKill);
            break;
        case ETutorialState.PracticeKill:
            GoToState(ETutorialState.FINISHED);
            break;
        }
    }
    
    IEnumerator onTutorialCompleted() {
        // TutorialPanel.OnTutorialStateChanged(ETutorialState.FINISHED);
        print($"Congratulations! You have completed the tutorial. Now loading the endless level (in 3 seconds).");
        yield return new WaitForSeconds(3);
        GameManager.Instance.MainCanvas.FadeToBlack();
        yield return new WaitForSecondsRealtime(UIMainCanvas.FADER_FADE_DURATION);
        SwitchToScene("True Endless");
    }
    
    void getAndSetSpawnGroups() {
        vacuumOnlySpawners = vacuumSpawnGroup.GetComponentsInChildren<TUT_EnemySpawner>();
        cannonOnlySpawners1 = cannonSpawnGroup1.GetComponentsInChildren<TUT_EnemySpawner>();
        cannonOnlySpawners2 = cannonSpawnGroup2.GetComponentsInChildren<TUT_EnemySpawner>();
        canRicSpawners = canRicSpawnGroup.GetComponentsInChildren<TUT_EnemySpawner>();
        practiceKillSpawners = practiceKillSpawnGroup.GetComponentsInChildren<TUT_EnemySpawner>();
    }
    
    void setupTriggers() {
        vacuumMovement1Finished.A_TriggerEntered += vacMove1Finished;
        vacuumMovement2Finished.A_TriggerEntered += vacMove2Finished;
        cannonMovement1Finished.A_TriggerEntered += canMove1Finished;
        cannonMovement2Finished.A_TriggerEntered += canMove2Finished;
    }
    
    void vacMove1Finished(Collider other) {
        playerSpawnPoint = spawnForVacMove2;
        vacuumMovement1Finished.A_TriggerEntered -= vacMove1Finished;
        vacuumMovement1Finished.gameObject.SetActive(false);
        GoToState(ETutorialState.VacuumMovement2);
    }
    
    void vacMove2Finished(Collider other) {
        playerSpawnPoint = spawnForCanMove1;
        vacuumMovement2Finished.A_TriggerEntered -= vacMove2Finished;
        vacuumMovement2Finished.gameObject.SetActive(false);
        GoToState(ETutorialState.CannonMovement1);
    }
    
    void canMove1Finished(Collider other) {
        playerSpawnPoint = spawnForCanMove2;
        cannonMovement1Finished.A_TriggerEntered -= canMove1Finished;
        cannonMovement1Finished.gameObject.SetActive(false);
        GoToState(ETutorialState.CannonMovement2);
    }
    
    void canMove2Finished(Collider other) {
        playerSpawnPoint = spawnForKillPractice;
        cannonMovement2Finished.A_TriggerEntered -= canMove2Finished;
        cannonMovement2Finished.gameObject.SetActive(false);
        GoToState(ETutorialState.VacuumKill);
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
    VacuumMovement1,
    VacuumMovement2,
    CannonMovement1,
    CannonMovement2,
    VacuumKill,
    CannonKill1,
    CannonKill2,
    CannonRicochet,
    PracticeKill,
    FINISHED
}