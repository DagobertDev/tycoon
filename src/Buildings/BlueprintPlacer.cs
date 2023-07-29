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
	private readonly MapSettings _mapSettings;

	public BlueprintPlacer(IGoldCounter goldCounter, Map map, World world, MapSettings mapSettings)
	{
		_goldCounter = goldCounter;
		_map = map;
		_world = world;
		_mapSettings = mapSettings;
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
			Name = blueprint.Name,
			ZIndex = blueprint is IWorker ? 1 : 0,
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
		entity.Set<Position>(transform.Origin);

		if (blueprint is IHasInventory inventory)
		{
			entity.Set(new Inventory(new Dictionary<Good, int>()));
			entity.Set(inventory.InventoryCapacity);
		}

		if (blueprint is IProductionSite productionSite)
		{
			entity.Set(productionSite.Producer);
			entity.Set<ProductionProgress>();

			if (productionSite.MaximumWorkers > 0)
				entity.Set(CanNotWorkReason.NoEmployee);
			else
				entity.Set<NoWorkersRequired>();

			entity.Set<MaximumWorkers>(productionSite.MaximumWorkers);
		}

		if (blueprint is IWorker worker)
		{
			entity.Set(Worker.Unemployed);
			entity.Set(worker.Speed);
			entity.Set(AgentState.Idling);
		}

		_map.AddChild(godotEntity);
	}

	public bool CanPlace(IBlueprint blueprint, Transform2D transform)
	{
		if (blueprint.Cost > _goldCounter.Gold)
			return false;

		if (!new Rect2(Vector2I.Zero, _mapSettings.Size).HasPoint(transform.Origin))
			return false;

		if (_shapeCast.Shape != blueprint.Shape)
			_shapeCast.Shape = blueprint.Shape;

		_shapeCast.GlobalTransform = transform;
		_shapeCast.ForceShapecastUpdate();
		return !_shapeCast.IsColliding();
	}
}
