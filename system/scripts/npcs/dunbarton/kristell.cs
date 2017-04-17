//--- Aura Script -----------------------------------------------------------
// Kristell
//--- Description -----------------------------------------------------------
// Priestess
//---------------------------------------------------------------------------

public class KristellScript : NpcScript
{
	public override void Load()
	{
		SetRace(10001);
		SetName("_kristell");
		SetBody(height: 0.97f);
		SetFace(skinColor: 15, eyeType: 3, eyeColor: 191);
		SetStand("human/female/anim/female_natural_stand_npc_Kristell");
		SetLocation(14, 34657, 42808, 0);
		SetGiftWeights(beauty: 1, individuality: 1, luxury: -1, toughness: 0, utility: 2, rarity: 1, meaning: 2, adult: 2, maniac: -1, anime: -1, sexy: 0);

		EquipItem(Pocket.Face, 3900, 0x00F8958F, 0x005A4862, 0x00714B4B);
		EquipItem(Pocket.Hair, 3017, 0x00EE937E, 0x00EE937E, 0x00EE937E);
		EquipItem(Pocket.Armor, 15009, 0x00303133, 0x00C6D8EA, 0x00DBC741);
		EquipItem(Pocket.Shoe, 17015, 0x00303133, 0x007BCDB7, 0x006E6565);

		AddPhrase("...");
		AddPhrase("I wish there was someone who could ring the bell on time...");
		AddPhrase("In the name of Lymilark...");
		AddPhrase("It's too much to go up and down these stairs to get here...");
		AddPhrase("The Church duties just keep coming. What should I do?");
		AddPhrase("The donations have decreased a little...");
		AddPhrase("There should be a message from the Pontiff's Office any day now.");
		AddPhrase("Why do these villagers obsess so much over their current lives?");
	}

	protected override async Task Talk()
	{
		SetBgm("NPC_Kristell.mp3");

		await Intro(L("This priestess, in her neat Lymilark priestess robe, has eyes and hair the color of red wine.<br/>Gazing into the distance, she wears the tilted cross, a symbol of Lymilark, on her neck.<br/>She wears dangling earrings made of the same material which emanate a gentle glow."));

		Msg("Welcome to the Church of Lymilark.", Button("Start a Conversation", "@talk"), Button("Shop", "@shop"));

		switch (await Select())
		{
			case "@talk":
				Player.GiveKeyword("temple");
				Greet();
				Msg(Hide.Name, GetMoodString(), FavorExpression());

				if (Player.IsUsingTitle(11001))
				{
					Msg("...I see... You have succeeded, <username/>.");
					Msg("Thank you for keeping your promise.");
					Msg("I wonder... if Tarlach can<br/>finally be at peace...?");
				}
				else if (Player.IsUsingTitle(11002))
				{
					Msg("Guardian of Erinn... There's nothing wrong with someone like you<br/>being called that, <username/>.<br/>Thank you... For saving Erinn...");
				}

				await Conversation();
				break;

			case "@shop":
				Msg("What is it that you are looking for?");
				OpenShop("KristellShop");
				return;
		}

		End("Thank you, <npcname/>. I'll see you later!");
	}

