//--- Aura Script -----------------------------------------------------------
// Eavan
//--- Description -----------------------------------------------------------
// Public Servant
//---------------------------------------------------------------------------

public class EavanScript : NpcScript
{
	public override void Load()
	{
		SetRace(10001);
		SetName("_eavan");
		SetBody(weight: 0.7f, upper: 0.7f, lower: 0.7f);
		SetFace(skinColor: 15, eyeType: 3, eyeColor: 3);
		SetStand("human/female/anim/female_natural_stand_npc_Eavan");
		SetLocation(14, 40024, 41041, 192);
		SetGiftWeights(beauty: 2, individuality: 2, luxury: 2, toughness: -1, utility: 0, rarity: 1, meaning: 0, adult: 2, maniac: -1, anime: -1, sexy: 2);

		EquipItem(Pocket.Face, 3900, 0x003B9C3F, 0x000896D4, 0x0093A6D4);
		EquipItem(Pocket.Hair, 3022, 0x00FFEEAA, 0x00FFEEAA, 0x00FFEEAA);
		EquipItem(Pocket.Armor, 15041, 0x00FFCCCC, 0x0080C5D3, 0x00A7ACB4);
		EquipItem(Pocket.Glove, 16015, 0x00FFFFFF, 0x00E6F2E2, 0x006161AC);
		EquipItem(Pocket.Shoe, 17008, 0x00DDAACC, 0x00F79B2F, 0x00E10175);

		AddPhrase("*Sigh* Back to work.");
		AddPhrase("Hmm. This letter is fairly well done. B+.");
		AddPhrase("Next person please!");
		AddPhrase("Next, please!");
		AddPhrase("Registration is this way!");
		AddPhrase("Teehee... Another love letter.");
		AddPhrase("The Adventurers' Association is this way!");
		AddPhrase("Ugh. I wish I could take a breather...");
		AddPhrase("What's with this letter? How unpleasant!");
		AddPhrase("Whew. I want to take a break...");
	}

	protected override async Task Talk()
	{
		SetBgm("NPC_Eavan.mp3");

		await Intro(L("Wearing a rosy pink blouse, her shoulders are gently covered by her blonde hair that seems to wave in the breeze.<br/>An oval face, a pair of calm eyes with depth, and a slightly small nose with a rounded tip...<br/>Beneath are the lips that shine in the same color as her blouse."));

		var buttons = Elements(Button("Start a Conversation", "@talk"), Button("Shop", "@shop"), Button("Retrieve Lost Items", "@lostandfound"));

		//buttons.Add(Button(L("About Daily Events"), "@daily_quest"));
		//buttons.Add(Button(L("Daily Dungeon Quest"), "@daily_dungeon_quest"));
		if (IsEnabled("GuildRobe") && Player.Guild != null && Player.Guild.LeaderName == Player.Name)
			buttons.Add(Button(L("Make a Guild Robe"), "@make_guildrobe"));
		//buttons.Add(Button(L("Join Guild Battle"), "@guildswar"));

		Msg("This is the Adventurers' Association.", buttons);

		switch (await Select())
		{
			case "@talk":
				Player.GiveKeyword("shop_goverment_office");
				Greet();
				Msg(Hide.Name, GetMoodString(), FavorExpression());

				if (Player.IsUsingTitle(11001))
					Msg("I imagine rescuing the Goddess was extremely difficult...<br/>It's because of you, <username/>,<br/>that Erinn is such a peaceful place now.");
				else if (Player.IsUsingTitle(11002))
					Msg("The Guardian of Erinn, <username/>...<br/>You are always welcomed here.");

				await Conversation();
				break;

			case "@shop":
				Msg("Welcome to the Adventurers' Association.");
				OpenShop("EavanShop");
				return;

			case "@lostandfound":
				Msg("At the Town Office, you can retrieve the items you've lost during your adventure.<br/>Unless you dropped an item on purpose while using Magical powers,<br/>you can usually retrieve it here with the blessing still cast on it.");
				Msg("You have to pay a small fee though, and only up to 20 items are stored.<br/>Any more than that, and the items will be lost starting with the oldest lost items first.");
				Msg("Unimplemented");
				break;

			//case "@daily_quest":
			//	Msg("Did you receive today's Daily Event quest?<br/>Every day, you'll get a mission for each region.<br/>For instance, you can complete one mission each<br/>at Tara and at Taillteann.");
			//	Msg("Once you have completed an event quest from one region,<br/>you will automatically receive the next region's event quest.");
			//	Msg("Expired daily event quests will automatically disappear, so<br/>don't forget to do them!");
			//	break;

			//case "@daily_dungeon_quest":
			//	Msg("Would you like to take on the once-a-day challenge of clearing Uladh Dungeon?", Button("Accept", "@ok"), Button("Refuse", "@no"));

			//	switch(await Select())
			//	{
			//		case "@ok":
			//			if (!Player.QuestActive(70079))
			//			{
			//				StartQuest(70079); //[Daily Quest] Uladh Dungeon
			//				Msg("Good luck.");
			//			}
			//			else
			//			{
			//				Msg("You already received the Daily Dungeon Quest. Come back tomorrow.");
			//			}
			//			break;

			//		case "@no":
			//			Msg("Guess you're too busy.");
			//			break;
			//	}
			//	break;

			case "@make_guildrobe":
				if (!IsEnabled("GuildRobe"))
					break;

				Msg(L("Your guild needs 1,000 GP and 50,000 Gold in order to design a Guild Robe. Would you like to make the design?"), Button(L("Yes"), "@yes"), Button(L("Not now"), "@no"));
				if (await Select() != "@yes")
					break;

				// Check GP and Gold
				var guild = Player.Guild;
				var gp = 1000;
				var gold = 50000;

				if (guild.Points < gp)
				{
					Msg(L("I am sorry, but your guild is short on GP. Please gather up more GP and then try again."));
					break;
				}
				else if (guild.Gold < gold)
				{
					Msg(L("I am sorry, but your guild is short on Gold. Please gather up more Gold and then try again."));
					break;
				}

				OpenGuildRobeCreation();
				End("Please design the Guild Robe.");
				break;
		}

		End("Thank you, <npcname/>. I'll see you later!");
	}

