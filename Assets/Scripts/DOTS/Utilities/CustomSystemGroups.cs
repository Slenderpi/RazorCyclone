using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial class PlayerPostUpdateGroup : ComponentSystemGroup { }

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PlayerPostUpdateGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial class EnemyLogicPostUpdateGroup : ComponentSystemGroup { }

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(EnemyLogicPostUpdateGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial class ProjectilePostUpdateGroup : ComponentSystemGroup { }

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ProjectilePostUpdateGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial class EnemyGeneralPostUpdateGroup : ComponentSystemGroup { } // E.g. process enemy health


// FIXED STEP

/// <summary>
/// Fixed step, before PhysicsSystemGroup.
/// 
/// For enemy movement code involving physics.
/// </summary>
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
public partial class PrePhysicsGroup : ComponentSystemGroup { }

/// <summary>
/// Fixed step, after PhysicsSystemGroup (so at the end of fixed step).
/// 
/// For handling physics-related events (e.g. enemies handling their cannon projectile hit event)
/// </summary>
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial class EnemyHitPhysicsGroup : ComponentSystemGroup { }

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(HurtboxColliderSystem))]
public partial class EnemyDamagePhysicsGroup : ComponentSystemGroup { }