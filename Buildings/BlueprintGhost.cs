using System;
using Godot;

namespace Tycoon.Buildings;

public partial class BlueprintGhost : Sprite2D
{
	private readonly IBlueprintPlacer _blueprintPlacer;
	private IBlueprint? _blueprint;

	public BlueprintGhost(IBlueprintPlacer blueprintPlacer)
	{
		_blueprintPlacer = blueprintPlacer;
	}

	public override void _Ready()
	{
		SetEnabled(false);
	}

	public override void _Process(double delta)
	{
		GlobalPosition = GetGlobalMousePosition();

		if (_blueprint is null)
		{
			throw new InvalidOperationException("Blueprint is null");
		}

		var canPlace = _blueprintPlacer.CanPlace(_blueprint, GlobalTransform);

		SelfModulate = canPlace ? Colors.White : Colors.Red;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsAction(InputActions.MouseclickLeft))
		{
			if (_blueprint is null)
			{
				throw new InvalidOperationException("Blueprint was not set");
			}

			if (!_blueprintPlacer.CanPlace(_blueprint, GlobalTransform))
			{
				return;
			}

			_blueprintPlacer.Place(_blueprint, GlobalTransform);
		}
		else if (@event.IsAction(InputActions.MouseclickRight))
		{
			Texture = null;
			SetEnabled(false);
		}
	}

	public void SetBuilding(IBlueprint blueprint)
	{
		_blueprint = blueprint;
		Texture = blueprint.Texture;
		SetEnabled(true);
	}

	private void SetEnabled(bool value)
	{
		SetProcess(value);
		SetProcessUnhandledInput(value);
	}
}
