using DefaultEcs.System;
using Microsoft.Extensions.DependencyInjection;

namespace Tycoon.Systems;

public static class SystemRegistration
{
	public static IServiceCollection AddSystems(this IServiceCollection serviceCollection) =>
		serviceCollection
			.AddSystem<InventoryCapacitySystem>()
			.AddSystem<InventoryFullSystem>()
			.AddSystem<RemoveCanNotWorkReasonNoneSystem>()
			.AddSystem<CheckInputSystem>()
			.AddSystem<ProductionWithWorkersSystem>()
			.AddSystem<ProductionWithoutWorkerSystem>()
			.AddSystem<ProductionCompletedSystem>()
			.AddSystem<FindWorkplaceSystem>()
			.AddSystem<NodeRemovalSystem>()
			.AddSystem<AISystem>()
			.AddSystem<WalkSystem>()
			.AddSystem<MovementSystem>()
			.AddSystem<Node2DPositionSystem>()
			.AddSystem<SupplyDemandSetupSystem>()
			.AddSystem<MarketSystem>();

	private static IServiceCollection AddSystem<T>(this IServiceCollection serviceCollection)
		where T : class, ISystem<double> => serviceCollection.AddSingleton<ISystem<double>, T>();
}
