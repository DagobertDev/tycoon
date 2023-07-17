using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

public sealed partial class ProductionCompletedSystem : AEntitySetSystem<double>
{
	[Update, UseBuffer]
	private static void Update(in Entity entity, in Producer producer, in Inventory inventory, ref ProductionProgress progress)
	{
		var resetProgress = progress >= 1;

		while (progress >= 1 && HasEnoughInput(producer, inventory))
		{
			progress -= 1;

			var currentValue = inventory.Value.GetValueOrDefault(producer.Output);
			inventory.Value[producer.Output] = currentValue + producer.OutputAmount;
			RemoveUsedInput(producer, inventory);
			entity.NotifyChanged<Inventory>();
		}

		if (resetProgress)
		{
			entity.Set<ProductionProgress>(0);
		}
	}

	private static bool HasEnoughInput(Producer producer, Inventory inventory)
	{
		return producer.Input == null || inventory.Value.GetValueOrDefault(producer.Input) >= producer.InputAmount;
	}

	private static void RemoveUsedInput(Producer producer, Inventory inventory)
	{
		if (producer.Input == null)
		{
			return;
		}

		inventory.Value[producer.Input] -= producer.InputAmount;
	}
}
