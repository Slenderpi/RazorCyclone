using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[BurstCompile]
public static class Util {

	public readonly static Unity.Mathematics.Random rng = new Unity.Mathematics.Random(1);


	
#pragma warning disable IDE1006 // Naming Styles
	/// <summary>
	/// Convenience method for squaring a float.
	/// </summary>
	/// <param name="x">The float to get squared.</param>
	/// <returns>x * x</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float pow2(float x) { return x * x; }

	/// <summary>
	/// Convenience method for squaring an int.
	/// </summary>
	/// <param name="x">The int to get squared.</param>
	/// <returns>x * x</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int pow2(int x) { return x * x; }

	/// <summary>
	/// Convenience method for cubing a float.
	/// </summary>
	/// <param name="x">The float to get cubed.</param>
	/// <returns>x * x * x</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float pow3(float x) { return x * x * x; }

	/// <summary>
	/// Convenience method for cubing an int.
	/// </summary>
	/// <param name="x">The int to get cubed.</param>
	/// <returns>x * x * x</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int pow3(int x) { return x * x * x; }

	/// <summary>
	/// Alternative to math.sign() for checking sign of float but returning as int.
	/// </summary>
	/// <param name="x"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int sign(float x) {
		return (x > 0.0f ? 1 : 0) - (x < 0.0f ? 1 : 0);
	}

	/// <summary>
	/// Alternative method to math.sign() for checking sign of float3 but returning as int3.
	/// </summary>
	/// <param name="v">Vector to get signs of.</param>
	/// <returns>new int3(sign(x), sign(y), sign(z))</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int3 sign(in float3 v) {
		return new int3(sign(v.x), sign(v.y), sign(v.z));
	}

	/// <summary>
	/// Checks if too float3s are equal. Returns the result as a boolean.<br/>
	/// <br/>
	/// The normal float3= operator returns a bool3. Man wtf
	/// </summary>
	/// <param name="u"></param>
	/// <param name="v"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool equal(in float3 u, in float3 v) {
		return IsNearZero(u - v);
	}

	/// <summary>
	/// Lerps between two normalized vectors, than normalize result.<br/>
	/// If the provided vectors are the same length, the effect is similar to slerping a vector.
	/// </summary>
	/// <param name="u">Initial vector.</param>
	/// <param name="v">Final vector.</param>
	/// <param name="t">Lerp alpha.</param>
	/// <returns>norm(lerp(u, v, t))</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 nlerp(in float3 u, in float3 v, float t) {
		// NOTE: Consider using the below commented code in an `nlerpSafe()` version
		//if (math.dot(u, v) < -0.9999f) {
		//	// If u an v are near equal, we return a default direction.
		//	float3 axis = math.abs(u.x) < 0.9f
		//		? new float3(1, 0, 0)
		//		: new float3(0, 1, 0);
		//	axis = math.normalize(math.cross(u, axis));
		//	quaternion rot = quaternion.AxisAngle(axis, math.PI * t);
		//	return math.mul(rot, u);
		//}
		return math.normalize(math.lerp(u, v, t));
	}
