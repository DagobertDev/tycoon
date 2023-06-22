using System;
using DefaultEcs;
using Godot;

namespace Tycoon.GUI;

public partial class Camera : Camera2D
{
	private int Speed => 500;
	private float MinimumZoom => 1 / 18f;
	private float MaximumZoom => 1;
	private float ZoomStep => 0.1f;
	private float InitialZoom => 0.5f;

	public override void _Ready()
	{
		if (Speed <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(Speed));
		}

		if (MinimumZoom <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(MinimumZoom));
		}

		if (MaximumZoom < MinimumZoom)
		{
			throw new ArgumentException(nameof(MaximumZoom));
		}

		if (ZoomStep is <= 0 or >= 1)
		{
			throw new ArgumentOutOfRangeException(nameof(ZoomStep));
		}

		Enabled = true;
		Zoom = Vector2.One * InitialZoom;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed(InputActions.ZoomIn))
		{
			Zoom /= 1 - ZoomStep;
			Zoom = Zoom.Clamp(Vector2.One * MinimumZoom, Vector2.One * MaximumZoom);
		}
		else if (@event.IsActionPressed(InputActions.ZoomOut))
		{
			Zoom *= 1 - ZoomStep;
			Zoom = Zoom.Clamp(Vector2.One * MinimumZoom, Vector2.One * MaximumZoom);
		}
		else if (@event is InputEventMouseMotion motion && Input.IsActionPressed(InputActions.MouseclickRight))
		{
			GlobalPosition -= motion.Relative / Zoom;
		}
	}

	public override void _Process(double delta)
	{
		var direction = Input.GetVector(
			InputActions.CameraLeft,
			InputActions.CameraRight,
			InputActions.CameraUp,
			InputActions.CameraDown);

		Position += Speed * (float)delta * direction;
	}

	public void Focus(Entity entity)
	{
		GlobalPosition = entity.Get<Node2D>().GlobalPosition;
	}
}
