//--- Aura Script -----------------------------------------------------------
// Alby Silver Gem Transformation
//--- Description -----------------------------------------------------------
// Script for Alby Basic.
//---------------------------------------------------------------------------

[DungeonScript("tircho_alby_trans_dungeon")]
public class AlbyTrans1DungeonScript : DungeonScript
{
	public override void OnBoss(Dungeon dungeon)
	{
		dungeon.AddBoss(30004, 1); // Masterless Lightshedder
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
				// Dagger
				var prefix = 0;
				switch (rnd.Next(3))
				{
					case 0: prefix = 20501; break; // Simple
					case 1: prefix = 20502; break; // Scrupulous
					case 2: prefix = 20201; break; // Hard
				}
				treasureChest.Add(Item.CreateEnchanted(40006, prefix, 0));
			}

			treasureChest.AddGold(rnd.Next(1072, 3680)); // Gold
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
			drops.Add(new DropData(itemId: 62004, chance: 38, amountMin: 1, amountMax: 2)); // Magic Powder
			drops.Add(new DropData(itemId: 51102, chance: 38, amountMin: 1, amountMax: 2)); // Mana Herb
			drops.Add(new DropData(itemId: 60042, chance: 9, amountMin: 1, amountMax: 5)); // Magical Silver Thread
			drops.Add(new DropData(itemId: 63101, chance: 9, amount: 1, expires: 600)); // Alby Basic Fomor Pass
			drops.Add(new DropData(itemId: 63116, chance: 2, amount: 1, expires: 480)); // Alby Intermediate Fomor Pass for One
			drops.Add(new DropData(itemId: 63117, chance: 2, amount: 1, expires: 480)); // Alby Intermediate Fomor Pass for Two
			drops.Add(new DropData(itemId: 63118, chance: 2, amount: 1, expires: 480)); // Alby Intermediate Fomor Pass for Four

			if (IsEnabled("AlbyAdvanced"))
			{
				drops.Add(new DropData(itemId: 63160, chance: 5, amount: 1, expires: 360)); // Alby Advanced 3-person Fomor Pass
				drops.Add(new DropData(itemId: 63161, chance: 5, amount: 1, expires: 360)); // Alby Advanced Fomor Pass
			}
		}

		return Item.GetRandomDrop(rnd, drops);
	}
}

/* 

public class AlbyTrans2DungeonScript : DungeonScript
{
	public override void OnBoss(Dungeon dungeon)
	{
		dungeon.AddBoss(30004, 1); // Masterless Darkfighter
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
				// Dagger
				var prefix = 0;
				switch (rnd.Next(3))
				{
					case 0: prefix = 20501; break; // Simple
					case 1: prefix = 20502; break; // Scrupulous
					case 2: prefix = 20201; break; // Hard
				}
				treasureChest.Add(Item.CreateEnchanted(40006, prefix, 0));
			}

			treasureChest.AddGold(rnd.Next(1072, 3680)); // Gold
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
			drops.Add(new DropData(itemId: 62004, chance: 38, amountMin: 1, amountMax: 2)); // Magic Powder
			drops.Add(new DropData(itemId: 51102, chance: 38, amountMin: 1, amountMax: 2)); // Mana Herb
			drops.Add(new DropData(itemId: 60042, chance: 9, amountMin: 1, amountMax: 5)); // Magical Silver Thread
			drops.Add(new DropData(itemId: 63101, chance: 9, amount: 1, expires: 600)); // Alby Basic Fomor Pass
			drops.Add(new DropData(itemId: 63116, chance: 2, amount: 1, expires: 480)); // Alby Intermediate Fomor Pass for One
			drops.Add(new DropData(itemId: 63117, chance: 2, amount: 1, expires: 480)); // Alby Intermediate Fomor Pass for Two
			drops.Add(new DropData(itemId: 63118, chance: 2, amount: 1, expires: 480)); // Alby Intermediate Fomor Pass for Four

			if (IsEnabled("AlbyAdvanced"))
			{
				drops.Add(new DropData(itemId: 63160, chance: 5, amount: 1, expires: 360)); // Alby Advanced 3-person Fomor Pass
				drops.Add(new DropData(itemId: 63161, chance: 5, amount: 1, expires: 360)); // Alby Advanced Fomor Pass
			}
		}

		return Item.GetRandomDrop(rnd, drops);
	}
}

*/