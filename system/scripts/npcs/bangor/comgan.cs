//--- Aura Script -----------------------------------------------------------
// Comgan
//--- Description -----------------------------------------------------------
// Priest/Healer
//---------------------------------------------------------------------------

public class ComganScript : NpcScript
{
	public override void Load()
	{
		SetRace(10002);
		SetName("_comgan");
		SetBody(height: 0.6f);
		SetFace(skinColor: 15, eyeType: 3, eyeColor: 55, mouthType: 1);
		SetStand("human/male/anim/male_natural_stand_npc_Duncan");
		SetLocation(31, 15329, 12122, 154);
		SetGiftWeights(beauty: 1, individuality: 2, luxury: -1, toughness: 2, utility: 2, rarity: 0, meaning: -1, adult: 2, maniac: -1, anime: 2, sexy: 0);

		EquipItem(Pocket.Face, 4900, 0x00F6EF20, 0x006472B6, 0x00D8CC5E);
		EquipItem(Pocket.Hair, 4003, 0x0FFFFFFF, 0x0FFFFFFF, 0x0FFFFFFF);
		EquipItem(Pocket.Armor, 15060, 0x00400000, 0x00F0EA9D, 0x00FFFFFF);
		EquipItem(Pocket.Shoe, 17015, 0x00000000, 0x00F4638B, 0x00F9EF64);

		AddPhrase("...");
		AddPhrase("I guess only people like me would understand what I'm saying...");
		AddPhrase("I need to build a Church soon...");
		AddPhrase("Lord Lymilark...");
		AddPhrase("Oh Lymilark, please provide me with strength and courage... Like you did that day...");
		AddPhrase("Selling gifts to build a church would be... Is that feasible?");
		AddPhrase("There are more important things in life than what we merely see...");
		AddPhrase("What must I do...");
		AddPhrase("What should I do to convert more people?");
		AddPhrase("What should I do...");
		AddPhrase("Why do people ignore what I say...");
	}

	protected override async Task Talk()
	{
		SetBgm("NPC_Comgan.mp3");

		await Intro(L("This boy is wearing a priest's robe with wide necklines showing that he has on many layers of clothing.<br/>The color of his thick hair looks like feather clouds floating above the Bangor sky.<br/>Blue eyes like a deep, tranquil ocean add a gentle radiance to his slightly tilted face."));

		Msg("Do you... believe in God?", Button("Start a Conversation", "@talk"), Button("Shop", "@shop"));

		switch (await Select())
		{
			case "@talk":
				Greet();
				Msg(Hide.Name, GetMoodString(), FavorExpression());

				if (Title == 11001)
				{
					Msg("...you rescued the Goddess?<br/>You? <username/>...?<br/>That's just incredible...<br/>But... can I really believe what you're claiming?");
					Msg("...It's just like how people find it so hard to believe that I'm a Priest...<br/>I can definitely relate.");
					Msg("Anyway, this helps me to place myself in another person's shoes,<br/>so I must thank you for that.");
				}
				else if (Title == 11002)
				{
					Msg("Thank you for saving Erinn, <username/>.<br/>Please continue to watch out for us.");
				}

				await Conversation();
				break;

			case "@shop":
				Msg("What is it that you need?<br/>Please take a look.");
				OpenShop("ComganShop");
				return;
		}

		End("Thank you, <npcname/>. I'll see you later!");
	}

