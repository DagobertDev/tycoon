using System;
using Godot;

namespace Tycoon.Buildings;

public class BlueprintPlacer : IBlueprintPlacer
{
	private readonly IGoldCounter _goldCounter;
	private readonly Map _map;
	private readonly ShapeCast2D _shapeCast;

	public BlueprintPlacer(IGoldCounter goldCounter, Map map)
	{
		_goldCounter = goldCounter;
		_map = map;
		_shapeCast = new ShapeCast2D
		{
			MaxResults = 1,
			TargetPosition = new Vector2(0, 0),
			CollideWithAreas = true,
			// Dummy shape
			Shape = new CircleShape2D
			{
				Radius = 0,
			},
		};

		_map.AddChild(_shapeCast);
	}

	public void Place(IBlueprint blueprint, Transform2D transform)
	{
		if (!CanPlace(blueprint, transform))
		{
			throw new InvalidOperationException("Can't place blueprint");
		}

		_goldCounter.Gold -= blueprint.Cost;

		var entity = new Area2D
		{
			GlobalTransform = transform,
			Name = blueprint.Name,
		};
		entity.AddChild(new CollisionShape2D
		{
			Shape = blueprint.Shape,
		});
		entity.AddChild(new Sprite2D
		{
			Texture = blueprint.Texture,
		});

		_map.AddChild(entity);
	}

	public bool CanPlace(IBlueprint blueprint, Transform2D transform)
	{
		if (blueprint.Cost > _goldCounter.Gold)
		{
			return false;
		}

		if (_shapeCast.Shape != blueprint.Shape)
		{
			_shapeCast.Shape = blueprint.Shape;
		}

		_shapeCast.GlobalTransform = transform;
		_shapeCast.ForceShapecastUpdate();
		return !_shapeCast.IsColliding();
	}
}
