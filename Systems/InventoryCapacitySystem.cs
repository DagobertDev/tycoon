using System.Diagnostics;
using System.Linq;
using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

public sealed partial class InventoryCapacitySystem : AEntitySetSystem<double>
{
	[Update]
	[UseBuffer]
	private static void Update(in Entity entity,
		[Added] [Changed] in Inventory inventory,
		[Added] [Changed] in InventoryCapacity capacity)
	{
		var usedSpace = inventory.Value.Values.Sum();
		var remainingSpace = capacity - usedSpace;
		entity.Set<RemainingInventorySpace>(remainingSpace);
		Debug.Assert(remainingSpace >= 0);
	}
}