	private void Greet()
	{
		if (DoingPtjForNpc())
		{
			Msg(FavorExpression(), L("<username/>, I trust that you are doing the task I've asked you to do?"));
		}
		else if (Memory <= 0)
		{
			Msg(FavorExpression(), L("I don't think we've met before... My name is <npcname/>.<br/>I'm the priest of this town. Nice to meet you."));
		}
		else if (Memory == 1)
		{
			Msg(FavorExpression(), L("If you're free, would you like to chat? Your name was..."));
			Msg(L("I am sorry. My memory is failing me today. I will try to remember next time."));

		}
		else if (Memory == 2)
		{
			Msg(FavorExpression(), L("Ah, <username/>? Welcome. Are you interested in God's teachings?"));
		}
		else if (Memory <= 6)
		{
			Msg(FavorExpression(), L("You're back, <username/>. I was just wondering if you would show up again."));
		}
		else
		{
			Msg(FavorExpression(), L("I enjoy your frequent visits, <username/>, as well as your attentiveness to my stories."));
		}

		UpdateRelationAfterGreet();
	}

	protected override async Task Keywords(string keyword)
	{
		switch (keyword)
		{
			case "personal_info":
				Msg(FavorExpression(), "I told you my name a while ago, right? My name is <npcname/>...<br/>I'm the priest of this town.");
				Msg("...<br/>Everyone is rather puzzled at my young age,<br/>but I am a priest who has been officially ordained with a certificate of approval from the Pontiff's office. *Chuckle*");
				Msg("I am not a shady character<br/>so please don't look at me so suspiciously.");
				ModifyRelation(Random(2), 0, Random(3));
				break;

			case "rumor":
				Msg(FavorExpression(), "This town is rather run-down, don't you think?<br/>I used to think that when I first arrived here...");
				Msg("Those who have been here for a long time would tell me that this place was once very prosperous.<br/>Had a church, even...");
				ModifyRelation(Random(2), 0, Random(3));
				break;

			case "shop_misc":
				Msg("You seem to be in need of something.<br/>You can go down the alley to the left of the Blacksmith's Shop. Look for Gilmore there.");
				Msg("May God bless you each step of the way...");
				break;

			case "shop_grocery":
				Msg("If you're looking for food,<br/>you can get some at Jennifer's Pub...");
				Msg("I would love to give you some food myself...<br/>But I don't have any either...");
				break;

			case "shop_healing":
				Msg("Are you feeling alright?<br/>Unfortunately, you won't find a Healer's House around here...");
				Msg("How about purchasing some potions or bandages from me?");
				Msg("Select 'Shop' to browse the items I have...");
				Msg("Don't think that I'm putting on a sales pitch for you.<br/>Please consider this a way for you to contribute something in building a church here...");
				break;

			case "shop_inn":
				Msg("It there was an inn around here<br/>I would try to rent it and start a church even if it was small...");
				Msg("But this town doesn't even have that...");
				break;

			case "shop_bank":
				Msg("Have you met Bryce already?");
				Msg("He's the one standing right next to the General Shop.<br/>He seems to take care of bank transactions in the shed next to it...");
				Msg("Come to think of it, this town doesn't even have an official Bank yet...");
				Msg("Sometimes when I see him, I can't help but think he'd be able to relieve some of his worries<br/>if he pays attention to Lymilark's teachings.");
				Msg("Ah, sorry. I shouldn't have said that.<br/>I shouldn't stick my nose into other people's business...");
				break;

			case "shop_smith":
				Msg("So, you are looking for Edern and Elen?<br/>They're right over there.");
				Msg("If you are looking for items to buy,<br/>Elen is the person to talk to.<br/>If you would like to have your items repaired, or talk about skills,<br/>Edern is the one.");
				Msg("May the blessings of Lymilark be with you...");
				break;

			case "skill_rest":
				Msg("The Resting skill is the one<br/>that helps you recover some HP.");
				Msg("You should learn it soon if you haven't done so already.");
				Msg("Hmm... Please don't look so offended...<br/>I think you misunderstood what I said. I was not underestimating you.<br/>Please accept my apology...");
				break;

			case "skill_range":
				Msg("Although you should be able to defend yourself,<br/>I don't believe it's a good idea to solve every problem with violence...");
				Msg("I'm sorry if I'm not giving you the advice you are looking for...");
				break;

			case "skill_instrument":
				Msg("Did you know that music<br/>is an excellent tool to praise God?");
				Msg("I believe that using it to promote themselves and their accomplishments,<br/>rather than to praise God, however,<br/>is a sin in and of itself....");
				break;

			case "skill_magnum_shot":
				Msg("I actually don't know much about such things...<br/>Perhaps Riocard at the Pub might know.");
				Msg("Riocard is actually very knowledgeable.<br/>I don't know about his educational background,<br/>but he knows a lot of things beyond his age.");
				Msg("But then again, I, of all people, should not be talking about age...");
				break;

			case "skill_counter_attack":
				Msg("The whole concept of \"eye for an eye\", I believe, is the reason why the society is always at a discord.");
				Msg("Forgive, forgive, and forgive again.<br/>Preserving the god-given peace<br/>cannot be accomplished through vengeance or revenge...");
				Msg("But, still, people ignore the value of peace<br/>and obsess over personal revenge...");
				Msg("It is such a shame...");
				break;

			case "skill_gathering":
				Msg("I learned that Gathering is a general skill that aids you in<br/>collecting things from nature by hand.");
				Msg("My job is to collect the emptyness of the people<br/>and fill them with God's wisdom.");
				break;

			case "pool":
				Msg("It would be good to have something like that...<br/>We had plenty of water in my hometown,<br/>and I remember going to play in it on a hot day...");
				Msg("This town doesn't even have enough drinking water, let alone for playing.<br/>You are just going to have to deal with the inconvenience...");
				break;

			case "farmland":
				Msg("Yes. There is no farmland<br/>nor any farmers in this town.");
				Msg("The soil of this place<br/>is so barren without any water that<br/>it is quite difficult for plants to take root.");
				Msg("I think it's fortunate that we have<br/>at least a few trees...");
				break;

			case "shop_headman":
				Msg("Sometimes I wonder<br/>why it is that we don't have a chief<br/>when we don't even have a town office...");
				Msg("I have yet to find a satisfying answer, though...");
				break;

			case "temple":
				Msg("That is my duty and desire.");
				Msg("To build a place of worship<br/>for Lord Lymilark in the town of Bangor...");
				Msg("I'm praying for this as usual today.<br/>If you could contribute as well...");
				break;

			case "school":
				Msg("What makes the future of this town bleak is<br/>that there is no school.");
				Msg("That kids like Sion, for example, don't go to school to study<br/>but start working early instead.<br/>It's a real tragedy...");
				Msg("The opportunity for education is something<br/>that must be given equally to everyone...");
				Msg("As I think of such things, I feel truly blessed that<br/>I was able to receive appropriate education<br/>at appropriate times.<br/>I can't explain it any other way...");
				Msg("After the Church is built, I wish to build a school<br/>and take the lead in spreading<br/>the teachings of Lymilark....");
				break;

			case "skill_windmill":
				Msg("I have heard from somewhere that<br/>it is a skill that allows you to deal with many people at a time by yourself.");
				Msg("I don't know much else than that...");
				break;

			case "skill_campfire":
				Msg("This town seems extra cautious<br/>when it comes to handling fire.");
				Msg("But there are so many furnaces and Blacksmith's Shops...");
				Msg("I think it may be because this town<br/>suffered a terrible fire in the past. What do you think?");
				break;

			case "shop_restaurant":
				Msg("Have you been to Jennifer's Pub?<br/>It may seem laughable that a priest would recommend you to visit the Pub...");
				Msg("But she also sells various kinds of food,<br/>so it will be helpful when you find yourself hungry.");
				break;

			case "shop_armory":
				Msg("If you are looking to buy weapons, you should go visit Elen.<br/>Yes, she's the pretty blonde lady at the Blacksmith's Shop.");
				Msg("Some people, when I tell them this, go visit Ibbie instead...");
				Msg("Ibbie has come to rebuke me before...");
				break;

			case "shop_cloth":
				Msg("For clothes, what we have at the General Shop is all we have.");
				Msg("You seem interested in what I'm wearing...<br/>But this was provided by the sect.");
				Msg("It is not something that's for sale...");
				Msg("Although, I have to wonder if I could work around it somehow<br/>if it helps me in building the Church.");
				break;

			case "shop_bookstore":
				Msg("A book is like a lamp that illuminates an<br/>unknown world to you.");
				Msg("The more books you read, the brighter the lamp will shine.");
				Msg("And if you could receive the blessings of the Lord through it,<br/>what more could you ask for?");
				break;

			case "shop_goverment_office":
				Msg("I think that sometimes.<br/>If there were a town office here<br/>and I received help from the office,<br/>it might be a little easier to build the Church...");
				Msg("Still, I cannot abandon the idea of<br/>building the Church and take the initiative<br/>to build a town office instead...");
				Msg("This is when I realize how difficult it is to<br/>testify to the word of God in Bangor...");
				break;

			case "graveyard":
				Msg("That is where people are buried when they die...<br/>But, in this town,<br/>there seems to be a different place to bury the dead.");
				Msg("I suspect that it is in a dungeon,<br/>but I don't know the details.");
				break;

			case "bow":
				Msg("You will probably be able to buy it at the Blacksmith's Shop.<br/>There are other useful items there besides that.<br/>You should certainly pay a visit, if you haven't been there.");
				break;

			case "lute":
				Msg("I have seen it being sold at Gilmore's General Shop.<br/>I didn't buy it, though. Someone else bought it and showed it to me.");
				Msg("He gave it to me as a gift to be used in building the Church...");
				Msg("I did sell it back later to the General Shop<br/>to raise funds for the Church...");
				Msg("By the way, is a Lute usually worth... 20 Golds?");
				break;

			case "complicity":
				Msg("Someone once made a biting remark to me,<br/>that I'm nothing more than a pawn trying to recruit people into our religion...");
				Msg("I didn't know how to accurately explain to him<br/>that is not the case.");
				Msg("It was agonizing...");
				break;

			case "tir_na_nog":
				Msg("Tir Na Nog is a divine paradise<br/>held up by the three major gods of this world.");
				Msg("Under the divine reign of Aton Cimeni who rules all things,<br/>and the Gods of love, peace, and freedom,<br/>people feel compelled to stay in Tir Na Nog...");
				Msg("...I firmly believe that I myself will be there someday.");
				break;

			case "mabinogi":
				Msg("Mabinogi is a song that tells the story<br/>of the war with the evil Fomor since the ancient days.<br/>It is a song of praise to the heroes that saved this world, as well as a song of mourning<br/>for the warriors who lost their lives for the sake of this world...");
				Msg("Every song has a story behind it,<br/>so I think it would do you well to listen to the song when you have a chance...");
				break;

			case "musicsheet":
				Msg("...Do you, by any chance, know some good songs?");
				Msg("I would like to hear a few, if at all possible...");
				Msg("It's not everyday you run into an excellent musician...");
				break;

			default:
				RndFavorMsg(
					"You are a curious one, aren't you?",
					"Hahaha... I am sorry, but I don't know much about that.",
					"I'll pray to the God of knowledge that your curiosity continues on.",
					"Good question... I don't have anything in particular to say about that.",
					"You are a curious one. Why don't you speak to others in the town about it?",
					"I'm flattered that you asked me that, but it saddens me that I have no answers for you...",
					"It will probably frustrate you if I tell you I don't know, but I can't help what I don't know.",
					"Don't be too disappointed because I don't know about that. Ignorance is better than failure that comes from pretensions."
				);
				ModifyRelation(0, 0, Random(3));
				break;
		}
	}
}

