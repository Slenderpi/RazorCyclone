using System;

public class GameAudioSettings {

	public int MasterVolume = 100;
	public int SoundVolume = 100;
	public int MusicVolume = 100;

	public static GameAudioSettings Default => new() {
		MasterVolume = 100,
		SoundVolume = 100,
		MusicVolume = 100
    };
	
	/// <summary>
	/// Creates a deep clone of this object.
	/// </summary>
	/// <returns>A deep clone.</returns>
	public GameAudioSettings Clone() {
		return new() {
			MasterVolume = MasterVolume,
			SoundVolume = SoundVolume,
			MusicVolume = MusicVolume
		};
	}

	public void SetFrom(GameAudioSettings other) {
		MasterVolume = other.MasterVolume;
		SoundVolume = other.SoundVolume;
		MusicVolume = other.MusicVolume;
	}

	public static bool operator ==(GameAudioSettings a, GameAudioSettings b) {
		if (ReferenceEquals(a, b)) return true;
		if (a is null || b is null) return false;
		return a.MasterVolume == b.MasterVolume
			&& a.SoundVolume == b.SoundVolume
			&& a.MusicVolume == b.MusicVolume;
	}

	public static bool operator !=(GameAudioSettings a, GameAudioSettings b) => !(a == b);

	public override bool Equals(object other) => this == other as GameAudioSettings;

	public override int GetHashCode() => HashCode.Combine(MasterVolume, SoundVolume, MusicVolume);

}
