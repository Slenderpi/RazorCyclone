using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class SparseVoxelOctreeGeneratorGO : MonoBehaviour {

    //public Vector3 BoundingVolumeCenter = new(0, 0, 0);
	public float BoundingVolumeSideLength = 100f;
	public float MinimumVoxelSize = 1f;

	[Tooltip("If true, the SVO algorithm will run step by step.\nAllows you to use the step button next to the playtest button. Useful for debugging.\nChanging this value at runtime will be ignored.")]
	public bool RunStepByStep = true;
	bool _runStepByStep = true;
	public bool DrawEmptySpaces = true;
	public bool DrawFilledSpaces = true;

	Vector3 BoundingVolumeCenter;

	uint LayersToCollideWith;

	bool flipFlop = true;
	float _algoDebugDrawTime = 0;
	bool done = false;

	OctreeNodeManaged svoRoot;
	Stack<OctreeExploreRequestManaged> exploreStack;
	OctreeExploreRequestManaged currRequest;
	bool currIsFilled;
	float currVoxelSize;



	private void Awake() {
		_runStepByStep = RunStepByStep;
		if (_runStepByStep)
			_algoDebugDrawTime = 9999999f;
		currVoxelSize = BoundingVolumeSideLength;
		LayersToCollideWith = ~(
			1u << 5 | // UI
			1u << 6 | // Player
			1u << 7 | // EnemyHitbox
			1u << 12 | // EnemyTrigger
			1u << 13 | // InvisBoidWall
			1u << 14 | // EnemyToWorld
			1u << 15 | // EnemyWeapon
			1u << 16 // MainCamera
		);
	}

	private void Start() {
		BoundingVolumeCenter = transform.position;

		svoRoot = new();

		svoRoot.PhysicalPosition = BoundingVolumeCenter;
		svoRoot.PhysicalSize = BoundingVolumeSideLength;

		exploreStack = new();
		exploreStack.Push(new(BoundingVolumeSideLength, BoundingVolumeCenter, svoRoot));

		Util.D_DrawCube(BoundingVolumeCenter, BoundingVolumeSideLength, Color.white);
	}

	private void Update() {
		// do test first
		// then do split
		// these two steps will be done one after another bc better debugging
		if (done) {
			if (!_runStepByStep)
				VisualizeOctree2();
			return;
		}
		if (_runStepByStep) {
			if (flipFlop) {
				if (exploreStack.Count == 0) {
					done = true;
					Debug.LogWarning("==== SVO DONE ====");
					return;
				}
				currRequest = exploreStack.Pop();
				DoCollisionTest();
			} else
				DoSplit();
			flipFlop = !flipFlop;
		} else {
			while (!done) {
				if (exploreStack.Count == 0) {
					done = true;
					Debug.LogWarning("==== SVO DONE ====");
					break;
				}
				currRequest = exploreStack.Pop();
				DoCollisionTest();
				DoSplit();
			}
			GroupFilledNodes();
			Debug.LogWarning("==== FILLED NODE GROUPING DONE ====");
		}
	}

	private void DoCollisionTest() {
		// Test for collision
		currIsFilled = Physics.CheckBox(currRequest.PhysicalPosition, new float3(currRequest.PhysicalSize / 2f), Quaternion.identity, 1 << 0);
		currRequest.Node.IsFilled = currIsFilled;

		Util.D_DrawCube(currRequest.PhysicalPosition, currRequest.PhysicalSize, Color.magenta, 0, false);
		Util.D_DrawStarPoint(currRequest.PhysicalPosition, currIsFilled ? Color.red : Color.green, 0, currRequest.PhysicalSize / 2f);
	}

	private void DoSplit() {
		if (!currRequest.Node.IsFilled) {
			if (DrawEmptySpaces)
				//Util.D_DrawCube(currRequest.PhysicalPosition, currRequest.PhysicalSize, Color.green, _algoDebugDrawTime);
				Util.D_DrawCube(currRequest.Node.PhysicalPosition, currRequest.Node.PhysicalSize, Color.green, _algoDebugDrawTime);
			return;
		}
		float halfSize = currRequest.PhysicalSize / 2f;
		if (halfSize < MinimumVoxelSize) {
			if (DrawFilledSpaces)
				//Util.D_DrawCube(currRequest.PhysicalPosition, currRequest.PhysicalSize, Color.red, _algoDebugDrawTime);
				Util.D_DrawCube(currRequest.Node.PhysicalPosition, currRequest.Node.PhysicalSize, Color.red, _algoDebugDrawTime);
			return;
		}
		float quartSize = halfSize / 2f;
		currRequest.Node.children = new OctreeNodeManaged[8];
		int index = 7;
		for (int y = -1; y <= 1; y += 2) { // One at -y, one at +y
			for (int x = -1; x <= 1; x += 2) { // One at -x, one at +x
				for (int z = -1; z <= 1; z += 2) { // One at -z, one at +z
					float3 offset = new float3(x, y, z) * quartSize;
					float3 splitPosition = currRequest.PhysicalPosition + offset;

					OctreeNodeManaged child = new();
					child.PhysicalPosition = splitPosition;
					child.PhysicalSize = halfSize;

					currRequest.Node.children[index--] = child;
					exploreStack.Push(new(halfSize, splitPosition, child));

					Util.D_DrawCube(splitPosition, halfSize, Color.cyan, 0, false);
				}
			}
		}
	}

	void GroupFilledNodes() {
		if (svoRoot != null)
			GroupFilledNodes_Internal(svoRoot);
		else
			Debug.LogError("GroupFilledNodes(): svoRoot is null!");
		Debug.LogWarning($"Times called: {timesCalled} | From !currIsFilled: {nif} | From children null: {cn} | from allChildrenAreFilled: {acaf} (vs not: {nacaf})");
	}

	int nif = 0;
	int cn = 0;
	int acaf = 0;
	int nacaf = 0;
	int timesCalled = 0;

	// Return false if curr is not pure collision
	bool GroupFilledNodes_Internal(OctreeNodeManaged curr) {
		if (!curr.IsFilled)
			return false;
		if (curr.children == null)
			return true;
		bool allChildrenAreFilled = true;
		foreach (OctreeNodeManaged n in curr.children)
			allChildrenAreFilled &= GroupFilledNodes_Internal(n);
		if (allChildrenAreFilled)
			curr.children = null;
		return allChildrenAreFilled;
	}

	void VisualizeOctree() {
		VisualizeOctree_Internal(svoRoot, BoundingVolumeCenter, BoundingVolumeSideLength);
	}

	void VisualizeOctree_Internal(OctreeNodeManaged node, float3 position, float size) {
		if (node == null) {
			Debug.LogWarning("Null reached");
			return;
		}
		if (node.IsFilled) {
			if (node.children == null) {
				if (DrawFilledSpaces)
					Util.D_DrawCube(position, size, Color.red);
			} else {
				float halfSize = size / 2f;
				float quartSize = halfSize / 2f;
				int index = 0;
				for (int y = 1; y >= -1; y -= 2) { // One at +y, one at -y
					for (int x = 1; x >= -1; x -= 2) { // One at +x, one at -x
						for (int z = 1; z >= -1; z -= 2) { // One at +z, one at -z
							VisualizeOctree_Internal(node.children[index++], position + new float3(x, y, z) * quartSize, halfSize);
						}
					}
				}
			}
		} else {
			if (DrawEmptySpaces)
				Util.D_DrawCube(position, size, Color.green);
		}
	}

	void VisualizeOctree2() {
		if (svoRoot != null)
			VisualizeOctree2_Internal(svoRoot);
	}

	void VisualizeOctree2_Internal(OctreeNodeManaged node) {
		if (node.IsFilled) {
			if (node.children == null) {
				if (DrawFilledSpaces)
					Util.D_DrawCube(node.PhysicalPosition, node.PhysicalSize, Color.red);
			} else {
				foreach (OctreeNodeManaged n in node.children)
					VisualizeOctree2_Internal(n);
			}
		} else {
			if (DrawEmptySpaces)
				Util.D_DrawCube(node.PhysicalPosition, node.PhysicalSize, Color.green);
		}
	}
}

class OctreeNodeManaged {
	public bool IsFilled;
	public OctreeNodeManaged[] children;

	public float3 PhysicalPosition;
	public float PhysicalSize;

	public OctreeNodeManaged() { }

	public OctreeNodeManaged(bool isFilled) {
		IsFilled = isFilled;
	}
}

class OctreeExploreRequestManaged {
	public float PhysicalSize;
	public float3 PhysicalPosition;
	public OctreeNodeManaged Node;

	public OctreeExploreRequestManaged(float physicalSize, float3 physicalPosition, OctreeNodeManaged node) {
		PhysicalSize = physicalSize;
		PhysicalPosition = physicalPosition;
		Node = node;
	}
}