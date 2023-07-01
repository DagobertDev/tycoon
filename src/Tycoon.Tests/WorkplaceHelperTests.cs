using DefaultEcs;
using Tycoon.Components;
using Tycoon.Systems.Helpers;

namespace Tycoon.Tests;

[TestFixture]
public class WorkplaceHelperTests
{
	private World _world;

	[SetUp]
	public void Setup()
	{
		_world = new World();
	}

	[Test]
	public void TestInitialWorkerCountIsZero()
	{
		var workplace = _world.CreateEntity();
		var workerCount = WorkplaceHelper.GetWorkerCount(workplace);
		Assert.That(workerCount, Is.Zero);
	}

	[Test]
	public void TestInitialWorkersAreEmpty()
	{
		var workplace = _world.CreateEntity();
		var workers = WorkplaceHelper.GetWorkers(workplace);
		Assert.That(workers.IsEmpty);
	}

	[Test]
	public void TestWorkerCountIsOneAfterAddingWorker()
	{
		var workplace = _world.CreateEntity();
		var worker = _world.CreateEntity();
		worker.Set(new Worker(workplace));
		var workerCount = WorkplaceHelper.GetWorkerCount(workplace);
		Assert.That(workerCount, Is.EqualTo(1));
	}

	[Test]
	public void TestWorkerCountIsZeroAfterRemovingWorker()
	{
		var workplace = _world.CreateEntity();
		var worker = _world.CreateEntity();
		worker.Set(new Worker(workplace));
		worker.Set(Worker.Unemployed);
		var workerCount = WorkplaceHelper.GetWorkerCount(workplace);
		Assert.That(workerCount, Is.Zero);
	}
}
