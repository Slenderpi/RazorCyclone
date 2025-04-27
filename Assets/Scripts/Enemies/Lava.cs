using System;
using UnityEngine;

public class Lava : MonoBehaviour {
    
    public static Action A_LavaRising;
    public static Action A_LavaNotRising;
    
    [Header("Lava Behavior and Stats")]
    public float LavaRiseSpeed = 0.5f;
    public float MaxLavaHeight = 25f;
    public float LavaLowerSpeed = 4f;
    float currRiseRate = 0;
    public float LavaDamage = 50;
    [Tooltip("When the player touches lava and bounces off of it, the vertical velocity of the bounce will be at least this value.")]
    public float MinimumVerticalBounceSpeed = 6;
#if UNITY_EDITOR
    public bool ForceDisable = false;
#endif
    
    [Header("References")]
    [SerializeField]
    TriggerNotifier LavaHitboxNotifier;
    
    [HideInInspector]
    public float currentHeight;
    
    int livingLavaEnemies = 0;
    float startY;
    
    
    
    void Awake() {
#if UNITY_EDITOR
        if (ForceDisable)
            gameObject.SetActive(false); 
#endif
    }
    
    void Start() {
        startY = transform.position.y;
        currentHeight = startY;
    }
    
    void FixedUpdate() {
        currentHeight += Time.fixedDeltaTime * currRiseRate;
        if (currentHeight > MaxLavaHeight) {
            currentHeight = MaxLavaHeight;
            currRiseRate = 0;
            A_LavaNotRising?.Invoke();
        } else if (livingLavaEnemies == 0 && currentHeight <= startY) {
            currRiseRate = 0;
            currentHeight = startY;
        }
        transform.position = new(transform.position.x, currentHeight, transform.position.y);
    }
    
    public void OnLavaEnemySpawned() {
#if UNITY_EDITOR
        if (ForceDisable) return;
#endif
        if (++livingLavaEnemies == 1) {
            if (currentHeight < MaxLavaHeight) {
                currRiseRate = LavaRiseSpeed;
                A_LavaRising?.Invoke();
            }
        }
    }
    
    public void OnLavaEnemyDefeated() {
#if UNITY_EDITOR
        if (ForceDisable) return;
#endif
        if (--livingLavaEnemies == 0) {
            currRiseRate = -LavaLowerSpeed;
            A_LavaNotRising?.Invoke();
        }
    }
    
}