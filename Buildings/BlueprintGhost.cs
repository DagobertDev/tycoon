using System;
using Godot;

namespace Tycoon.Buildings;

public partial class BlueprintGhost : Sprite2D
{
	private static readonly Color CanBuildColor = Colors.White.Lerp(Colors.Transparent, 0.5f);
	private static readonly Color CanNotBuildColor = Colors.Red.Lerp(Colors.Transparent, 0.2f);
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

		SelfModulate = canPlace ? CanBuildColor : CanNotBuildColor;
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
			GetViewport().SetInputAsHandled();
		}
		else if (@event.IsAction(InputActions.MouseclickRight))
		{
			Texture = null;
			SetEnabled(false);
			GetViewport().SetInputAsHandled();
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
