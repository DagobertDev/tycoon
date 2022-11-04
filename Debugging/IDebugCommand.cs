namespace Tycoon.Debugging;

public interface IDebugCommand
{
	string Name { get; }
	string Description { get; }
	string? Execute(string[] parameters);
}
