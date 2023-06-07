using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

public sealed partial class RemoveCanNotWorkReasonNoneSystem : AEntitySetSystem<double>
{
	[WithPredicate]
	private static bool NoReason(in CanNotWorkReason reason)
	{
		return reason == CanNotWorkReason.None;
	}

	[Update, UseBuffer]
	private static void Update(in Entity entity)
	{
		entity.Remove<CanNotWorkReason>();
	}
}
