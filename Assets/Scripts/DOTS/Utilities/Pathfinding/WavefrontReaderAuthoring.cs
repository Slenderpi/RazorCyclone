using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

public class WavefrontReaderAuthoring : MonoBehaviour {

    //public bool EnableTester = false;
    //public float TesterMoveSpeed;
    //public float TesterRotateSpeed;
    
    
    
    class Baker : Baker<WavefrontReaderAuthoring> {
        public override void Bake(WavefrontReaderAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new WavefrontReader());
            //if (auth.EnableTester)
            //    AddComponent(entity, new WavefrontReaderTester(auth.TesterMoveSpeed, auth.TesterRotateSpeed));
        }
    }
    
}



public struct WavefrontReader : IComponentData {
    /// <summary>
    /// Not normalized (it's an int3).
    /// </summary>
    public int3 DescentDirection;
}



//public struct WavefrontReaderTester : IComponentData, IEnableableComponent {
//    public float MoveSpeed;
//    public float RotationSpeed;

//    public WavefrontReaderTester(float moveSpeed, float rotationSpeed) {
//        MoveSpeed = moveSpeed;
//        RotationSpeed = rotationSpeed;
//    }
//}

//[UpdateAfter(typeof(WavefrontPropagator))]
//public partial struct WavefrontReaderTesterSystem : ISystem {
//    [BurstCompile]
//    public void OnUpdate(ref SystemState state) {
//        WavefrontReaderTesterJob job = new() {
//            deltaTime = SystemAPI.Time.DeltaTime
//        };
//        job.Schedule();
//    }
//}

//[BurstCompile]
//partial struct WavefrontReaderTesterJob : IJobEntity {
//    public float deltaTime;
    
//    public void Execute(ref LocalTransform transform, ref PhysicsVelocity physicsVelocity, in PhysicsMass physicsMass, in WavefrontReader wavefrontReader, in WavefrontReaderTester wavefrontReaderTester) {
//        //physicsVelocity.Angular = float3.zero;
//        float3 dir = wavefrontReader.DescentDirection;
//        float3 rotUpVector = math.up();

//        if (dir.x == 0 && dir.z == 0) {
//            if (dir.y == 0) {
//				physicsVelocity.Linear = float3.zero;
//				Util.D_DrawBox(transform.Position, 2.5f, Color.magenta, deltaTime);
//				return;
//			} else {
//                rotUpVector = math.back();
//			}
//		}
//		dir = math.normalize(dir);

//		transform.Rotation = math.slerp(
//            transform.Rotation,
//            quaternion.LookRotation(dir, rotUpVector),
//            deltaTime * wavefrontReaderTester.RotationSpeed
//        );

//        physicsVelocity.ApplyLinearImpulse(physicsMass, dir * wavefrontReaderTester.MoveSpeed * deltaTime);
//        physicsVelocity.Angular = float3.zero;

//		Util.D_DrawBox(transform.Position, 2.5f, Color.magenta, deltaTime);

//		//physicsVelocity.Linear = dir * wavefrontReaderTester.MoveSpeed;
//    }
//}