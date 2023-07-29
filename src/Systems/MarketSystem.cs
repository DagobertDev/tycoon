using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

public sealed partial class MarketSystem : AEntitySetSystem<double>
{
	private readonly EntityMultiMap<Supply> _supply;

	public MarketSystem(World world) : base(world, CreateEntityContainer, null, 0)
	{
		_supply = world.GetEntities().AsMultiMap(new SupplyComparer());
	}

	[WithPredicate]
	private static bool HasDemand(in Producer producer) => producer.Input != null;

	[Update, UseBuffer]
	private void Update(in Entity entity, in Demand demand, in Inventory demandInventory, in RemainingInventorySpace inventorySpace)
	{
		if (inventorySpace == 0)
		{
			return;
		}

		var good = demand.Good;

		var dummySupply = new Supply(good);

		if (!_supply.TryGetEntities(dummySupply, out var suppliers))
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

		entity.NotifyChanged<Inventory>();
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

	private sealed class SupplyComparer : IEqualityComparer<Supply>
	{
		public bool Equals(Supply x, Supply y)
		{
			if (x == y)
			{
				return true;
			}

			return x.Good == y.Good;
		}

		public int GetHashCode(Supply obj) => obj.Good.GetHashCode();
	}
}
