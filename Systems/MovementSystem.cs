using DefaultEcs;
using DefaultEcs.System;
using Godot;
using Tycoon.Components;

namespace Tycoon.Systems;

public sealed partial class MovementSystem : AEntitySetSystem<double>
{
	[Update, UseBuffer]
	private static void Update(double delta, in Entity entity, in Node2D node, in Speed speed, in Destination destination)
	{
		node.GlobalPosition = node.GlobalPosition.MoveToward(destination, (float)(speed * delta));

		if (node.GlobalPosition.DistanceSquaredTo(destination) < 10)
		{
			entity.Remove<Destination>();
		}
	}
}
