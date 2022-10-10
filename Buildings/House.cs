using Godot;

namespace Tycoon.Buildings;

public class House : IBlueprint
{
	public string Name => "House";
	public int Cost => 5;
	public Texture2D Texture { get; } = GD.Load<Texture2D>("res://icon.svg");

	public Shape2D Shape { get; } = new RectangleShape2D
	{
		Size = new Vector2(128, 128),
	};
}
