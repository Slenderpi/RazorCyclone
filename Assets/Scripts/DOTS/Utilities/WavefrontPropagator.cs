using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct WavefrontPropagator : ISystem {

	public double WavefrontUpdateDelay;

	//public float3 GoalPosition;

	PointCloudConfig pcc;
	Entity BufferEntity;

	double lastWavefrontUpdateTime;


	public void OnCreate(ref SystemState state) {
		/** PASTE POINT CLOUD GENERATED CODE HERE **/
		pcc = new() {
			DistBetweenPoints = 3f,
			PointRadius = 0.1f,

			numX = 33,
			numY = 10,
			numZ = 33,
			cornerPosition = new float3(-50f, 0f, -50f)
		};
		NativeArray<bool> PointCloud = new(
			new bool[] { true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true },
			Allocator.Temp
		);
		/** ------------------------------------- **/

		WavefrontUpdateDelay = 1;
		lastWavefrontUpdateTime = WavefrontUpdateDelay * -2;

		if (!SystemAPI.HasSingleton<WavefrontPointCloudSingleton>()) {
			BufferEntity = state.EntityManager.CreateEntity();
			state.EntityManager.AddComponent<WavefrontPointCloudSingleton>(BufferEntity);
			state.EntityManager.AddBuffer<PointCloudValue>(BufferEntity).EnsureCapacity(PointCloud.Length);
			state.EntityManager.AddBuffer<WavefrontValue>(BufferEntity).EnsureCapacity(PointCloud.Length);
		} else {
			Debug.LogWarning("WARN: A WavefrontPointCloudSingleton was found. This shouldn't be occuring. Using found value.");
		}
		BufferEntity = SystemAPI.GetSingletonEntity<WavefrontPointCloudSingleton>();
		DynamicBuffer<PointCloudValue> PointCloudValueBuffer = SystemAPI.GetBuffer<PointCloudValue>(BufferEntity);
		DynamicBuffer<WavefrontValue> WavefrontValueBuffer = SystemAPI.GetBuffer<WavefrontValue>(BufferEntity);

		foreach (bool b in PointCloud) {
			PointCloudValueBuffer.Add(new PointCloudValue(b));
			WavefrontValueBuffer.Add(b ? new WavefrontValue(0, int3.zero) : new WavefrontValue(-1, new(0, 1, 0)));
		}

		PointCloudValueBuffer = SystemAPI.GetBuffer<PointCloudValue>(BufferEntity);
		WavefrontValueBuffer = SystemAPI.GetBuffer<WavefrontValue>(BufferEntity);

		DoWavefront(ref state, new(0, 5, 0), PointCloudValueBuffer, ref WavefrontValueBuffer);
		DetermineDescentDirections(ref WavefrontValueBuffer);

		//Util.D_VisualizePointCloud(PointCloud, pcc)
	}

	public void OnUpdate(ref SystemState state) {
		DynamicBuffer<PointCloudValue> PointCloudValueBuffer = SystemAPI.GetBuffer<PointCloudValue>(BufferEntity);
		DynamicBuffer<WavefrontValue> WavefrontValueBuffer = SystemAPI.GetBuffer<WavefrontValue>(BufferEntity);
		if (SystemAPI.Time.ElapsedTime - lastWavefrontUpdateTime >= WavefrontUpdateDelay) {
			lastWavefrontUpdateTime = SystemAPI.Time.ElapsedTime;

			// Find a WavefrontGoalTarget. If there a multiple, the first one found is chosen.
			EntityQuery eq = SystemAPI.QueryBuilder().WithAll<LocalTransform, WavefrontGoalTarget>().Build();
			NativeArray<WavefrontGoalTarget> wgtarr = eq.ToComponentDataArray<WavefrontGoalTarget>(Allocator.Temp);
			if (wgtarr.Length == 0)
				return;
			bool shouldVisualizeHeatmap = wgtarr[0].VisualizeWavefrontHeatmap;
			WavefrontUpdateDelay = wgtarr[0].WavefrontUpdateDelay;
			float3 goalPosition = eq.ToComponentDataArray<LocalTransform>(Allocator.Temp)[0].Position;

			// Do wavefront propagation
			DoWavefront(ref state, goalPosition, PointCloudValueBuffer, ref WavefrontValueBuffer);
			DetermineDescentDirections(ref WavefrontValueBuffer);
			if (shouldVisualizeHeatmap)
				VisualizeWavefrontHeatmap((float)WavefrontUpdateDelay + 0.001f, WavefrontValueBuffer);
		}

		// Update readers
		//new UpdateWavefrontReadersJob() {
		//	deltaTime = SystemAPI.Time.DeltaTime
		//}.ScheduleParallel();
		foreach (var (reader, transform) in SystemAPI.Query<RefRW<WavefrontReader>, RefRO<LocalTransform>>()) {
			int3 currPoint = PositionToPoint(transform.ValueRO.Position);
			if (IsPointInWall(currPoint, PointCloudValueBuffer))
				continue;
			float3 pos = transform.ValueRO.Position;
			int ind = PositionToIndex(pos);
			int3 ddir = WavefrontValueBuffer[ind].DescentDirection;
			reader.ValueRW.DescentDirection = WavefrontValueBuffer[PointToIndex(currPoint)].DescentDirection;
		}
	}

	[BurstCompile]
	public void DoWavefront(ref SystemState state, in float3 GoalPosition, in DynamicBuffer<PointCloudValue> PointCloudValueBuffer, ref DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		// Get the point associated with this position
		int3 startPoint = PositionToPoint(GoalPosition);

		// Surface the point if it's in a wall
		if (!SurfacePoint(ref startPoint, PointCloudValueBuffer)) {
			//Debug.LogWarning($"WARN: The position ({GoalPosition.x}, {GoalPosition.y}, {GoalPosition.z}) was surfaced out of bounds. Wavefront will be skipped this frame.");
			return;
		}
		Util.D_DrawBox(PointToPosition(startPoint), new(pcc.DistBetweenPoints), Color.white, (float)WavefrontUpdateDelay);
		int startIndex = PointToIndex(startPoint);

		// Reset wavefront values
		for (int i = 0; i < WavefrontValueBuffer.Length; i++)
			if (WavefrontValueBuffer[i].DistCost != -1)
				WavefrontValueBuffer[i] = new WavefrontValue(0, int3.zero);

		// Assume maximum queue size to be the 'surface area' of the point cloud
		NativeQueue<int> indexQueue = new(Allocator.Temp);
		// Update and enqueue neighbors of starting position
		(NativeArray<int> neighbors, int numNeighbors) = UpdateAndGetNeighbors(startIndex, ref WavefrontValueBuffer);
		for (int i = 0; i < numNeighbors; i++)
			indexQueue.Enqueue(neighbors[i]);
		neighbors.Dispose();

		// Set starting point as a wall value so that it gets ignored by the neighbor search
		WavefrontValueBuffer[startIndex] = new WavefrontValue(-1, int3.zero);

		// Perform wavefront propagation on the rest of the points
		while (!indexQueue.IsEmpty()) {
			(neighbors, numNeighbors) = UpdateAndGetNeighbors(indexQueue.Dequeue(), ref WavefrontValueBuffer);
			for (int i = 0; i < numNeighbors; i++)
				indexQueue.Enqueue(neighbors[i]);
			neighbors.Dispose();
		}
		indexQueue.Dispose();

		// Set starting point value as 0 again
		WavefrontValueBuffer[startIndex] = new WavefrontValue(0, int3.zero);

		//Debug.Log("Wavefront propagator finished.");
	}

	[BurstCompile]
	public (NativeArray<int>, int) UpdateAndGetNeighbors(int index, ref DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		NativeArray<int> neighbors = new(6, Allocator.Temp); // There can be up to 6 neighbors
		int currIndex = 0;
		int3 point = IndexToPoint(index);
		int distCostToAssign = WavefrontValueBuffer[index].DistCost + 1;
		int currDistCost = WavefrontValueBuffer[index].DistCost;

		/*
		 *	All orthogonal tests are:
		 *		00 00 01 = 1
		 *		00 00 10 = 2
		 *		00 01 00 = 4
		 *		00 10 00 = 8
		 *		01 00 00 = 16
		 *		10 00 00 = 32
		 *	
		 *	All the diagonal tests are:
		 *		00 01 01
		 *		00 01 10
		 *		00 10 01
		 *		00 10 10
		 *		01 00 01
		 *		01 00 10
		 *		01 01 00
		 *		01 01 01
		 *		01 01 10
		 *		01 10 00
		 *		01 10 01
		 *		01 10 10
		 *		10 00 01
		 *		10 00 10
		 *		10 01 00
		 *		10 01 01
		 *		10 01 10
		 *		10 10 00 
		 *		10 10 01 
		 *		10 10 10
		 * 
		 * 5, 6, 9, 10, 17, 18, 20, 21, 22, 24, 25, 26, 33, 34, 36, 37, 38, 40, 41, 42
		 */

		byte valids = 0;
		byte orthogonal = 1;

		// Traverse orthogonal neighbors
		for (int i = 0; i < 6; i++) {
			int3 neighborPoint = new(
				point.x + orthogonal switch { 16 => -1, 32 => 1, _ => 0 },
				point.y + orthogonal switch { 4 => -1, 8 => 1, _ => 0 },
				point.z + orthogonal switch { 1 => -1, 2 => 1, _ => 0 }
			);
			if (TryUpdateNeighbor(neighborPoint, currDistCost + 1, ref WavefrontValueBuffer)) {
				valids |= orthogonal;
				neighbors[currIndex++] = PointToIndex(neighborPoint);
			}
			orthogonal <<= 1;
		}

		return (neighbors, currIndex);
	}

	[BurstCompile]
	public bool TryUpdateNeighbor(in int3 neighborPoint, int newCost, ref DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		int index = PointToIndex(neighborPoint);
		if (IsPointInBounds(neighborPoint) && WavefrontValueBuffer[index].DistCost == 0) {
			WavefrontValueBuffer[index] = new WavefrontValue(newCost, int3.zero);
			return true;
		}
		return false;
	}

	[BurstCompile]
	public void DetermineDescentDirections(ref DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		NativeArray<byte> diagonals = new(new byte[] { 5, 6, 9, 10, 17, 18, 20, 21, 22, 24, 25, 26, 33, 34, 36, 37, 38, 40, 41, 42 }, Allocator.Temp);
		for (int i = 0; i < WavefrontValueBuffer.Length; i++) {
			int currCost = WavefrontValueBuffer[i].DistCost;
			if (currCost <= 0) // Ignore walls and the goal point
				continue;
			int3 currPoint = IndexToPoint(i);
			int3 leastPoint = GetNeighborPointWithLeastCost(currPoint, diagonals, ref WavefrontValueBuffer);
			WavefrontValueBuffer[i] = new WavefrontValue(
				currCost,
				math.lengthsq(leastPoint) == 0 ? new(0, 1, 0) : leastPoint - currPoint
			);
		}
	}

	/// <summary>
	/// Given a point, searches its neighbors for the point with the least DistCost.<br/>
	/// Neighbors that are diagonal to the point are only checked if all bordering
	/// orthogonals are not walls. This helps ensure that entities pathfinding with the
	/// wavefront don't run into corners.
	/// </summary>
	/// <param name="currPoint">The point whose neighbors will be searched. This point can be in the wall.</param>
	/// <param name="diagonals">Pre-determined list of bytes whose binaries represent the diagonals.</param>
	/// <param name="wavefrontValueBuffer">Wavefront value buffer.</param>
	/// <returns>If currPoint is trapped in a wall, int3.zero is returned.<br/>
	/// Otherwise, the neighboring point with the least DistCost is returned.</returns>
	[BurstCompile]
	public int3 GetNeighborPointWithLeastCost(in int3 currPoint, in NativeArray<byte> diagonals, ref DynamicBuffer<WavefrontValue> wavefrontValueBuffer) {
		int leastCost = 99999999;
		int3 leastPoint = int3.zero;
		byte orthogonal = 1; // Bit shift left for each orthogonal
		byte valids = 0;

		// Check orthogonal neighbors first
		for (int i = 0; i < 6; i++) {
			int3 neighborPoint = new(
				currPoint.x + orthogonal switch { 16 => -1, 32 => 1, _ => 0},
				currPoint.y + orthogonal switch { 4 => -1, 8 => 1, _ => 0},
				currPoint.z + orthogonal switch { 1 => -1, 2 => 1, _ => 0}
			);
			if (IsPointInBounds(neighborPoint)) {
				int cost = wavefrontValueBuffer[PointToIndex(neighborPoint)].DistCost;
				if (cost != -1) {
					valids |= orthogonal;
					if (cost < leastCost) {
						leastCost = cost;
						leastPoint = neighborPoint;
					}
				}
			}
			orthogonal <<= 1;
		}

		if (valids == 0) {
			// This point is trapped in a wall, is in (overlapping) a wall, or is unreachable
			// Assuming we never have a point that's trapped in a wall, and we never have
			// an unreachable point, this point must be in (overlapping) a wall. External
			// functions calling this function should handle this situation according to that.
			return leastPoint; // Should still be int3.zero, which we treat as an error value
		}

		// Orthogonal neighbors tell us which diagonal neighbors are traversable.
		// Now check diagonal neighbors
		foreach (byte dir in diagonals) {
			int possibleDir = valids & dir;
			if (possibleDir != dir) // If any orthogonal part of a diagonal is blocked, don't check it
				continue;
			int3 neighborPoint = new(
				currPoint.x + (possibleDir & 48) switch { 16 => -1, 32 => 1, _ => 0 },
				currPoint.y + (possibleDir & 12) switch { 4 => -1, 8 => 1, _ => 0 },
				currPoint.z + (possibleDir & 3) switch { 1 => -1, 2 => 1, _ => 0 }
			);
			if (!IsPointInBounds(neighborPoint))
				continue;
			int cost = wavefrontValueBuffer[PointToIndex(neighborPoint)].DistCost;
			if (cost != -1 && cost <= leastCost) {
				leastCost = cost;
				leastPoint = neighborPoint;
			}
		}

		return leastPoint;
	}

	/// <summary>
	/// If a point is in a wall, the point will be 'floated' to the surface of the wall.<br/>
	/// </summary>
	/// <param name="point">The point to surface.</param>
	/// <param name="PointCloudValueBuffer">The point cloud.</param>
	/// <returns>True if the point is successfully surfaced.<br/>
	/// False if the surface of the wall is beyond the bounds of the point cloud/wavefront
	/// described by the pcc.</returns>
	[BurstCompile]
	public bool SurfacePoint(ref int3 point, in DynamicBuffer<PointCloudValue> PointCloudValueBuffer) {
		while (IsPointInWall(point, PointCloudValueBuffer)) {
			point.y += 1;
			if (point.y >= pcc.numY)
				return false;
		}
		return true;
	}

	/// <summary>
	/// Determines if a point is in the bounds of the point cloud/wavefront as determined by
	/// the pcc.<br/>
	/// Bound limits include 0 and exclude the num. I.e. for x, the limit interval is [0, pcc.numX).
	/// </summary>
	/// <param name="point">A point to test.</param>
	/// <returns>True if in bounds (e.g. for x, true if within [0, pcc.numX)).</returns>
	[BurstCompile]
	public bool IsPointInBounds(in int3 point) {
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
	public bool IsPointInBounds(int x, int y, int z) {
		return x >= 0 && y >= 0 && z >= 0 && x < pcc.numX && y < pcc.numY && z < pcc.numZ;
	}

	/// <summary>
	/// Determines if a point in the point cloud/wavefront is a wall point.
	/// </summary>
	/// <param name="point">The point to test.</param>
	/// <param name="PointCloudValueBuffer">The point cloud.</param>
	/// <returns>True if the point represents a wall point.</returns>
	[BurstCompile]
	public bool IsPointInWall(in int3 point, in DynamicBuffer<PointCloudValue> PointCloudValueBuffer) {
		return !IsIndexInAir(PointToIndex(point), PointCloudValueBuffer);
	}

	/// <summary>
	/// Determines if an index in the point cloud/wavefront is in the air.
	/// </summary>
	/// <param name="index">The index to test.</param>
	/// <param name="PointCloudValueBuffer">The point cloud.</param>
	/// <returns>Simply returns the IsUnobstructed value of the respective
	/// PointCloudValue in the point cloud.</returns>
	[BurstCompile]
	public bool IsIndexInAir(int index, in DynamicBuffer<PointCloudValue> PointCloudValueBuffer) {
		return PointCloudValueBuffer[index].IsUnobstructed;
	}

	/// <summary>
	/// Given a position in the world, determines its point in a point cloud/wavefront.<br/>
	/// The position will be clamped to the bounds of the point cloud/wavefront.
	/// </summary>
	/// <param name="pos">Position in world</param>
	/// <returns>Closest position in a point cloud/wavefront represented by the PointCloudConfig</returns>
	[BurstCompile]
	public int3 PositionToPoint(in float3 pos) {
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
	public int3 PositionToPoint(float x, float y, float z) {
		return new(
			(int)math.clamp((x - pcc.cornerPosition.x) / pcc.DistBetweenPoints, 0, pcc.numX - 1),
			(int)math.clamp((y - pcc.cornerPosition.y) / pcc.DistBetweenPoints, 0, pcc.numY - 1),
			(int)math.clamp((z - pcc.cornerPosition.z) / pcc.DistBetweenPoints, 0, pcc.numZ - 1)
		);
	}

	/// <summary>
	/// Given a point in the point cloud/wavefront, determines the position in the world it represents.
	/// </summary>
	/// <param name="point">Point in the point cloud/wavefront</param>
	/// <returns>Associated position in the world</returns>
	[BurstCompile]
	public float3 PointToPosition(in int3 point) {
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
	public float3 PointToPosition(int x, int y, int z) {
		return new(
			x * pcc.DistBetweenPoints + pcc.cornerPosition.x + pcc.DistBetweenPoints / 2f,
			y * pcc.DistBetweenPoints + pcc.cornerPosition.y + pcc.DistBetweenPoints / 2f,
			z * pcc.DistBetweenPoints + pcc.cornerPosition.z + pcc.DistBetweenPoints / 2f
		);
	}

	/// <summary>
	/// Given a point in the point cloud, determines the respective index in the PointCloudValue/WavefrontValue list.<br/>
	/// The math for it is: i = x * YZ + y * Z + z
	/// </summary>
	/// <param name="point">Point in the point cloud/wavefront</param>
	/// <returns>Index in the point cloud/wavefront list</returns>
	[BurstCompile]
	public int PointToIndex(in int3 point) {
		return PointToIndex(point.x, point.y, point.z);
	}

	/// <summary>
	/// Given a point in the point cloud, determines the respective index in the PointCloudValue/WavefrontValue list.<br/>
	/// The math for it is: i = x * YZ + y * Z + z
	/// </summary>
	/// <param name="point">Point in the point cloud/wavefront</param>
	/// <returns>Index in the point cloud/wavefront list</returns>
	[BurstCompile]
	public int PointToIndex(int x, int y, int z) {
		return x * pcc.numY * pcc.numZ + y * pcc.numZ + z;
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
	public int3 IndexToPoint(int index) {
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
		int YZ = pcc.numY * pcc.numZ;
		return new(
			index / YZ,
			index % YZ / pcc.numZ,
			index % pcc.numZ
		);
	}

	/// <summary>
	/// Converts a world position to the corresponding index in the point cloud/wavefront buffer.
	/// </summary>
	/// <param name="position">Position in the world</param>
	/// <returns>Corresponding index in the point cloud/wavefront buffer</returns>
	[BurstCompile]
	public int PositionToIndex(in float3 position) {
		return PointToIndex(PositionToPoint(position));
	}

	[BurstCompile]
	public void VisualizeWavefrontHeatmap(in DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		VisualizeWavefrontHeatmap(9999999f, WavefrontValueBuffer);
	}

	[BurstCompile]
	public void VisualizeWavefrontHeatmap(float drawDuration, in DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		Color WALL_COLOR = Color.black;

		//Debug.Log("Visualizing current wavefront as a heatmap.");
		float maxDistCost = -1;
		foreach (WavefrontValue wfv in WavefrontValueBuffer)
			if (wfv.DistCost > maxDistCost)
				maxDistCost = wfv.DistCost;
		if (maxDistCost <= 0) {
			Debug.LogWarning("WARN: Max dist cost in this wave front was found to be <= 0. Using a value of 1 for heatmap visual.");
			maxDistCost = 1;
		}
		//Debug.Log($"Maximum DistCost of this wavefront: {maxDistCost}.");
		float3 pointBoxSize = new(pcc.PointRadius);
		float3 airBoxSize = new(0.3f);
		for (int x = 0; x < pcc.numX; x++)
			for (int y = 0; y < pcc.numY; y++)
				for (int z = 0; z < pcc.numZ; z++) {
					WavefrontValue val = WavefrontValueBuffer[PointToIndex(x, y, z)];
					float3 position = PointToPosition(x, y, z);
					if (val.DistCost == -1) {
						// Visualize wall
						//Util.D_DrawPoint(
						//	position,
						//	WALL_COLOR,
						//	drawDuration,
						//	0.15f,
						//	true
						//);
						//Util.D_DrawBox(position, pointBoxSize, WALL_COLOR, drawDuration);
						Util.D_DrawArrowCenteredAt(position, val.DescentDirection, 1, WALL_COLOR, drawDuration);
					} else {
						// Draw a heatmap-colored point
						Color c = Util.MakeHeatmapColor(val.DistCost / maxDistCost);
						//Util.D_DrawPoint(
						//	position,
						//	c,
						//	drawDuration,
						//	0.4f,
						//	true
						//);
						//Util.D_DrawBox(position, airBoxSize, c, drawDuration);
						Util.D_DrawArrowCenteredAt(position, val.DescentDirection, 1, c, drawDuration);
					}
				}
	}

}

public struct WavefrontPointCloudSingleton : IComponentData {}

public struct PointCloudValue : IBufferElementData {
	public bool IsUnobstructed;

	public PointCloudValue(bool isUnobstructed) {
		IsUnobstructed = isUnobstructed;
	}
}

public struct WavefrontValue : IBufferElementData {
	public int DistCost;
	public int3 DescentDirection;

	public WavefrontValue(int distCost, int3 descentDirection) {
		DistCost = distCost;
		DescentDirection = descentDirection;
	}
}

//[BurstCompile]
//partial struct UpdateWavefrontReadersJob : IJobEntity {
//	public float deltaTime;

//	public void Execute(ref WavefrontReader reader, in LocalTransform transform) {
//		reader.WavefrontDirection
//	}
//}