using UnityEngine;

[CreateAssetMenu(fileName = "CentipedeEnemyData", menuName = "ScriptableObjects/Enemies/CentipedeEnemy", order = 7)]
public class CentipedeEnemySO : ScriptableObject {
    
    [Header("Centipede Config")]
    [Tooltip("Movement speed the head will move at.")]
    public float MoveSpeed = 5;
    [Tooltip("Distance the body segments will maintain from the previous segment.")]
    public float FollowOffset = 1; // Distance to keep from previous
    [Tooltip("The time between each missile firing.")]
    public float MissileFireDelay = 5;
    [Tooltip("Additional time to prevent the entire centipede from firing at once.\nExample: using 0.1, the head will fire after MissileFireDelay, the first body will fire after MissileFireDelay + 0.1, the next at MissileFireDelay + 0.2, etc.")]
    public float MissileFireDelayOffset = 0.1f;
    [Tooltip("The length of time the missile door animates opening for.\nDoes not affect MissileFireDelay.")]
    public float MissileDoorAnimTime = 0.2f;
    [Tooltip("The length of the time the missile door stays open right after firing the missile.\nDoes not affect MissileFireDelay.")]
    public float MissileDoorLeaveOpenTime = 0.2f;
    [Tooltip("Prefab of missile. Needs to be of type CentipedeMissile.")]
    public CentipedeMissile MissilePrefab;
    [Tooltip("VFX to use when a missile gets launched.")]
    public GameObject MissileFireEffectPrefab;
    [Tooltip("Material of the centipede's head.")]
    public Material HeadMaterial;
    
}