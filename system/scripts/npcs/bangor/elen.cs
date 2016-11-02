//--- Aura Script -----------------------------------------------------------
// Elen
//--- Description -----------------------------------------------------------
// Blacksmith-in-training and Weapons Dealer
//---------------------------------------------------------------------------

public class ElenScript : NpcScript
{
	public override void Load()
	{
		SetRace(10001);
		SetName("_elen");
		SetBody(height: 0.6f, upper: 1.1f, lower: 1.1f);
		SetFace(skinColor: 25, eyeType: 3, eyeColor: 54, mouthType: 1);
		SetStand("human/female/anim/female_natural_stand_npc_elen");
		SetLocation(31, 11353, 12960, 15);
		SetGiftWeights(beauty: 1, individuality: 2, luxury: -1, toughness: 2, utility: 2, rarity: 0, meaning: -1, adult: 2, maniac: -1, anime: 2, sexy: 0);

		EquipItem(Pocket.Face, 3900, 0x0061696E, 0x00F30E67, 0x00008289);
		EquipItem(Pocket.Hair, 3005, 0x00FFE680, 0x00FFE680, 0x00FFE680);
		EquipItem(Pocket.Armor, 15029, 0x00FFFFFF, 0x00942370, 0x00EFE1C2);
		EquipItem(Pocket.Shoe, 17019, 0x002B6280, 0x0067676C, 0x00005DAA);
		EquipItem(Pocket.Head, 18024, 0x007D2224, 0x00FFFFFF, 0x000088CD);
		EquipItem(Pocket.RightHand1, 40024, 0x00FACB5F, 0x004F3C26, 0x00FAB052);

		AddPhrase("Lets see... I still have some left..");
		AddPhrase("Nothing is free!");
		AddPhrase("Grandpa worries too much.");
		AddPhrase("Come over here if you are interested in blacksmith work.");
		AddPhrase("Mom always neglects me...");
		AddPhrase("If my beauty mesmerizes you, at least have the guts to come and tell me so.");
		AddPhrase("The real fun is in creating, not repairing.");
		AddPhrase("I'm not too bad at blacksmith work myself, you know.");
		AddPhrase("How about some excitement in this town?");
		AddPhrase("Heh. That boy over there is kind of cute. I'd get along with him really well.");
		AddPhrase("It's rather slow today...");
	}

