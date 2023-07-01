using System;
using System.Reactive.Subjects;
using Godot;
using Tycoon.Debugging;

namespace Tycoon.GUI;

public partial class GameView : CanvasLayer
{
	public GameView(IGoldCounter goldCounter, BuildControl buildControl, EntityMenu entityMenu, DebugConsole debugConsole)
	{
		var goldLabel = new Label
		{
			LayoutMode = 1,
			AnchorsPreset = (int)Control.LayoutPreset.CenterTop,
		};

		var disposable = goldCounter.GoldObservable.Subscribe(gold => goldLabel.Text = $"Gold: {gold}");
		goldLabel.TreeExited += disposable.Dispose;
		AddChild(goldLabel);

		var fpsCounter = new Label
		{
			LayoutMode = 1,
			AnchorsPreset = (int)Control.LayoutPreset.TopRight,
		};

		_fps.Subscribe(_ => fpsCounter.Text = $"FPS: {Engine.GetFramesPerSecond()}");

		AddChild(fpsCounter);

		buildControl.LayoutMode = 1;
		buildControl.AnchorsPreset = (int)Control.LayoutPreset.CenterBottom;
		AddChild(buildControl);
		AddChild(entityMenu);
		AddChild(debugConsole);
	}

	private readonly ISubject<double> _fps = new Subject<double>();

	public override void _Process(double delta)
	{
		_fps.OnNext(Engine.GetFramesPerSecond());
	}
}
