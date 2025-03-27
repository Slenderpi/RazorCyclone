using UnityEngine;

[CreateAssetMenu(fileName = "LavaEnemyData", menuName = "ScriptableObjects/Enemies/LavaEnemy", order = 7)]
public class LavaEnemySO : ScriptableObject {
    
    [Header("Lava Enemy Config")]
    public float WeakpointExposeDuration = 5;
    
}