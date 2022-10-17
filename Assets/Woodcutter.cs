using Godot;
using Tycoon.Buildings;

namespace Tycoon.Assets;

public class Woodcutter : IBlueprint
{
	public string Name => "Woodcutter";
	public int Cost => 5;
	public Texture2D Texture { get; } = GD.Load<Texture2D>("res://Assets/woodcutter.png");

	public Shape2D Shape { get; } = new RectangleShape2D
	{
		Size = new Vector2(512, 512),
	};
}
