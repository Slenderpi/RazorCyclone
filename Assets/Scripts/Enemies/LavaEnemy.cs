using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LavaEnemy : WeakPointedEnemy {
    
    [Header("Lava Enemy Configuration")]
    [SerializeField]
    float directionChangeDelay = 3;
    
    Lava lava;
    Vector3 movementDirection;
    WaitForSeconds mvmntWait;
    
    
    
    void Start() {
        lava = GameManager.Instance.currentSceneRunner.lava;
        lava.OnLavaEnemySpawned();
        StartCoroutine(changeMovementDirection());
    }
    
    void FixedUpdate() {
        rb.AddForce(movementDirection * 20, ForceMode.Acceleration);
        transform.position = new Vector3(
            transform.position.x,
            lava.transform.position.y,
            transform.position.z
        );
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(movementDirection, Vector3.up),
            Mathf.Min(1, Time.fixedDeltaTime / 0.25f)
        );
    }
    
    IEnumerator changeMovementDirection() {
        yield return mvmntWait;
        setRandomMoveDir();
        StartCoroutine(changeMovementDirection());
    }

    protected override void OnDefeated() {
        lava.OnLavaEnemyDefeated();
        base.OnDefeated();
    }

    protected override void Init() {
        base.Init();
        mvmntWait = new WaitForSeconds(directionChangeDelay);
        movementDirection = new Vector3(1, 0, 0);
        setRandomMoveDir();
    }
    
    void setRandomMoveDir() {
        float angle = Random.value * 2f * Mathf.PI;
        Vector2 v2 = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        movementDirection.x = v2.x;
        movementDirection.z = v2.y;
    }

}