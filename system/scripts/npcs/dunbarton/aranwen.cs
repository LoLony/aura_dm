//--- Aura Script -----------------------------------------------------------
// Aranwen
//--- Description -----------------------------------------------------------
// Teacher
//---------------------------------------------------------------------------

public class AranwenScript : NpcScript
{
	private const int ArrowRevolverQuest = 105;
	private const int BookOnArrowRevolver = 63505;
	private const int BookOnArrowRevolverPage10 = 40060;
	private const int AdvancedSkillScrollId = 70500;

	public override void Load()
	{
		SetRace(10001);
		SetName("_aranwen");
		SetBody(height: 1.15f, weight: 0.9f, upper: 1.1f, lower: 0.8f);
		SetFace(skinColor: 15, eyeType: 3, eyeColor: 192);
		SetLocation(14, 43378, 40048, 125);
		SetGiftWeights(beauty: 1, individuality: 1, luxury: -1, toughness: 2, utility: 2, rarity: -1, meaning: 2, adult: 2, maniac: -1, anime: -1, sexy: 0);

		EquipItem(Pocket.Face, 3900, 0x00344300, 0x0000163E, 0x008B0021);
		EquipItem(Pocket.Hair, 3026, 0x00BDC2E5, 0x00BDC2E5, 0x00BDC2E5);
		EquipItem(Pocket.Armor, 13008, 0x00C6D8EA, 0x00C6D8EA, 0x00635985);
		EquipItem(Pocket.Glove, 16503, 0x00C6D8EA, 0x00B20859, 0x00A7131C);
		EquipItem(Pocket.Shoe, 17504, 0x00C6D8EA, 0x00C6D8EA, 0x003F6577);
		EquipItem(Pocket.RightHand1, 40012, 0x00C0C0C0, 0x008C84A4, 0x00403C47);

		AddPhrase("...");
		AddPhrase("A sword does not betray its own will.");
		AddPhrase("A sword is not a stick. I don't feel any tension from you!");
		AddPhrase("Aren't you well?");
		AddPhrase("Focus when you're practicing.");
		AddPhrase("Hahaha.");
		AddPhrase("If you're done resting, let's keep practicing!");
		AddPhrase("It's those people who really need to learn swordsmanship.");
		AddPhrase("Put more into the wrists!");
		AddPhrase("That student may need to rest a while.");
	}

	protected override async Task Talk()
	{
		SetBgm("NPC_Aranwen.mp3");

		await Intro(L("A lady decked out in shining armor is confidently training students in swordsmanship in front of the school.<br/>Unlike a typical swordswoman, her moves seem delicate and elegant.<br/>Her long, braided silver hair falls down her back, leaving her eyes sternly fixed on me."));

		Msg("What brings you here?", Button("Start a Conversation", "@talk"), Button("Shop", "@shop"), Button("Modify Item", "@upgrade"));

		switch (await Select())
		{
			case "@talk":
				Greet();
				Msg(Hide.Name, GetMoodString(), FavorExpression());

				if (Player.IsUsingTitle(11001))
				{
					Msg("It's the duty of a warrior<br/>to offer help to the weak.");
					Msg("If I were you, I wouldn't boast about such acts, as you were just doing your job.<br/>...Even if the one you ended up rescuing is a Goddess.");
				}
				else if (Player.IsUsingTitle(11002))
				{
					Msg("Guardian of Erinn...<br/>If it were anyone else,<br/>I would tell them to stop being so arrogant...");
					Msg("But with you, <username/>, you are definitely qualified.<br/>Good job.");
				}

				await Conversation();
				break;

			case "@shop":
				Msg("Are you looking for a party quest scroll?");
				OpenShop("AranwenShop");
				return;

			case "@upgrade":
				Msg("Please select the weapon you'd like to modify.<br/>Each weapon can be modified according to its kind.<upgrade />");

				while (true)
				{
					var reply = await Select();

					if (!reply.StartsWith("@upgrade:"))
						break;

					var result = Upgrade(reply);
					if (result.Success)
						Msg("The modification you've asked for has been done.<br/>Is there anything you want to modify?");
					else
						Msg("(Error)");
				}
				Msg("A bow is weaker than a crossbow?<br/>That's because you don't know a bow very well.<br/>Crossbows are advanced weapons for sure,<br/>but a weapon that reflects your strength and senses is closer to nature than machinery.<upgrade hide='true'/>");
				break;
		}

		End("Thank you, <npcname/>. I'll see you later!");
	}

