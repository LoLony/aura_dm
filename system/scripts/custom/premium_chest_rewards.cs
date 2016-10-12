[ItemScript(91039)]
public class PremiumChestRewards : ItemScript
{
	public override void OnUse(Creature cr, Item i, string param)
	{
		if (i.Data.Id == 91039) // Premium Chest
		{
			if (cr.Inventory.Has(70156))
			{
				cr.Inventory.Remove(70156,1);
				cr.Inventory.Remove(91039,1);
				List<DropData> list;
				list = new List<DropData>();
				list.Add(new DropData(itemId: 52033, chance: 25, amountMin: 3, amountMax: 6)); 	// Red Coin
				list.Add(new DropData(itemId: 52032, chance: 25, amountMin: 3, amountMax: 6)); 	// Blue Coin
				list.Add(new DropData(itemId: 40907, chance: 5)); 	// Dark Knight Two-handed Sword
				list.Add(new DropData(itemId: 40907, chance: 1, suffix: 31202)); 	// [Jackal] Dark Knight Two-handed Sword
				list.Add(new DropData(itemId: 40274, chance: 5)); 	// Taillteann Two-handed Sword
				list.Add(new DropData(itemId: 40274, chance: 1, suffix: 31202)); 	// [Jackal] Taillteann Two-handed Sword
				list.Add(new DropData(itemId: 40030, chance: 5)); 	// Two-handed Sword
				list.Add(new DropData(itemId: 40030, chance: 1, suffix: 21008)); 	// [Arc Lich's] Two-handed Sword
				list.Add(new DropData(itemId: 63069, chance: 10)); 	// Metal Dye Ampoule
				list.Add(new DropData(itemId: 63255, chance: 10)); 	// Fixed Metal Dye Ampoule
				list.Add(new DropData(itemId: 63024, chance: 10)); 	// Dye Ampoule
				list.Add(new DropData(itemId: 91138, chance: 10)); 	// Fixed Color Dye Ampoule
				int rand = Random(1,4);
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
				Send.ServerMessage(cr, Localization.Get("No Key to the Premium Chest in inventory."));
			}
		}
	}
}