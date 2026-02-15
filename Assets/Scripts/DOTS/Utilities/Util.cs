using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public static class Util {

	public readonly static Unity.Mathematics.Random rng = new Unity.Mathematics.Random(1);



	//readonly static GeneralBoid DEFAULT_GENERAL_BOID = new GeneralBoid() {
	//	MaxSteeringVelocity = 15,
	//	MaxSteeringForce = 10,
	//	WanderLimitRadius = 0.5f,
	//	WanderLimitDist = 0.5f,
	//	WanderChangeDist = 0.15f,
	//	MaxWanderForce = 5,
	//	WanderMinimumDelay = 0,
	//	AvoidanceTestType = AvoidanceTestMode.None,
	//	AvoidanceMaxLookDist = 4,
	//	AvoidanceWhiskerAngle = 30f,
	//	AvoidanceMinIntensity = 1,
	//	AvoidanceMaxIntensity = 10,
	//	AvoidanceMaxSteeringForce = 10,
	//	AvoidInvisBoidWalls = false
	//};

	//public static GeneralBoid BuildGeneralBoidComponentDataDefault(GeneralBoidSO so) {
	//	return DEFAULT_GENERAL_BOID;
	//}

	//public static GeneralBoid BuildGeneralBoidComponentDataFromSO(GeneralBoidSO so) {
	//	return new GeneralBoid() {
	//		MaxSteeringVelocity = so.MaxSteeringVelocity,
	//		MaxSteeringForce = so.MaxSteeringForce,
	//		WanderLimitRadius = so.WanderLimitRadius,
	//		WanderLimitDist = so.WanderLimitDist,
	//		WanderChangeDist = so.WanderChangeDist,
	//		MaxWanderForce = so.MaxWanderForce,
	//		WanderMinimumDelay = so.WanderMinimumDelay,
	//		AvoidanceTestType = so.AvoidanceTestType,
	//		AvoidanceMaxLookDist = so.AvoidanceMaxLookDist,
	//		AvoidanceWhiskerAngle = so.AvoidanceWhiskerAngle,
	//		AvoidanceMinIntensity = so.AvoidanceMinIntensity,
	//		AvoidanceMaxIntensity = so.AvoidanceMaxIntensity,
	//		AvoidanceMaxSteeringForce = so.MaxSteeringForce,
	//		AvoidInvisBoidWalls = so.AvoidInvisBoidWalls,

	//		wanderPoint = float3.zero,
	//		lastWanderStepTime = -1000f
	//	};
	//}

	//public static float3 StepWanderPoint2D(float3 prevWanderPoint, float lastWanderStepTime, float wanderMinimumDelay, float wanderLimitRadius, float wanderChangeDist) {
	//	return BoidSteerer.StepWanderPoint2D(prevWanderPoint, wanderLimitRadius, wanderChangeDist);
	//}

	/// <summary>
	/// Convenience method for squaring a float.
	/// </summary>
	/// <param name="x">The float to get squared.</param>
	/// <returns>x * x</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE1006 // Naming Styles
	public static float pow2(float x) { return x * x; }
#pragma warning restore IDE1006 // Naming Styles

	public static uint GenerateSeed(Transform transform) {
		// uh um random weird code go
		float3 pos = transform.position;
		float val = (
			pos.x +
			pos.y * -pos.y +
			pos.z * pos.z * pos.z
		);
		if (val < 0) val = 1 + -val * 3;
		if (val < 1) {
			float3 rot = math.EulerXYZ(transform.rotation);
			val = 1 + (
				47 * (math.abs(rot.x) + math.abs(pos.y) + 1) +
				11 * (math.abs(rot.y) + math.abs(pos.z) + 1) +
				71 * (math.abs(rot.z) + math.abs(pos.z) + 1
			));
		}
		return (uint)val;
	}

	/// <summary>
	/// A value of 0 is cold, a value of 1 is hot.<br/>
	/// The heatmap is in 5 colors. In cold to hot:<br/>
	/// 0-------0.25-----0.5--------0.75------1<br/>
	/// Blue -> Cyan -> Green -> Yellow -> Red
	/// </summary>
	/// <param name="perc"></param>
	/// <returns></returns>
	public static Color MakeHeatmapColor(float heat) {
		if (heat < 0.25f)
			return Color.Lerp(Color.blue, Color.cyan, heat / 0.25f);
		else if (heat < 0.5f)
			return Color.Lerp(Color.cyan, Color.green, (heat - 0.25f) / 0.25f);
		else if (heat < 0.75f)
			return Color.Lerp(Color.green, Color.yellow, (heat - 0.5f) / 0.25f);
		else
			return Color.Lerp(Color.yellow, Color.red, (heat - 0.75f) / 0.25f);
	}

	///// <summary>
	///// Returns an up vector for use in:
	///// <code>quaternion.LookRotation(lookVector, UpForLookRotation(lookVector)).</code>
	///// </summary>
	///// <param name="lookVector">The look vector you plan to use for quaternion.LookRotation().</param>
	///// <returns>Normally, (0, 1, 0).<br/>
	///// However, if lookVector is too close to that, returns (0, 0, 1) instead.</returns>
	//[BurstCompile]
	//public static float3 UpForLookRotation(in float3 lookVector) {
	//	return lookVector.z != 0 || lookVector.x != 0 || math.abs(lookVector.y - 1f) > 0.0001f ?
	//		math.up() :
	//		math.back();
	//}

	/// <summary>
	/// Flattens the given vector to the XZ plane.
	/// </summary>
	/// <param name="outV"></param>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FlattenY(ref float3 outV) {
		outV.y = 0;
	}

	/// <summary>
	/// Returns an up vector for use in:
	/// <code>quaternion.LookRotation(lookVector, UpForLookRotation(lookVector)).</code>
	/// </summary>
	/// <param name="lookVector">The look vector you plan to use for quaternion.LookRotation().</param>
	/// <returns>Normally, math.up().<br/>
	/// However, if lookVector is too close to that, returns math.back() instead.</returns>
	[BurstCompile]
	public static void UpForLookRotation(in float3 lookVector, out float3 upVector) {
		if (lookVector.z != 0 || lookVector.x != 0 || math.abs(lookVector.y - 1f) <= 1e-6f)
			upVector = math.up();
		else
			upVector = math.back();
	}

	[BurstCompile]
	public static bool IsNearZero(in float3 v) {
		return math.lengthsq(v) <= 0.00001f;
	}



	///////////////////////////////////////////////////// DEBUGGING

	[BurstCompile]
	public static void D_DrawPoint(in float3 position, in Color c) {
		D_DrawPoint(position, c, 9999999f);
	}

	[BurstCompile]
	public static void D_DrawPoint(in float3 position, in Color c, float t) {
		D_DrawPoint(position, c, t, 0.15f, false);
	}

	[BurstCompile]
	public static void D_DrawPoint(in float3 position, in Color c, float t, float radius, bool depthTest) {
		Debug.DrawRay(position + math.forward() * radius, 2 * radius * math.back(), c, t, depthTest);
		Debug.DrawRay(position + math.right() * radius, 2 * radius * math.left(), c, t, depthTest);
	}

	[BurstCompile]
	public static void D_DrawBox(in float3 centerPosition, in float3 size, in Color c) {
		D_DrawBox(centerPosition, size, c, 9999999f);
	}

	[BurstCompile]
	public static void D_DrawBox(in float3 centerPosition, in float3 size, in Color c, float t) {
		D_DrawBox(centerPosition, size, c, t, true);
	}

	[BurstCompile]
	public static void D_DrawBox(in float3 centerPosition, in float3 size, in Color c, float t, bool depthTest) {
		float3 topRightFront = centerPosition + size / 2f;
		float3 topLeftFront = topRightFront + math.left() * size.x;
		float3 topRightBack = topRightFront + math.back() * size.z;
		float3 topLeftBack = topLeftFront + math.back() * size.z;
		float3 botRightFront = topRightFront + math.down() * size.y;
		float3 botLeftFront = topLeftFront + math.down() * size.y;
		float3 botRightBack = topRightBack + math.down() * size.y;
		float3 botLeftBack = topLeftBack + math.down() * size.y;

		/* Top rect */
		Debug.DrawLine(topRightFront, topLeftFront, c, t, depthTest); // top front
		Debug.DrawLine(topRightFront, topRightBack, c, t, depthTest); // top right
		Debug.DrawLine(topLeftFront, topLeftBack, c, t, depthTest); // top left
		Debug.DrawLine(topRightBack, topLeftBack, c, t, depthTest); // top back

		/* Bottom rect */
		Debug.DrawLine(botRightFront, botLeftFront, c, t, depthTest); // bot front
		Debug.DrawLine(botRightFront, botRightBack, c, t, depthTest); // bot right
		Debug.DrawLine(botLeftFront, botLeftBack, c, t, depthTest); // bot left
		Debug.DrawLine(botRightBack, botLeftBack, c, t, depthTest); // bot back

		/* 4 Poles */
		Debug.DrawLine(topRightFront, botRightFront, c, t, depthTest); // front right
		Debug.DrawLine(topLeftFront, botLeftFront, c, t, depthTest); // front left
		Debug.DrawLine(topRightBack, botRightBack, c, t, depthTest); // back right
		Debug.DrawLine(topLeftBack, botLeftBack, c, t, depthTest); // back left
	}

	[BurstCompile]
	public static void D_DrawArrowCenteredAt(in float3 position, in float3 direction, float length, in Color c) {
		D_DrawArrowCenteredAt(position, direction, length, c, 9999999f);
	}

	[BurstCompile]
	public static void D_DrawArrowCenteredAt(in float3 position, in float3 direction, float length, in Color c, float t) {
		D_DrawArrowCenteredAt(position, direction, length, c, t, false);
	}

	[BurstCompile]
	public static void D_DrawArrowCenteredAt(in float3 position, in float3 direction, float length, in Color c, float t, bool depthTest) {
		if (math.lengthsq(direction) == 0) {
			D_DrawPoint(position, Color.white, t, length, depthTest);
			return;
		}
		float3 normalizedDir = math.normalize(direction);
		float3 lengthenedDir = normalizedDir * length;

		float3 crosser = math.up();
		if (direction.x == 0 && direction.y != 0 && direction.z == 0)
			crosser = math.forward();
		float3 norm = math.normalize(math.cross(normalizedDir, crosser)) * length * 0.3f;
		float3 tipPosition = position + lengthenedDir * 0.5f;
		float3 alongPosition = position + lengthenedDir * 0.5f * 0.3f;
		float3 rightTipPosition = alongPosition + norm;
		float3 leftTipPosition = alongPosition - norm;

		Debug.DrawLine(position - lengthenedDir * 0.5f, tipPosition, c, t, depthTest);
		Debug.DrawLine(tipPosition, rightTipPosition, c, t, depthTest);
		Debug.DrawLine(tipPosition, leftTipPosition, c, t, depthTest);
	}

	[BurstCompile]
	public static void D_DrawArrowStartingAt(in float3 position, in float3 direction, float length, in Color c) {
		D_DrawArrowStartingAt(position, direction, length, c, 9999999f);
	}

	[BurstCompile]
	public static void D_DrawArrowStartingAt(in float3 position, in float3 direction, float length, in Color c, float t) {
		D_DrawArrowStartingAt(position, direction, length, c, t, false);
	}

	[BurstCompile]
	public static void D_DrawArrowStartingAt(in float3 position, in float3 direction, float length, in Color c, float t, bool depthTest) {
		if (math.lengthsq(direction) == 0) {
			D_DrawPoint(position, Color.white, t, length, depthTest);
			return;
		}
		float3 normalizedDir = math.normalize(direction);
		float3 lengthenedDir = normalizedDir * length;

		float3 crosser = direction.y == 0 || direction.x != 0 || direction.z != 0 ? math.up() : math.forward();
		float3 norm = math.normalize(math.cross(normalizedDir, crosser)) * length * 0.3f;
		float3 tipPosition = position + lengthenedDir;
		float3 alongPosition = position + lengthenedDir * 0.7f;
		float3 rightTipPosition = alongPosition + norm;
		float3 leftTipPosition = alongPosition - norm;

		Debug.DrawLine(position, tipPosition, c, t, depthTest);
		Debug.DrawLine(tipPosition, rightTipPosition, c, t, depthTest);
		Debug.DrawLine(tipPosition, leftTipPosition, c, t, depthTest);
	}

	[BurstCompile]
	public static void D_DrawArrowFromTo(in float3 arrowStartPosition, in float3 arrowHeadPosition, in Color c) {
		D_DrawArrowFromTo(arrowStartPosition, arrowHeadPosition, c, 9999999f, false);
	}

	[BurstCompile]
	public static void D_DrawArrowFromTo(in float3 arrowStartPosition, in float3 arrowHeadPosition, in Color c, float t) {
		D_DrawArrowFromTo(arrowStartPosition, arrowHeadPosition, c, t, false);
	}

	[BurstCompile]
	public static void D_DrawArrowFromTo(in float3 arrowStartPosition, in float3 arrowHeadPosition, in Color c, float t, bool depthTest) {
		float3 v = arrowHeadPosition - arrowStartPosition;
		D_DrawArrowStartingAt(arrowStartPosition, v, math.length(v), c, t, depthTest);
	}

	[BurstCompile]
	public static void D_VisualizePointCloud(in NativeArray<bool> pointCloud, in PointCloudConfig pcc) {
		float3 pointOffset = new(pcc.DistBetweenPoints / 2f);
		float3 pointBoxSize = new(pcc.PointRadius * 2f);
		int YZ = pcc.numY * pcc.numZ;
		float3 size = new float3(pcc.numX, pcc.numY, pcc.numZ) * pcc.DistBetweenPoints;
		D_DrawBox(pcc.cornerPosition + size / 2f, size, Color.cyan);
		for (int x = 0; x < pcc.numX; x++)
			for (int y = 0; y < pcc.numY; y++)
				for (int z = 0; z < pcc.numZ; z++) {
					bool isVisible = pointCloud[x * YZ + y * pcc.numZ + z];
					//if (isVisible)
					//	continue;
					float3 pos = pcc.cornerPosition + new float3(x, y, z) * pcc.DistBetweenPoints + pointOffset;
					D_DrawPoint(
						pos,
						isVisible ? Color.green : Color.red,
						9999999f,
						0.15f,
						true
					);
					if (!isVisible)
						D_DrawBox(pos, pointBoxSize, Color.red);
				}
	}

}