using Unity.Entities;

public struct DeadEnemyTag : IComponentData {
	public EEnemyType EnemyType;
	public EEnemyDeathSource DeathSource;
}

public enum EEnemyDeathSource {
	Vacuum,
	Cannon,
	CannonRicochet,
	Lava
}

/// <summary>
/// For differentiating between a 'basic' and an 'empowered' version of an enemy.
/// </summary>
public enum EEnemyForm {
	Basic,
	Empowered
}