using UnityEngine;

[CreateAssetMenu(fileName = "HunterBasicGameplayData", menuName = "ScriptableObjects/EnemyGameplay/HunterBasic", order = 2)]
public class SO_HunterBasic : ScriptableObject {
	[Tooltip("The amount of damage this Hunter Basic will deal to the Player.")]
	public float Damage = 15f;
}