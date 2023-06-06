using System;
using System.Reactive.Subjects;

namespace Tycoon;

public class GoldCounter : IGoldCounter
{
	public int Gold
	{
		get => _goldObservable.Value;
		set => _goldObservable.OnNext(value);
	}

	private readonly BehaviorSubject<int> _goldObservable = new(default);
	public IObservable<int> GoldObservable => _goldObservable;
}
