using Godot;
using Microsoft.Extensions.DependencyInjection;
using Tycoon.GUI;

namespace Tycoon;

public partial class Main : Node
{
	public override void _Ready()
	{
		var services = new ServiceCollection();
		RegisterServices(services);
		var serviceProvider = services.BuildServiceProvider();

		var goldCounter = serviceProvider.GetRequiredService<GoldLabel>();
		goldCounter.AnchorsPreset = (int)Control.LayoutPreset.CenterTop;
		AddChild(goldCounter);

		var fpsCounter = serviceProvider.GetRequiredService<FPSCounter>();
		fpsCounter.AnchorsPreset = (int)Control.LayoutPreset.TopRight;
		AddChild(fpsCounter);
	}

	private static void RegisterServices(IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<IGoldCounter, GoldCounter>();
		serviceCollection.AddTransient<GoldLabel>();
		serviceCollection.AddTransient<FPSCounter>();
	}
}
