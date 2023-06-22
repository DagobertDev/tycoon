using System;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using Tycoon.Components;

namespace Tycoon.Systems;

[With(typeof(Speed))]
[Without(typeof(Destination))]
public sealed partial class WalkSystem : AEntitySetSystem<double>
{
	[ConstructorParameter]
	private readonly MapSettings _mapSettings;

	[Update, UseBuffer]
	private void Update(in Entity entity, in Node2D node2D)
	{
		const int maxOffset = 1000;

		var xOffset = Random.Shared.Next(-maxOffset, maxOffset);
		var yOffset = Random.Shared.Next(-maxOffset, maxOffset);

		var destination = node2D.GlobalPosition + new Vector2(xOffset, yOffset);
		destination = destination.Clamp(Vector2.Zero, _mapSettings.Size);

		entity.Set<Destination>(destination);
	}
}
