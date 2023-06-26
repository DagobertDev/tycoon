using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

[Without(typeof(CanNotWorkReason))]
public sealed partial class ProductionSystem : AEntitySetSystem<double>
{
	private readonly EntityMultiMap<Worker> _workers;

	public ProductionSystem(World world) : base(world, CreateEntityContainer, null, 0)
	{
		_workers = world.GetEntities().AsMultiMap<Worker>();
	}

	[Update]
	[UseBuffer]
    private void Update(double delta, in Entity entity, in Producer producer, in Inventory inventory,
		ref ProductionProgress progress)
	{
		if (entity.Has<NoWorkersRequired>())
		{
			progress += producer.ProgressPerSecond * delta;
		}
		else
		{
			foreach (var worker in _workers[new Worker(entity)])
			{
				progress += producer.ProgressPerSecond * delta;
			}
		}

		while (progress >= 1 && HasEnoughInput(producer, inventory))
		{
			progress -= 1;

			var currentValue = inventory.Value.GetValueOrDefault(producer.Good);
			inventory.Value[producer.Good] = currentValue + producer.OutputAmount;
			RemoveUsedInput(producer, inventory);
			entity.NotifyChanged<Inventory>();
		}

		entity.NotifyChanged<ProductionProgress>();
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
