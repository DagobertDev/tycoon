using System;
using Godot;

namespace Tycoon.GUI;

public partial class GoldLabel : Label
{
	private readonly IGoldCounter _goldCounter;

	public GoldLabel(IGoldCounter goldCounter)
	{
		_goldCounter = goldCounter ?? throw new ArgumentNullException(nameof(goldCounter));
	}

	public override void _Process(double delta)
	{
		Text = $"Gold: {_goldCounter.Gold}";
	}
}
