//--- Aura Script -----------------------------------------------------------
// Wraith Knight
//--- Description -----------------------------------------------------------
// A ghost of a soldier more dead than alive
//---------------------------------------------------------------------------

using System;

public class WraithScript : NpcScript
{
	public override void Load()
	{
        SetRace(17701);
        SetName("Arabraxos");
        SetBody(height: 1.51f);
        SetColor(0x00202020, 0x00202020, 0x00202020);
        SetStand("human/anim/tool/tequip_c/uni_tool_tequip_c21_stand_offensive");
        SetLocation(1, 16000, 39000, 219);

        /* EquipItem(Pocket.Head, 18661, 0x00202020, 0x00202020, 0x00202020);
        EquipItem(Pocket.Armor, 13006, 0x00202020, 0x00202020, 0x00202020);
        EquipItem(Pocket.Glove, 16502, 0x00202020, 0x00202020, 0x00202020);
        EquipItem(Pocket.Shoe, 17505, 0x00202020, 0x00202020, 0x00202020);
        EquipItem(Pocket.RightHand1, 40083, 0x00353535, 0x0054524D, 0x0054524D);
        EquipItem(Pocket.LeftHand1, 46006, 0x00202020, 0x00202020, 0x00202020); */ // old

        EquipItem(Pocket.Armor, 13043, 0x009FA3A7, 0x00808080, 0x00808080);
        EquipItem(Pocket.Glove, 16529, 0x003D3C3A, 0x00808080, 0x00808080);
        EquipItem(Pocket.Shoe, 17514, 0x003B3838, 0x005A5A5A, 0x00867B7D);
        EquipItem(Pocket.Head, 18520, 0x00383634, 0x00B3A6A4, 0x00416DAF);
        EquipItem(Pocket.RightHand1, 40241, 0x00B7B7B7, 0x00CCCCCC, 0x00242320);

		AddPhrase("Ahh...");
		AddPhrase("......");
		AddPhrase("Hmm...!");
		AddPhrase("Hmm...?");
	}

    [On("ErinnTimeTick")]
    public void OnErinnTimeTick(ErinnTime time)
    {
        if (time.Minute == 0 && (time.Hour % 2 == 0)) // Every six hours, WarpFlash to another region if it is not this one
        {
            /* switch (Convert.ToInt32(Random() % 6))
            {
                case 0:
                    if (this.NPC.RegionId != 1) {NPC.WarpFlash(1, 12800, 38400);}
                    break;
                case 1:
                    if (this.NPC.RegionId != 14) {NPC.WarpFlash(14, 29000, 58400);}
                    break;
                case 2:
                    if (this.NPC.RegionId != 30) {NPC.WarpFlash(30, 36200, 18400);}
                    break;
                case 3:
                    if (this.NPC.RegionId != 52) {NPC.WarpFlash(52, 43500, 24200);}
                    break;
                case 4:
                    if (this.NPC.RegionId != 35) {NPC.WarpFlash(35, 12800, 38300);}
                    break;
                case 5:
                    if (this.NPC.RegionId != 16) {NPC.WarpFlash(16, 28300, 23800);}
                    break;
            } */
        }
        else        // If it is not x:00, there's a 0~6 chance to move
        {
            int chanceToMove = Convert.ToInt32(Random() % 6);
            if (chanceToMove == 1)
            {
                Position currentPos = this.NPC.GetPosition();
                int newX = currentPos.X;
                int newY = currentPos.Y;
                newX += Convert.ToInt32(Random(-300, 300) % 100);
                newY += Convert.ToInt32(Random(-300, 300) % 100);
                Position newPos = new Position(newX, newY);
                this.NPC.Move(newPos, true);
            }
        }
    }

