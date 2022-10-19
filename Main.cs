using System;
using Godot;
using Microsoft.Extensions.DependencyInjection;
using Tycoon.Assets;
using Tycoon.Buildings;
using Tycoon.GUI;

namespace Tycoon;

public partial class Main : Node
{
	public override void _Ready()
	{
		var services = new ServiceCollection();
		RegisterServices(services);
		var serviceProvider = services.BuildServiceProvider();

		var map = serviceProvider.GetRequiredService<Map>();
		AddChild(map);
		serviceProvider.GetRequiredService<IGoldCounter>().Gold = 100;

		var camera = serviceProvider.GetRequiredService<Camera>();
		AddChild(camera);

		var entityMenuTrigger = serviceProvider.GetRequiredService<EntityMenu>();
		AddChild(entityMenuTrigger);

		BuildGUI(serviceProvider);
	}

	private static void RegisterServices(IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<IGoldCounter, GoldCounter>()
			.AddTransient<GoldLabel>()
			.AddTransient<FPSCounter>()
			.AddTransient<BuildControl>()
			.AddSingleton<IBlueprint, House>()
			.AddSingleton<IBlueprint, Woodcutter>()
			.AddSingleton<Map>()
			.AddSingleton<IBlueprintPlacer, BlueprintPlacer>()
			.AddSingleton<BlueprintGhost>()
			.AddSingleton<Camera>()
			.AddSingleton<EntityMenu>();
	}

	private void BuildGUI(IServiceProvider serviceProvider)
	{
		var guiLayer = new CanvasLayer();

		var goldCounter = serviceProvider.GetRequiredService<GoldLabel>();
		goldCounter.AnchorsPreset = (int)Control.LayoutPreset.CenterTop;
		guiLayer.AddChild(goldCounter);

		var fpsCounter = serviceProvider.GetRequiredService<FPSCounter>();
		fpsCounter.AnchorsPreset = (int)Control.LayoutPreset.TopRight;
		guiLayer.AddChild(fpsCounter);

		var buildingControl = serviceProvider.GetRequiredService<BuildControl>();
		buildingControl.AnchorsPreset = (int)Control.LayoutPreset.CenterBottom;
		guiLayer.AddChild(buildingControl);

		AddChild(guiLayer);

		var buildingGhost = serviceProvider.GetRequiredService<BlueprintGhost>();
		AddChild(buildingGhost);
	}
}
