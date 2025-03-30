using UnityEngine;

[CreateAssetMenu(fileName = "HunterBasicEnemyData", menuName = "ScriptableObjects/Enemies/HunterBasicEnemy", order = 3)]
public class HunterBasicEnemySO : ScriptableObject {
    
    public float StunDuration = 5f;
    public float ShieldDrag = 0.2f;
    public float StunDrag = 0.8f;
    public Material ShieldActiveMaterial;
    public Material ShieldInactiveMaterial;
    
}