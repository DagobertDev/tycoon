using Godot;

namespace Tycoon;

public partial class Map : Polygon2D
{
	public Map(MapSettings mapSettings)
	{
		var size = mapSettings.Size;
		
		YSortEnabled = true;
		Polygon = new []
		{
			new Vector2(0, 0),
			new Vector2(size.X, 0),
			new Vector2(size.X, size.Y),
			new Vector2(0, size.Y),
		};
		SelfModulate = new Color(0, 0.623529f, 0); // Grass green
	}
}
