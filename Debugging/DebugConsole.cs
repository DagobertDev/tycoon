using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Tycoon.Debugging;

public partial class DebugConsole : Panel
{
	private readonly IEnumerable<IDebugCommand> _commands;
	private readonly Label _log = new();

	public DebugConsole(IEnumerable<IDebugCommand> commands)
	{
		_commands = commands;
	}

	public override void _Ready()
	{
		Visible = false;

		AnchorsPreset = (int)LayoutPreset.TopWide;
		CustomMinimumSize = new Vector2I(0, 100);

		AddChild(_log);

		var textEdit = new LineEdit();
		textEdit.LayoutMode = 1;
		textEdit.AnchorsPreset = (int)LayoutPreset.BottomWide;
		textEdit.TextSubmitted += TryRunningCommand;
		AddChild(textEdit);
	}

	private void TryRunningCommand(string args)
	{
		var content = args.Split(' ');

		if (content.Length == 0)
		{
			return;
		}

		var name = content[0];
		var command = _commands.SingleOrDefault(command =>
			string.Equals(command.Name, name, StringComparison.OrdinalIgnoreCase));

		if (command is null)
		{
			_log.Text = "Command is not known";
			return;
		}

		var result = command.Execute(content.Skip(1).ToArray());

		if (result is null)
		{
			return;
		}

		_log.Text = result;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed(InputActions.DebugMenu))
		{
			Visible = !Visible;
			GetTree().Root.SetInputAsHandled();
		}
	}
}
