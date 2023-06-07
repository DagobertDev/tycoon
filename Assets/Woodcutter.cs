using Godot;
using Tycoon.Buildings;
using Tycoon.Components;

namespace Tycoon.Assets;

public class Woodcutter : IBlueprint, IProductionSite
{
	public string Name => "Woodcutter";
	public int Cost => 5;
	public Texture2D Texture { get; } = GD.Load<Texture2D>("res://Assets/woodcutter.png");

	public Shape2D Shape { get; } = new RectangleShape2D
	{
		Size = new Vector2(512, 512),
	};

	public Producer Producer { get; } = new(Goods.Wood, 2, 0.1);
	public int MaximumWorkers => 2;
	public InventoryCapacity InventoryCapacity => 100;
}
