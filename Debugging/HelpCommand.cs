using System;
using System.Collections.Generic;
using System.Linq;

namespace Tycoon.Debugging;

public class HelpCommand : IDebugCommand
{
	private readonly Lazy<IEnumerable<IDebugCommand>> _commands;

	public HelpCommand(Lazy<IEnumerable<IDebugCommand>> commands)
	{
		_commands = commands;
	}

	public string Name => "Help";
	public string Description => "Lists all available commands";

	public string? Execute(string[] parameters)
	{
		if (parameters.Length == 0)
		{
			return $"Available commands: {string.Concat(_commands.Value.Select(command => command.Name))}";
		}

		var commandName = parameters[0];
		var command = _commands.Value.SingleOrDefault(command =>
			string.Equals(command.Name, commandName, StringComparison.OrdinalIgnoreCase));

		return command is null ? $"Command {commandName} not found" : $"{command.Name}: {command.Description}";
	}
}
