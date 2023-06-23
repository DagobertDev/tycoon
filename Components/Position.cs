using Godot;

namespace Tycoon.Components;

public readonly record struct Position(Vector2 Value)
{
	public static implicit operator Vector2(Position position)
	{
		return position.Value;
	}

	public static implicit operator Position(Vector2 value)
	{
		return new Position(value);
	}
}
