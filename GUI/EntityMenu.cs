using DefaultEcs;
using Godot;
using Tycoon.Components;
using Tycoon.Systems;

namespace Tycoon.GUI;

public partial class EntityMenu : ShapeCast2D
{
	private readonly INodeEntityMapper _nodeEntityMapper;
	private readonly PopupMenu _popup = new();
	private Entity? _entity;

	public EntityMenu(INodeEntityMapper nodeEntityMapper)
	{
		_nodeEntityMapper = nodeEntityMapper;
	}

	public override void _Ready()
	{
		SetPhysicsProcess(false);
		SetProcess(false);
		CollideWithAreas = true;
		Shape = new CircleShape2D { Radius = 1 };
		Enabled = false;
		AddChild(_popup);
		_popup.VisibilityChanged += () => SetProcess(_entity != null);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed(InputActions.MouseclickLeft))
		{
			SetPhysicsProcess(true);
			GlobalPosition = GetGlobalMousePosition();
			Enabled = true;
			GetViewport().SetInputAsHandled();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsColliding())
		{
			var collision = GetCollider(0);
			var entityNode = (Node2D)collision;
			_entity = _nodeEntityMapper.GetEntity(entityNode);
			_popup.Show();
			SetProcess(true);
		}

		SetPhysicsProcess(false);
		Enabled = false;
	}

	public override void _Process(double delta)
	{
		_popup.Clear();

		if (_entity is not { IsAlive: true })
		{
			return;
		}

		var entity = _entity.Value;
		var node = entity.Get<Node2D>();

		_popup.AddItem($"Name: {node.Name}");

		if (entity.Has<Inventory>())
		{
			_popup.AddSeparator();

			var inventoryCapacity = entity.Get<InventoryCapacity>().Value;
			var usedInventoryCapacity = inventoryCapacity - entity.Get<RemainingInventorySpace>();

			_popup.AddItem(
				$"Inventory ({usedInventoryCapacity}/{inventoryCapacity})");

			foreach (var goodAndAmount in entity.Get<Inventory>().Value)
			{
				_popup.AddItem($"{goodAndAmount.Key}: {goodAndAmount.Value}");
			}
		}

		if (entity.Has<CanNotWorkReason>())
		{
			_popup.AddSeparator();
			_popup.AddItem($"Can't work because: {entity.Get<CanNotWorkReason>()}");
		}
	}
}
