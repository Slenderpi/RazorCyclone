// UNCOMMENT THE LINE BELOW TO DISABLE THE GENERATOR. This will also stop the authoring of the PointCloudConfig
//#define FORCE_DISABLE_POINT_CLOUD_GENERATOR

using System.IO;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;

public class PointCloudGenerator : MonoBehaviour {

	public static PointCloudGenerator Instance;

	[Tooltip("If enabled, PointCloudGenerator will run when you enter play mode.\nIf False, the generator will not run, but the PCC will still be added. Uncomment the define at the beginning of this file to fully disable the generator.")]
	public bool GENERATE_POINT_CLOUD = false;

	public float3 CenterPosition;
	[Tooltip("Example: a value of 5 means 2.5 on either side of CenterPosition.x")]
	[Min(1)]
	public float DimensionX = 1f;
	[Tooltip("Example: a value of 5 means 2.5 on either side of CenterPosition.y")]
	[Min(1f)]
	public float DimensionY = 1f;
	[Tooltip("Example: a value of 5 means 2.5 on either side of CenterPosition.z")]
	[Min(1f)]
	public float DimensionZ = 1f;
	[Tooltip("Distance from one point to another in the generated point cloud grid.")]
	[Min(0.1f)]
	public float DistanceBetweenPoints = 1f;
	[Tooltip("The distance a collider can be from the point to consider it obstructive.\nIt is reasonable to keep this value less than half of DistanceBetweenPoints.")]
	[Min(0)]
	public float PointRadius = 0.5f;



	private void Awake() {
		Instance = this;
	}

	class Baker : Baker<PointCloudGenerator> {
		public override void Bake(PointCloudGenerator auth) {
#if !FORCE_DISABLE_POINT_CLOUD_GENERATOR
			Entity entity = GetEntity(TransformUsageFlags.None);
			//AddComponent(entity, new RandomGenerator() {
			//	rng = new Unity.Mathematics.Random(Util.GenerateSeed(auth.transform))
			//});
			float3 dims = new(auth.DimensionX, auth.DimensionY, auth.DimensionZ);
			PointCloudConfig pcc = new() {
				IsEnabled = auth.GENERATE_POINT_CLOUD ? 1 : 0,

				DistBetweenPoints = auth.DistanceBetweenPoints,
				PointRadius = auth.PointRadius,

				numX = (int)(auth.DimensionX / auth.DistanceBetweenPoints),
				numY = (int)(auth.DimensionY / auth.DistanceBetweenPoints),
				numZ = (int)(auth.DimensionZ / auth.DistanceBetweenPoints),
				cornerPosition = auth.CenterPosition - dims / 2f
			};
			pcc.count = pcc.numX * pcc.numY * pcc.numZ;
			AddComponent(entity, pcc);
#endif
		}
	}

}

[BurstCompile]
public struct PointCloudConfig : IComponentData {
	// Input
	public int IsEnabled;
	public float DistBetweenPoints; // Distance from one point to another
	public float PointRadius; // Max distance a collider can be from this point to consider this point invalid

	// Computed
	public int numX;
	public int numY;
	public int numZ;
	public float3 cornerPosition;
	/// <summary>
	/// Total number of points in the point cloud.
	/// </summary>
	public int count;



