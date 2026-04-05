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
	/// <summary>
	/// Each resolution has its share of available refresh rates. This array tracks them.<br/>
	/// Key: option index in _resolutionsNoRefreshRate.<br/>
	/// Value: a list of available refresh rates for this resolution. Refresh rates sorted in descending order.
	/// </summary>
	readonly List<RefreshRate>[] _refreshRateOptions;
	///// <summary>
	///// All available refresh rates on the current system, sorted by refresh rate in descending order.
	///// </summary>
	//readonly RefreshRate[] _refreshRatios;



	public ResolutionOptions() {
		_resolutions = Screen.resolutions.ToArray();
		Array.Sort(_resolutions, (a, b) => {
			int cmp = b.width.CompareTo(a.width);
			if (cmp == 0)
				cmp = b.height.CompareTo(a.height);
			return cmp != 0 ? cmp : b.refreshRateRatio.CompareTo(a.refreshRateRatio);
		});
		_resolutionsNoRefreshRate = _resolutions.Select(r => new int2(r.width, r.height)).Distinct().ToArray();
		_refreshRateOptions = new List<RefreshRate>[_resolutionsNoRefreshRate.Length];
		Resolution lastRes = _resolutions[0];
		_refreshRateOptions[0] = new() { lastRes.refreshRateRatio };
		int currResIndex = 0;
		foreach (Resolution r in _resolutions) {
			if (r.height != lastRes.height || r.width != lastRes.width) {
				_refreshRateOptions[++currResIndex] = new() { lastRes.refreshRateRatio };
				lastRes = r;
			} else {
				// Don't add duplicates
				if (!Util.equal(_refreshRateOptions[currResIndex][^1], r.refreshRateRatio))
					_refreshRateOptions[currResIndex].Add(r.refreshRateRatio);
			}
		}
		{
			string str = "";
			foreach (Resolution r in _resolutions) {
				str += $"{r.width,4} x {r.height,-4} @ {r.refreshRateRatio.value}\n";
			}
			Debug.Log($"ALL RESOLUTIONS WITH ALL REFRESH RATES (count: {_resolutions.Length}):\n" + str);
			str = $"RESOLUTIONS, condensed, with refresh rate options (count: {_resolutionsNoRefreshRate.Length}):";
			for (int i = 0; i < _resolutionsNoRefreshRate.Length; i++) {
				str += $"\n{i,2}: {_resolutionsNoRefreshRate[i].x,4} x {_resolutionsNoRefreshRate[i].y,-4} |";
				foreach (RefreshRate rr in _refreshRateOptions[i])
					str += $" ({rr.value}) ";
			}
			Debug.Log(str);
		}

		Debug.LogWarning("---- ==== DONE ==== ----");
	}

	public Resolution[] GetAllResolutions() => _resolutions;

	public int2[] GetAllResolutionsNoRefreshRate() => _resolutionsNoRefreshRate;

	public RefreshRate GetRefreshRateFrom(int resolutionOption, int refreshRateOption) => _refreshRateOptions[resolutionOption][refreshRateOption];

	public List<string> GetRefreshRatesOptionsForResolution(int resolutionOption) => _refreshRateOptions[resolutionOption].Select(rr => $"{rr.value} Hz").ToList();

	/// <summary>
	/// Given the current resolution option and a RefreshRate, finds either the index of the RefreshRate in this resolution
	/// option's list of RefreshRates, or, if the exact RefreshRate does not exist, finds the next best one.<br/>
	/// <br/>
	/// Example:<br/>
	/// List of RefreshRates for a given resolution: [144, 120, 85, 60, 30]<br/>
	/// An input refreshRate of 85 returns index 2 (85);<br/>
	/// An input refreshRate of 88 returns index 1 (120);<br/>
	/// An input refreshRate of 24 returns index 4 (30);
	/// </summary>
	/// <param name="refreshRate">Input RefreshRate.</param>
	/// <param name="resolutionOption">The resolution option to search in.</param>
	/// <returns>The index of the RefreshRate if found, or the index of the next higher RefresRate if not.</returns>
	public int GetRefreshRateOptionSameOrBetterTo(in RefreshRate refreshRate, int resolutionOption) {
		// TODO OPTIONAL: The refresh rates are stored in descending order. Can use binary search to get the value
		List<RefreshRate> rates = _refreshRateOptions[resolutionOption];
		if (rates[0].value < refreshRate.value)
			// This handles the case where the given refreshRate is higher than everything in this option list
			return 0;
		int i = 0;
		for (; i < rates.Count; i++)
			if (Util.equal(rates[i], refreshRate))
				return i;
			else if (rates[i].value < refreshRate.value)
				return i - 1;
		return i - 1;
	}

	/// <summary>
	/// Given the current resolution option and a RefreshRate, finds the RefreshRate option that is closest in value
	/// to the given RefreshRate.<br/>
	/// <br/>
	/// Example:<br/>
	/// List of RefreshRates for a given resolution: [144, 120, 85, 60, 30]<br/>
	/// An input refreshRate of 85 returns index 2 (85);<br/>
	/// An input refreshRate of 80 returns index 2 (85);<br/>
	/// An input refreshRate of 70 returns index 3 (60);
	/// </summary>
	/// <param name="refreshRate">RefreshRate to be close to.</param>
	/// <param name="resolutionOption">The resolution option to search in.</param>
	/// <returns>Index of the RefreshRate option.</returns>
	public int GetRefreshRateOptionClosestToThis(in RefreshRate refreshRate, int resolutionOption) {
		// TODO OPTIONAL: The refresh rates are stored in descending order. Can use binary search to get the value
		List<RefreshRate> rates = _refreshRateOptions[resolutionOption];
		int closestIndex = 0;
		double bestDiff = math.abs(rates[closestIndex].value - refreshRate.value);
		for (int i = 1; i < rates.Count; i++) {
			double diff = math.abs(rates[i].value - refreshRate.value);
			if (diff < bestDiff) {
				bestDiff = diff;
				closestIndex = i;
			}
		}
		return closestIndex;
	}

	//public RefreshRate[] GetAllRefreshRates() => _refreshRatios;

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