	protected override async Task Talk()
	{
		SetBgm("NPC_Elen.mp3");

		await Intro(L("Her lovely blonde hair, pushed back with a red and white headband to keep it out of her face, comes down to her waist in a wave and covers her entire back.<br/>Her small face with dark emerald eyes shines brightly and her full lips create an inquisitive look.<br/>The sleeveless shirt she is wearing due to the heat of the shop exposes her soft tanned skin, showing how healthy she is."));

		Msg("Mmm? Is there something you would like to say to me?", Button("Start Conversation", "@talk"), Button("Shop", "@shop"), Button("Repair Item", "@repair"), Button("Modify Item", "@upgrade"));

		switch (await Select())
		{
			case "@talk":
				Greet();
				Msg(Hide.Name, GetMoodString(), FavorExpression());

				if (Title == 11001)
				{
					Msg("Hmm...<br/>Don't you think your equipment is kind of shabby for someone with a title like yours?");
					Msg("You can find good merchandise in our Blacksmith's Shop, if you're interested. Why not check it out?");
				}
				else if (Title == 11002)
				{
					Msg("Oh my. You're the Guardian, right?<br/>...I'm not trying to tease you, I'm serious!");
				}

				await Conversation();
				break;

			case "@shop":
				Msg("Welcome to the best Blacksmith's Shop in Bangor.<br/>We deal with almost anything made of metal. Is there anything in particular you're looking for?");
				OpenShop("ElenShop");
				return;

			case "@repair":
				Msg(L("Is there something you want to repair?<br/>I'm far from being as good as my grandpa,<br/>but I am a blacksmith myself, so I'll do my best to live up to the title.<repair rate='90' stringid='(*/smith_repairable/*)' />"));

				while (true)
				{
					var repair = await Select();

					if (!repair.StartsWith("@repair"))
						break;

					var result = Repair(repair, 90, "/smith_repairable/");
					if (!result.HadGold)
					{
						RndMsg(
							L("Do you have the Gold for it?<br/>I can't do it for free with my grandpa watching."),
							L("I don't know...<br/>Do you have the Gold for it?"),
							L("Do you have the repair fee?")
						);
					}
					else if (result.Points == 1)
					{
						if (result.Fails == 0)
							RndMsg(
								L("Repair is finished for 1 point."),
								L("1 point has been successfully repaired."),
								L("1 Durability point has been repaired.")
							);
						else
							RndMsg(
								L("I'm sorry. I made a mistake."),
								L("Hmm... I did everything as I was taught...<br/>I'm sorry. It's not repaired.")
							);
					}
					else if (result.Points > 1)
					{
						if (result.Fails == 0)
							RndMsg(
								L("It's been completely repaired.<br/>Perhaps this would impress my grandpa."),
								L("The repair is perfect."),
								L("The repair work is completely done!")
							);
						else
							Msg(string.Format(L("The repair job is finished, but...<br/>I made {0} mistake(s).<br/>So I only ended up repairing {1} point(s). I'm sorry."), result.Fails, result.Successes));
					}
				}

				Msg(L("If you don't trust me, talk to grandpa.<br/>He's the best blacksmith in town.<repair hide='true'/>"));
				break;

			case "@upgrade":
				Msg(L("Mmm? You are asking me for an item modification?<br/>Ha ha. If you are, <username/>,<br/>I'll do it just for you!<br/>You know that armor can't be worn by anyone else once it's modified, right?<upgrade />"));

				while (true)
				{
					var reply = await Select();

					if (!reply.StartsWith("@upgrade:"))
						break;

					var result = Upgrade(reply);
					if (result.Success)
						Msg(L("Ta-da! The modification was successful! How about that? Are you impressed? Cool!<br/>Anything else you want to modify?"));
					else
						Msg(L("(Error)"));
				}

				Msg(L("Then, can I get back to my other tasks?<br/>Just let me know if you have something else to modify.<br/><upgrade hide='true'/>"));
				break;
		}

		End("Thank you, <npcname/>. I'll see you later!");
	}

	private void Greet()
	{
		if (DoingPtjForNpc())
		{
			Msg(FavorExpression(), L("What is it?<br/>Are you doing the work at the shop?"));
		}
		else if (Memory <= 0)
		{
			Msg(FavorExpression(), L("Welcome! But... I've never seen you around here before."));
		}
		else if (Memory == 1)
		{
			Msg(FavorExpression(), L("You must be quite interested in the blacksmith work."));
		}
		else if (Memory == 2)
		{
			Msg(FavorExpression(), L("I think we've met, but I can't really... Well, doesn't matter. Welcome!"));
		}
		else if (Memory <= 6)
		{
			Msg(FavorExpression(), L("Hi, <username/>. What brings you here?"));
		}
		else
		{
			Msg(FavorExpression(), L("<username/>? I've been waiting for you. Welcome back."));
		}

		UpdateRelationAfterGreet();
	}

