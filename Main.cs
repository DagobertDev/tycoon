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
			.AddTransient<FPSCounter>()
			.AddTransient<BuildControl>()
			.AddSingleton<Map>()
			.AddSingleton<IBlueprintPlacer, BlueprintPlacer>()
			.AddSingleton<BlueprintGhost>()
			.AddSingleton<Camera>()
			.AddSingleton<EntityMenu>()
			.AddSingleton<DebugConsole>()
			.AddSingleton(new MapSettings
			{
				Size = Vector2I.One * 100_000,
			})
			.AddBlueprints()
			.AddSystems()
			.AddDebugCommands();
	}

	private void BuildGUI(IServiceProvider serviceProvider)
	{
		var guiLayer = new CanvasLayer();

		var goldCounter = serviceProvider.GetRequiredService<IGoldCounter>();
		
		var goldLabel = new Label
		{
			LayoutMode = 1,
			AnchorsPreset = (int)Control.LayoutPreset.CenterTop,
		};

		var disposable = goldCounter.GoldObservable.Subscribe(gold => goldLabel.Text = $"Gold: {gold}");
		goldLabel.TreeExited += disposable.Dispose;
		
		guiLayer.AddChild(goldLabel);

		var fpsCounter = serviceProvider.GetRequiredService<FPSCounter>();
		fpsCounter.LayoutMode = 1;
		fpsCounter.AnchorsPreset = (int)Control.LayoutPreset.TopRight;
		guiLayer.AddChild(fpsCounter);

		var buildingControl = serviceProvider.GetRequiredService<BuildControl>();
		buildingControl.LayoutMode = 1;
		buildingControl.AnchorsPreset = (int)Control.LayoutPreset.CenterBottom;
		guiLayer.AddChild(buildingControl);
		
		AddChild(guiLayer);

		var entityMenuTrigger = serviceProvider.GetRequiredService<EntityMenu>();
		AddChild(entityMenuTrigger);

		var buildingGhost = serviceProvider.GetRequiredService<BlueprintGhost>();
		AddChild(buildingGhost);

		var debugConsole = serviceProvider.GetRequiredService<DebugConsole>();
		guiLayer.AddChild(debugConsole);
	}
}
