using System;
using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

public sealed partial class FindWorkplaceSystem : AEntitySetSystem<double>
{
	private readonly EntitySet _workplaces;
	private readonly EntityMultiMap<Worker> _workers;

	public FindWorkplaceSystem(World world) : base(world, CreateEntityContainer, null, 0)
	{
		_workplaces = world.GetEntities().With<HasFreeWorkplace>().AsSet();
		_workers = world.GetEntities().AsMultiMap<Worker>();
	}

	[WithPredicate]
	private static bool HasNoWorkplace(in Worker worker)
	{
		return worker == Worker.Unemployed;
	}

	[Update, UseBuffer]
	private void Update(in Entity worker)
	{
		if (_workplaces.Count == 0)
		{
			return;
		}

		var workplace = SelectBestWorkplace(_workplaces.GetEntities(), worker);
		var workerComponent = new Worker(workplace);
		worker.Set(workerComponent);
		workplace.RemoveFlag(CanNotWorkReason.NoEmployee);

		if (_workers[workerComponent].Length == workplace.Get<MaximumWorkers>())
		{
			workplace.Remove<HasFreeWorkplace>();
		}
	}

	private static Entity SelectBestWorkplace(ReadOnlySpan<Entity> workplaces, Entity worker)
	{
		return workplaces[0];
	}
}