	protected override async Task Keywords(string keyword)
	{
		switch (keyword)
		{
			case "personal_info":
				Msg(FavorExpression(), "I'm <npcname/>. And you are... <username/>?<br/>I see. Nice to meet you.");
				ModifyRelation(Random(2), 0, Random(3));
				break;

			case "rumor":
				Msg(FavorExpression(), "That man over there is my grandpa.<br/>He's the best blacksmith in town.");
				Msg("I was told that other blacksmiths in the neighboring towns<br/>all learned their trade under my grandpa.<br/>Heh. Cool, huh?");
				ModifyRelation(Random(2), 0, Random(3));
				break;

			case "about_skill":
				// Learning Refining is handled in the quest script
				Msg("How's refining coming along?<br/>There are lots of furnaces around,<br/>so go talk to Sion over there and he'll teach you how to use one.");
				break;

			case "shop_misc":
				Msg("Oh, you mean Gilmore's General Shop?<br/>It's just around here. Turn over here and you'll see it.");
				Msg("By the way...");
				Msg("Gilmore is really mean.<br/>So just get what you need and get out of there, OK?");
				break;

			case "shop_grocery":
				Msg("I can't say that it's very bright of you to look for food at the Blacksmith's Shop,<br/>but just for you, <username/>, I'll tell you. Heehee...");
				Msg("See that Pub over there?<br/>Go there and talk to Jennifer.<br/>She probably sells food, too.");
				break;

			case "shop_healing":
				Msg("Hmm... We don't really have a Healer's House in this town.<br/>But, if it's potions or bandages you're looking for,<br/>Comgan probably has some, so why don't you ask him?");
				break;

			case "shop_bank":
				Msg("We don't really have a bank building,<br/>but if you're looking to store items,<br/>go talk to Bryce over there in front of the storage shed?");
				break;

			case "shop_smith":
				Msg("Hmm...");
				Msg("Where do you think<br/>you're standing right now?");
				break;

			case "skill_rest":
				Msg("It's a useful skill to have if learned thoroughly.");
				Msg("Then again, what skill isn't?");
				break;

			case "skill_range":
				Msg("To say that you must have an instrument<br/>in order to play music may be<br/>a biased opinion of people.");
				Msg("The sound of the hammer in the Blacksmith's Shop<br/>is music to our ears, you know.");
				break;

			case "skill_instrument":
				Msg("If you're going to practice, please go to the dungeon.<br/>The work of a blacksmith has a flow and you're disrupting it.");
				break;

			case "skill_tailoring":
				Msg("It's not a bad skill to have I suppose, but...");
				Msg("It's something anyone could do with a Tailoring Kit, you know?");
				break;

			case "skill_magnum_shot":
				GiveKeyword("bow");
				Msg("Did you know?<br/>Magnum Shot is powerful,<br/>but it becomes even more powerful if you use a better bow.");
				Msg("If you want to see for yourself, try it with a few bows.<br/>It would help our business too... What do you say?");
				break;

			case "skill_counter_attack":
				Msg("Did you know?<br/>Melee Counterattack is powerful,<br/>but it becomes more powerful when used it with a weapon,<br/>even if it's a dagger or a short sword.");
				Msg("If you want to see for yourself, try it with a few weapons.<br/>It would help our business too... What do you say?");
				break;

			case "skill_smash":
				Msg("Did you know?<br/>Smash is powerful,<br/>but it becomes more powerful when used with a weapon,<br/>even if it's a dagger or a short sword.");
				Msg("If you want to see for yourself, try it with a few weapons.<br/>It would help our business too... What do you say?");
				break;

			case "skill_gathering":
				Msg("See that building over there with the Watermill?<br/>You can go to Barri Dungeon from that entrance.");
				Msg("I'm sure my grandpa would appreciate it<br/>if you brought him some ore from the dungeon. Tee hee.<br/>After all, grandpa values propriety... Hee hee hee.");
				break;

			case "square":
				Msg("...?");
				break;

			case "pool":
				Msg("Did someone tell you that there is a reservoir here?<br/>Who?");
				break;

			case "farmland":
				Msg("Hehe... You don't get it.<br/>This town is a mining town.<br/>There no need to farm here.");
				break;

			case "brook":
				Msg("What are you talking about?");
				break;

			case "shop_headman":
				Msg("Well, actually, my grandpa would make a great chief...<br/>What do you think?");
				break;

			case "temple":
				Msg("There is no church in this town.");
				Msg("I wonder where Comgan sleeps.");
				break;

			case "school":
				Msg("I never really felt the need for school in this town.<br/>After all, learning things through hands-on experience is the best.");
				Msg("Do you go to school?");
				break;

			case "skill_campfire":
				Msg("There's plenty of fire in the furnace over there.<br/>Why would you want more fire?");
				break;

			case "shop_restaurant":
				Msg("If you need food, go talk to<br/>Jennifer at the Pub.");
				break;

			case "shop_armory":
				Msg("In this town, weapons are directly<br/>built at the Blacksmith's Shop.");
				Msg("What good would it do to have a wholesale shop<br/>and a retail shop separately when the town produces it?");
				Msg("Finish the conversation and click 'Shop'.");
				break;

			case "shop_cloth":
				Msg("I think I might have seen it being sold at the General Shop.");
				Msg("Well, you know.<br/>Gilmore doesn't really seem to know what's fashionable...");
				break;

			case "shop_bookstore":
				GiveKeyword("shop_misc");
				Msg("Looking for books?<br/>I don't know... At the General Shop, perhaps?");
				break;

			case "shop_goverment_office":
				Msg("Hmm... I think we can do without it.");
				Msg("We are doing well as is, you know.");
				break;

			case "graveyard":
				Msg("Well, I don't know. I'm not too interested in such things...");
				break;

			case "bow":
				Msg("We happen to sell bows here.<br/>Why don't you press the 'Shop' button and see what I have?");
				break;

			case "lute":
				Msg("After all, you can keep rhythm with a hammer,<br/>but you can't hammer metal with a Lute.");
				break;

			case "complicity":
				Msg("Are you... Looking at me in a...");
				break;

			case "tir_na_nog":
				Msg("You mean the land of youth?<br/>I've heard it from Comgan before.");
				Msg("That kid believes so firmly in such nonsense.<br/>How about you? Don't tell me you believe it too...");
				break;

			case "mabinogi":
				Msg("My dad used to tell me the story when I was young.<br/>I don't remember it very well now, though...");
				Msg("After all, practical stories<br/>are better than any fairy tales, you know?");
				Msg("How to make metal stronger,<br/>how to clean up the cut edge of metal... Those kinds of things.");
				break;

			case "musicsheet":
				Msg("Hehe... I don't really know about such things.");
				break;

			default:
				RndFavorMsg(
					"I don't really know.",
					"Why don't you ask someone else?",
					"Hmm... Do you really have to know?",
					"I don't know anything about that.",
					"Hmm? Can you run that by me again?",
					"I don't know. Never heard anything about it.",
					"Just bringing up a topic like that out of the blue doesn't help."
				);
				ModifyRelation(0, 0, Random(3));
				break;
		}
	}
}

