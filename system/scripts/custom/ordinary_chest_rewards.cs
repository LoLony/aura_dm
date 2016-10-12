[ItemScript(91038)]
public class Chest1Rewards : ItemScript
{
	public override void OnUse(Creature cr, Item i, string param)
	{
		if (i.Data.Id == 91038) // Ordinary Chest
		{
			if (cr.Inventory.Has(70155))
			{
				cr.Inventory.Remove(70155,1);
				cr.Inventory.Remove(91038,1);
				List<DropData> list;
				list = new List<DropData>();
				list.Add(new DropData(itemId: 52033, chance: 10, amountMin: 1, amountMax: 3)); 	// Red Coin
				list.Add(new DropData(itemId: 52032, chance: 10, amountMin: 1, amountMax: 3)); 	// Blue Coin
				list.Add(new DropData(itemId: 40907, chance: 5)); 	// Dark Knight Two-handed Sword
				list.Add(new DropData(itemId: 40274, chance: 5)); 	// Taillteann Two-handed Sword
				list.Add(new DropData(itemId: 40030, chance: 5)); 	// Two-handed Sword
				list.Add(new DropData(itemId: 63255, chance: 20)); 	// Fixed Metal Dye Ampoule
				list.Add(new DropData(itemId: 91138, chance: 20)); 	// Fixed Color Dye Ampoule

				int rand = Random(1,2);
				for(int ctr=0;ctr<rand;ctr++) {
					var rnd = RandomProvider.Get();
					var item = Item.GetRandomDrop(rnd, list);
					if (item != null)
					{
						cr.Inventory.Add(item, true);
						Send.ServerMessage(cr, Localization.Get("You opened the chest and found " + item.Data.Name + "."));
					}
				}
				Send.Notice(cr, Localization.Get("You opened the chest and found " + rand + " items."));
			}
			else
			{
				Send.ServerMessage(cr, Localization.Get("No Key to the Ordinary Chest in inventory."));
			}
		}
	}
}