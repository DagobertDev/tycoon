using DefaultEcs.System;
using Microsoft.Extensions.DependencyInjection;

namespace Tycoon.Systems;

public static class SystemRegistration
{
	public static IServiceCollection AddSystems(this IServiceCollection serviceCollection)
	{
		return serviceCollection.AddSingleton<ISystem<double>, ProductionSystem>();
	}
}