public class ElenShop : NpcShopScript
{
	public override void Setup()
	{
		Add("Weapon", 45001, 20);  // 20 Arrows
		Add("Weapon", 45001, 100); // 100 Arrows
		Add("Weapon", 40023);      // Gathering Knife
		Add("Weapon", 40022);      // Gathering Axe
		Add("Weapon", 45002, 50);  // 50 bolts
		Add("Weapon", 45002, 200); // 100 bolts
		Add("Weapon", 40027);      // Weeding Hole
		Add("Weapon", 40003);      // Short Bow
		Add("Weapon", 40026);      // Sickle
		Add("Weapon", 40006);      // Dagger
		Add("Weapon", 40243);      // Battle Short Sword
		Add("Weapon", 40015);      // Fluted Short Sword
		Add("Weapon", 40025);      // Pickaxe
		Add("Weapon", 40005);      // Short Sword
		Add("Weapon", 40007);      // Hatchet
		Add("Weapon", 40014);      // Composite Bow
		Add("Weapon", 40024);      // Black Smith Hammer
		Add("Weapon", 40013);      // Long Bow
		Add("Weapon", 40011);      // Broad Sword
		Add("Weapon", 40010);      // longsword
		Add("Weapon", 40016);      // War Hammer
		Add("Weapon", 40012);      // Bastard Sword
		Add("Weapon", 40242);      // Battle Sword
		Add("Weapon", 46001);      // Round Shield
		Add("Weapon", 40030);      // Two Handed Sword
		Add("Weapon", 40033);      // Claymore
		Add("Weapon", 46006);      // Kite Shield

		Add("Shoes & Gloves", 16004); // Studded Bracelet
		Add("Shoes & Gloves", 16008); // Core's Thief Gloves
		Add("Shoes & Gloves", 16000); // Leather Gloves
		Add("Shoes & Gloves", 17021); // Lorica Sandals
		Add("Shoes & Gloves", 17014); // Leather Shoes
		Add("Shoes & Gloves", 16009); // Tork's Hunter Gloves
		Add("Shoes & Gloves", 17001); // Ladies Leather Boots
		Add("Shoes & Gloves", 17005); // Hunter Boots
		Add("Shoes & Gloves", 17015); // Combat Shoes
		Add("Shoes & Gloves", 17016); // Field Combat Shoes
		Add("Shoes & Gloves", 17020); // Thief Shoes
		Add("Shoes & Gloves", 16005); // Wood Plate Cannon
		Add("Shoes & Gloves", 16014); // Lorica Gloves
		Add("Shoes & Gloves", 16501); // Leather Protector
		Add("Shoes & Gloves", 16017); // standard gloves
		Add("Shoes & Gloves", 16007); // Cores Ninja Gloves
		Add("Shoes & Gloves", 17506); // Long Greaves
		Add("Shoes & Gloves", 16500); // Ulna Protector Gloves
		Add("Shoes & Gloves", 17501); // Solleret Shoes	
		Add("Shoes & Gloves", 17500); // High Polean Plate Boots
		Add("Shoes & Gloves", 16504); // Counter Gauntlet
		Add("Shoes & Gloves", 16505); // Fluted Gauntlet
		Add("Shoes & Gloves", 17505); // Plate Boots

		Add("Helmet", 18513); // Spiked Cap
		Add("Helmet", 18503); // Cuirassier Helm
		Add("Helmet", 18500); // Ring Mail Helm
		Add("Helmet", 18504); // Cross Full Helm
		Add("Helmet", 18502); // Bone Helm
		Add("Helmet", 18501); // Guardian Helm
		Add("Helmet", 18505); // Spiked Helm
		Add("Helmet", 18506); // Wing Half Helm 
		Add("Helmet", 18508); // Slit Full Helm
		Add("Helmet", 18509); // Bascinet

		Add("Armor", 14006); // Linen Cuirass (F)
		Add("Armor", 14009); // Linen Cuirass (M)
		Add("Armor", 14001); // Light Leather Armor (F)
		Add("Armor", 14010); // Light Leather Armor (M)
		Add("Armor", 14004); // Cloth Mail
		Add("Armor", 14008); // Full Leather Set
		Add("Armor", 14003); // Studded Cuirassier
		Add("Armor", 14007); // Padded Armor with breastplate
		Add("Armor", 14013); // Lorica Segmentata
		Add("Armor", 14005); // Drandos Leather Mail (F)
		Add("Armor", 14011); // Drandos Leather Mail (M)
		Add("Armor", 13017); // Surcoat Chain Mail
		Add("Armor", 13001); // Melka Chain Mail
		Add("Armor", 13010); // Round Pauldron Chainmail
		Add("Armor", 13022); // Rose Plate Armor (Type P)
		Add("Armor", 13053); // Spika Silver Plate Armor (Giants)
		Add("Armor", 13031); // Spika Silver Plate Armor

		Add("Event"); // Empty

		if (IsEnabled("FighterJob"))
		{
			Add("Weapon", 40179); // Spiked Knuckle
			Add("Weapon", 40180); // Hobnail Knuckle
			Add("Weapon", 40244); // Bear Knuckle
		}

		if (IsEnabled("FineArrows"))
		{
			Add("Arrowhead", 64011); // Bundle of Arrowheads
			Add("Arrowhead", 64015); // Bundle of Boltheads
			Add("Arrowhead", 64013); // Bundle of Fine Arrowheads
			Add("Arrowhead", 64016); // Bundle of Fine Boltheads
			Add("Arrowhead", 64014); // Bundle of Finest Arrowheads
			Add("Arrowhead", 64017); // Bundle of Finest Boltheads
		}
	}
}
