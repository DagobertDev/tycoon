namespace Tycoon.Components;

public record Producer(Good Output, int OutputAmount, double ProgressPerSecond, Good? Input = null, int InputAmount = 0);
