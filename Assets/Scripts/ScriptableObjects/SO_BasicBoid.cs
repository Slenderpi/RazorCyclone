using UnityEngine;

[CreateAssetMenu(fileName = "BasicBoidData", menuName = "ScriptableObjects/BasicBoid", order = 2)]
public class SO_BasicBoid : GeneralBoidSO {
    
    [Header("Basic Boid Config")]
    public BoidBehaviour BehaviourType;
    public BoidRotationType RotationType;
    public bool CanFly = false;
    
}