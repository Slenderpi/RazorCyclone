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
	//readonly int2[] _uniqueResolutions;
	//readonly int2[] _aspectRatios;
	/// <summary>
	/// All available refresh rates on the current system, sorted by refresh rate in descending order.
	/// </summary>
	readonly RefreshRate[] _refreshRatios;



	public ResolutionOptions() {
		_resolutions = Screen.resolutions.Distinct().ToArray();
		Array.Sort(_resolutions, (a, b) => {
			int cmp = (b.width * b.height).CompareTo(a.width * a.height);
			return cmp != 0 ? cmp : b.refreshRateRatio.CompareTo(a.refreshRateRatio);
		});
		string str = "ALL RESOLUTIONS:\n";
		string str2 = "ALL REFRESH RATES:\n";
		{
			foreach (Resolution r in _resolutions) {
				str += $"{r.width} x {r.height} @ {r.refreshRateRatio.value}\n";
			}
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

	public RefreshRate[] GetAllRefreshRates() => _refreshRatios;

	/// <summary>
	/// Determines if a specific resolution is one of the known resolutions, irregardless of refresh rate.
	/// </summary>
	/// <param name="r">The resolution to check.</param>
	/// <returns>True if the resolution is found.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsKnownResolution(Resolution r) {
		return IndexOf(r) != -1;
	}

	/// <summary>
	/// Finds the index of a specific resolution within the internal list of resolutions, irregardless of refresh rate.
	/// </summary>
	/// <param name="r">The resolution to find.</param>
	/// <returns>The index of the resolution, or -1 if not found.</returns>
	public int IndexOf(Resolution r) {
		// TODO OPTIONAL: _resolutions is sorted in descending. Could implement a binary search method.
		for (int i = 0; i < _resolutions.Length; i++)
			if (_resolutions[i].width == r.width && _resolutions[i].height == r.height)
				return i;
		return -1;
	}

}