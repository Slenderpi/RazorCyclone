using System;
using UnityEngine;

public class GameScreenSettings {

	public FullScreenMode FullScreenMode;
	/// <summary>
	/// The index of the current resolution in ResolutionOptions._resolutionsNoRefreshRate. Determines the current resolution.
	/// </summary>
	public int CurrentResolutionOptionChoice;
	public RefreshRate CurrentRefreshRate;
	//public int CurrentRefreshRateOptionChoice;
	public bool IsVsyncEnabled;
	/// <summary>
	/// The FPS limit. A value of -1 means unlimited.
	/// </summary>
	public int FpsLimit;
	public int FieldOfView;

	public Resolution Resolution => new() {
		width = GameManager.Resolutions.GetAllResolutionsNoRefreshRate()[CurrentResolutionOptionChoice].x,
		height = GameManager.Resolutions.GetAllResolutionsNoRefreshRate()[CurrentResolutionOptionChoice].y,
		refreshRateRatio = CurrentRefreshRate
	};

	public static GameScreenSettings Default => new() {
		FullScreenMode = FullScreenMode.ExclusiveFullScreen,
		CurrentResolutionOptionChoice = 0,
		CurrentRefreshRate = new() {
			numerator = 60,
			denominator = 1
		},
		IsVsyncEnabled = true,
		FpsLimit = -1,
		FieldOfView = 90
	};

	/// <summary>
	/// Creates a deep clone of this object.
	/// </summary>
	/// <returns>A deep clone.</returns>
	public GameScreenSettings Clone() {
		return new() {
			FullScreenMode = FullScreenMode,
			CurrentResolutionOptionChoice = CurrentResolutionOptionChoice,
			CurrentRefreshRate = CurrentRefreshRate,
			IsVsyncEnabled = IsVsyncEnabled,
			FpsLimit = FpsLimit,
			FieldOfView = FieldOfView
		};
	}

	/// <summary>
	/// Sets settings based on what the current Screen is set to. A sort of 'Init' function.
	/// </summary>
	public void SetValuesBasedOnCurrentScreen(ResolutionOptions resops) {
		FullScreenMode = Screen.fullScreenMode;
		Resolution screenResolution = Screen.currentResolution;
		CurrentResolutionOptionChoice = resops.IndexOf(new(screenResolution.width, screenResolution.height));
		IsVsyncEnabled = QualitySettings.vSyncCount > 0;
		FpsLimit = Application.targetFrameRate;
		{
			Debug.Log($"[INIT SCREEN]: FullScreenMode: {FullScreenMode} | Resolution: {screenResolution.width} x {screenResolution.height} @ {CurrentRefreshRate} (option {CurrentResolutionOptionChoice}) | IsVsyncEnabled: {IsVsyncEnabled} | FpsLimit: {FpsLimit}");
		}
	}

	public void SetFrom(GameScreenSettings other) {
		FullScreenMode = other.FullScreenMode;
		CurrentResolutionOptionChoice = other.CurrentResolutionOptionChoice;
		CurrentRefreshRate = other.CurrentRefreshRate;
		IsVsyncEnabled = other.IsVsyncEnabled;
		FpsLimit = other.FpsLimit;
		FieldOfView = other.FieldOfView;
	}

	public static bool operator ==(GameScreenSettings a, GameScreenSettings b) {
		if (ReferenceEquals(a, b)) return true;
		if (a is null || b is null) return false;
		return Mathf.Approximately(a.FpsLimit, b.FpsLimit)
			&& Mathf.Approximately(a.FieldOfView, b.FieldOfView)
			&& a.FullScreenMode == b.FullScreenMode
			&& a.CurrentResolutionOptionChoice == b.CurrentResolutionOptionChoice
			&& Util.equal(a.CurrentRefreshRate, b.CurrentRefreshRate)
			&& a.IsVsyncEnabled == b.IsVsyncEnabled;
	}

	public static bool operator !=(GameScreenSettings a, GameScreenSettings b) => !(a == b);

	public override bool Equals(object other) => this == other as GameScreenSettings;

	public override int GetHashCode() => HashCode.Combine(FullScreenMode, CurrentResolutionOptionChoice, CurrentRefreshRate, IsVsyncEnabled, FpsLimit, FieldOfView);

}
