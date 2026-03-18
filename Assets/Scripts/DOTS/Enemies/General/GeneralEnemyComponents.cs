using System;
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
/// For differentiating between a 'basic' and an 'empowered' version of an enemy.<br/>
/// This enum is in bitflags, but is not meant to be used as so.
/// This is more for compression of memory that some enemies can take advantage of.
/// </summary>
[Flags]
public enum EEnemyForm {
	Basic = 1,
	Empowered = 2,
}