using System.Collections.Generic;
using Godot;
using Tycoon.Buildings;

namespace Tycoon.GUI;

public partial class BuildControl : HBoxContainer
{
	private static readonly Vector2I ButtonSize = new(100, 100);
	private readonly BlueprintGhost _blueprintGhost;

	public BuildControl(IEnumerable<IBlueprint> buildings, BlueprintGhost blueprintGhost)
	{
		_blueprintGhost = blueprintGhost;

		foreach (var building in buildings)
		{
			var button = new Button
			{
				Text = building.Name,
				CustomMinimumSize = ButtonSize,
			};

			button.Pressed += () => ButtonOnPressed(building);
			AddChild(button);
		}
	}

	private void ButtonOnPressed(IBlueprint blueprint)
	{
		_blueprintGhost.SetBuilding(blueprint);
	}
}
