using Godot;
using Tycoon.Components;
using Tycoon.Systems;

namespace Tycoon.GUI;

public partial class EntityMenu : ShapeCast2D
{
	private readonly INodeEntityMapper _nodeEntityMapper;
	private readonly PopupMenu _popup = new();

	public EntityMenu(INodeEntityMapper nodeEntityMapper)
	{
		_nodeEntityMapper = nodeEntityMapper;
	}

	public override void _Ready()
	{
		SetPhysicsProcess(false);
		CollideWithAreas = true;
		Shape = new CircleShape2D { Radius = 1 };
		Enabled = false;
		AddChild(_popup);
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

			_popup.Clear();
			_popup.AddItem($"Name: {entityNode.Name}");

			var entity = _nodeEntityMapper.GetEntity(entityNode);
			if (entity.Has<Inventory>() && entity.Get<Inventory>().Value.Count > 0)
			{
				_popup.AddSeparator();

				foreach (var goodAndAmount in entity.Get<Inventory>().Value)
				{
					_popup.AddItem($"{goodAndAmount.Key}: {goodAndAmount.Value}");
				}
			}

			_popup.Show();
		}

		SetPhysicsProcess(false);
		Enabled = false;
	}
}
