using UnityEngine;

[CreateAssetMenu(fileName = "LavaWPEnemyData", menuName = "ScriptableObjects/Enemies/LavaWeakpointEnemy", order = 6)]
public class LavaWPEnemySO : ScriptableObject {
    
    [Header("Lava Weakpoint Config")]
    [Tooltip("The weakpoint's height will be at the lava's max height + this amount.")]
    public float WeakpointHeightAboveLava = 4;
    public float AnimationDuration = 2;
    
}