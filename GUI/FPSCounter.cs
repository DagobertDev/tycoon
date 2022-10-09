using Godot;

namespace Tycoon.GUI;

public partial class FPSCounter : Label
{
	public override void _Process(double delta)
	{
		Text = $"FPS: {Engine.GetFramesPerSecond()}";
	}
}
