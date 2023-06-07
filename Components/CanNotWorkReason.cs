using System;

namespace Tycoon.Components;

[Flags]
public enum CanNotWorkReason
{
	None = 0,
	InventoryFull = 1,
	NoEmployee = 2,
}
