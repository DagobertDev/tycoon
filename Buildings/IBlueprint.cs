using Godot;

namespace Tycoon.Buildings;

public interface IBlueprint
{
	int Cost { get; }
	string Name { get; }
	Texture2D Texture { get; }
}
