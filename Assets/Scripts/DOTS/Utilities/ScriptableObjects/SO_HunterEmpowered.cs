using UnityEngine;

[CreateAssetMenu(fileName = "HunterEmpoweredGameplayData", menuName = "ScriptableObjects/EnemyGameplay/HunterEmpowered", order = 3)]
public class SO_HunterEmpowered : ScriptableObject {
	[Tooltip("The amount of damage this Hunter Empowered will deal to the Player.")]
	public float Damage = 30f;
	[Tooltip("How long this Hunter will stay stunned for, in seconds.")]
	public float StunDuration = 5f;
	[Tooltip("The RicochetTarget priority this Hunter will have when it is not stunned.")]
	public uint NormalRicochetPriority = 2100u;
	[Tooltip("The RicochetTarget priority this Hunter will have while it is stunned.")]
	public uint StunnedRicochetPriority = 2000u;
}