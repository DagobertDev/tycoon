using Godot;

namespace Tycoon.Buildings;

public interface IBlueprintPlacer
{
	void Place(IBlueprint blueprint, Transform2D transform);
	bool CanPlace(IBlueprint blueprint, Transform2D transform);
}
