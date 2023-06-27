using DefaultEcs;
using DefaultEcs.System;
using Tycoon.Components;

namespace Tycoon.Systems;

[With(typeof(NoWorkersRequired))]
[Without(typeof(CanNotWorkReason))]
public sealed partial class ProductionWithoutWorkerSystem : AEntitySetSystem<double>
{
	[Update]
	private static void Update(double delta, in Entity entity, in Producer producer, ref ProductionProgress progress)
	{
		progress += producer.ProgressPerSecond * delta;
		entity.NotifyChanged<ProductionProgress>();
	}
}
