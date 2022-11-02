using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

public sealed partial class InventoryFullSystem : AEntitySetSystem<double>
{
	[Update]
	[UseBuffer]
	private static void Update(in Entity entity,
		[Added] [Changed] in Producer producer,
		[Added] [Changed] in RemainingInventorySpace remainingSpace)
	{
		if (remainingSpace - producer.OutputAmount < 0)
		{
			var canNotWorkReason = CanNotWorkReason.None;

			if (entity.Has<CanNotWorkReason>())
			{
				canNotWorkReason = entity.Get<CanNotWorkReason>();
			}

			canNotWorkReason |= CanNotWorkReason.InventoryFull;
			entity.Set(canNotWorkReason);
		}
	}
}
