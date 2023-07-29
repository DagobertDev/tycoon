namespace Tycoon.Components;

public readonly record struct ProductionProgress(double Value)
{
	public static implicit operator double(ProductionProgress progress) =>
		progress.Value;

	public static implicit operator ProductionProgress(double value) =>
		new ProductionProgress(value);
}
