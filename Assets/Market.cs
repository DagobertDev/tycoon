using Godot;
using Tycoon.Buildings;

namespace Tycoon.Assets;

public class Market : IBlueprint
{
	public string Name => "Market";
	public int Cost => 0;
	public Texture2D Texture { get; } = GD.Load<Texture2D>("res://Assets/market.png");

	public Shape2D Shape { get; } = new RectangleShape2D
	{
		Size = new Vector2(1024, 1024),
	};
}
