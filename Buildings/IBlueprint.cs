using Godot;

namespace Tycoon.Buildings;

public interface IBlueprint
{
	string Name { get; }
	int Cost { get; }
	Texture2D Texture { get; }
	Shape2D Shape { get; }
}
