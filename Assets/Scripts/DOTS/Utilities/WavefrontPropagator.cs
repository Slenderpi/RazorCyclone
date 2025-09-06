using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
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
			DistBetweenPoints = 4f,
			PointRadius = 0f,

			numX = 25,
			numY = 7,
			numZ = 25,
			cornerPosition = new float3(-50f, 0f, -50f)
		};
		NativeArray<bool> PointCloud = new(
			new bool[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true },
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

		Debug.Log($"Capacity: {WavefrontValueBuffer.Capacity} | Length: {WavefrontValueBuffer.Length}");

		foreach (bool b in PointCloud) {
			PointCloudValueBuffer.Add(new PointCloudValue(b));
			WavefrontValueBuffer.Add(b ? new WavefrontValue(0, int3.zero) : new WavefrontValue(-1, new(0, 1, 0)));
		}

		PointCloudValueBuffer = SystemAPI.GetBuffer<PointCloudValue>(BufferEntity);
		WavefrontValueBuffer = SystemAPI.GetBuffer<WavefrontValue>(BufferEntity);

		DoWavefront(ref state, new(0, 5, 0), PointCloudValueBuffer, ref WavefrontValueBuffer);

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
			Debug.Log(
				$"Position: {pos} | Index : {ind} | DescentDirection: {ddir}"
			);
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

		// Set starting point value as 0 again
		WavefrontValueBuffer[startIndex] = new WavefrontValue(0, int3.zero);

		//Debug.Log("Wavefront propagator finished.");
	}

	[BurstCompile]
	public (NativeArray<int>, int) UpdateAndGetNeighbors(int index, ref DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		//NativeArray<int> neighbors = new(26, Allocator.Temp); // There can be up to 26 neighbors
		NativeArray<int> neighbors = new(6, Allocator.Temp); // There can be up to 6 neighbors
		int currIndex = 0;
		int3 point = IndexToPoint(index);
		int distCostToAssign = WavefrontValueBuffer[index].DistCost + 1;

		for (int i = 0; i < 6; i++) {
			//if (i == 13) continue; // (0, 0, 0) is skipped
			//					   // Convert i to a ternary number, though each digit is (in base 10) decreased by 1
			//					   // so that we can have digit values {-1, 0, 1} for neighbor offsetting
			//int3 offset = new(
			//	i / 9 - 1, // If i could go higher then we'd do i % 27 / 9 - 1 but i will always be < 27
			//	i % 9 / 3 - 1,
			//	i % 3 - 1
			//);
			int3 offset = new(
				i == 2 ? -1 : i == 3 ? 1 : 0,
				i == 0 ? -1 : i == 1 ? 1 : 0,
				i == 4 ? -1 : i == 5 ? 1 : 0
			);
			int3 neighborPoint = point + offset;
			if (IsPointInBounds(neighborPoint) && WavefrontValueBuffer[PointToIndex(neighborPoint)].DistCost == 0) {
				int indexOfNeighbor = PointToIndex(neighborPoint);
				neighbors[currIndex] = indexOfNeighbor;
				WavefrontValueBuffer[indexOfNeighbor] = new WavefrontValue(
					distCostToAssign,
					-offset
				);
				currIndex++;
			}
		}

		return (neighbors, currIndex);
	}

	[BurstCompile]
	public bool SurfacePoint(ref int3 point, in DynamicBuffer<PointCloudValue> PointCloudValueBuffer) {
		while (IsPointInWall(point, PointCloudValueBuffer)) {
			point.y += 1;
			if (point.y >= pcc.numY)
				return false;
		}
		return true;
	}

	[BurstCompile]
	public bool IsPointInBounds(in int3 point) {
		return point.x >= 0 && point.y >= 0 && point.z >= 0 && point.x < pcc.numX && point.y < pcc.numY && point.z < pcc.numZ;
	}

	[BurstCompile]
	public bool IsPointInWall(in int3 point, in DynamicBuffer<PointCloudValue> PointCloudValueBuffer) {
		return !IsIndexInAir(PointToIndex(point), PointCloudValueBuffer);
	}

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