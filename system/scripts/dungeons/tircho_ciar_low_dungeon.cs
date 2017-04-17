//--- Aura Script -----------------------------------------------------------
// Ciar Basic Dungeon
//--- Description -----------------------------------------------------------
// Script for Ciar Basic.
//---------------------------------------------------------------------------

[DungeonScript("tircho_ciar_low_dungeon")]
public class CiarBasicDungeonScript : DungeonScript
{
	public override void OnBoss(Dungeon dungeon)
	{
		dungeon.AddBoss(130002, 1); // Small Golem
		dungeon.AddBoss(11010, 6); // Metal Skeleton

		dungeon.PlayCutscene("bossroom_MetalskeletonArmorA_Golem");
	}

	public override void OnCleared(Dungeon dungeon)
	{
		var rnd = RandomProvider.Get();
		var creators = dungeon.GetCreators();

		for (int i = 0; i < creators.Count; ++i)
		{
			var member = creators[i];
			var treasureChest = new TreasureChest();

			if (i == 0)
			{
				// Broad Stick
				int prefix = 0, suffix = 0;
				switch (rnd.Next(3))
				{
					case 0: prefix = 207; break; // Fox
					case 1: prefix = 306; break; // Sharp
					case 2: prefix = 303; break; // Rusty
				}
				switch (rnd.Next(3))
				{
					case 0: suffix = 11106; break; // Blood
					case 1: suffix = 10806; break; // Understanding
					case 2: suffix = 10704; break; // Slug
				}
				treasureChest.Add(Item.CreateEnchanted(40019, prefix, suffix));

				// Goro's Ring (G1)
				if (member.HasKeyword("g1_13"))
					treasureChest.Add(Item.Create(73060));
			}

			treasureChest.AddGold(rnd.Next(1232, 4064)); // Gold
			treasureChest.Add(GetRandomTreasureItem(rnd)); // Random item

			dungeon.AddChest(treasureChest);

			member.GiveItemWithEffect(Item.CreateKey(70028, "chest"));
		}
	}

	List<DropData> drops;
	public Item GetRandomTreasureItem(Random rnd)
	{
		if (drops == null)
		{
			drops = new List<DropData>();
			drops.Add(new DropData(itemId: 62004, chance: 15, amountMin: 1, amountMax: 2)); // Magic Powder
			drops.Add(new DropData(itemId: 51102, chance: 15, amountMin: 1, amountMax: 2)); // Mana Herb
			drops.Add(new DropData(itemId: 51003, chance: 15, amountMin: 1, amountMax: 2)); // HP 50 Potion
			drops.Add(new DropData(itemId: 51008, chance: 15, amountMin: 1, amountMax: 2)); // MP 50 Potion
			drops.Add(new DropData(itemId: 51013, chance: 15, amountMin: 1, amountMax: 2)); // Stamina 50 Potion
			drops.Add(new DropData(itemId: 71049, chance: 4, amountMin: 2, amountMax: 5)); // Snake Fomor Scroll
			drops.Add(new DropData(itemId: 63104, chance: 4, amount: 1, expires: 600)); // Ciar Basic Fomor Pass
			drops.Add(new DropData(itemId: 63123, chance: 3, amount: 1, expires: 480)); // Ciar Intermediate Fomor Pass for One
			drops.Add(new DropData(itemId: 63124, chance: 3, amount: 1, expires: 480)); // Ciar Intermediate Fomor Pass for Two
			drops.Add(new DropData(itemId: 63125, chance: 3, amount: 1, expires: 480)); // Ciar Intermediate Fomor Pass for Four
			drops.Add(new DropData(itemId: 40015, chance: 1, amount: 1, color1: 0xFFDB60, durability: 0)); // Fluted Short Sword (gold)

			if (IsEnabled("CiarAdvanced"))
			{
				drops.Add(new DropData(itemId: 63136, chance: 5, amount: 1, expires: 360)); // Ciar Adv. Fomor Pass for 2
				drops.Add(new DropData(itemId: 63137, chance: 5, amount: 1, expires: 360)); // Ciar Adv. Fomor Pass for 3
				drops.Add(new DropData(itemId: 63138, chance: 5, amount: 1, expires: 360)); // Ciar Adv. Fomor Pass
			}
		}

		return Item.GetRandomDrop(rnd, drops);
	}
}
