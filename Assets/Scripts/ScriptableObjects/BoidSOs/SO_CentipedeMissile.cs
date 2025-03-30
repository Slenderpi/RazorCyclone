using UnityEngine;

[CreateAssetMenu(fileName = "CentipedeMissileBoidData", menuName = "ScriptableObjects/Boids/CentipedeMissileBoid", order = 8)]
public class SO_CentipedeMissile : GeneralBoidSO {
    
    [Header("Centipede Missile Boid parameters")]
    [Tooltip("If false, missiles will use Pursuit.")]
    public bool UseSeek = true;
    
}