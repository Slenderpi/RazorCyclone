using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class ResolutionOptions {

	/// <summary>
	/// All resolutions available on the current system, sorted by resolution (descending) and then refresh rate (descending).
	/// </summary>
	readonly Resolution[] _resolutions;
	/// <summary>
	/// All resolutions available on the current system, sorted by resolution in descending order.
	/// </summary>
	readonly int2[] _resolutionsNoRefreshRate;
	//readonly int2[] _uniqueResolutions;
	//readonly int2[] _aspectRatios;
	/// <summary>
	/// All available refresh rates on the current system, sorted by refresh rate in descending order.
	/// </summary>
	readonly RefreshRate[] _refreshRatios;



	public ResolutionOptions() {
		_resolutions = Screen.resolutions.ToArray();
		Array.Sort(_resolutions, (a, b) => {
			int cmp = b.width.CompareTo(a.width);
			if (cmp == 0)
				cmp = b.height.CompareTo(a.height);
			return cmp != 0 ? cmp : b.refreshRateRatio.CompareTo(a.refreshRateRatio);
		});
		_resolutionsNoRefreshRate = _resolutions.Select(r => new int2(r.width, r.height)).Distinct().ToArray();
		string str = "";
		string str2 = "ALL REFRESH RATES:\n";
		{
			foreach (Resolution r in _resolutions) {
				str += $"{r.width,4} x {r.height,-4} @ {r.refreshRateRatio.value}\n";
			}
			Debug.Log($"ALL RESOLUTIONS (count: {_resolutions.Length}):\n" + str);
			str = $"ALL RESOLUTIONS (no refresh rate) (count: {_resolutionsNoRefreshRate.Length}):\n";
			for (int i = 0; i < _resolutionsNoRefreshRate.Length; i++)
				str += $"{i,2}: {_resolutionsNoRefreshRate[i].x,4} x {_resolutionsNoRefreshRate[i].y,-4}\n";
			Debug.Log(str);
			//str = "Aspect ratios:\n";
		}
		//HashSet<int2> aspRatios = new();
		HashSet<RefreshRate> refRatios = new();
		foreach (Resolution r in _resolutions) {
			int2 aspRatio = Util.simplify(new(r.width, r.height));
			{
				//str += (!aspRatios.Contains(aspRatio) ? "ADDED: " : "skip: ") + $"{aspRatio.x}:{aspRatio.y} ({r.width} / {r.height})\n";
				str2 += (!refRatios.Contains(r.refreshRateRatio) ? "ADDED: " : "skip: ") + $"{r.refreshRateRatio.value}\n";
			}
			refRatios.Add(r.refreshRateRatio);
			//aspRatios.Add(aspRatio);
		}
		{
			//Debug.Log(str);
			Debug.Log(str2);
			//str = "Final aspect ratios:\n";
		}

		//_aspectRatios = aspRatios.ToArray();
		//Array.Sort(_aspectRatios, (a, b) => {
		//	int cmp = b.x.CompareTo(a.x);
		//	return cmp != 0 ? cmp : a.y.CompareTo(b.y);
		//});
		//{
		//	for (int i = 0; i < _aspectRatios.Length; i++)
		//		str += $"{i + 1,2} | {_aspectRatios[i].x}:{_aspectRatios[i].y}\n";
		//	Debug.Log(str);
		//}

		_refreshRatios = refRatios.ToArray();
		Array.Sort(_refreshRatios, (a, b) => b.CompareTo(a));
		{
			str = "Final refresh rate options:\n";
			for (int i = 0; i < _refreshRatios.Length; i++) {
				str += $"{i + 1,2} | {_refreshRatios[i].value} Hz\n";
			}
			Debug.Log(str);
		}

		//{
		//	str = "Unique resolutions:\n";
		//	_uniqueResolutions = Screen.resolutions
		//							   .Select(r => new int2(r.width, r.height))
		//							   .Distinct()
		//							   .OrderByDescending(r => r.x * r.y)
		//							   .ToArray();
		//	foreach (int2 res in _uniqueResolutions) {
		//		str += $"{res.x,4} x {res.y,-4}\n";
		//	}
		//	Debug.Log(str);
		//}

		Debug.LogWarning("---- ==== DONE ==== ----");
	}

	public Resolution[] GetAllResolutions() => _resolutions;

	public int2[] GetAllResolutionsNoRefreshRate() => _resolutionsNoRefreshRate;

	public RefreshRate[] GetAllRefreshRates() => _refreshRatios;

	/// <summary>
	/// Determines if a specific resolution is one of the known resolutions, irregardless of refresh rate.
	/// </summary>
	/// <param name="r">The resolution to check.</param>
	/// <returns>True if the resolution is found.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsKnownResolution(int2 r) {
		return IndexOf(r) != -1;
	}

	/// <summary>
	/// Finds the index of a specific resolution within the internal list of resolutions, irregardless of refresh rate.
	/// </summary>
	/// <param name="r">The resolution to find.</param>
	/// <returns>The index of the resolution, or -1 if not found.</returns>
	public int IndexOf(int2 r) {
		// TODO OPTIONAL: _resolutions is sorted in descending. Could implement a binary search method.
		for (int i = 0; i < _resolutions.Length; i++)
			if (_resolutionsNoRefreshRate[i].x == r.x && _resolutionsNoRefreshRate[i].y == r.y)
				return i;
		return -1;
	}

}