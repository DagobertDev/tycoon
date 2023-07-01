using DefaultEcs;

namespace Tycoon.Components;

public readonly record struct Worker(Entity Workplace)
{
	public static Worker Unemployed { get; } = new(default);
}
