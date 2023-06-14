using System;
using Godot;
using Tycoon.Debugging;

namespace Tycoon.GUI;

public partial class GameView : CanvasLayer
{
	public GameView(IGoldCounter goldCounter, FPSCounter fpsCounter, BuildControl buildControl, EntityMenu entityMenu, DebugConsole debugConsole)
	{
		var goldLabel = new Label
		{
			LayoutMode = 1,
			AnchorsPreset = (int)Control.LayoutPreset.CenterTop,
		};

		var disposable = goldCounter.GoldObservable.Subscribe(gold => goldLabel.Text = $"Gold: {gold}");
		goldLabel.TreeExited += disposable.Dispose;
		AddChild(goldLabel);

		fpsCounter.LayoutMode = 1;
		fpsCounter.AnchorsPreset = (int)Control.LayoutPreset.TopRight;
		AddChild(fpsCounter);
		buildControl.LayoutMode = 1;
		buildControl.AnchorsPreset = (int)Control.LayoutPreset.CenterBottom;
		AddChild(buildControl);
		AddChild(entityMenu);
		AddChild(debugConsole);
	}
}
