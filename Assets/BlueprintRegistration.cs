using Microsoft.Extensions.DependencyInjection;
using Tycoon.Buildings;

namespace Tycoon.Assets;

public static class BlueprintRegistration
{
	public static IServiceCollection AddBlueprints(this IServiceCollection serviceCollection)
	{
		return serviceCollection
			.AddSingleton<IBlueprint, House>()
			.AddSingleton<IBlueprint, Woodcutter>()
			.AddSingleton<IBlueprint, Field>();
	}
}
