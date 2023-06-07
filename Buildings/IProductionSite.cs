using Tycoon.Components;

namespace Tycoon.Buildings;

public interface IProductionSite : IHasInventory
{
	Producer Producer { get; }
	int MaximumWorkers { get; }
}
