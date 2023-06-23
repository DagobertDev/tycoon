using DefaultEcs.System;
using Godot;
using Tycoon.Components;

namespace Tycoon.Systems;

/// <summary>
/// Updates the <see cref="Node2D"/> components position to be equal to the entities <see cref="Position"/>
/// </summary>
public sealed partial class Node2DPositionSystem : AEntitySetSystem<double>
{
	[Update]
	private static void Update(in Node2D node2D, [Added, Changed] in Position position)
	{
		node2D.GlobalPosition = position;
	}
}
