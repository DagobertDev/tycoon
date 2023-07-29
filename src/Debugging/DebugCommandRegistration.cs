using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Tycoon.Debugging;

public static class DebugCommandRegistration
{
	public static IServiceCollection AddDebugCommands(this IServiceCollection serviceCollection)
	{
		var blueprints = typeof(DebugCommandRegistration).Assembly.GetTypes()
			.Where(x => !x.IsAbstract && x.IsAssignableTo(typeof(IDebugCommand)));

		foreach (var blueprint in blueprints)
			serviceCollection.AddSingleton(typeof(IDebugCommand), blueprint);

		serviceCollection.AddSingleton(serviceProvider =>
			new Lazy<IEnumerable<IDebugCommand>>(serviceProvider.GetServices<IDebugCommand>));

		return serviceCollection;
	}
}
