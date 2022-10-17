using Godot;
using Tycoon.Buildings;

namespace Tycoon.Assets;

public class House : IBlueprint
{
	public string Name => "House";
	public int Cost => 5;
	public Texture2D Texture { get; } = GD.Load<Texture2D>("res://Assets/house.png");

	public Shape2D Shape { get; } = new RectangleShape2D
	{
		Size = new Vector2(512, 512),
	};
}
