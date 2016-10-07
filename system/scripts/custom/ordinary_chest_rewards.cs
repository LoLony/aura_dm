[ItemScript(91038)]
public class Chest1Rewards : ItemScript
{
	public override void OnUse(Creature cr, Item i, string param)
	{
		if (i.Data.Id == 91038) // Ordinary Chest
		{
			if (cr.Inventory.Has(70155))
			{
				Send.ServerMessage(cr, Localization.Get("Player has the key."));
			}
			else
			{
				Send.ServerMessage(cr, Localization.Get("Player does not have the key."));
			}

		}
	}
}