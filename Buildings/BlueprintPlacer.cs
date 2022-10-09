using System;
using Godot;

namespace Tycoon.Buildings;

public class BlueprintPlacer : IBlueprintPlacer
{
	private readonly IGoldCounter _goldCounter;
	private readonly Map _map;

	public BlueprintPlacer(IGoldCounter goldCounter, Map map)
	{
		_goldCounter = goldCounter;
		_map = map;
	}

	public void Place(IBlueprint blueprint, Transform2D transform)
	{
		if (!CanPlace(blueprint, transform))
		{
			throw new InvalidOperationException("Can't place blueprint");
		}

		_goldCounter.Gold -= blueprint.Cost;

		var sprite = new Sprite2D
		{
			Texture = blueprint.Texture,
			GlobalTransform = transform,
		};

		_map.AddChild(sprite);
	}

	public bool CanPlace(IBlueprint blueprint, Transform2D transform)
	{
		return blueprint.Cost <= _goldCounter.Gold;
	}
}
