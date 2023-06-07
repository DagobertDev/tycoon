using DefaultEcs;
using System;

namespace Tycoon.Systems;

public static class EnumComponentExtensions
{
	public static void AddFlag<T>(this Entity entity, T flag) where T : struct, Enum, IConvertible
	{
		T currentValue = default;

		if (entity.Has<T>())
		{
			currentValue = entity.Get<T>();
		}

		var newValue = currentValue.ToInt64(null) | flag.ToInt64(null);
		entity.Set((T)Enum.ToObject(typeof(T), newValue));
	}

	public static void RemoveFlag<T>(this Entity entity, T flag) where T : struct, Enum, IConvertible
	{
		if (!entity.Has<T>())
		{
			return;
		}

		var currentValue = entity.Get<T>();
		var newValue = currentValue.ToInt64(null) & ~flag.ToInt64(null);
		entity.Set((T)Enum.ToObject(typeof(T), newValue));
	}
}
