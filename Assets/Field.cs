using Godot;
using Tycoon.Buildings;
using Tycoon.Components;

namespace Tycoon.Assets;

public class Field : IBlueprint, IProductionSite
{
	public string Name => "Field";
	public int Cost => 1;
	public Texture2D Texture { get; } = GD.Load<Texture2D>("res://Assets/field.png");

	public Shape2D Shape { get; } = new RectangleShape2D
	{
		Size = new Vector2(256, 512),
	};

	public Producer Producer { get; } = new(Goods.Wheat, 1, 2);
	public InventoryCapacity InventoryCapacity => 100;
}
