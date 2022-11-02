using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

[Without(typeof(CanNotWorkReason))]
public sealed partial class ProductionSystem : AEntitySetSystem<double>
{
	[Update]
	[UseBuffer]
	private static void Update(double delta, in Entity entity, in Producer producer, in Inventory inventory,
		ref ProductionProgress progress)
	{
		progress += producer.ProgressPerSecond * delta;

		while (progress >= 1)
		{
			progress -= 1;

			var currentValue = inventory.Value.GetValueOrDefault(producer.Good);
			inventory.Value[producer.Good] = currentValue + producer.OutputAmount;
			entity.NotifyChanged<Inventory>();
		}

		entity.NotifyChanged<ProductionProgress>();
	}
}