	private void Greet()
	{
		if (Memory <= 0)
		{
			Msg(FavorExpression(), L("Yes? Please don't block my view."));
		}
		else if (Memory == 1)
		{
			Msg(FavorExpression(), L("Hmm. <username/>, right?<br/>Of course."));
		}
		else if (Memory == 2)
		{
			Msg(FavorExpression(), L("<username/>, it really is you. What brings you here?"));
		}
		else if (Memory <= 6)
		{
			Msg(FavorExpression(), L("<username/>, what brings you here?"));
		}
		else
		{
			Msg(FavorExpression(), L("I've been seeing you quite often lately, <username/>."));
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
					Player.GiveKeyword("school");
					Msg("Let me introduce myself.<br/>My name is <npcname/>. I teach combat skills at the Dunbarton School.");
					ModifyRelation(1, 0, 0);
				}
				else
				{
					Msg(FavorExpression(), "If you are looking to learn combat arts, it's probably better<br/>to talk about classes or training rather than hold personal conversations.<br/>But then, I suppose there is lots to learn in this town other than combat skills.");
					ModifyRelation(Random(2), 0, Random(3));
				}
				break;

			case "rumor":
				Player.GiveKeyword("shop_armory");
				Msg(FavorExpression(), "If you need a weapon for the training,<br/>why don't you go see Nerys in the south side?<br/>She runs the Weapons Shop.");
				ModifyRelation(Random(2), 0, Random(3));
				break;

			// Handled by Arrow Revolver quest script
			//case "about_skill":
			//	break;

			case "shop_misc":
				Msg("Hmm. Looking for the General Shop?<br/>You'll find it down there across the Square.");
				Msg("Walter should be standing by the door.<br/>You can buy instruments, music scores, gifts, and tailoring goods such as sewing patterns.");
				break;

			case "shop_grocery":
				Player.GiveKeyword("shop_restaurant");
				Msg("If you are looking to buy cooking ingredients,<br/>the Restaurant will be your best bet.");
				break;

			case "shop_healing":
				Msg("A Healer's House? Are you looking for Manus?<br/>Manus runs a Healer's House near<br/>the Weapons Shop in the southern part of town.");
				Msg("Even if you're not ill<br/>and you're simply looking for things like potions,<br/>that's the place to go.");
				break;

			case "shop_inn":
				Msg("There is no inn in this town.");
				break;

			case "shop_bank":
				Msg("If you're looking for a bank, you can go to<br/>the Erskin Bank in the west end of the Square.<br/>Talk to Austeyn there for anything involving money or items.");
				break;

			case "shop_smith":
				Player.GiveKeyword("shop_armory");
				Msg("There is no blacksmith's shop in this town, but<br/>if you are looking for anything like weapons or armor,<br/>why don't you head south and visit the Weapons Shop?");
				break;

			case "skill_range":
				Player.GiveKeyword("bow");
				Msg("I suppose I could take my time and verbally explain it to you,<br/>but you should be able to quickly get the hang of it<br/>once you equip and use a bow a few times.");
				break;

			case "skill_tailoring":
				Player.GiveKeyword("shop_cloth");
				Msg("It would be most logical to get Simon's help<br/>at the Clothing Shop.");
				break;

			case "skill_magnum_shot":
				Msg("Magnum Shot?<br/>Haven't you learned such a basic skill alrerady?<br/>You must seriously lack training.");
				Msg("It may be rather time-consuming, but<br/>please go back to Tir Chonaill.<br/>Ranald will teach you the skill.");
				break;

			case "skill_counter_attack":
				Msg("If you don't know the Counterattack skill yet, that is definitely a problem.<br/>Very well. First, you'll need to fight a powerful monster and get hit by its Counterattack.");
				Msg("Monsters like bears use Counterattack<br/>so watch how they use it and take a hit,<br/>and you should be able to quickly get the hang of it without any particular training.");
				Msg("In fact, if you are not willing to take the hit,<br/>there is no other way to learn that skill.<br/>Simply reading books will not help.");
				break;

			case "skill_smash":
				Msg("Smash...<br/>For the Smash skill, why don't you go to the Bookstore and<br/>look for a book on it?");
				Msg("You should learn it by yourself before bothering<br/>people with questions.<br/>You should be ashamed of yourself.");
				break;

			case "square":
				Msg("The Square is just over here.<br/>Perhaps it totally escaped you<br/>because it's so large.");
				break;

			case "farmland":
				Msg("Strangely, large rats have been seen<br/>in large numbers in the farmlands recently.<br/>This obviously isn't normal.");
				Msg("If you are willing,<br/>would you go and take some out?<br/>You'll be appreciated by many.");
				break;

			case "brook":
				Msg("Adelia Stream...<br/>I believe you're speaking of the<br/>stream in Tir Chonaill...");
				Msg("Shouldn't you be asking<br/>these questions<br/>in Tir Chonaill?");
				break;

			case "shop_headman":
				Msg("A chief?<br/>This town is ruled by a Lord,<br/>so there is no such person as a chief here.");
				break;

			case "temple":
				Msg("You must have something to discuss with Priestess Kristell.<br/>You'll find her at the Church up north.");
				Msg("You can also take the stairs that head<br/>northwest to the Square.<br/>There are other ways to get there, too,<br/>so it shouldn't be too difficult to find it.");
				break;

			case "school":
				Msg("Mmm? This is the only school around here.");
				break;

			case "skill_windmill":
				Player.RemoveKeyword("skill_windmill");
				Msg("Are you curious about the Windmill skill?<br/>It is a useful skill to have when you're surrounded by enemies.<br/>Very well. I will teach you the Windmill skill.");
				break;

			case "shop_restaurant":
				Msg("If you're looking for a restaurant, you are looking for Glenis' place.<br/>She not only sells food, but also a lot of cooking ingredients, so<br/>you should pay a visit if you need something.");
				Msg("The Restaurant is in the north alley of the Square.");
				break;

			case "shop_armory":
				Msg("Nerys is the owner of the Weapons Shop.<br/>Keep following the road that leads down south<br/>and you'll see her mending weapons outside.");
				Msg("She may seem a little aloof,<br/>but don't let that get to you too much<br/>and you'll get used to it.");
				break;

			case "shop_cloth":
				Msg("There is no decent clothing shop in this town...<br/>But, if you must, go visit Simon's place.<br/>You should be able to find something that fits right away.");
				break;

			case "shop_bookstore":
				Msg("You mean Aeira's Bookstore.<br/>It's just around here.<br/>Follow the road in front of the school up north.");
				Msg("Many types of books go through that place,<br/>so even if you don't find what you want right away,<br/>keep visiting and you'll soon get it.");
				break;

			case "shop_goverment_office":
				Msg("Are you looking for Eavan?<br/>The Lord and the Captain of the Royal Guards<br/>are very hard to reach. ");
				Msg("If you're really looking for Eavan,<br/>go over to that large building to the north of the Square.");
				break;

			case "bow":
				Player.GiveKeyword("shop_armory");
				Msg("Hey! You'll have to go to the Weapons Shop to buy bows.<br/>We don't give out bows at the school.<br/>We can only teach you how to fight<br/>with them.");
				break;

			case "lute":
				Player.GiveKeyword("shop_misc");
				Msg("I saw a lute at the General Shop once.<br/>I'm not too interested in it<br/>so I don't have much more to tell you.");
				break;

			case "tir_na_nog":
				Msg("Tir Na Nog...<br/>It's a Utopia.");
				Msg("But I think it's more important to be<br/>faithful to the present than to dream of such things.");
				Msg("Life is short, and time flies too quickly to simply waste it on dreaming.");
				break;

			case "mabinogi":
				Msg("Mabinogi is a song that bards sing to<br/>praise the heroes.");
				Msg("If you become an outstanding warrior,<br/>bards from future generations might<br/>sing of you someday.");
				break;

			default:
				RndFavorMsg(
					"Will you tell me about it when you find out more?",
					"Being a teacher doesn't mean that I know everything.",
					"Hey! Asking me about such things is a waste of time.",
					"I don't know anything about it. Why don't you ask others?",
					"I don't know too much about anything other than combat skills.",
					"I don't know anything about it. I'm sorry I can't be much help.",
					"It doesn't seem bad but... I don't think I can help you with it.",
					"If you keep bringing up topics like this, I can't say much to you.",
					"Other people do ask me about something like that occasionally...<br/>Still, it is not something I should pretend to know."
				);
				ModifyRelation(0, 0, Random(3));
				break;
		}
	}
}

public class AranwenShop : NpcShopScript
{
	public override void Setup()
	{
		AddQuest("Party Quest", 100021, 5);  // [PQ] The Hunt for Red Bears (10)
		AddQuest("Party Quest", 100022, 30); // [PQ] The Hunt for Red Bears (30)
		AddQuest("Party Quest", 100032, 30); // [PQ] Hunt Down the Brown Grizzly Bears (30)
		AddQuest("Party Quest", 100033, 30); // [PQ] Hunt Down the Red Grizzly Bears (30)
		AddQuest("Party Quest", 100034, 30); // [PQ] Hunt Down the Black Grizzly Bears (30)
		AddQuest("Party Quest", 100048, 30); // [PQ] Hunt Down the Grizzly Bears (20)
		AddQuest("Party Quest", 100049, 30); // [PQ] Hunt Down the Young Grizzly Bears (30)
		AddQuest("Party Quest", 100050, 30); // [PQ] Hunt Down the Grizzly Bears (20)
		AddQuest("Party Quest", 100051, 30); // [PQ] Hunt Down the Young Grizzly Bears (30)
	}
}