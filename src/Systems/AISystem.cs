using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

public sealed partial class AISystem : AEntitySetSystem<double>
{
	[WithPredicate]
	private static bool Idling(in AgentState state)
	{
		return state == AgentState.Idling;
	}

	[Update, UseBuffer]
	private static void Update(in Entity entity, in Worker worker)
	{
		if (worker == Worker.Unemployed)
		{
			entity.Set(AgentState.WalkingAround);
		}
		else
		{
			entity.Set(AgentState.GoingToWork);
			var destination = worker.Workplace.Get<Position>();
			entity.Set<Destination>(destination.Value);
		}
	}
}