	/// <summary>
	/// Determines if a point is in the bounds of the point cloud/wavefront as determined by
	/// the pcc.<br/>
	/// Bound limits include 0 and exclude the num. I.e. for x, the limit interval is [0, pcc.numX).
	/// </summary>
	/// <param name="point">A point to test.</param>
	/// <returns>True if in bounds (e.g. for x, true if within [0, pcc.numX)).</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool IsPointInBounds(in int3 point) {
		return IsPointInBounds(point.x, point.y, point.z);
	}

	/// <summary>
	/// Determines if a point is in the bounds of the point cloud/wavefront as determined by
	/// the pcc.<br/>
	/// Bound limits include 0 and exclude the num. I.e. for x, the limit interval is [0, pcc.numX).
	/// </summary>
	/// <param name="x">X value of a point to test.</param>
	/// <param name="y">Y value of a point to test.</param>
	/// <param name="z">Z value of a point to test.</param>
	/// <returns>True if in bounds (e.g. for x, true if within [0, pcc.numX)).</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool IsPointInBounds(int x, int y, int z) {
		return x >= 0 && y >= 0 && z >= 0 && x < numX && y < numY && z < numZ;
	}

	/// <summary>
	/// Given a position in the world, determines its point in a point cloud/wavefront.<br/>
	/// The position will be clamped to the bounds of the point cloud/wavefront.
	/// </summary>
	/// <param name="pos">Position in world</param>
	/// <returns>Closest position in a point cloud/wavefront represented by the PointCloudConfig</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int3 PositionToPoint(in float3 pos) {
		return PositionToPoint(pos.x, pos.y, pos.z);
	}

	/// <summary>
	/// Given a position in the world, determines its point in a point cloud/wavefront.<br/>
	/// The position will be clamped to the bounds of the point cloud/wavefront.
	/// </summary>
	/// <param name="x">X position in world</param>
	/// <param name="y">Y position in world</param>
	/// <param name="z">Z position in world</param>
	/// <returns>Closest position in a point cloud/wavefront represented by the PointCloudConfig</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int3 PositionToPoint(float x, float y, float z) {
		return new(
			(int)math.clamp((x - cornerPosition.x) / DistBetweenPoints, 0, numX - 1),
			(int)math.clamp((y - cornerPosition.y) / DistBetweenPoints, 0, numY - 1),
			(int)math.clamp((z - cornerPosition.z) / DistBetweenPoints, 0, numZ - 1)
		);
	}

	/// <summary>
	/// Given a point in the point cloud/wavefront, determines the position in the world it represents.
	/// </summary>
	/// <param name="point">Point in the point cloud/wavefront</param>
	/// <returns>Associated position in the world</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly float3 PointToPosition(in int3 point) {
		return PointToPosition(point.x, point.y, point.z);
	}

	/// <summary>
	/// Given a point in the point cloud/wavefront, determines the position in the world it represents.
	/// </summary>
	/// <param name="x">The X of the point in the point cloud/wavefront</param>
	/// <param name="y">The Y of the point in the point cloud/wavefront</param>
	/// <param name="z">The Z of the point in the point cloud/wavefront</param>
	/// <returns>Associated position in the world</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly float3 PointToPosition(int x, int y, int z) {
		return new(
			x * DistBetweenPoints + cornerPosition.x + DistBetweenPoints / 2f,
			y * DistBetweenPoints + cornerPosition.y + DistBetweenPoints / 2f,
			z * DistBetweenPoints + cornerPosition.z + DistBetweenPoints / 2f
		);
	}

	/// <summary>
	/// Given a point in the point cloud, determines the respective index in the PointCloudValue/WavefrontValue list.<br/>
	/// The math for it is: i = x * YZ + y * Z + z
	/// </summary>
	/// <param name="point">Point in the point cloud/wavefront</param>
	/// <returns>Index in the point cloud/wavefront list</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int PointToIndex(in int3 point) {
		return PointToIndex(point.x, point.y, point.z);
	}

	/// <summary>
	/// Given a point in the point cloud, determines the respective index in the PointCloudValue/WavefrontValue list.<br/>
	/// The math for it is: i = x * YZ + y * Z + z
	/// </summary>
	/// <param name="point">Point in the point cloud/wavefront</param>
	/// <returns>Index in the point cloud/wavefront list</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int PointToIndex(int x, int y, int z) {
		return x * numY * numZ + y * numZ + z;
	}

	/// <summary>
	/// Given an index in the PointCloudValue/WavefrontValue list, determines the point location.<br/>
	/// x = i / YZ<br/>
	/// y = i % YZ / Z<br/>
	/// z = i % Z
	/// </summary>
	/// <param name="index">Index in the point cloud/wavefront list</param>
	/// <returns>Point in the point cloud/wavefront</returns>
	[BurstCompile]
	public readonly int3 IndexToPoint(int index) {
		/*
		  Y				Z
		X 0  3  6  9  | 1  4  7  10 | 2  5  8  11
		  12 15 18 21 | 13 16 19 22 | 14 17 20 23
		X = {0, 1}			|2|
		Y = {0, 1, 2, 3}	|4|
		Z = {0, 1, 2}		|3|
		Array is:
		0		  1			 2			3		   4		  5			 6
		(0, 0, 0) (0, 0, 1), (0, 0, 2), (0, 1, 0), (0, 1, 1), (0, 1, 2), (0, 2, 0)

		YZ = 12

		x = i / YZ		// each point increases by YZ when moving only on x
		y = i % YZ / Z	// each point increases by Z after looping it with YZ when moving only on y
		z = i % Z		// each point increases by Z when moving only on z
		3  should be (0, 1, 0) | x = 3  / 12 = 0 | y = (3  % 12) / 3 = 1 | z = 3  % 3 = 0
		6  should be (0, 2, 0) | x = 6  / 12 = 0 | y = (6  % 12) / 3 = 2 | z = 6  % 3 = 0
		7  should be (0, 2, 1) | x = 7  / 12 = 0 | y = (7  % 12) / 3 = 2 | z = 7  % 3 = 1
		19 should be (1, 2, 1) | x = 19 / 12 = 1 | y = (19 % 12) / 3 = 2 | z = 19 % 3 = 1
		 */
		int YZ = numY * numZ;
		return new(
			index / YZ,
			index % YZ / numZ,
			index % numZ
		);
	}

	/// <summary>
	/// Converts a world position to the corresponding index in the point cloud/wavefront buffer.
	/// </summary>
	/// <param name="position">Position in the world</param>
	/// <returns>Corresponding index in the point cloud/wavefront buffer</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int PositionToIndex(in float3 position) {
		return PointToIndex(PositionToPoint(position));
	}
}

