using Unity.Entities;

public struct DeadEnemyTag : IComponentData {
	public EEnemyType EnemyType;
	public EEnemyDeathSource DeathSource;
}

public enum EEnemyDeathSource {
	Vacuum,
	Cannon,
	Lava
}