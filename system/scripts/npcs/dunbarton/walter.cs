//--- Aura Script -----------------------------------------------------------
// Walter
//--- Description -----------------------------------------------------------
// General Shop
//---------------------------------------------------------------------------

public class WalterScript : NpcScript
{
	public override void Load()
	{
		SetRace(10002);
		SetName("_walter");
		SetBody(height: 1.1f, weight: 1.2f, lower: 1.2f);
		SetFace(skinColor: 22, eyeType: 13, eyeColor: 27);
		SetLocation(14, 35770, 39528, 252);
		SetGiftWeights(beauty: 0, individuality: 1, luxury: -1, toughness: 2, utility: 2, rarity: -1, meaning: 0, adult: 2, maniac: 0, anime: 0, sexy: 0);

		EquipItem(Pocket.Face, 4903, 0x000D97D0, 0x000079A4, 0x00001641);
		EquipItem(Pocket.Hair, 4027, 0x00554433, 0x00554433, 0x00554433);
		EquipItem(Pocket.Armor, 15044, 0x00665033, 0x00DDDDDD, 0x00D5DBE4);
		EquipItem(Pocket.Shoe, 17009, 0x009D7012, 0x00D3E3F4, 0x00EEA23D);

		AddPhrase("Ahem!");
		AddPhrase("Ahem... Ow...my throat...");
		AddPhrase("Hello there!");
		AddPhrase("Hmm...");
		AddPhrase("Is there any specific item you're looking for?");
		AddPhrase("Please don't touch that.");
		AddPhrase("That one is 20 Gold.");
		AddPhrase("That's 30 Gold for four.");
		AddPhrase("That's 50 Gold for three.");
		AddPhrase("What are you looking for?");
		AddPhrase("What do you need?");
	}

	protected override async Task Talk()
	{
		SetBgm("NPC_Walter.mp3");

		await Intro(L("A middle-aged man with a dark complexion and average height, <npcname/> is wearing suspenders and stroking his stubby fingers.<br/>Under his dark-brown eyes, his tightly sealed lips are covered by a thick moustache.<br/>You can see his moustache and his Adam's apple slightly move as if he is about to say something."));

		Msg("Um? What do you want?", Button("Start a Conversation", "@talk"), Button("Shop", "@shop"), Button("Repair Item", "@repair"), Button("Modify Item", "@upgrade"));

		switch (await Select())
		{
			case "@talk":
				Greet();
				Msg(Hide.Name, GetMoodString(), FavorExpression());

				if (Title == 11001)
				{
					Msg("...");
					Msg("...");
					Msg("What do you think about my daughter...?");
					Msg("...I didn't mean to give you the book so late... I apologize.");
				}
				if (Title == 11002)
				{
					Msg("...I sense you are an amazing person.");
				}

				await Conversation();
				break;

			case "@shop":
				Msg("What are you looking for?");
				OpenShop("WalterShop");
				return;

			case "@repair":
				Msg("Repair? What is it that you want to repair? Let's have a look.<br/>I can take care of general goods like instruments, glasses, and tools.<br/>My skills are not what they used to be, so I won't charge you a lot...<repair rate='92' stringid='(*/misc_repairable/*)|(*/cashchair/*)' />");

				while (true)
				{
					var repair = await Select();

					if (!repair.StartsWith("@repair"))
						break;

					var result = Repair(repair, 92, "/misc_repairable/", "/cashchair/");
					if (!result.HadGold)
					{
						RndMsg(
							"You got the money?",
							"You don't even have money and you're asking me to fix this?",
							"If you want to repair that, bring me the money first."
						);
					}
					else if (result.Points == 1)
					{
						if (result.Fails == 0)
							RndMsg(
								"1 point has been repaired.",
								"It's been repaired as you wanted.",
								"It's done, 1 point."
							);
						else
							Msg("Hmm... I've made some mistakes."); // Should be 2
					}
					else if (result.Points > 1)
					{
						if (result.Fails == 0)
							RndMsg(
								"Perfectly repaired as you wanted.",
								"It's done."
							);
						else
							// TODO: Use string format once we have XML dialogues.
							Msg("There have been some mistakes, just " + result.Fails + " point(s)...<br/>Anyway, it's done.");
					}
				}

				Msg("If you're not careful with it, it will break easily.<br/>So take good care of it.<repair hide='true'/>");
				break;

			case "@upgrade":
				Msg("...<br/>Give me what you want to modify.<br/>I'm sure you have checked the number and type of the modification you want?<upgrade />");

				while (true)
				{
					var reply = await Select();

					if (!reply.StartsWith("@upgrade:"))
						break;

					var result = Upgrade(reply);
					if (result.Success)
						Msg("Well... It's done.<br/>...More?");
					else
						Msg("(Error)");
				}

				Msg("This is it? Well, then...<upgrade hide='true'/>");
				break;
		}

		End("Thank you, <npcname/>. I'll see you later!");
	}

