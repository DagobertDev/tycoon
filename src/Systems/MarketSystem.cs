using System;
using System.Collections.Generic;
using System.Diagnostics;
using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

public sealed partial class MarketSystem : AEntitySetSystem<double>
{
	private readonly EntityMultiMap<Producer> _supply;

	public MarketSystem(World world) : base(world, CreateEntityContainer, null, 0)
	{
		_supply = world.GetEntities().AsMultiMap(new SupplyComparer());
	}

	[WithPredicate]
	private static bool HasDemand(in Producer producer)
	{
		return producer.Input != null;
	}

	[Update, UseBuffer]
	private void Update(in Entity demand, in Producer producer, in Inventory demandInventory, in RemainingInventorySpace inventorySpace)
	{
		if (inventorySpace == 0)
		{
			return;
		}

		var good = producer.Input;

		Debug.Assert(good != null);

		var dummyProducer = new Producer(good, default, default);

		if (!_supply.TryGetEntities(dummyProducer, out var suppliers))
		{
			return;
		}

		var supply = ChooseBestSupply(suppliers, good);

		if (supply is null)
		{
			return;
		}

		var supplyInventory = supply.Value.Get<Inventory>();
		var supplyAmount = supplyInventory.Value[good];
		var exchangedAmount = Math.Min(supplyAmount, inventorySpace);

		if (demandInventory.Value.ContainsKey(good))
		{
			demandInventory.Value[good] += exchangedAmount;
		}
		else
		{
			demandInventory.Value[good] = exchangedAmount;
		}

		supplyInventory.Value[good] = supplyAmount - exchangedAmount;

		demand.NotifyChanged<Inventory>();
		supply.Value.NotifyChanged<Inventory>();
	}

	private static Entity? ChooseBestSupply(ReadOnlySpan<Entity> suppliers, Good good)
	{
		foreach (var supplier in suppliers)
		{
			var inventory = supplier.Get<Inventory>();
			var amount = inventory.Value.GetValueOrDefault(good);
			if (amount > 0)
			{
				return supplier;
			}
		}

		return null;
	}

	private sealed class SupplyComparer : IEqualityComparer<Producer>
	{
		public bool Equals(Producer? x, Producer? y)
		{
			if (x == y)
			{
				return true;
			}

			if (x is null || y is null)
			{
				return false;
			}

			return x.Good == y.Good;
		}

		public int GetHashCode(Producer obj)
		{
			return obj.Good.GetHashCode();
		}
	}
}
