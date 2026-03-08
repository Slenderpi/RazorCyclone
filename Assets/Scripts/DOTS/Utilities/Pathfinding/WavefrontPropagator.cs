using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TransformSystemGroup))]
public partial struct WavefrontPropagator : ISystem {

	public double WavefrontUpdateDelay;

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
		pcc.count = pcc.numX * pcc.numY * pcc.numZ;

		WavefrontUpdateDelay = 0.4f;
		lastWavefrontUpdateTime = WavefrontUpdateDelay * -2;

		if (!SystemAPI.HasSingleton<WavefrontPointCloudSingleton>()) {
			EntityManager em = state.EntityManager;
			BufferEntity = em.CreateEntity();
			em.AddComponent<WavefrontPointCloudSingleton>(BufferEntity);
			em.AddBuffer<PointCloudValue>(BufferEntity).EnsureCapacity(PointCloud.Length);
			em.AddBuffer<WavefrontValue>(BufferEntity).EnsureCapacity(PointCloud.Length);
			//em.AddComponentData(BufferEntity, pcc);
		} else {
			Debug.LogWarning("WARN: A WavefrontPointCloudSingleton was found. This shouldn't be occuring. Using found value.");
		}
		BufferEntity = SystemAPI.GetSingletonEntity<WavefrontPointCloudSingleton>();
		DynamicBuffer<PointCloudValue> PointCloudValueBuffer = SystemAPI.GetBuffer<PointCloudValue>(BufferEntity);
		DynamicBuffer<WavefrontValue> WavefrontValueBuffer = SystemAPI.GetBuffer<WavefrontValue>(BufferEntity);

		foreach (bool b in PointCloud) {
			PointCloudValueBuffer.Add(new PointCloudValue(b));
			WavefrontValueBuffer.Add(b ? new WavefrontValue(0) : new WavefrontValue(-1));
		}

		PointCloudValueBuffer = SystemAPI.GetBuffer<PointCloudValue>(BufferEntity);
		WavefrontValueBuffer = SystemAPI.GetBuffer<WavefrontValue>(BufferEntity);

		DoWavefront(ref state, new(0, 5, 0), PointCloudValueBuffer, ref WavefrontValueBuffer);
		//VisualizeWavefrontHeatmap(WavefrontUpdateDelay, WavefrontValueBuffer);
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
#if UNITY_EDITOR
			bool shouldVisualizeHeatmap = wgtarr[0].VisualizeWavefrontHeatmap;
			WavefrontUpdateDelay = wgtarr[0].WavefrontUpdateDelay;
#endif
			float3 goalPosition = eq.ToComponentDataArray<LocalTransform>(Allocator.Temp)[0].Position;

