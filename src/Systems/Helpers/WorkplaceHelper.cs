using System;
using System.Collections.Concurrent;
using DefaultEcs;
using Tycoon.Components;

namespace Tycoon.Systems.Helpers;

public static class WorkplaceHelper
{
	private static readonly ConcurrentDictionary<World, EntityMultiMap<Worker>> _worldWorkers = new();

	/// <summary>
	/// Returns the workers that work at the <see cref="workplace"/>
	/// </summary>
	public static ReadOnlySpan<Entity> GetWorkers(Entity workplace)
	{
		var workers = GetWorkersInternal(workplace);
		return workers;
	}

	/// <summary>
	/// Returns the number of workers that work at the <see cref="workplace"/>
	/// </summary>
	public static int GetWorkerCount(Entity workplace)
	{
		var workers = GetWorkersInternal(workplace);
		return workers.Length;
	}

	public static bool HasFreeWorkplace(Entity workplace)
	{
		return GetWorkerCount(workplace) < workplace.Get<MaximumWorkers>();
	}

	private static ReadOnlySpan<Entity> GetWorkersInternal(Entity workplace)
	{
		var world = workplace.World;

		if (!_worldWorkers.TryGetValue(world, out var workplaceWorkers))
		{
			workplaceWorkers = world.GetEntities().AsMultiMap<Worker>();
			_worldWorkers[world] = workplaceWorkers;
		}

		if (workplaceWorkers.TryGetEntities(new Worker(workplace), out var workers))
		{
			return workers;
		}

		return ReadOnlySpan<Entity>.Empty;
	}
}
