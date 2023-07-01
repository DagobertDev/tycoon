namespace Tycoon.Components;

public record Producer(Good Good, int OutputAmount, double ProgressPerSecond, Good? Input = null, int InputAmount = 0);