public class ComganShop : NpcShopScript
{
	public override void Setup()
	{
		Add("Potions", 51001);     // HP 10 Potion
		Add("Potions", 51002, 1);  // HP 30 Potion x1
		Add("Potions", 51002, 10); // HP 30 Potion x10
		Add("Potions", 51002, 20); // HP 30 Potion x20
		Add("Potions", 51011);     // Stamina 10 Potion
		Add("Potions", 51012, 1);  // Stamina 30 Potion x1
		Add("Potions", 51012, 10); // Stamina 30 Potion x10
		Add("Potions", 51012, 20); // Stamina 30 Potion x20

		Add("First Aid Kits", 60005, 10); // Bandage x10
		Add("First Aid Kits", 60005, 20); // Bandage x20
		Add("First Aid Kits", 63000, 10); // Phoenix Feather x10
		Add("First Aid Kits", 63000, 20); // Phoenix Feather x20
		Add("First Aid Kits", 63001, 1);  // Wings of a Goddess x1
		Add("First Aid Kits", 63001, 5);  // Wings of a Goddess x5

		AddQuest("Quest", 71031, 30); // Collect the Bat's Fomor Scrolls
		AddQuest("Quest", 71032, 30); // Collect the Mimic's Fomor Scrolls
		AddQuest("Quest", 71037, 30); // Collect the Goblin's Fomor Scrolls
		AddQuest("Quest", 71044, 30); // Collect the Imp's Fomor Scrolls
		AddQuest("Quest", 71066, 30); // Collect the Flying Sword's Fomor Scrolls

		AddQuest("Party Quest", 100035, 20);   // [PQ] Hunt Down the Brown Dire Wolves (30)
		AddQuest("Party Quest", 100038, 30);   // [PQ] Hunt Down the White Dire Wolves (30)
		AddQuest("Party Quest", 100054, 30);   // [PQ] Hunt Down the Kobold Bandits (30)
		AddQuest("Party Quest", 100056, 20);   // [PQ] Hunt Down the Laghodessas (30)
		AddQuest("Party Quest", 100058, 20);   // [PQ] Hunt Down the Green Gremlins (10)
		AddQuest("Party Quest", 100059, 20);   // [PQ] Hunt down the Gray Gremlins (10)
		AddQuest("Party Quest", 100060, 20);   // [PQ] Hunt Down the Brown Gremlins (10)
		AddQuest("Party Quest", 100061, 20);   // [PQ] Hunt Down the Flying Swords (10)
		AddQuest("Party Quest", 100082, 500);  // [PQ] Defeat the Werewolf (Barri Basic)

		if (IsEnabled("BarriAdvanced"))
		{
			AddQuest("Party Quest", 100083, 500);  // [PQ] Defeat the New Gremlin (Barri Adv. for 2)
			AddQuest("Party Quest", 100084, 500);  // [PQ] Defeat the New Gremlin (Barri Adv. for 3)
			AddQuest("Party Quest", 100085, 1000); // [PQ] Defeat the New Gremlin (Barri Adv.)
		}

		if (IsEnabled("G16HotSpringRenewal"))
		{
			Add("Etc.", 91563, 1); // Hot Spring Ticket x1
			Add("Etc.", 91563, 5); // Hot Spring Ticket x5
		}

		if (IsEnabled("PuppetMasterJob"))
		{
			Add("Potions", 51201, 1);  // Marionette 30 Potion x1
			Add("Potions", 51201, 10); // Marionette 30 Potion x10
			Add("Potions", 51201, 20); // Marionette 30 Potion x20
			Add("Potions", 51202, 1);  // Marionette 50 Potion x1
			Add("Potions", 51202, 10); // Marionette 50 Potion x10
			Add("Potions", 51202, 20); // Marionette 50 Potion x20

			Add("First Aid Kits", 63715, 10); // Fine Marionette Repair Set x10
			Add("First Aid Kits", 63715, 20); // Fine Marionette Repair Set x20
			Add("First Aid Kits", 63716, 10); // Marionette Repair Set x10
			Add("First Aid Kits", 63716, 20); // Marionette Repair Set x20
		}
	}
}