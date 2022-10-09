using Godot;

namespace Tycoon.Buildings;

public class House : IBlueprint
{
	public string Name => "House";
	public Texture2D Texture { get; } = GD.Load<Texture2D>("res://icon.svg");
	public int Cost => 5;
}
