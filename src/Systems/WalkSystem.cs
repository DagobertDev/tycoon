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
	[WithPredicate]
	private static bool IsWalkingAround(in AgentState state)
	{
		return state == AgentState.WalkingAround;
	}

	[ConstructorParameter]
	private readonly MapSettings _mapSettings;

	[Update, UseBuffer]
	private void Update(in Entity entity, in Position position)
	{
		const int maxOffset = 1000;

		var xOffset = Random.Shared.Next(-maxOffset, maxOffset);
		var yOffset = Random.Shared.Next(-maxOffset, maxOffset);

		var destination = position + new Vector2(xOffset, yOffset);
		destination = destination.Clamp(Vector2.Zero, _mapSettings.Size);

		entity.Set<Destination>(destination);
	}
}