	private void Greet()
	{
		if (DoingPtjForNpc())
		{
			Msg(FavorExpression(), L("Are you a part-timer?"));
		}
		else if (Memory <= 0)
		{
			Msg(FavorExpression(), L("...What do you want?"));
		}
		else if (Memory == 1)
		{
			Msg(FavorExpression(), L("...Welcome."));
		}
		else if (Memory == 2)
		{
			Msg(FavorExpression(), L("I see you here often..."));
		}
		else if (Memory <= 6)
		{
			Msg(FavorExpression(), L("...<username/>, right? Anyway, welcome..."));
		}
		else
		{
			Msg(FavorExpression(), L("<username/>, how are you doing?"));
		}

		UpdateRelationAfterGreet();
	}

	protected override async Task Keywords(string keyword)
	{
		switch (keyword)
		{
			case "personal_info":
				if (Memory == 1)
				{
					Msg(FavorExpression(), "You a traveler...? I'm <npcname/>.");
					ModifyRelation(1, 0, 0);
				}
				else
				{
					Msg(FavorExpression(), "I'm the owner of this General Shop. Take a look around.");
					ModifyRelation(Random(2), 0, Random(3));
				}
				break;

			case "rumor":
				GiveKeyword("shop_armory");
				Msg(FavorExpression(), "If you need something, you're at the right place.<br/>But you'll have to go down to Nerys' Shop for weapons.<br/>The Weapons Shop.");
				ModifyRelation(Random(2), 0, Random(3));

				// Ongoing Field Boss Msg
				// There is news about a Goblin Bandits assault at North Plains of Dunbarton.
				break;

			case "shop_misc":
				Msg("Right over here.");
				break;

			case "shop_grocery":
				GiveKeyword("shop_restaurant");
				Msg("Are you talking about the Restaurant?");
				break;

			case "shop_healing":
				Msg("Ask that person over there.<br/>If you don't want to ask, just look at the Minimap.");
				break;

			case "shop_inn":
				Msg("There's no such thing here.");
				break;

			case "shop_bank":
				Msg("Go south and it's the second building.");
				break;

			case "shop_smith":
				GiveKeyword("shop_armory");
				Msg("There's no such place here.<br/>Maybe in the Weapons Shop...");
				break;

			case "skill_tailoring":
				Msg("Somebody in this town should know but...<br/>I can't guarantee they will tell you.");
				break;

			case "skill_magnum_shot":
				Msg("Shooting really strong arrows, maybe...");
				break;

			case "skill_smash":
				Msg("Are you some kind of a thug...?");
				break;

			case "square":
				Msg("Isn't it right in front of us?");
				break;

			case "farmland":
				Msg("Over the wall is mostly farmlands.");
				break;

			case "shop_headman":
				Msg("A chief? What chief?");
				break;

			case "temple":
				Msg("Priestess Kristell?<br/>Just go straight up this alley,<br/>and you'll see her.");
				break;

			case "school":
				Msg("He's over there.<br/>Say hello to Stewart for me.");
				break;

			case "skill_windmill":
				Msg("Who told you that<br/>you need skills to run the windmill?");
				break;

			case "skill_campfire":
				Msg("Ask someone who lives in the field.<br/>Tracy would know.");
				break;

			case "shop_restaurant":
				Msg("Just follow this alley, and it's right there.");
				break;

			case "shop_armory":
				GiveKeyword("shop_healing");
				Msg("Go near the Healer's House.");
				break;

			case "shop_cloth":
				Msg("It's right beside you.<br/>...You think that's funny?");
				break;

			case "shop_bookstore":
				Msg("What's your relationship with my daughter!?");
				break;

			case "shop_goverment_office":
				Msg("Go straight to the opposite side.");
				break;

			case "bow":
				Msg("Ask at the Weapons Shop.");
				break;

			case "lute":
				Msg("I happen to have it.<br/>Do you want one?");
				break;

			case "musicsheet":
				Msg("I have a few.<br/>Press 'Trade'.");
				break;

			default:
				RndFavorMsg(
					"I'm not sure.",
					"Ask someone else.",
					"I'm not interested.",
					"I don't really care about that.",
					"You're really bothering me. That's quite enough. Enough already."
				);
				ModifyRelation(0, 0, Random(3));
				break;
		}
	}
}

