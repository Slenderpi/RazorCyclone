using UnityEngine;

[CreateAssetMenu(fileName = "CentipedeMissileBoidData", menuName = "ScriptableObjects/Boids/CentipedeMissileBoid", order = 8)]
public class SO_CentipedeMissile : GeneralBoidSO {
    
    [Header("Centipede Missile Boid parameters")]
    [Tooltip("The missile will use Pursuit at the start of its flight for a short duration, and this is that duration.\nAfterwards, it will travel in a straight line.")]
    public float AimDuration = 0.5f;
    
}