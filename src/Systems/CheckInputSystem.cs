using System.Collections.Generic;
using System.Diagnostics;
using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

public sealed partial class CheckInputSystem : AEntitySetSystem<double>
{
	[WithPredicate]
	private static bool RequiresInput(in Producer producer)
	{
		return producer.Input != null;
	}

	[Update]
	private static void Update(in Entity entity, [Added, Changed] in Producer producer, [Added, Changed] in Inventory inventory)
	{
		Debug.Assert(producer.Input != null);

		var currentAmount = inventory.Value.GetValueOrDefault(producer.Input);

		if (currentAmount >= producer.InputAmount)
		{
			entity.RemoveFlag(CanNotWorkReason.NoInput);
		}
		else
		{
			entity.AddFlag(CanNotWorkReason.NoInput);
		}
	}
}
