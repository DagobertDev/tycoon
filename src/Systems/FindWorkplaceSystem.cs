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
		_workplaces = world.GetEntities().With<MaximumWorkers>().AsSet();
		world.SubscribeComponentRemoved<MaximumWorkers>(HandleWorkplaceDeletion);
	}

	[WithPredicate]
	private static bool HasNoWorkplace(in Worker worker) => worker == Worker.Unemployed;

	[Update, UseBuffer]
	private void Update(in Entity worker)
	{
		var nullableWorkplace = SelectBestWorkplace(_workplaces.GetEntities(), worker);

		if (nullableWorkplace == null)
		{
			return;
		}

		var workplace = nullableWorkplace.Value;
		var workerComponent = new Worker(workplace);
		worker.Set(workerComponent);
		workplace.RemoveFlag(CanNotWorkReason.NoEmployee);
	}

	private void HandleWorkplaceDeletion(in Entity workplace, in MaximumWorkers _)
	{
		var workers = WorkplaceHelper.GetWorkers(workplace);

		foreach (var worker in workers)
		{
			worker.Set(Worker.Unemployed);
		}
	}

	private static Entity? SelectBestWorkplace(ReadOnlySpan<Entity> workplaces, Entity worker)
	{
		foreach (var workplace in workplaces)
		{
			if (WorkplaceHelper.HasFreeWorkplace(workplace))
			{
				return workplace;
			}
		}

		return null;
	}
}
