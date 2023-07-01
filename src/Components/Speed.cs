namespace Tycoon.Components;

public readonly record struct Speed(double Value)
{
	public static implicit operator double(Speed speed)
	{
		return speed.Value;
	}

	public static implicit operator Speed(double value)
	{
		return new Speed(value);
	}
}
