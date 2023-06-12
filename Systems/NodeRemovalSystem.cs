using DefaultEcs;
using DefaultEcs.System;
using Godot;

namespace Tycoon.Systems;

public class NodeRemovalSystem : ISystem<double>
{
	public NodeRemovalSystem(World world)
	{
		world.SubscribeComponentRemoved((in Entity _, in Node2D node) => node.QueueFree());
	}

	public void Dispose() { }

	public void Update(double state) { }

	public bool IsEnabled { get; set; }
}
