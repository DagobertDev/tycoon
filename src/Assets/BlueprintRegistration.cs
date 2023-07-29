using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Tycoon.Buildings;

namespace Tycoon.Assets;

public static class BlueprintRegistration
{
	public static IServiceCollection AddBlueprints(this IServiceCollection serviceCollection)
	{
		var blueprints = typeof(BlueprintRegistration).Assembly.GetTypes()
			.Where(x => !x.IsAbstract && x.IsAssignableTo(typeof(IBlueprint)));

		foreach (var blueprint in blueprints)
			serviceCollection.AddSingleton(typeof(IBlueprint), blueprint);

		return serviceCollection;
	}
}