	private void Greet()
	{
		if (Player.IsDoingPtjFor(NPC))
		{
			Msg(FavorExpression(), L("So, how is your task coming along?<br/>Please don't forget to report before the deadline."));
		}
		else if (Memory <= 0)
		{
			Player.GiveKeyword("shop_goverment_office");
			Msg(FavorExpression(), L("Welcome to Dunbarton.<br/>My name is <npcname/>, the Town Office worker who takes care of all the business related to the Adventurers' Association."));
		}
		else if (Memory == 1)
		{
			Msg(FavorExpression(), L("Hmm. I've seen someone that looks like you before."));
		}
		else if (Memory == 2)
		{
			Msg(FavorExpression(), L("I think your name is <username/>... Is that right?"));
		}
		else if (Memory <= 6)
		{
			Msg(FavorExpression(), L("Welcome again, <username/>."));
		}
		else
		{
			Msg(FavorExpression(), L("<username/>, you're back."));
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
					Msg(FavorExpression(), "You said your name was... <username/>, right? Tell me what's going on.");
					ModifyRelation(1, 0, 0);
				}
				else
				{
					Msg(FavorExpression(), "I've been kind of busy today.<br/>There are lots of people looking for work.");
					ModifyRelation(Random(2), 0, Random(3));
				}
				break;

			case "rumor":
				Msg(FavorExpression(), "Dunbarton is a city located near the border of the Kingdom of Aliech.<br/>It attracts a lot of travelers who are looking for adventure.<br/>If you'd like to improve your skills, how about going to the school?");
				ModifyRelation(Random(2), 0, Random(3));
				break;

			case "shop_misc":
				Msg("Looking for the General Shop?<br/>It's to the west of the Town Office.<br/>It's just over there, so you should be able to find it easily.<br/>Once you're there, talk to Walter.");
				break;

			case "shop_grocery":
				Player.GiveKeyword("shop_restaurant");
				Msg("A grocery store? The Restaurant carries cooking ingredients too,<br/>so why don't you just go there?<br/>It's close from here.");
				break;

			case "shop_healing":
				Msg("Hmm... You mean Manus' place.<br/>Follow the path to the right of the Square and go straight south.<br/>It's the L-shaped building right next to the bend in the road.<br/>Watch for the sign and you'll easily spot it.");
				break;

			case "shop_inn":
				Msg("This town doesn't have an inn yet.");
				break;

			case "shop_bank":
				Msg("The Bank is just near the General Shop.<br/>Talk to Austeyn.");
				break;

			case "shop_smith":
				Player.GiveKeyword("shop_armory");
				Msg("Hmm. We don't have a Blacksmith's Shop here.<br/>If it is weapons or armor you are looking for,<br/>why don't you check out Nerys' Weapons Shop?");
				break;

			case "skill_range":
				Msg("Huh? What is that?");
				break;

			case "skill_instrument":
				Msg("If you know how to play music,<br/>would you play a song for me?");
				break;

			case "skill_composing":
				Msg("I'm not really good at it.<br/>Most townsfolk here are also tone deaf<br/>so I'm not sure you'll find someone who knows how to do it.");
				break;

			case "skill_tailoring":
				Msg("Hmm. Do I look like a housewife<br/>who stays home all day working?");
				break;

			case "square":
				Msg("Hmm. Do I look like a housewife<br/>who stays home all day working?");
				break;

			case "pool":
				Msg("I don't know if we have one around here...<br/>All we have is a well. Nothing big like that.");
				break;

			case "farmland":
				Msg("It's just outside from here.<br/>Are you looking for anything in particular?");
				break;

			case "brook":
				Msg("Well...<br/>I heard that there is a stream by that name<br/>somewhere far up north.");
				break;

			case "shop_headman":
				Msg("The leader of this town is a Lord.<br/>It's not proper to call him a chief.");
				break;

			case "temple":
				Msg("To get to the church, follow along the alley next to the Restaurant west of here.<br/>You'll see the Lymilark cross tower as well as Priestess Kristell,<br/>so you should be able to find it easily.");
				break;

			case "school":
				Msg("A school? It's not over here.<br/>You should go east. Turn right at the Bookstore<br/>and go up from there.");
				break;

			case "skill_campfire":
				Msg("I'm sure it's a graceful skill,<br/>but I don't think it's for a frail lady.");
				break;

			case "shop_restaurant":
				Msg("The Restaurant is right next to us.<br/>You might have missed it because of the alley...<br/>Talk to Glenis.");
				break;

			case "shop_armory":
				Msg("If you mean Nerys' Weapons Shop,<br/>follow the road down south and you'll see it.");
				break;

			case "shop_cloth":
				Msg("Do you want to buy clothes?<br/>Go straight west.<br/>Don't leave the Square, though.");
				break;

			case "shop_bookstore":
				Msg("Head east.<br/>Go down the alley when you see it.<br/>The Minimap should help you.");
				break;

			case "shop_goverment_office":
				Msg("Mmm? This is the Town Office.<br/>Oh... Sorry, but you are not allowed to enter.");
				break;

			case "bow":
				Msg("I'm not so interested in weapons...");
				break;

			case "lute":
				Player.GiveKeyword("shop_misc");
				Msg("You can buy a Lute at the General Shop.");
				break;

			case "musicsheet":
				Msg("I don't really know what to look for in something like that.");
				break;

			default:
				RndFavorMsg(
					"?",
					"??",
					"...?",
					"I don't really know.",
					"What did you just say?",
					"I can't really say anything about that.",
					"Please understand even if I may not know much.",
					"I don't think I could help you much with that.",
					"I don't have anything worthwhile to contribute to that.",
					"Would you hold on to that topic until later? I'm a bit swamped at the moment..."
				);

				// These probably belong in a favor check, I'll have to figure them out later
				// "I don't know much about all that. <username/>, you do understand, don't you?"
				// "You know, something's just come up and I'm a bit busy right now.<br/>Do you mind coming back another day?"
				// "Must be that you've been exploring for so long now, right?<br/><username/>, you can certainly know a whole lot."
				// "I'm feeling achy all over today.<br/>I think I need to get some rest now. I'm so sorry, <username/>.<br/>I feel like I've heard something like that before... Perhaps I can find some notes I've jotted down in my expedition journal?"
				ModifyRelation(0, 0, Random(3));
				break;
		}
	}
}

