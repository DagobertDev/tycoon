using DefaultEcs;
using Godot;
using Tycoon.Components;
using Tycoon.Systems;

namespace Tycoon.GUI;

public partial class EntityMenu : PanelContainer
{
	private readonly EntityMultiMap<Worker> _workers;
	private readonly INodeEntityMapper _nodeEntityMapper;
	private readonly VBoxContainer _container = new();
	private readonly Map _map;
	private ShapeCast2D _shapeCast;
	private Entity? _entity;

	public EntityMenu(INodeEntityMapper nodeEntityMapper, World world, Map map)
	{
		_nodeEntityMapper = nodeEntityMapper;
		_map = map;
		_workers = world.GetEntities().AsMultiMap<Worker>();
		_shapeCast = new ShapeCast2D
		{
			CollideWithAreas = true,
			Shape = new CircleShape2D { Radius = 1 },
			Enabled = false,
		};
	}

	public override void _Ready()
	{
		SetPhysicsProcess(false);
		SetProcess(false);
		_map.AddChild(_shapeCast);

		var margin = new MarginContainer();
		margin.AddThemeConstantOverride("margin_top", 10);
		margin.AddThemeConstantOverride("margin_left", 10);
		margin.AddThemeConstantOverride("margin_right", 10);
		margin.AddThemeConstantOverride("margin_bottom", 10);
		margin.AddChild(_container);
		AddChild(margin);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed(InputActions.MouseclickLeft))
		{
			SetPhysicsProcess(true);
			_shapeCast.GlobalPosition = _shapeCast.GetGlobalMousePosition();
			_shapeCast.Enabled = true;
			GetViewport().SetInputAsHandled();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_shapeCast.IsColliding())
		{
			var collision = _shapeCast.GetCollider(0);
			var entityNode = (Node2D)collision;
			_entity = _nodeEntityMapper.GetEntity(entityNode);
			Visible = true;
			SetProcess(true);
		}

		SetPhysicsProcess(false);
		_shapeCast.Enabled = false;
	}

	public override void _Process(double delta)
	{
		foreach (var child in _container.GetChildren())
		{
			child.QueueFree();
		}

		if (_entity is not { IsAlive: true })
		{
			return;
		}

		var entity = _entity.Value;
		var node = entity.Get<Node2D>();

		AddItem($"Name: {node.Name}");

		if (entity.Has<Inventory>())
		{
			AddSeparator();

			var inventoryCapacity = entity.Get<InventoryCapacity>().Value;
			var usedInventoryCapacity = inventoryCapacity - entity.Get<RemainingInventorySpace>();

			AddItem(
				$"Inventory ({usedInventoryCapacity}/{inventoryCapacity})");

			foreach (var goodAndAmount in entity.Get<Inventory>().Value)
			{
				AddItem($"{goodAndAmount.Key}: {goodAndAmount.Value}");
			}
		}

		if (entity.Has<CanNotWorkReason>())
		{
			AddSeparator();
			AddItem($"Can't work because: {entity.Get<CanNotWorkReason>()}");
		}

		if (entity.Has<MaximumWorkers>())
		{
			AddSeparator();

			var employeeCount = 0;

			if (_workers.TryGetEntities(new Worker(entity), out var employees))
			{
				employeeCount = employees.Length;
			}

			AddItem($"Workers: {employeeCount}/{entity.Get<MaximumWorkers>().Value}");
		}
	}

	private void AddItem(string text)
	{
		_container.AddChild(new Label
		{
			Text = text,
		});
	}

	private void AddSeparator()
	{
		_container.AddChild(new HSeparator());
	}
}
