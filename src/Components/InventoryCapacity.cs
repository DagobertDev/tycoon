namespace Tycoon.Components;

public readonly record struct InventoryCapacity(int Value)
{
	public static implicit operator int(InventoryCapacity capacity)
	{
		return capacity.Value;
	}

	public static implicit operator InventoryCapacity(int value)
	{
		return new InventoryCapacity(value);
	}
}
