using Tycoon.Components;

namespace Tycoon.Buildings;

public interface IProductionSite : IHasInventory
{
	Producer Producer { get; }
	bool RequiresWorker { get; }
}
