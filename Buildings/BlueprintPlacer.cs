using System;
using System.Collections.Generic;
using DefaultEcs;
using Godot;
using Tycoon.Components;

namespace Tycoon.Buildings;

public class BlueprintPlacer : IBlueprintPlacer
{
	private readonly IGoldCounter _goldCounter;
	private readonly Map _map;
	private readonly ShapeCast2D _shapeCast;
	private readonly World _world;

	public BlueprintPlacer(IGoldCounter goldCounter, Map map, World world)
	{
		_goldCounter = goldCounter;
		_map = map;
		_world = world;
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

		var godotEntity = new Area2D
		{
			GlobalTransform = transform,
			Name = blueprint.Name,
		};
		godotEntity.AddChild(new CollisionShape2D
		{
			Shape = blueprint.Shape,
		});
		godotEntity.AddChild(new Sprite2D
		{
			Texture = blueprint.Texture,
		});

		var entity = _world.CreateEntity();
		entity.Set<Node2D>(godotEntity);

		if (blueprint is IHasInventory inventory)
		{
			entity.Set(new Inventory(new Dictionary<Good, int>()));
			entity.Set(inventory.InventoryCapacity);
		}

		if (blueprint is IProductionSite productionSite)
		{
			entity.Set(productionSite.Producer);
			entity.Set<ProductionProgress>();
		}

		_map.AddChild(godotEntity);
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
