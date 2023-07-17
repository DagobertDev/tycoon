using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

public sealed partial class SupplyDemandSetupSystem : AEntitySetSystem<double>
{
	[Update, UseBuffer]
	private static void Update(in Entity entity, [Added, Changed] in Producer producer)
	{
		entity.Set(new Supply(producer.Good));

		if (producer.Input != null)
		{
			entity.Set(new Demand(producer.Input));
		}
	}
}
