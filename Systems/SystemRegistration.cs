using DefaultEcs.System;
using Microsoft.Extensions.DependencyInjection;

namespace Tycoon.Systems;

public static class SystemRegistration
{
	public static IServiceCollection AddSystems(this IServiceCollection serviceCollection)
	{
		return serviceCollection
			.AddSystem<InventoryCapacitySystem>()
			.AddSystem<InventoryFullSystem>()
			.AddSystem<ProductionSystem>();
	}

	private static IServiceCollection AddSystem<T>(this IServiceCollection serviceCollection)
		where T : class, ISystem<double>
	{
		return serviceCollection.AddSingleton<ISystem<double>, T>();
	}
}
