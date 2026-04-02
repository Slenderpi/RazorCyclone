using System;

public class GameSettings {

    public GameControlSettings ControlSettings;

    public GameScreenSettings ScreenSettings;

    public GameQualitySettings QualitySettings;

    public GameAudioSettings AudioSettings;

    public static GameSettings Default => new() {
        ControlSettings = GameControlSettings.Default,
        ScreenSettings = GameScreenSettings.Default,
        QualitySettings = GameQualitySettings.Default,
        AudioSettings = GameAudioSettings.Default
    };

    /// <summary>
    /// Creates a deep clone of this object.
    /// </summary>
    /// <returns>A deep clone.</returns>
    public GameSettings Clone() {
        return new() {
            ControlSettings = ControlSettings.Clone(),
            ScreenSettings = ScreenSettings.Clone(),
            QualitySettings = QualitySettings.Clone(),
            AudioSettings = AudioSettings.Clone()
		};
	}

	public static bool operator ==(GameSettings a, GameSettings b) {
		if (ReferenceEquals(a, b)) return true;
		if (a is null || b is null) return false;
		return a.ControlSettings == b.ControlSettings
            && a.ScreenSettings == b.ScreenSettings
            && a.QualitySettings == b.QualitySettings
            && a.AudioSettings == b.AudioSettings;
	}

	public static bool operator !=(GameSettings a, GameSettings b) => !(a == b);

	public override bool Equals(object other) => this == other as GameSettings;

	public override int GetHashCode() => HashCode.Combine(ControlSettings, ScreenSettings, QualitySettings, AudioSettings);

}