public class WalterShop : NpcShopScript
{
	public override void Setup()
	{
		Add("General Goods", 2001);       // Gold Pouch
		Add("General Goods", 2005);       // Item Bag (7x5)
		Add("General Goods", 2006);       // Big Gold Pouch
		Add("General Goods", 2024);       // Item Bag (7x6)
		Add("General Goods", 2026);       // Item Bag (44)
		Add("General Goods", 2029);       // Item Bag (8x6)
		Add("General Goods", 2038);       // Item Bag (8X10)
		Add("General Goods", 18028);      // Folding Glasses
		Add("General Goods", 18158);      // Conky Glasses
		Add("General Goods", 40004);      // Lute
		Add("General Goods", 40004);      // Lute
		Add("General Goods", 40004);      // Lute
		Add("General Goods", 40017);      // Mandolin
		Add("General Goods", 40017);      // Mandolin
		Add("General Goods", 40017);      // Mandolin
		Add("General Goods", 40215);      // Small Drum
		Add("General Goods", 60045);      // Handicraft Kit
		Add("General Goods", 61001);      // Score Scroll
		Add("General Goods", 61001);      // Score Scroll
		Add("General Goods", 61001);      // Score Scroll
		Add("General Goods", 61001);      // Score Scroll
		Add("General Goods", 62021, 100); // Six-sided Die x100
		Add("General Goods", 63020);      // Empty Bottle
		Add("General Goods", 64018, 10);  // Paper x10
		Add("General Goods", 64018, 100); // Paper x100

		Add("Tailoring", 60001);    // Tailoring Kit
		Add("Tailoring", 60015, 1); // Cheap Finishing Thread x1
		Add("Tailoring", 60015, 5); // Cheap Finishing Thread x5
		Add("Tailoring", 60016, 1); // Common Finishing Thread x1
		Add("Tailoring", 60016, 5); // Common Finishing Thread x5
		Add("Tailoring", 60017, 1); // Fine Finishing Thread x1
		Add("Tailoring", 60017, 5); // Fine Finishing Thread x5
		Add("Tailoring", 60018, 1); // Finest Finishing Thread x1
		Add("Tailoring", 60018, 5); // Finest Finishing Thread x5
		Add("Tailoring", 60019, 1); // Cheap Fabric Pouch x1
		Add("Tailoring", 60019, 5); // Cheap Fabric Pouch x5
		Add("Tailoring", 60020, 1); // Common Fabric Pouch x1
		Add("Tailoring", 60020, 5); // Common Fabric Pouch x5
		Add("Tailoring", 60031);    // Regular Silk Weaving Gloves
		Add("Tailoring", 60046);    // Finest Silk Weaving Gloves
		Add("Tailoring", 60055);    // Fine Silk Weaving Gloves
		Add("Tailoring", 60056);    // Finest Fabric Weaving Gloves
		Add("Tailoring", 60057);    // Fine Fabric Weaving Gloves

		Add("Sewing Patterns", 60000, "FORMID:4:111;", 4600);   // Sewing Pattern - Guardian Gloves
		Add("Sewing Patterns", 60000, "FORMID:4:112;", 200);    // Sewing Pattern - Wizard Hat
		Add("Sewing Patterns", 60000, "FORMID:4:113;", 1250);   // Sewing Pattern - Leather Bandana
		Add("Sewing Patterns", 60000, "FORMID:4:114;", 400);    // Sewing Pattern - Hairband
		Add("Sewing Patterns", 60000, "FORMID:4:115;", 1600);   // Sewing Pattern - Mongo's Long Skirt (F)
		Add("Sewing Patterns", 60000, "FORMID:4:116;", 16000);  // Sewing Pattern - Light Leather Mail (F)
		Add("Sewing Patterns", 60000, "FORMID:4:117;", 5500);   // Sewing Pattern - Lirina's Long Skirt (F)
		Add("Sewing Patterns", 60000, "FORMID:4:118;", 3400);   // Sewing Pattern - Cores' Ninja Suit (M)
		Add("Sewing Patterns", 60000, "FORMID:4:119;", 112500); // Sewing Pattern - Cores' Thief Suit (M)
		Add("Sewing Patterns", 60000, "FORMID:4:120;", 16000);  // Sewing Pattern - Light Leather Mail (M)
		Add("Sewing Patterns", 60000, "FORMID:4:121;", 3000);   // Sewing Pattern - Common Silk Weaving Gloves
		Add("Sewing Patterns", 60000, "FORMID:4:122;", 5000);   // Sewing Pattern - Professional Silk Weaving Gloves
		Add("Sewing Patterns", 60000, "FORMID:4:123;", 125000); // Sewing Pattern - Ring-Type Mini Leather Dress (F)
		Add("Sewing Patterns", 60000, "FORMID:4:126;", 77000);  // Sewing Pattern - Lueys' Vest Wear
		Add("Sewing Patterns", 60000, "FORMID:4:127;", 55000);  // Sewing Pattern - Broad-brimmed Feather Hat
		Add("Sewing Patterns", 60000, "FORMID:4:128;", 11000);  // Sewing Pattern - Tork's Little-brimmed Hat
		Add("Sewing Patterns", 60044, "FORMID:4:132;", 4000);   // Sewing Pattern - Cores' Thief Gloves
		Add("Sewing Patterns", 60044, "FORMID:4:171;", 172000); // Sewing Pattern - High-class Leather Armor
		Add("Sewing Patterns", 60044, "FORMID:4:172;", 86400);  // Sewing Pattern - Middle-class Leather Armor
		Add("Sewing Patterns", 60044, "FORMID:4:173;", 32500);  // Sewing Pattern - Basic Leather Armor
		Add("Sewing Patterns", 60044, "FORMID:4:182;", 72200);  // Sewing Pattern - Terra Diamond-shaped Leather Boots
		Add("Sewing Patterns", 60044, "FORMID:4:185;", 121000); // Sewing Pattern - Spark Leather Armor

		Add("Gift", 52008); // Anthology
		Add("Gift", 52009); // Cubic Puzzle
		Add("Gift", 52011); // Socks
		Add("Gift", 52017); // Underwear Set
		Add("Gift", 52018); // Hammer

		Add("Cooking Appliances", 40042); // Cooking Knife
		Add("Cooking Appliances", 40043); // Rolling Pin
		Add("Cooking Appliances", 40044); // Ladle
		Add("Cooking Appliances", 46004); // Cooking Pot
		Add("Cooking Appliances", 46005); // Cooking Table

		Add("Event"); // Empty

		if (IsEnabled("PetBirds"))
			Add("General Goods", 40093); // Pet Instructor Stick

		if (IsEnabled("AntiMacroCaptcha"))
		{
			Add("General Goods", 51227, 1);  // Ticking Quiz Bomb x1
			Add("General Goods", 51227, 20); // Ticking Quiz Bomb x20
		}

		if (IsEnabled("Kiosk"))
			Add("General Goods", 2037); // Kiosk

		if (IsEnabled("ItemSeal2"))
		{
			Add("General Goods", 91364, 1);  // Seal Scroll (1-day) x1
			Add("General Goods", 91364, 10); // Seal Scroll (1-day) x10
			Add("General Goods", 91365, 1);  // Seal Scroll (7-day) x1
			Add("General Goods", 91365, 10); // Seal Scroll (7-day) x10
			Add("General Goods", 91366, 1);  // Seal Scroll (30-day) x1
			Add("General Goods", 91366, 10); // Seal Scroll (30-day) x10
		}

		if (IsEnabled("Singing"))
		{
			Add("General Goods", 41124); // Standing Microphone
			Add("General Goods", 41125); // Wireless Microphone
		}

		if (IsEnabled("PropInstruments"))
			Add("General Goods", 41123); // Cello

		if (IsEnabled("Reforges"))
			Add("General Goods", 85571); // Reforging Tool
	}
}