using UnityEngine;

[CreateAssetMenu(fileName = "HunterEnemyData", menuName = "ScriptableObjects/Enemies/HunterEnemy", order = 3)]
public class HunterEnemySO : ScriptableObject {
    
    public float StunDuration = 5f;
    public float ShieldDrag = 0.2f;
    public float StunDrag = 0.8f;
    public Material ShieldActiveMaterial;
    public Material ShieldInactiveMaterial;
    
}