using Godot;
using Tycoon.Buildings;
using Tycoon.Components;

namespace Tycoon.Assets;

public class Bakery : IBlueprint, IProductionSite
{
	public string Name => "Bakery";
	public int Cost => 1;
	public Texture2D Texture { get; } = GD.Load<Texture2D>("res://Assets/house.png");

	public Shape2D Shape { get; } = new RectangleShape2D
	{
		Size = new Vector2(512, 512),
	};

	public Producer Producer { get; } = new(Goods.Bread, 2, 0.1, Goods.Wheat, 5);
	public int MaximumWorkers => 5;
	public InventoryCapacity InventoryCapacity => 100;
}