    protected override async Task Talk()
	{

		await Intro(
			"At first it appears to be a knight in dark armor,",
			"but upon second glance, you see there's nobody inside.",
			"The armor seems to be moving on its own. A deep voice bellows out from within..."
		);

		Msg("Greetings, wayward Milletian...", Button("Start a Conversation", "@talk"), Button("Shop", "@shop"), Button("Repair Item", "@repair"));

		switch (await Select())
		{
			case "@talk":
				Greet();
				Msg(Hide.Name, GetMoodString(), FavorExpression());
				await Conversation();
				break;

			case "@shop":
                Msg("");
                OpenShop("WraithShop");
				/* if (this.NPC.RegionId == 1)
                {
                    Msg("(Th'Rok hopes you'll find the items it found on the plains of Tir satisfactory.)");
                    OpenShop("ThrokTirShop");
                }
                else if (this.NPC.RegionId == 14)
                {
                    Msg("(Th'Rok hopes you'll find the items it found on the streets of Dunbarton interesting.)");
                    OpenShop("ThrokDunbartonShop");
                }
                else if (this.NPC.RegionId == 30)
                {
                    Msg("(Th'Rok hopes you'll find the items it plundered in Reighinalt worth your time.)");
                    OpenShop("ThrokGairechShop");
                }
                else if (this.NPC.RegionId == 52)
                {
                    Msg("(Th'Rok hopes you'll find the items it found in the fountain of Emain Macha attractive.)");
                    OpenShop("ThrokEmainShop");
                }
                else if (this.NPC.RegionId == 35)
                {
                    Msg("(Th'Rok found these items from adventurers left behind. He hopes you can use them better.)");
                    OpenShop("ThrokTirAWShop");
                }
                else if (this.NPC.RegionId == 16)
                {
                    Msg("(Th'Rok hopes you'll find the items it found in the woods of Dugald Isle useful.)");
                    OpenShop("ThrokDugaldIsleShop");
                } */
				return;

			case "@repair":
				Msg("(Th'Rok can fix anything.)<br/><repair rate='95' stringid='(*/smith_repairable/*)|(*/cloth/*)|(*/glove/*)|(*/bracelet/*)|(*/shoes/*)|(*/headgear/*)|(*/robe/*)|(*/headband/*)|(*/misc_repairable/*)|(*/cashchair/*)|(*/magic_school_repairable/*)' />");

				while (true)
				{
					var repair = await Select();

					if (!repair.StartsWith("@repair"))
						break;

					var result = Repair(repair, 95, "/smith_repairable/", "/cloth/", "/glove/", "/bracelet/", "/shoes/", "/headgear/", "/robe/", "/headband/", "/misc_repairable/", "/cashchair/", "/magic_school_repairable/");
					if (!result.HadGold)
					{
						RndMsg(
                            "(Th'Rok is sad because you don't have enough gold for this.)",
                            "(Th'Rok wishes he could do it for less, but he needs the gold. For something.)",
                            "(Th'Rok refuses to work more until you bring more gold.)"
                        );
					}
					else if (result.Points == 1)
					{
						if (result.Fails == 0)
							RndMsg(
                                "(Th'Rok is excited to show you one point is fixed.)",
                                "(Th'Rok hands you the gear back, one point repaired.)",
                                "(Th'Rok has repaired the gear by one point.)"
                            );
						else
							Msg("(Th'Rok hands you the gear back glumly- it's been damaged in repair.)");
					}
					else if (result.Points > 1)
					{
						if (result.Fails == 0)
							Msg("(Th'Rok has fixed it perfectly.)");
						else
							// TODO: Use string format once we have XML dialogues.
							Msg("(Th'Rok made " + result.Fails + " mistake(s), but still repaired " + result.Successes + " point(s).)");
					}
				}

				Msg("(Th'Rok was glad to be of service to you.)");
				break;
		}

		End("(Th'Rok wishes you safe travels on your journey.)");
	}

    private void Greet()
    {
        if (Memory <= 0)
        {
            Msg(FavorExpression(), L("(Th'Rok looks at you and tries to figure out what you desire.)"));
        }
        else if (Memory == 1)
        {
            Msg(FavorExpression(), L("(Th'Rok looks at you and tries to figure out what you desire.)"));
        }
        else if (Memory == 2)
        {
            Msg(FavorExpression(), L("(Th'Rok looks puzzled and impressed to see you.)"));
        }
        else if (Memory <= 6)
        {
            Msg(FavorExpression(), L("(Th'Rok is glad to see you're doing well in these dangerous times.)"));
        }
        else
        {
            Msg(FavorExpression(), L("(Th'Rok remembers you, and conveys it knows your name... <username>.)"));
        }

        UpdateRelationAfterGreet();
    }

	protected override async Task Keywords(string keyword)
	{
		switch (keyword)
		{
			case "personal_info":
				Msg("People say this golem's name is Th'Rok.<br/>Nobody knows where it came from, but it can't disagree.");
				ModifyRelation(Random(2), 0, Random(2));
				break;

			case "rumor":
				if (this.NPC.RegionId == 1)
                {
                    Msg("(Th'Rok knows that this place is called Tir Chonaill.)<br/>(Many a Milletian have wandered in from this way..)");
                }
                else if (this.NPC.RegionId == 14)
                {
                    Msg("(Th'Rok knows that this place is called Dunbarton.)<br/>(Many a Milletian have wandered in from this way..)");
                }
                else if (this.NPC.RegionId == 30)
                {
                    Msg("(Th'Rok knows that this place is called Bangor.)<br/>(Technically, it is known as Gairech, but Th'Rok digresses.)");
                }
                else if (this.NPC.RegionId == 52)
                {
                    Msg("(Th'Rok knows that this place is called Emain Macha.)<br/>(Th'Rok notes it is empty of late..)");
                }
                else if (this.NPC.RegionId == 35)
                {
                    Msg("(Th'Rok knows that this place is called Another World.)<br/>(Th'Rok believes a goddess is being held here.)");
                }
                else if (this.NPC.RegionId == 16)
                {
                    Msg("(Th'Rok knows that this place is called Dugald Isle.)<br/>(His friends, the ents, hide in fear here.)");
                }
                ModifyRelation(Random(2), 0, Random(2));
				break;

			case "shop_misc":
				Msg("(Th'Rok has all kinds of things for sale.)");
				break;

			default:
				RndMsg(
					"(Th'Rok doesn't know anything about that.)",
					"(Th'Rok stares at you, clueless.)",
					"(Th'Rok has never heard of that before.)",
					"(Th'Rok wants to know who told you about this in the first place.)",
					"(Th'Rok is equally clueless.)"
				);
				ModifyRelation(0, 0, Random(2));
				break;
		}
	}
}

