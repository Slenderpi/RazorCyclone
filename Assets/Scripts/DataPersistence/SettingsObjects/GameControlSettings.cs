using System;

public class GameControlSettings {
	public float MouseSensitivity;

	public static GameControlSettings Default => new() {
		MouseSensitivity = 45f
	};

	/// <summary>
	/// Creates a deep clone of this object.
	/// </summary>
	/// <returns>A deep clone.</returns>
	public GameControlSettings Clone() {
		return new() {
			MouseSensitivity = MouseSensitivity
		};
	}

	public void SetFrom(GameControlSettings other) {
		MouseSensitivity = other.MouseSensitivity;
	}

	public static bool operator ==(GameControlSettings a, GameControlSettings b) {
		if (ReferenceEquals(a, b)) return true;
		if (a is null || b is null) return false;
		return a.MouseSensitivity == b.MouseSensitivity;
	}

	public static bool operator !=(GameControlSettings a, GameControlSettings b) => !(a == b);

	public override bool Equals(object other) => this == other as GameControlSettings;

	public override int GetHashCode() => HashCode.Combine(MouseSensitivity);

}
