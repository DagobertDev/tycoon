namespace Tycoon.Components;

public readonly record struct RemainingInventorySpace(int Value)
{
	public static implicit operator int(RemainingInventorySpace space) =>
		space.Value;

	public static implicit operator RemainingInventorySpace(int value) =>
		new RemainingInventorySpace(value);
}