public class WraithShop : NpcShopScript
{
    public override void Setup()
    {
        RareItem.Add(new DropData(40440, 20f));    // Life Exploration Fishing Rod, 20
        RareItem.Add(new DropData(18605, 10f));    // Ladeca Helm, 10
        RareItem.Add(new DropData(40385, 5f));     // Ladeca Ice Wand, 5
        RareItem.Add(new DropData(41130, 5f));     // Ladeca's Light Short Sword, 5
        RareItem.Add(new DropData(41131, 5f));     // Ladeca's Light Short Sword, 5
        RareItem.Add(new DropData(13015, 1f));     // Brigandine, 1
        // RareItem.Add(new DropData(51041, 30f));    // Good Luck Potion, 30 (out until scripted)
        RareItem.Add(new DropData(51042, 40f));    // Poison Bottle, 40
        RareItem.Add(new DropData(62058, 10f));    // Ancient Magic Powder, 10
        RareItem.Add(new DropData(63025, 30f));    // Massive Holy Water of Lymilark, 30

        Random rng1 = new Random();
        Item pickedItem = Item.GetRandomDrop(rng1, RareItem);

        // Adds 0~2 Rare Items
        var rnd = RandomProvider.Get();
        var addCount = rnd.Next(0, 3);

        for (int i = 0; i < addCount; i++)
        {
            rng1 = new Random();
            pickedItem = Item.GetRandomDrop(rng1, RareItem);

            int pickID1 = pickedItem.Data.Id;

            switch(pickID1)      // here's where items get their pricing, enchanting, tweaking.
            {
                case 13015:
                {
                    var item = Item.CreateEnchanted(13015, 21202, 0);               // Marble -> Brigandine
                    var price = Convert.ToInt32(item.OptionInfo.Price * 3.5);       // Cost Multiplier: 3.5x (some freakish ~850k figure)
                    var stock = rnd.Next(1, 4);
                    Add("Rare Items", item, price, stock);                          // What does this do? Pulled from edern.cs
                    break;
                }
                /* case 00000:
                {
                    // do whatever
                    // CreateEnchantedItem
                    // CreateEnchant
                    // etc ...
                    // Add(...);
                } */
                default:        // else, just sell item at normal NPC price.
                {
                    Add("Rare Items", pickID1);
                    break;
                }

            }
        }

        TreasureItem.Add(new DropData(70155, 100f));    // Ordinary Key, 100
        TreasureItem.Add(new DropData(91038, 500f));    // Ordinary Chest, 500
        TreasureItem.Add(new DropData(70156, 10f));     // Premium Key, 10
        TreasureItem.Add(new DropData(91039, 100f));    // Premium Chest, 100
        TreasureItem.Add(new DropData(91208, 1f));      // Ancient Key, 1
        TreasureItem.Add(new DropData(91206, 10f));     // Ancient Chest, 10
        TreasureItem.Add(new DropData(91209, 20f));     // Ancient Chest Fragment, 20

        Random rng2 = new Random();
        Item pickedTreasure = Item.GetRandomDrop(rng2, TreasureItem);
        int pickID2 = pickedTreasure.Data.Id;
        Add("Treasures", pickID2); // RNG Treasure for this ErinnTime day

        Add("Dungeon Pass", 52004);
        Add("Dungeon Pass", 52005);
        Add("Dungeon Pass", 52006);
        Add("Dungeon Pass", 52007);

        Add("Event", 50532); // Broken Ice Piece
        Add("Event", 63063); // Glowing Frost Crystal
    }

    private List<DropData> RareItem = new List<DropData>();
    private List<DropData> TreasureItem = new List<DropData>();

    protected override void OnErinnMidnightTick(ErinnTime time)
    {
        // Run base (color randomization)
        base.OnErinnMidnightTick(time);

        //re-add shop update script here when finished
    }
}