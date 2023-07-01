using System;
using DefaultEcs;
using Godot;

namespace Tycoon.GUI;

public partial class Camera : Camera2D
{
	private Vector2I _movementDirection;
	private int Speed => 500;
	private float MinimumZoom => 1 / 18f;
	private float MaximumZoom => 1;
	private float ZoomStep => 0.1f;
	private float InitialZoom => 0.5f;

	private Entity _selectedEntity;

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
			_selectedEntity = default;
		}
		else if (@event.IsActionPressed(InputActions.CameraLeft))
		{
			_movementDirection.X = -1;
		}
		else if (@event.IsActionReleased(InputActions.CameraLeft) && _movementDirection.X == -1)
		{
			_movementDirection.X = 0;
		}
		else if (@event.IsActionPressed(InputActions.CameraRight))
		{
			_movementDirection.X = 1;
		}
		else if (@event.IsActionReleased(InputActions.CameraRight) && _movementDirection.X == 1)
		{
			_movementDirection.X = 0;
		}
		else if (@event.IsActionPressed(InputActions.CameraUp))
		{
			_movementDirection.Y = -1;
		}
		else if (@event.IsActionReleased(InputActions.CameraUp) && _movementDirection.Y == -1)
		{
			_movementDirection.Y = 0;
		}
		else if (@event.IsActionPressed(InputActions.CameraDown))
		{
			_movementDirection.Y = 1;
		}
		else if (@event.IsActionReleased(InputActions.CameraDown) && _movementDirection.Y == 1)
		{
			_movementDirection.Y = 0;
		}
	}

	public override void _Process(double delta)
	{
		Position += Speed * (float)delta * new Vector2(_movementDirection.X, _movementDirection.Y);

		if (_selectedEntity.IsAlive && _movementDirection == Vector2.Zero)
		{
			Focus(_selectedEntity);
		}
		else
		{
			_selectedEntity = default;
		}
	}

	public void Focus(Entity entity)
	{
		GlobalPosition = entity.Get<Node2D>().GlobalPosition;

		if (_selectedEntity != entity)
		{
			_selectedEntity = default;
		}
	}

	public void Follow(Entity entity)
	{
		_selectedEntity = entity;
	}
}