			// Do wavefront propagation
			DoWavefront(ref state, goalPosition, PointCloudValueBuffer, ref WavefrontValueBuffer);
#if UNITY_EDITOR
			if (shouldVisualizeHeatmap)
				VisualizeWavefrontHeatmap((float)WavefrontUpdateDelay + 0.001f, WavefrontValueBuffer);
#endif
		}

		// Update readers
		//new UpdateWavefrontReadersJob() {
		//	deltaTime = SystemAPI.Time.DeltaTime
		//}.ScheduleParallel();
		NativeArray<byte> diagonals = new(new byte[] { 5, 6, 9, 10, 17, 18, 20, 21, 22, 24, 25, 26, 33, 34, 36, 37, 38, 40, 41, 42 }, Allocator.Temp);
		foreach (var (reader, transform) in SystemAPI.Query<RefRW<WavefrontReader>, RefRO<LocalTransform>>()) {
			int3 currPoint = pcc.PositionToPoint(transform.ValueRO.Position);
			if (IsPointInWall(currPoint, PointCloudValueBuffer))
				continue;
			float3 pos = transform.ValueRO.Position;
			int ind = pcc.PositionToIndex(pos);
			reader.ValueRW.DescentDirection = GetDescentDirection(currPoint, diagonals, WavefrontValueBuffer);
		}
	}

	[BurstCompile]
	public void DoWavefront(ref SystemState state, in float3 GoalPosition, in DynamicBuffer<PointCloudValue> PointCloudValueBuffer, ref DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		// Get the point associated with this position
		int3 startPoint = pcc.PositionToPoint(GoalPosition);

		// Surface the point if it's in a wall
		if (!SurfacePoint(ref startPoint, PointCloudValueBuffer)) {
			//Debug.LogWarning($"WARN: The position ({GoalPosition.x}, {GoalPosition.y}, {GoalPosition.z}) was surfaced out of bounds. Wavefront will be skipped this frame.");
			return;
		}
		Util.D_DrawBox(pcc.PointToPosition(startPoint), new(pcc.DistBetweenPoints), Color.white, (float)WavefrontUpdateDelay);
		int startIndex = pcc.PointToIndex(startPoint);

		// Reset wavefront values
		for (int i = 0; i < WavefrontValueBuffer.Length; i++)
			if (WavefrontValueBuffer[i].DistCost != -1)
				WavefrontValueBuffer[i] = new WavefrontValue(0);

		// Create simple queue for BFS
		NativeQueue<int> indexQueue = new(Allocator.Temp);
		// Update and enqueue neighbors of starting position
		(NativeArray<int> neighbors, int numNeighbors) = UpdateAndGetNeighbors(startIndex, ref WavefrontValueBuffer);
		for (int i = 0; i < numNeighbors; i++)
			indexQueue.Enqueue(neighbors[i]);
		neighbors.Dispose();

		// Set starting point as a wall value so that it gets ignored by the neighbor search
		WavefrontValueBuffer[startIndex] = new WavefrontValue(-1);

		// Perform wavefront propagation (BFS) on the rest of the points
		while (!indexQueue.IsEmpty()) {
			(neighbors, numNeighbors) = UpdateAndGetNeighbors(indexQueue.Dequeue(), ref WavefrontValueBuffer);
			for (int i = 0; i < numNeighbors; i++)
				indexQueue.Enqueue(neighbors[i]);
			neighbors.Dispose();
		}
		indexQueue.Dispose();

		// Set starting point value as 0 again
		WavefrontValueBuffer[startIndex] = new WavefrontValue(0);
	}

	[BurstCompile]
	public (NativeArray<int>, int) UpdateAndGetNeighbors(int index, ref DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		NativeArray<int> neighbors = new(6, Allocator.Temp); // There can be up to 6 neighbors
		int currIndex = 0;
		int3 point = pcc.IndexToPoint(index);
		int newCost = WavefrontValueBuffer[index].DistCost + 1;

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
		 * Corresponding integer values:
		 * 5, 6, 9, 10, 17, 18, 20, 21, 22, 24, 25, 26, 33, 34, 36, 37, 38, 40, 41, 42
		 */

		byte orthogonal = 1; // Bit shift left for each orthogonal

		// Traverse orthogonal neighbors
		for (int i = 0; i < 6; i++) {
			int3 neighborPoint = new(
				point.x + orthogonal switch { 16 => -1, 32 => 1, _ => 0 },
				point.y + orthogonal switch { 4 => -1, 8 => 1, _ => 0 },
				point.z + orthogonal switch { 1 => -1, 2 => 1, _ => 0 }
			);
			if (TryUpdateNeighbor(neighborPoint, newCost, ref WavefrontValueBuffer))
				neighbors[currIndex++] = pcc.PointToIndex(neighborPoint);
			orthogonal <<= 1;
		}

		return (neighbors, currIndex);
	}

	/// <summary>
	/// Updates the given point with a new DistCost if the point is valid (in bounds, 
	/// not a wall, and unvisited).
	/// </summary>
	/// <param name="neighborPoint">Point whose DistCost needs to be updated.</param>
	/// <param name="newCost">The new DistCost to update the point to.</param>
	/// <param name="WavefrontValueBuffer">Wavefront value buffer.</param>
	/// <returns>False if neighborPoint was valid (in bounds, not a wall, and unvisited)</returns>
	[BurstCompile]
	public bool TryUpdateNeighbor(in int3 neighborPoint, int newCost, ref DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		int index = pcc.PointToIndex(neighborPoint);
		if (pcc.IsPointInBounds(neighborPoint) && WavefrontValueBuffer[index].DistCost == 0) {
			WavefrontValueBuffer[index] = new WavefrontValue(newCost);
			return true;
		}
		return false;
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
	public readonly int3 GetNeighborPointWithLeastCost(in int3 currPoint, in NativeArray<byte> diagonals, in DynamicBuffer<WavefrontValue> wavefrontValueBuffer) {
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
			if (pcc.IsPointInBounds(neighborPoint)) {
				int cost = wavefrontValueBuffer[pcc.PointToIndex(neighborPoint)].DistCost;
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
			if (!pcc.IsPointInBounds(neighborPoint))
				continue;
			int cost = wavefrontValueBuffer[pcc.PointToIndex(neighborPoint)].DistCost;
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
	/// Determines if a point in the point cloud/wavefront is a wall point.
	/// </summary>
	/// <param name="point">The point to test.</param>
	/// <param name="PointCloudValueBuffer">The point cloud.</param>
	/// <returns>True if the point represents a wall point.</returns>
	[BurstCompile]
	public readonly bool IsPointInWall(in int3 point, in DynamicBuffer<PointCloudValue> PointCloudValueBuffer) {
		return !IsIndexInAir(pcc.PointToIndex(point), PointCloudValueBuffer);
	}

	/// <summary>
	/// Determines if an index in the point cloud/wavefront is in the air.
	/// </summary>
	/// <param name="index">The index to test.</param>
	/// <param name="PointCloudValueBuffer">The point cloud.</param>
	/// <returns>Simply returns the IsUnobstructed value of the respective
	/// PointCloudValue in the point cloud.</returns>
	[BurstCompile]
	public readonly bool IsIndexInAir(int index, in DynamicBuffer<PointCloudValue> PointCloudValueBuffer) => PointCloudValueBuffer[index].IsUnobstructed;

	[BurstCompile]
	public readonly int3 GetDescentDirection(in int3 point, in NativeArray<byte> diagonals, in DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		int3 leastPoint = GetNeighborPointWithLeastCost(point, diagonals, WavefrontValueBuffer);
		return math.lengthsq(leastPoint) == 0 ? new(0, 1, 0) : leastPoint - point;
	}

	[BurstCompile]
	public NativeArray<int3> DetermineDescentDirections(in DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		NativeArray<int3> DescentDirections = new(WavefrontValueBuffer.Length, Allocator.Temp);
		NativeArray<byte> diagonals = new(new byte[] { 5, 6, 9, 10, 17, 18, 20, 21, 22, 24, 25, 26, 33, 34, 36, 37, 38, 40, 41, 42 }, Allocator.Temp);
		for (int i = 0; i < WavefrontValueBuffer.Length; i++) {
			if (WavefrontValueBuffer[i].DistCost <= 0) // Ignore walls and the goal point
				continue;
			int3 currPoint = pcc.IndexToPoint(i);
			DescentDirections[i] = GetDescentDirection(currPoint, diagonals, WavefrontValueBuffer);
		}
		return DescentDirections;
	}

	[BurstCompile]
	public void VisualizeWavefrontHeatmap(in DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		VisualizeWavefrontHeatmap(9999999f, WavefrontValueBuffer);
	}

	[BurstCompile]
	public void VisualizeWavefrontHeatmap(float drawDuration, in DynamicBuffer<WavefrontValue> WavefrontValueBuffer) {
		Color WALL_COLOR = Color.black;

		NativeArray<int3> DescentDirections = DetermineDescentDirections(WavefrontValueBuffer);

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
					int ind = pcc.PointToIndex(x, y, z);
					int cost = WavefrontValueBuffer[ind].DistCost;
					int3 descDir = DescentDirections[ind];
					float3 position = pcc.PointToPosition(x, y, z);
					if (cost == -1) {
						// Visualize wall
						//Util.D_DrawPoint(
						//	position,
						//	WALL_COLOR,
						//	drawDuration,
						//	0.15f,
						//	true
						//);
						//Util.D_DrawBox(position, pointBoxSize, WALL_COLOR, drawDuration);
						Util.D_DrawArrowCenteredAt(position, descDir, 1, WALL_COLOR, drawDuration);
					} else {
						// Draw a heatmap-colored point
						Color c = Util.MakeHeatmapColor(cost / maxDistCost);
						//Util.D_DrawPoint(
						//	position,
						//	c,
						//	drawDuration,
						//	0.4f,
						//	true
						//);
						//Util.D_DrawBox(position, airBoxSize, c, drawDuration);
						Util.D_DrawArrowCenteredAt(position, descDir, 1, c, drawDuration);
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

	public WavefrontValue(int distCost) {
		DistCost = distCost;
	}
}

//[BurstCompile]
//partial struct UpdateWavefrontReadersJob : IJobEntity {
//	public float deltaTime;

//	public void Execute(ref WavefrontReader reader, in LocalTransform transform) {
//		reader.WavefrontDirection
//	}
//}