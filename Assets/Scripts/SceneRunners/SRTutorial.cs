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
    
    [HideInInspector]
    public EDamageType requiredDamageType = EDamageType.Enemy; // Initialize to impossible type
    int enemiesRequiredThisState;
    int enemiesKilled;
    
    UITutorialPanel TutorialPanel;
    
    ETutorialState currState;
    
    
    
    public override void BeginScene() {
        TutorialPanel = GameManager.Instance.MainCanvas.TutorialPanel;
        getAndSetSpawnGroups();
        base.BeginScene();
        GoToState(ETutorialState.IntroduceVacuum);
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
    
    void getAndSetSpawnGroups() {
        vacuumOnlySpawners = vacuumSpawnGroup.GetComponentsInChildren<TUT_EnemySpawner>();
        canonOnlySpawners = canonSpawnGroup.GetComponentsInChildren<TUT_EnemySpawner>();
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
    IntroduceVacuum,
    IntroduceCanon,
    KillTheWave
}