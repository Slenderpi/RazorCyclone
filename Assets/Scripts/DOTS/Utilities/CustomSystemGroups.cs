using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

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