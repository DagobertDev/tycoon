using DefaultEcs;
using Tycoon.Systems;

namespace Tycoon.Tests;

[TestFixture]
public class EnumComponentExtensionsTest
{
	private Entity _entity;

	[SetUp]
	public void Setup()
	{
		var world = new World();
		_entity = world.CreateEntity();
	}

	[TestCaseSource(nameof(GetAllTestValues))]
	public void AddFlagAddsFlag(TestEnum value)
	{
		_entity.AddFlag(value);
		Assert.That(_entity.Has<TestEnum>(), Is.True);
		Assert.That(_entity.Get<TestEnum>(), Is.EqualTo(value));
	}

	[Test]
	[TestCaseSource(nameof(GetAllTestValues))]
	public void RemoveFlagWithoutExistingFlagDoesNothing(TestEnum value)
	{
		_entity.RemoveFlag(value);
		Assert.That(_entity.Has<TestEnum>(), Is.False);
	}

	private static IEnumerable<TestEnum> GetAllTestValues()
	{
		return Enum.GetValues<TestEnum>();
	}

	public enum TestEnum
	{
		Zero = 0,
		One = 1,
		Two = 2,
		Four = 4,
	}
}
