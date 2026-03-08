using Unity.Entities;
using Unity.Burst;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using System.Linq;

[UpdateBefore(typeof(PlayerPostUpdateGroup))]
partial struct PointCloudRaycastSystem : ISystem {

    EntityQuery eqPlayer;

    PointCloudConfig pcc;
    Entity BufferEntity;
    bool hasFoundPointCloudSingleton;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PointCloudValue>();
        state.RequireForUpdate<WavefrontPointCloudSingleton>();
        state.RequireForUpdate<PointCloudConfig>();
        state.RequireForUpdate<PointCloudRaycast>();
        state.RequireForUpdate<Player>();

        eqPlayer = new EntityQueryBuilder(Allocator.Persistent).WithAll<Player, LocalTransform>().Build(ref state);
		hasFoundPointCloudSingleton = false;
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!hasFoundPointCloudSingleton) {
            hasFoundPointCloudSingleton = true;
            pcc = SystemAPI.GetSingleton<PointCloudConfig>();
            BufferEntity = SystemAPI.GetSingletonEntity<WavefrontPointCloudSingleton>();
		}
        float3 PlayerPos = state.EntityManager.GetComponentData<LocalTransform>(eqPlayer.GetSingletonEntity()).Position; // eqPlayer.GetSingleton<LocalTransform>().Position
        NativeArray<byte> CachedLosChecks = new(pcc.count, Allocator.TempJob);
        Debug.Log($"CachedLosChecks should have length {pcc.count}. It has {CachedLosChecks.Length}");
		state.Dependency = new PointCloudRaycastJob() {
            CachedLosChecks = CachedLosChecks,
            pc = state.EntityManager.GetBuffer<PointCloudValue>(BufferEntity).AsNativeArray(),
            pcc = pcc,
			PlayerPos = PlayerPos,
            goalPoint = pcc.PositionToPoint(PlayerPos)
        }.Schedule(state.Dependency);
        state.Dependency = CachedLosChecks.Dispose(state.Dependency);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        //eqPointCloudConfig.Dispose();
        //eqPlayer.Dispose();
    }

    [BurstCompile]
    partial struct PointCloudRaycastJob : IJobEntity {
        // 0=unset, 1=HasLos, other=NoLos
        public NativeArray<byte> CachedLosChecks;
        [ReadOnly]
        public NativeArray<PointCloudValue> pc;
        public PointCloudConfig pcc;
        public float3 PlayerPos;
        public int3 goalPoint; // temp?

		[BurstCompile]
        public void Execute(
            ref PointCloudRaycast cast,
            in LocalTransform trans
        ) {
            /**
             * THE BELOW ALGORITHM IS BASED ON "A Fast Voxel Traversal Algorithm" (http://www.cse.yorku.ca/~amana/research/grid.pdf)
             */
            int currIndex = CachedLosChecks[pcc.PositionToIndex(trans.Position)];
			if (CachedLosChecks[currIndex] != 0) {
                cast.HasLos = CachedLosChecks[currIndex] == 1;
                return;
            }
			int3 currPoint = pcc.PositionToPoint(trans.Position);
			float3 toPlayer = PlayerPos - trans.Position;
			int3 step = Util.sign(toPlayer);
            float halfDist = pcc.DistBetweenPoints / 2f;
			float3 currPointPos = pcc.PointToPosition(currPoint);
			float3 tMax = new(
                ((step.x >= 1 ? currPointPos.x + halfDist : currPointPos.x - halfDist) - trans.Position.x) / toPlayer.x,
                ((step.y >= 1 ? currPointPos.y + halfDist : currPointPos.y - halfDist) - trans.Position.y) / toPlayer.y,
				((step.z >= 1 ? currPointPos.z + halfDist : currPointPos.z - halfDist) - trans.Position.z) / toPlayer.z
			);
            float3 tDelta = new(
                Util.IsNearZero(toPlayer.x) ? float.MaxValue : pcc.DistBetweenPoints / math.abs(toPlayer.x),
				Util.IsNearZero(toPlayer.y) ? float.MaxValue : pcc.DistBetweenPoints / math.abs(toPlayer.y),
				Util.IsNearZero(toPlayer.z) ? float.MaxValue : pcc.DistBetweenPoints / math.abs(toPlayer.z)
			);
            bool hasLos = true;
            while (!Util.equal(currPoint, goalPoint)) { // temp sentinel
                // UNCOMMENT BELOW FOR RAYCAST DEBUGGING
				//Util.D_VisualizePointAsBox(pcc, currPoint, pc[pcc.PointToIndex(currPoint)].IsUnobstructed);
				if (!pc[pcc.PointToIndex(currPoint)].IsUnobstructed) {
                    hasLos = false;
                    break;
                }
                if (tMax.x < tMax.y && tMax.x < tMax.z) {
                    currPoint.x += step.x;
                    tMax.x += tDelta.x;
                } else if (tMax.y < tMax.z) {
					currPoint.y += step.y;
					tMax.y += tDelta.y;
				} else {
					currPoint.z += step.z;
					tMax.z += tDelta.z;
				}
            }
            cast.HasLos = hasLos;
            CachedLosChecks[currIndex] = hasLos ? (byte)1 : (byte)2;
        }
    }
    
}