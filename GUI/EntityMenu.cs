using Godot;

namespace Tycoon.GUI;

public partial class EntityMenu : ShapeCast2D
{
	private readonly PopupMenu _popup = new();

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
			var entity = (Node)collision;

			_popup.Clear();
			_popup.AddItem($"Name: {entity.Name}");
			_popup.Show();
		}

		SetPhysicsProcess(false);
		Enabled = false;
	}
}