	private void Greet()
	{
		if (Memory <= 0)
		{
			Player.GiveKeyword("temple");
			Msg(FavorExpression(), L("I am Priestess <npcname/>. Nice to meet you."));
		}
		else if (Memory == 1)
		{
			Msg(FavorExpression(), L("Welcome to the Dunbarton church, <username/>."));
		}
		else if (Memory == 2)
		{
			Msg(FavorExpression(), L("Good to see you, <username/>."));
		}
		else if (Memory <= 6)
		{
			Msg(FavorExpression(), L("You seem burdened, <username/>...<br/>When you are in agony, relying on God will help you."));
		}
		else
		{
			Msg(FavorExpression(), L("<username/>. You are back. You look like you are looking for help...<br/>Would you, by any chance, be interested in the teachings of Lymilark?"));
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
					Msg("Nice to meet you, <username/>.");
					ModifyRelation(1, 0, 0);
				}
				else
				{
					Player.GiveKeyword("temple");
					Msg(FavorExpression(), "I am the priestess at this Church.<br/>Have you ever heard about Lord Lymilark's deep love and compassion towards humans?<br/>You probably have.");
					ModifyRelation(Random(2), 0, Random(3));
				}
				break;

			case "rumor":
				Player.GiveKeyword("shop_restaurant");
				Msg(FavorExpression(), "You can satisfy the hunger of the soul at the Church.<br/>For the hunger of the body, you should visit the Restaurant.<br/>Glenis' Restaurant is popular around here,<br/>so you should be able to find it easily.");
				ModifyRelation(Random(2), 0, Random(3));
				break;

			case "shop_misc":
				Player.GiveKeyword("musicsheet");
				Msg("Looking for the General Shop?<br/>The General Shop is down this way.<br/>Go down to the Square from here<br/>and look for Walter.");
				Msg("It might be useful for you to know that<br/>the General Shop also carries music scores and instruments.");
				break;

			case "shop_grocery":
				Player.GiveKeyword("shop_restaurant");
				Msg("A grocery store? We usually buy our ingredients at the Restaurant.<br/>Did you get on Glenis' bad side or something?");
				Msg("If not,<br/>why don't you just go there for the ingredients?");
				break;

			case "shop_healing":
				Msg("A Healer's House...<br/>You must be looking for Manus.<br/>Manus lives down south in the town.<br/>Go look for the healer sign around there.");
				Msg("Hmm. Consulting your Minimap<br/>or asking around would be<br/>a good way, too.");
				break;

			case "shop_inn":
				Msg("An inn? There is no inn in this town.<br/>Now that you mention it, it is rather strange.");
				break;

			case "shop_bank":
				Msg("If you are looking for a bank, go talk to Austeyn.<br/>To see him, walk down to the right end of the Square.<br/>He may seem easygoing, but he also catches everything..");
				Msg("That is to say, you can talk with him for only a short while and<br/>he will be able to read you like a book.");
				Msg("Lately, I have been thinking that<br/>it's something that comes from life experience.");
				break;

			case "shop_smith":
				Player.GiveKeyword("shop_armory");
				Msg("It's the first time I've heard anyone asking about a blacksmith's shop in this town.<br/>If you are looking for weapons or armor,<br/>why don't you stop by Nerys' Weapons Shop?");
				break;

			case "skill_range":
				Msg("Lymilark once taught the following.");
				Msg("'Fighting begetteth fighting,<br/>and that fighting begetteth more fighting...<br/>Therefore, in harmony and understanding ye shall endeavor to live instead of fighting,<br/>embracing and loving one another instead of jealousy and envy.");
				Msg("Repeated fighting in the end shall be the ruin of all...'");
				Msg("Self-defense is important, but<br/>I hope that you do not focus so much on combat.");
				break;

			case "skill_instrument":
				Msg("Hmm. There is an instrument at the Church but,<br/>unfortunately, I am not very proficient at it yet so<br/>many people are helping me on that.");
				break;

			case "skill_composing":
				Player.GiveKeyword("musicsheet");
				Msg("I have enough trouble simply reading the score and playing.<br/>Only those who are blessed can do such amazing things.");
				break;

			case "skill_tailoring":
				Player.GiveKeyword("shop_cloth");
				Msg("Why don't you go ask Simon at the Clothing Shop?<br/>I have yet to see someone<br/>who is as skilled as Simon.");
				break;

			case "skill_magnum_shot":
			case "skill_counter_attack":
			case "skill_smash":
				Msg("Lymilark our Lord gave us many teachings.<br/>Would you like to listen to one of them?");
				Msg("'Thou shalt not attack others.<br/>Ask thyself if anyone suffered wounds inflicted by thee<br/>without knowing and repent thy sins.");
				Msg("The wounded are also our Lord's beloved.<br/>How dare a mere being like thee<br/>attack thine brother and hurt him.");
				Msg("Reckon ye as a father the heart of God.'");
				Msg("Right now, you are interested only in fighting skills, but...<br/>I believe the time will come when you will ask yourself why,<br/>and repent your sins.");
				break;

			case "square":
				Msg("The Square is just over there.<br/>Go down the stairs and you will be right there.");
				break;

			case "farmland":
				Msg("The farmlands are just outside the town.<br/>Would you like to look around?");
				Msg("But please, do not go gleaning without permission.");
				break;

			case "shop_headman":
				Msg("We don't have a chief in our town.");
				break;

			case "temple":
				Msg("Yes, this is the Church<br/>where we worship Lymilark.");
				break;

			case "school":
				Msg("They teach combat skills and magic at the School.<br/>You will see it over the Square.");
				Msg("But I find it unfortunate that they neglect to<br/>instruct the teachings of Lymilark.");
				Msg("'Love is made possible only when your hearts<br/>are emptied and someone else fills them.<br/>Remember ye this word and endeavor to love.'");
				Msg("That is all I can say to you.");
				break;

			case "skill_campfire":
				Msg("Hmm. Discussing with someone<br/>who understands you about the love of the lord and the principles of this world<br/>around the campfire sounds like an interesting experience.");
				break;

			case "shop_restaurant":
				Msg("The Restaurant is just down there.<br/>It's near the General Shop, so you should be able to find it easily.<br/>Glenis owns the place so<br/>talk to her if you need anything.");
				Msg("She also sells cooking ingredients.<br/>And... it may be a Restaurant, but it is rather small<br/>so you may not be able to enter inside.");
				break;

			case "shop_armory":
				Msg("Nerys' Weapons Shop is near the<br/>south entrance.");
				Msg("Don't take it too personally if Nerys seems a little too aloof.<br/>She is just very shy around strangers.<br/>You will be fine once you get to know her a little.");
				break;

			case "shop_cloth":
				Msg("The Clothing Shop...<br/>You must be looking for Simon's shop.<br/>It is near the Square.");
				Msg("If you need clothes, make sure you pay a visit there.<br/>There are lots of decent clothes there.");
				Msg("Simon has made this robe for me for free.<br/>It is really light and comfortable.");
				Msg("If you get to go there,<br/>would you mind telling him I said hi?");
				break;

			case "shop_bookstore":
				Msg("A bookstore... You must be speaking of<br/>Aeira's Bookstore by that School over there.");
				Msg("Aeira may be young, but she is a deep thinker.<br/>From reading a lot of books, perhaps...?");
				Msg("Heehee. Still, she is a kid.<br/>She can't be compared to an adult.");
				break;

			case "shop_goverment_office":
				Msg("You are looking for Eavan, are you not?<br/>The Town Office is that large building you see down there.");
				Msg("It is fairly close by<br/>so you can get there in a few.");
				Msg("By the way... Eavan is the most popular girl in our town. Hehehe.");
				break;

			case "bow":
				Player.GiveKeyword("shop_armory");
				Msg("You can purchase a bow at<br/>Nerys' Weapons Shop.<br/>But please don't go around shooting<br/>innocent animals or do other mean things.");
				Msg("A while ago, Manus the healer told me that he was having a difficult time with it.<br/>Please, I beg you...");
				break;

			case "lute":
				Player.GiveKeyword("shop_misc");
				Msg("If you are looking for a lute,<br/>you will be able to buy it at the General Shop.");
				Msg("By the way, it has been a while since Walter last attended church...");
				Msg("And I'm worried that he may be concerned with something these days.");
				break;

			case "tir_na_nog":
				Msg("Many people engage in debates on the existence of<br/>Tir Na Nog, the eternal Utopia.");
				Msg("I am but a mere human and unable to understand such things,<br/>but there is this teaching by our lord Lymilark.");
				Msg("'Why turn your faces away from self-evident truth?<br/>Face ye the truth and follow in obedience,<br/>for it is right.'");
				Msg("I offer this word to you. I hope you will take it to heart.");
				break;

			case "mabinogi":
				Msg("Mabinogi is a tale of heroes, but<br/>it is also the history of bloodshed between<br/>humans and the evil Fomors as well as<br/>between humans themselves, woven into a worldly tune.");
				Msg("If you keep listening to such songs,<br/>one day your soul will become ill and your body will suffer.");
				Msg("Lord Lymilark once said this,");
				Msg("'If ye hate someone,<br/>your emotions wound not the hated<br/>but your souls.");
				Msg("Feeling and embracing a wounded soul<br/>is a time of great pain however quick it may be. '");
				Msg("I do not deny Mabinogi, but...<br/>Please keep this in mind as you listen to it.");
				break;

			case "musicsheet":
				Msg("There are many worldly tunes these days.<br/>Tunes that contain the grief and the passion of the composer...<br/>As I listen, such emotions are<br/>directly conveyed to my heart.");
				Msg("Lord Lymilark once said this,");
				Msg("'What worries carry ye in your hearts?<br/>Why passion hold ye in your hearts?<br/>Will ye to the love for your lord<br/>compare all these things?");
				Msg("How is it that to meaningless things ye spend your strength and passion?'<br/>Yes. Everything is meaningless before the love of our lord,<br/>and yet everyone lives stained by the world.");
				Msg("Hmm. Pardon my long speech,<br/>but you need to hear this, too.");
				break;

			default:
				RndFavorMsg(
					"Well, it is news to me.",
					"I do not know such things very well. Ha ha.",
					"Hmm. I'm not sure.<br/>Why don't you ask someone else?",
					"You are very knowledgeable. I don't know much about that.",
					"I am sorry. I don't know much about it, so it's pointless to ask me.",
					"Let's see... I might have heard that somewhere... But I am not so sure.",
					"Oh... I thought it was a topic I knew about, but I suppose not. Pardon me.",
					"I don't really know... But if you find out more, will you please let me know?"
				);
				ModifyRelation(0, 0, Random(3));
				break;
		}
	}
}

public class KristellShop : NpcShopScript
{
	public override void Setup()
	{
		Add("Gift", 52012); // Candlestick
		Add("Gift", 52024); // Bouquet
		Add("Gift", 52013); // Flowerpot
		Add("Gift", 52020); // Flowerpot
	}
}