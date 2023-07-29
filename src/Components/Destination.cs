using Godot;

namespace Tycoon.Components;

public readonly record struct Destination(Vector2 Value)
{
	public static implicit operator Vector2(Destination destination)
	{
		return destination.Value;
	}

	public static implicit operator Destination(Vector2 value)
	{
		return new Destination(value);
	}
}
