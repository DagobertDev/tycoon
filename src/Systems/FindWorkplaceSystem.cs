using System;
using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;
using Tycoon.Systems.Helpers;

namespace Tycoon.Systems;

public sealed partial class FindWorkplaceSystem : AEntitySetSystem<double>
{
	private readonly EntitySet _workplaces;

	public FindWorkplaceSystem(World world) : base(world, CreateEntityContainer, null, 0)
	{
		_workplaces = world.GetEntities().With<HasFreeWorkplace>().AsSet();
		world.SubscribeComponentRemoved<MaximumWorkers>(HandleWorkplaceDeletion);
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

		if (WorkplaceHelper.GetWorkerCount(workplace) == workplace.Get<MaximumWorkers>())
		{
			workplace.Remove<HasFreeWorkplace>();
		}
	}

	private void HandleWorkplaceDeletion(in Entity workplace, in MaximumWorkers _)
	{
		var workers = WorkplaceHelper.GetWorkers(workplace);

		foreach (var worker in workers)
		{
			worker.Set(Worker.Unemployed);
		}
	}

	private static Entity SelectBestWorkplace(ReadOnlySpan<Entity> workplaces, Entity worker)
	{
		return workplaces[0];
	}
}