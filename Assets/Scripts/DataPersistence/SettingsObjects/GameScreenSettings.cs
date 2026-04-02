using System;
using UnityEngine;

public class GameScreenSettings {

	public bool IsFullscreen;
	public Resolution ScreenResolution;
	public bool IsCustomResolution;
	public bool IsVsyncEnabled;
	public float FpsLimit;
	public float FieldOfView;

	public static GameScreenSettings Default => new() {
		IsFullscreen = true,
		ScreenResolution = new() {
			width = 1920,
			height = 1080,
			refreshRateRatio = new() {
				numerator = 60,
				denominator = 1
			}
		},
		IsCustomResolution = false,
		IsVsyncEnabled = true,
		FpsLimit = 0f,
		FieldOfView = 90f
	};

	/// <summary>
	/// Creates a deep clone of this object.
	/// </summary>
	/// <returns>A deep clone.</returns>
	public GameScreenSettings Clone() {
		return new() {
			IsFullscreen = IsFullscreen,
			ScreenResolution = ScreenResolution,
			IsCustomResolution = IsCustomResolution,
			IsVsyncEnabled = IsVsyncEnabled,
			FpsLimit = FpsLimit,
			FieldOfView = FieldOfView
		};
	}

	/// <summary>
	/// Sets settings based on what the current Screen is set to. A sort of 'Init' function.
	/// </summary>
	public void SetValuesBasedOnCurrentScreen() {
		ScreenResolution = Screen.currentResolution;
		IsFullscreen = Screen.fullScreen;
		IsVsyncEnabled = QualitySettings.vSyncCount > 0;
	}

	public void SetFrom(GameScreenSettings other) {
		IsFullscreen = other.IsFullscreen;
		ScreenResolution = other.ScreenResolution;
		IsCustomResolution = other.IsCustomResolution;
		IsVsyncEnabled = other.IsVsyncEnabled;
		FpsLimit = other.FpsLimit;
		FieldOfView = other.FieldOfView;
	}

	public static bool operator ==(GameScreenSettings a, GameScreenSettings b) {
		if (ReferenceEquals(a, b)) return true;
		if (a is null || b is null) return false;
		return Mathf.Approximately(a.FpsLimit, b.FpsLimit)
			&& Mathf.Approximately(a.FieldOfView, b.FieldOfView)
			&& a.IsFullscreen == b.IsFullscreen
			&& a.IsCustomResolution == b.IsCustomResolution
			&& a.IsVsyncEnabled == b.IsVsyncEnabled
			&& a.ScreenResolution.width == b.ScreenResolution.width
			&& a.ScreenResolution.height == b.ScreenResolution.height
			&& a.ScreenResolution.refreshRateRatio.numerator == b.ScreenResolution.refreshRateRatio.numerator
			&& a.ScreenResolution.refreshRateRatio.denominator == b.ScreenResolution.refreshRateRatio.denominator;
	}

	public static bool operator !=(GameScreenSettings a, GameScreenSettings b) => !(a == b);

	public override bool Equals(object other) => this == other as GameScreenSettings;

	public override int GetHashCode() => HashCode.Combine(IsFullscreen, ScreenResolution, IsCustomResolution, IsVsyncEnabled, FpsLimit, FieldOfView);

}
