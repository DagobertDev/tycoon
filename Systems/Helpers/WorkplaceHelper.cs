using System;
using System.Collections.Concurrent;
using DefaultEcs;
using Tycoon.Components;

namespace Tycoon.Systems.Helpers;

public static class WorkplaceHelper
{
	private static readonly ConcurrentDictionary<World, EntityMultiMap<Worker>> _worldWorkers = new();

	/// <summary>
	/// Returns the number of workers that work at the <see cref="workplace"/>
	/// </summary>
	public static int GetWorkerCount(Entity workplace)
	{
		var workers = GetWorkersInternal(workplace);
		return workers.Length;
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
