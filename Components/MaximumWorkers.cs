namespace Tycoon.Components;

public readonly record struct MaximumWorkers(int Value)
{
	public static implicit operator int(MaximumWorkers capacity)
	{
		return capacity.Value;
	}

	public static implicit operator MaximumWorkers(int value)
	{
		return new MaximumWorkers(value);
	}
}