#pragma warning restore IDE1006 // Naming Styles

	/// <summary>
	/// Predict the position of a target.
	/// </summary>
	/// <param name="pos">Position of predictor.</param>
	/// <param name="targetPos">Position of predicted (target).</param>
	/// <param name="velocity">Velocity of predictor.</param>
	/// <param name="targetVel">Velocity of predicted (target).</param>
	/// <returns>The predicted position of the target.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 PredictPosition(in float3 pos, in float3 targetPos, in float3 velocity, in float3 targetVel) {
		return IsNearZero(targetVel) ? targetPos : targetPos + targetVel * CalculatePredictTime(pos, targetPos, velocity, targetVel);
	}

	/// <summary>
	/// Math function for use by PredictPosition(). 
	/// </summary>
	/// <param name="pos">Position of predictor.</param>
	/// <param name="targetPos">Position of predicted (target).</param>
	/// <param name="velocity">Velocity of predictor.</param>
	/// <param name="targetVel">Velocity of predicted (target).</param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static float CalculatePredictTime(in float3 pos, in float3 targetPos, in float3 velocity, in float3 targetVel) {
		return math.sqrt(math.distancesq(targetPos, pos) / math.distancesq(velocity, targetVel));
	}

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
		if (lookVector.z != 0 || lookVector.x != 0 || IsNearZero(math.abs(lookVector.y - 1f)))
			upVector = math.up();
		else
			upVector = math.back();
	}

	/// <summary>
	/// Checks if the provided vector's (squared) length is less than a small hardcoded float.
	/// </summary>
	/// <param name="v">Vector to test.</param>
	/// <returns>True if less than 1e-9.</returns>
	[BurstCompile]
	public static bool IsNearZero(in float3 v) {
		return math.lengthsq(v) <= 1e-9f;
	}

	/// <summary>
	/// Checks if the distance between two vectors is less than or equal to a specific maximum.<br/>
	/// Useful for checking if two positions are within a certain distance of each other.
	/// </summary>
	/// <param name="pos0">First position (vector).</param>
	/// <param name="pos1">Second position (vector).</param>
	/// <param name="maxDist">Maximum distance.</param>
	/// <returns>lengthsq(pos0 - pos1) <= maxDist^2</returns>
	/// <exception cref="NotImplementedException"></exception>
	[BurstCompile]
	public static bool DistLeq(in float3 pos0, in float3 pos1, float maxDist) {
		return math.lengthsq(pos0 - pos1) <= pow2(maxDist);
	}

	/// <summary>
	/// Checks if the distance between two vectors is strictly less than a specific maximum.<br/>
	/// Useful for checking if two positions are within a certain distance of each other.
	/// </summary>
	/// <param name="pos0">First position (vector).</param>
	/// <param name="pos1">Second position (vector).</param>
	/// <param name="maxDist">Maximum distance.</param>
	/// <returns>lengthsq(pos0 - pos1) < maxDist^2</returns>
	/// <exception cref="NotImplementedException"></exception>
	[BurstCompile]
	public static bool DistLt(in float3 pos0, in float3 pos1, float maxDist) {
		return math.lengthsq(pos0 - pos1) < pow2(maxDist);
	}

	/// <summary>
	/// Checks if the distance between two vectors is greater than or equal to a specific minimum.<br/>
	/// Useful for checking if two positions are greater than a certain distance of each other.
	/// </summary>
	/// <param name="pos0">First position (vector).</param>
	/// <param name="pos1">Second position (vector).</param>
	/// <param name="maxDist">Minimum distance.</param>
	/// <returns>lengthsq(pos0 - pos1) >= minDist^2</returns>
	/// <exception cref="NotImplementedException"></exception>
	[BurstCompile]
	public static bool DistGeq(in float3 pos0, in float3 pos1, float minDist) {
		return math.lengthsq(pos0 - pos1) >= pow2(minDist);
	}

	/// <summary>
	/// Checks if the distance between two vectors is strictly greater than a specific minimum.<br/>
	/// Useful for checking if two positions are greater than a certain distance of each other.
	/// </summary>
	/// <param name="pos0">First position (vector).</param>
	/// <param name="pos1">Second position (vector).</param>
	/// <param name="maxDist">Minimum distance.</param>
	/// <returns>lengthsq(pos0 - pos1) > minDist^2</returns>
	/// <exception cref="NotImplementedException"></exception>
	[BurstCompile]
	public static bool DistGt(in float3 pos0, in float3 pos1, float minDist) {
		return math.lengthsq(pos0 - pos1) > pow2(minDist);
	}

	/// <summary>
	/// A burst-compatible way to get the name of an EEnemyType enum.
	/// </summary>
	/// <param name="e"></param>
	/// <returns></returns>
	public static FixedString32Bytes EEnemyTypeName(EEnemyType e) {
		return e switch {
			EEnemyType.CannonFodder => "Cannon Fodder",
			EEnemyType.HunterBasic => "Hunter Basic",
			EEnemyType.Hunter => "Hunter Empowered",
			EEnemyType.CrabBasic => "Crab Basic",
			EEnemyType.Crab => "Crab Empowered",
			EEnemyType.Turtle => "Turtle",
			EEnemyType.Centipede => "Centipede",
			EEnemyType.COUNT => "COUNT",
			EEnemyType.EnemyBase => "EnemyBase",
			EEnemyType.Weakpoint => "Weakpoint",
			EEnemyType.CentipedeMissile => "Centipede Missile",
			_ => "Unknown",
		};
	}

	/// <summary>
	/// A burst-compatible way to get the name of an EEnemyType enum.
	/// </summary>
	/// <param name="e"></param>
	/// <returns></returns>
	public static FixedString32Bytes EEnemyFormName(EEnemyForm e) {
		return e switch {
			EEnemyForm.Basic => "Basic",
			EEnemyForm.Empowered => "Empowered",
			_ => "Invalid",
		};
	}



	///////////////////////////////////////////////////// DEBUGGING

	[BurstCompile]
	public static void D_DrawPoint(in float3 position, in Color c, float t=0f, float radius=0.15f, bool depthTest=false) {
		Debug.DrawRay(position + math.forward() * radius, 2 * radius * math.back(), c, t, depthTest);
		Debug.DrawRay(position + math.right() * radius, 2 * radius * math.left(), c, t, depthTest);
	}

	[BurstCompile]
	public static void D_DrawBox(in float3 centerPosition, in float3 size, in Color c, float t=0f, bool depthTest=true) {
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
	public static void D_DrawArrowCenteredAt(in float3 position, in float3 direction, float length, in Color c, float t=0f, bool depthTest=false) {
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
	public static void D_DrawArrowStartingAt(in float3 position, in float3 direction, float length, in Color c, float t=0f, bool depthTest=false) {
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
	public static void D_DrawArrowFromTo(in float3 arrowStartPosition, in float3 arrowHeadPosition, in Color c, float t=0f, bool depthTest=false) {
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

	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void D_VisualizePointAsBox(in PointCloudConfig pcc, in int3 point, bool isUnobstructed) {
		D_VisualizePointAsBox(pcc, point, isUnobstructed ? Color.green : Color.red);
	}

	[BurstCompile]
	public static void D_VisualizePointAsBox(in PointCloudConfig pcc, in int3 point, in Color c, bool depthTest=true, float t=0f) {
		D_DrawBox(
			pcc.cornerPosition + new float3(point) * pcc.DistBetweenPoints + pcc.DistBetweenPoints / 2f,
			pcc.DistBetweenPoints,
			c,
			t,
			depthTest
		);
	}

}