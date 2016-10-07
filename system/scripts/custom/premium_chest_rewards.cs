[ItemScript(91039)]
public class PremiumChestRewards : ItemScript
{
	public override void OnUse(Creature cr, Item i, string param)
	{
		if (i.Data.Id == 91039) // Premium Chest
		{
			if (cr.Inventory.Has(70156))
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