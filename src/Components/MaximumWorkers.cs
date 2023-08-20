namespace Tycoon.Components;

public readonly record struct MaximumWorkers(int Value)
{
	public static implicit operator int(MaximumWorkers capacity) => capacity.Value;

	public static implicit operator MaximumWorkers(int value) =>
		new(value);
}
