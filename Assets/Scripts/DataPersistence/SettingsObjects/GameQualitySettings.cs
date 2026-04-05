using System;

public class GameQualitySettings {

	// TODO
	public QualityLevel ShadowQuality;


    public static GameQualitySettings Default => new() {
		ShadowQuality = QualityLevel.High
    };

    /// <summary>
    /// Creates a deep clone of this object.
    /// </summary>
    /// <returns>A deep clone.</returns>
    public GameQualitySettings Clone() {
        return new() {
			ShadowQuality = ShadowQuality
        };
	}

	public void SetFrom(GameQualitySettings other) {
		ShadowQuality = other.ShadowQuality;
	}

	public static bool operator ==(GameQualitySettings a, GameQualitySettings b) {
		if (ReferenceEquals(a, b)) return true;
		if (a is null || b is null) return false;
		return a.ShadowQuality == b.ShadowQuality;
	}

	public static bool operator !=(GameQualitySettings a, GameQualitySettings b) => !(a == b);

	public override bool Equals(object other) => this == other as GameQualitySettings;

	public override int GetHashCode() => HashCode.Combine(ShadowQuality);

}

public enum QualityLevel {
	Low,
	Medium,
	High,
	Ultra
}