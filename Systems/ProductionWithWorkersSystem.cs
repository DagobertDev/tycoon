using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

[Without(typeof(NoWorkersRequired))]
[Without(typeof(CanNotWorkReason))]
public sealed partial class ProductionWithWorkersSystem : AEntitySetSystem<double>
{
	private readonly EntityMultiMap<Worker> _workers;

	public ProductionWithWorkersSystem(World world) : base(world, CreateEntityContainer, null, 0)
	{
		_workers = world.GetEntities().AsMultiMap<Worker>();
	}

	[Update]
	private void Update(double delta, in Entity entity, in Producer producer, in Inventory inventory,
	                    ref ProductionProgress progress)
	{
		foreach (var worker in _workers[new Worker(entity)])
		{
			progress += producer.ProgressPerSecond * delta;
		}

		entity.NotifyChanged<ProductionProgress>();
	}
}
