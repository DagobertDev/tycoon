using System;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using Microsoft.Extensions.DependencyInjection;
using Tycoon.Assets;
using Tycoon.Buildings;
using Tycoon.Debugging;
using Tycoon.GUI;
using Tycoon.Systems;

namespace Tycoon;

public partial class Main : Node
{
	private ISystem<double> _system = null!;

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

		var systems = serviceProvider.GetServices<ISystem<double>>();
		_system = new SequentialSystem<double>(systems);

		BuildGUI(serviceProvider);
	}

	public override void _Process(double delta)
	{
		_system.Update(delta);
	}

	private static void RegisterServices(IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<World>()
			.AddSingleton<INodeEntityMapper, NodeEntityMapper>()
			.AddSingleton<IGoldCounter, GoldCounter>()
			.AddTransient<GoldLabel>()
			.AddTransient<FPSCounter>()
			.AddTransient<BuildControl>()
			.AddSingleton<Map>()
			.AddSingleton<IBlueprintPlacer, BlueprintPlacer>()
			.AddSingleton<BlueprintGhost>()
			.AddSingleton<Camera>()
			.AddSingleton<EntityMenu>()
			.AddSingleton<DebugConsole>()
			.AddBlueprints()
			.AddSystems()
			.AddDebugCommands();
	}

	private void BuildGUI(IServiceProvider serviceProvider)
	{
		var guiLayer = new CanvasLayer();

		var goldCounter = serviceProvider.GetRequiredService<GoldLabel>();
		guiLayer.AddChild(goldCounter);
		goldCounter.AnchorsPreset = (int)Control.LayoutPreset.CenterTop;

		var fpsCounter = serviceProvider.GetRequiredService<FPSCounter>();
		guiLayer.AddChild(fpsCounter);
		fpsCounter.AnchorsPreset = (int)Control.LayoutPreset.TopRight;

		var buildingControl = serviceProvider.GetRequiredService<BuildControl>();
		guiLayer.AddChild(buildingControl);
		buildingControl.AnchorsPreset = (int)Control.LayoutPreset.CenterBottom;
		
		AddChild(guiLayer);

		var entityMenuTrigger = serviceProvider.GetRequiredService<EntityMenu>();
		AddChild(entityMenuTrigger);

		var buildingGhost = serviceProvider.GetRequiredService<BlueprintGhost>();
		AddChild(buildingGhost);

		var debugConsole = serviceProvider.GetRequiredService<DebugConsole>();
		guiLayer.AddChild(debugConsole);
	}
}
