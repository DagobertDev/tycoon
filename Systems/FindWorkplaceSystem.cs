using System;
using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

public sealed partial class FindWorkplaceSystem : AEntitySetSystem<double>
{
	private readonly EntitySet _workplaces;

	public FindWorkplaceSystem(World world) : base(world, CreateEntityContainer, null, 0)
	{
		_workplaces = world.GetEntities().With((in CanNotWorkReason value) => value.HasFlag(CanNotWorkReason.NoEmployee)).AsSet();
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
		worker.Set(new Worker(workplace));
		workplace.RemoveFlag(CanNotWorkReason.NoEmployee);
	}

	private static Entity SelectBestWorkplace(ReadOnlySpan<Entity> workplaces, Entity worker)
	{
		return workplaces[0];
	}
}
