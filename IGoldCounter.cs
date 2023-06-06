using System;

namespace Tycoon;

public interface IGoldCounter
{
	int Gold { get; set; }
	IObservable<int> GoldObservable { get; }
}
