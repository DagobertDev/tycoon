using Godot;

namespace Tycoon.Components;

public readonly record struct Position(Vector2 Value)
{
	public static implicit operator Vector2(Position position) => position.Value;

	public static implicit operator Position(Vector2 value) => new(value);
}
