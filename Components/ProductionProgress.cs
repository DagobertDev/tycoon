namespace Tycoon.Components;

public readonly record struct ProductionProgress(double Value)
{
	public static implicit operator double(ProductionProgress progress)
	{
		return progress.Value;
	}

	public static implicit operator ProductionProgress(double value)
	{
		return new ProductionProgress(value);
	}
}
