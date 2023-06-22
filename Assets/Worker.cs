using Godot;
using Tycoon.Buildings;
using Tycoon.Components;

namespace Tycoon.Assets;

public class Worker : IBlueprint, IWorker
{
	public string Name => "Worker";
	public int Cost => 0;
	public Texture2D Texture { get; } = GD.Load<Texture2D>("res://Assets/human.png");
	public Shape2D Shape { get; } = new CircleShape2D
	{
		Radius = 32,
	};

	public Speed Speed => 200;
}
