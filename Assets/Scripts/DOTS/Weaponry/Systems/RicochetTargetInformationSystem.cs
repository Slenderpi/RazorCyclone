using Unity.Entities;
using Unity.Burst;
using UnityEngine;
using Unity.Physics;
using Unity.Transforms;
using Unity.Physics.Systems;
using Unity.Collections;

/// <summary>
/// Instead of querying for RicochetTarget, LocalTransform, PhysicsVelocity, and Entity, the RicocheTarget component
/// will simply keep track of all that information in itself.<br/>
/// This system will update that information.
/// </summary>
[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateBefore(typeof(PlayerCannonProjectileSystem))]
partial struct RicochetTargetInformationSystem : ISystem {

	ComponentLookup<PhysicsVelocity> clPhysicsVelocity;

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<RicochetTarget>();
		clPhysicsVelocity = state.GetComponentLookup<PhysicsVelocity>(isReadOnly: true);
	}
	
	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		clPhysicsVelocity.Update(ref state);
		state.Dependency = new RicochetTargetInformationJob() {
			clPhysicsVelocity = clPhysicsVelocity
		}.ScheduleParallel(state.Dependency);
	}

	[BurstCompile]
	partial struct RicochetTargetInformationJob : IJobEntity {
		[ReadOnly]
		public ComponentLookup<PhysicsVelocity> clPhysicsVelocity;

		[BurstCompile]
		public void Execute(Entity entity, ref RicochetTarget ricTarg, in LocalTransform trans) {
			if (!ricTarg.Enabled)
				return;
			ricTarg.LocalPosition = trans.Position;
			//Util.D_DrawPoint(trans.Position, Color.green, 0.02f);
			if (ricTarg.HasPhysicsVelocity) {
				ricTarg.Velocity = clPhysicsVelocity[entity].Linear;
			}
		}
	}
	
}