namespace Tycoon.Components;

public readonly record struct InventoryCapacity(int Value)
{
	public static implicit operator int(InventoryCapacity capacity) => capacity.Value;

	public static implicit operator InventoryCapacity(int value) =>
		new(value);
}
