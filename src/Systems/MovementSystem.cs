using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

public sealed partial class MovementSystem : AEntitySetSystem<double>
{
	[Update, UseBuffer]
	private static void Update(double delta, in Entity entity, ref Position position, in Speed speed, in Destination destination)
	{
		position = position.Value.MoveToward(destination, (float)(speed * delta));
		entity.NotifyChanged<Position>();

		if (position.Value.DistanceSquaredTo(destination) < 10)
		{
			entity.Remove<Destination>();
			entity.Set(AgentState.Idling);
		}
	}
}