public class EavanShop : NpcShopScript
{
	public override void Setup()
	{
		AddQuest("Quest", 71006, 30); // Collect the Skeleton's Fomor Scrolls
		AddQuest("Quest", 71007, 30); // Collect the Red Skeleton's Fomor Scrolls
		AddQuest("Quest", 71008, 30); // Collect the Metal Skeleton's Fomor Scrolls
		AddQuest("Quest", 71012, 30); // Collect the Skeleton Wolf's Fomor Scrolls
		AddQuest("Quest", 71017, 30); // Collect the White Spider's Fomor Scrolls
		AddQuest("Quest", 71018, 30); // Collect the Black Spider's Fomor Scrolls
		AddQuest("Quest", 71019, 30); // Collect the Red Spider's Fomor Scrolls
		AddQuest("Quest", 71021, 30); // Collect the Brown Fox's Fomor Scrolls
		AddQuest("Quest", 71022, 30); // Collect the Red Fox's Fomor Scrolls
		AddQuest("Quest", 71023, 30); // Collect the Gray Fox's Fomor Scrolls
		AddQuest("Quest", 71025, 30); // Collect the Brown Bear's Fomor Scrolls
		AddQuest("Quest", 71026, 30); // Collect the Red Bear's Fomor Scrolls
		AddQuest("Quest", 71028, 30); // Collect the Brown Grizzly Bear's Fomor Scrolls
		AddQuest("Quest", 71029, 30); // Collect the Red Grizzly Bear's Fomor Scrolls
		AddQuest("Quest", 71030, 30); // Collect the Black Grizzly Bear's Fomor Scrolls
		AddQuest("Quest", 71031, 30); // Collect the Bat's Fomor Scrolls
		AddQuest("Quest", 71032, 30); // Collect the Mimic's Fomor Scrolls
		AddQuest("Quest", 71034, 30); // Collect the Brown Town Rat's Fomor Scrolls
		AddQuest("Quest", 71035, 30); // Collect the Gray Town Rat's Fomor Scrolls
		AddQuest("Quest", 71040, 30); // Collect the Kobold's Fomor Scrolls
		AddQuest("Quest", 71041, 30); // Collect the Poison Kobold's Fomor Scrolls
		AddQuest("Quest", 71042, 30); // Collect the Gold Kobold's Fomor Scrolls
		AddQuest("Quest", 71044, 30); // Collect the Imp's Fomor Scrolls
		AddQuest("Quest", 71045, 30); // Collect the Wisp's Fomor Scrolls
		AddQuest("Quest", 71064, 30); // Collect the Ice Sprite's Fomor Scrolls
		AddQuest("Quest", 71065, 30); // Collect the Fire Sprite's Fomor Scrolls
		AddQuest("Quest", 71066, 30); // Collect the Flying Sword's Fomor Scrolls

		AddQuest("Party Quest", 100026, 30);  // [PQ] Hunt Down the Skeletons (30)
		AddQuest("Party Quest", 100028, 30);  // [PQ] Hunt Down the Red Skeletons (30)
		AddQuest("Party Quest", 100030, 30);  // [PQ] Hunt Down the Metal Skeletons (30)
		AddQuest("Party Quest", 100045, 30);  // [PQ] Hunt Down the Skeleton Wolves (30)
		AddQuest("Party Quest", 100047, 30);  // [PQ] Hunt Skeletons (30)
		AddQuest("Party Quest", 100041, 5);   // [PQ] Hunt Down the Gold Goblins (10)
		AddQuest("Party Quest", 100042, 30);  // [PQ] Hunt Down the Kobolds (30)
		AddQuest("Party Quest", 100043, 30);  // [PQ] Hunt Down the Poison Kobolds (30)
		AddQuest("Party Quest", 100046, 30);  // [PQ] Hunt Kobolds (30)
		AddQuest("Party Quest", 100086, 500); // [PQ] Defeat the Lycanthrope (Rabbie Basic)

		Add("Gift", 52014); // Teddy Bear
		Add("Gift", 52016); // Bunny Doll
		Add("Gift", 52015); // Pearl Necklace
		Add("Gift", 52025); // Gift Ring

		if (IsEnabled("SystemGuild"))
		{
			Add("Guild", 63040); // Guild Formation Permit
			Add("Guild", 63041); // Guild Stone Installation Permit

			//AddQuest("Guild Quest", InsertQuestId, 1200); // [Guild] Eliminate the Demi Lich
			//AddQuest("Guild Quest", InsertQuestId, 1200); // [Guild] Eliminate Banshee
			AddQuest("Guild Quest", 110006, 1200); // [Guild] Eliminate the Goblin Bandits
			AddQuest("Guild Quest", 110007, 1200); // [Guild] Eliminate the Giant Ogre
			AddQuest("Guild Quest", 110008, 1200); // [Guild] Eliminate the Giant Bear
		}

		if (IsEnabled("GuildRobe"))
		{
			Add("Guild Robe", false, (creature, npc) => creature.Guild != null && creature.Guild.HasRobe);
			Add("Guild Robe", 19047); // Guild Robe, color is handled automatically by the client
		}

		if (IsEnabled("RabbieArena"))
		{
			Add("Arena", 63050, 10);  // Rabbie Battle Arena Coin x10
			Add("Arena", 63050, 20);  // Rabbie Battle Arena Coin x20
			Add("Arena", 63050, 50);  // Rabbie Battle Arena Coin x50
			Add("Arena", 63050, 100); // Rabbie Battle Arena Coin x100
		}

		if (IsEnabled("RabbieAdvanced"))
		{
			AddQuest("Party Quest", 100087, 500);  // [PQ] Defeat the Black Succubus (Rabbie Adv. for 2)
			AddQuest("Party Quest", 100088, 500);  // [PQ] Defeat the Red Succubus (Rabbie Adv. for 3)
			AddQuest("Party Quest", 100089, 1000); // [PQ] Defeat the Red Succubus (Rabbie Adv.)
		}
	}
}
