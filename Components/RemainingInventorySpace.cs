namespace Tycoon.Components;

public readonly record struct RemainingInventorySpace(int Value)
{
	public static implicit operator int(RemainingInventorySpace space)
	{
		return space.Value;
	}

	public static implicit operator RemainingInventorySpace(int value)
	{
		return new RemainingInventorySpace(value);
	}
}