/*
 * Determining open areas in the level
 * Parameters:
 *      - vector: Center position
 *      - vector: X/Y/Z full width/height/depth limits
 *      - float: dist between each point
 *  
 * Output:
 *      - Some method of representing this 3d matrix. Each point's value is a bool that
 *      determines if the point is in open air or in a wall
 *      - This should be saved to a file so that during gameplay it doesn't need to be
 *      recomputed
 * 
 * The output can be used by the wavefront algorithm during gameplay to then determine
 * a vector field for enemies to use for pathing to the player
 * 
 * Should have a separate script that can visualize open area spots
 */
//[UpdateAfter(typeof(TransformSystemGroup))]
//[UpdateBefore(typeof(WavefrontPropagator))]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(SceneSystemGroup))]
partial struct PointCloudGeneratorSystem : ISystem {

	public PointCloudConfig Instance;

	public bool hasRunGenerator;

	PhysicsWorld pw;
	uint LayersPointsCollideWith;



	public void OnCreate(ref SystemState state) {
#if FORCE_DISABLE_POINT_CLOUD_GENERATOR
		state.Enabled = false;
#else
		state.RequireForUpdate<PointCloudConfig>();
#endif
	}

	public void OnUpdate(ref SystemState state) {
		NativeArray<PointCloudConfig> pccs = SystemAPI.QueryBuilder()
			.WithPresent<PointCloudConfig>()
			.Build()
			.ToComponentDataArray<PointCloudConfig>(Allocator.Temp);
		if (pccs.Length == 0) {
			float maxAttemptTime = 3f;
			if (SystemAPI.Time.ElapsedTime < maxAttemptTime)
				return;
			Debug.LogError($"ERROR: Point Cloud Generator could not find a PointCloudConfig component (looked for {maxAttemptTime} seconds).");
			return;
		}
		if (pccs.Length > 1)
			Debug.LogWarning(
				$"WARN: Point Cloud Generator found more than one PointCloudConfig component (found {pccs.Length}). Only the first found will be used."
			);
		PointCloudConfig pcc = pccs[0];
		if (pcc.IsEnabled == 0) {
			Debug.Log("The point cloud generator is disabled. The generator's system will be disabled.");
			state.Enabled = false;
			return;
		}
		state.Enabled = false;
		pw = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
		// Since Default layer only doesn't work, set based on what we definitely do not collide with
		LayersPointsCollideWith = ~(
			1u << 5 | // UI
			1u << 7 | // EnemyHitbox
			1u << 12 | // EnemyTrigger
			1u << 13 | // InvisBoidWall
			1u << 14 | // EnemyToWorld
			1u << 15 | // EnemyWeapon
			1u << 16 // MainCamera
		);

		NativeArray<bool> PointCloud = GeneratePointCloud(pcc);
		
		string outFile = "GeneratedPointCloudCode.txt";
		outFile = Path.Combine(Application.persistentDataPath, outFile);

		/* Save pcc info */
		string outStr = @$"		pcc = new() {{
			DistBetweenPoints = {pcc.DistBetweenPoints}f,
			PointRadius = {pcc.PointRadius}f,

			numX = {pcc.numX},
			numY = {pcc.numY},
			numZ = {pcc.numZ},
			cornerPosition = new {pcc.cornerPosition}
		}};";

		/* Save generated point cloud */
		string pcstr = "{ ";
		for (int i = 0; i < PointCloud.Length - 1; i++)
			pcstr += PointCloud[i] ? "true," : "false,";
		pcstr += PointCloud[^1] ? "true }" : "false}";
		outStr += @$"
		NativeArray<bool> PointCloud = new(
			new bool[] {pcstr},
			Allocator.Temp
		);";

		File.WriteAllText(outFile, outStr);
		Debug.Log($"Generated code written to '{outFile}'");
	}

	[BurstCompile]
	NativeArray<bool> GeneratePointCloud(PointCloudConfig pcc) {
		Debug.Log("Generating point cloud...");
		NativeArray<bool> pointCloud = new(pcc.numX * pcc.numY * pcc.numZ, Allocator.Temp);

		/*
		NOTE: Each point should be considered at the 'center' of the point. i.e. the coordinate for (1, 3, 4)
		might directly calculate to, say, (10, 30, 40), but this point represents the grid-space defined by
		(10, 30, 40) and (20, 40, 50), meaning the true point location represented by (1, 3, 4) is actually
		at (5, 35, 45).

		  Y				Z
		X 0  3  6  9  | 1  4  7  10 | 2  5  8  11
		  12 15 18 21 | 13 16 19 22 | 14 17 20 23
		X = {0, 1}
		Y = {0, 1, 2, 3}
		Z = {0, 1, 2}
		Array is:
		0		  1			 2			3		   4		  5			 6
		(0, 0, 0) (0, 0, 1), (0, 0, 2), (0, 1, 0), (0, 1, 1), (0, 1, 2), (0, 2, 0)

		15 should be (1, 1, 0), 1 * 12 + 1 * 3 + 0 = 15, correct
		22 should be (1, 3, 1), 1 * 12 + 3 * 3 + 1 = 22, correct

		then,
		i = x * Y * Z + y * Z + z
		*/
		//float3 size = new(pcc.FullLimitX, pcc.FullLimitY, pcc.FullLimitZ);
		float3 pointOffset = new(pcc.DistBetweenPoints / 2f);
		int YZ = pcc.numY * pcc.numZ;
		for (int x = 0; x < pcc.numX; x++)
			for (int y = 0; y < pcc.numY; y++)
				for (int z = 0; z < pcc.numZ; z++) {
					int i = x * YZ + y * pcc.numZ + z;
					float3 point = pcc.cornerPosition + new float3(x, y, z) * pcc.DistBetweenPoints + pointOffset;
					pointCloud[i] = IsPointValid(point, pcc.PointRadius);
				}

		Debug.Log("Point cloud generated!");
		return pointCloud;
	}

	[BurstCompile]
	bool IsPointValid(float3 point, float radius) {
		var pdi = new PointDistanceInput {
			Position = point,
			MaxDistance = radius,
			Filter = new CollisionFilter {
				BelongsTo = ~0u,
				CollidesWith = LayersPointsCollideWith,
				GroupIndex = 0
			}
		};
		return !pw.CalculateDistance(pdi, out DistanceHit hit);
	}

}