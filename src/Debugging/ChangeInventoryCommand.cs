using System.Collections.Generic;
using Tycoon.Components;
using Tycoon.GUI;

namespace Tycoon.Debugging;

public class ChangeInventoryCommand : IDebugCommand
{
	private readonly EntityMenu _entityMenu;

	public ChangeInventoryCommand(EntityMenu entityMenu)
	{
		_entityMenu = entityMenu;
	}

	public string Name => "ChangeInventory";
	public string Description => "Modifies the inventory of the selected building.\n" +
	                             "Possible options:\n" +
	                             "add <good> <amount>\n" +
	                             "remove <good> <amount>\n" +
	                             "set <good> <amount>\n";
	public string? Execute(string[] parameters)
	{
		var entity = _entityMenu.Entity;

		if (!entity.IsAlive)
			return "No valid entity selected";

		if (parameters.Length != 3)
			return "Exactly 3 parameters expected";

		var command = parameters[0];
		var goodString = parameters[1];
		var amountString = parameters[2];

		// TODO: Make this safer
		var good = new Good(goodString);

		if (!int.TryParse(amountString, out var changedAmount))
			return $"'{amountString}' is not a valid amount.";

		if (!entity.Has<Inventory>())
			entity.Set(new Inventory(new Dictionary<Good, int>()));

		var inventory = entity.Get<Inventory>().Value;
		var value = inventory.GetValueOrDefault(good);

		switch (command)
		{
			case "add":
				value += changedAmount;
				break;
			case "remove":
				value -= changedAmount;
				break;
			case "set":
				value = changedAmount;
				break;
			default:
				return $"Operation '{command}' is not supported";
		}

		inventory[good] = value;
		entity.NotifyChanged<Inventory>();
		return $"Amount of '{good}' is now {value}.";
	}
}
