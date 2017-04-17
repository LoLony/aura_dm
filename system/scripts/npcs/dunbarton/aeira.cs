//--- Aura Script -----------------------------------------------------------
// Aeira
//--- Description -----------------------------------------------------------
// Bookstore Owner
//---------------------------------------------------------------------------

public class AeiraScript : NpcScript
{
	public override void Load()
	{
		SetRace(10001);
		SetName("_aeira");
		SetBody(height: 0.8f);
		SetFace(skinColor: 16, eyeType: 2, eyeColor: 27, mouthType: 1);
		SetStand("human/female/anim/female_natural_stand_npc_Aeira");
		SetLocation(14, 44978, 43143, 158);
		SetGiftWeights(beauty: 0, individuality: 0, luxury: -1, toughness: 0, utility: 1, rarity: 2, meaning: 2, adult: -1, maniac: 2, anime: 2, sexy: -1);

		EquipItem(Pocket.Face, 3900, 0x0090CEF1, 0x00006B55, 0x006E6162);
		EquipItem(Pocket.Hair, 3022, 0x00664444, 0x00664444, 0x00664444);
		EquipItem(Pocket.Armor, 15042, 0x00EBAE98, 0x00354E34, 0x00E3E4EE);
		EquipItem(Pocket.Shoe, 17024, 0x00A0505E, 0x00F8784F, 0x00006E41);
		EquipItem(Pocket.Head, 18028, 0x00746C54, 0x00C0C0C0, 0x00007C8C);

		AddPhrase("*cough* The books are too dusty...");
		AddPhrase("*Whistle*");
		AddPhrase("Hahaha.");
		AddPhrase("Hmm. I can't really see...");
		AddPhrase("Hmm. The Bookstore is kind of small.");
		AddPhrase("I wonder if this book would sell?");
		AddPhrase("I wonder what Stewart is up to?");
		AddPhrase("Kristell... She's unfair.");
		AddPhrase("Oh, hello!");
		AddPhrase("Umm... So...");
		AddPhrase("Whew... I should just finish up the transcription.");
	}

	protected override async Task Talk()
	{
		SetBgm("NPC_Aeira.mp3");

		await Intro(L("This girl seems to be in her late teens with big thick glasses resting at the tip of her nose.<br/>Behind the glasses are two large brown eyes shining brilliantly.<br/>Wearing a loose-fitting dress, she has a ribbon made of soft and thin material around her neck."));

		Msg("So, what can I help you with?", Button("Start a Conversation", "@talk"), Button("Shop", "@shop"));

		switch (await Select())
		{
			case "@talk":
				Greet();
				Msg(Hide.Name, GetMoodString(), FavorExpression());

				if (Player.IsUsingTitle(11001))
				{
					Msg("Come to think of it... You're <username/>, right?<br/>The one who came looking for all those odd books. Haha.");
					Msg("Thanks to you, I spent a lot of time and effort searching for those books, too.<br/>And since you really inconvenienced me in a lot of ways,<br/>I think it's only right that you return the favor.");
					Msg("Haha.<br/>Just kidding. Look at you, all nervous, <username/>.");
					Msg("Hmm... Well? Did the books I'd found help you at all?<br/>Congratulations on what you've accomplished.");
					Msg("I look forward to doing more business with you.<br/>And come by the Bookstore more often!");
				}
				else if (Player.IsUsingTitle(11002))
				{
					Msg("Wow... <username/>, you really<br/>rescued Erinn?<br/>I wasn't sure before, but you really are an amazing person.<br/>Please continue to watch over my Bookstore!");
				}

				await Conversation();
				break;

			case "@shop":
				Msg("Welcome to the Bookstore.");
				OpenShop("AeiraShop");
				return;
		}

		End("Thank you, <npcname/>. I'll see you later!");
	}

	private void Greet()
	{
		if (Player.IsDoingPtjFor(NPC))
		{
			Msg(FavorExpression(), L("How is the work coming along?<br/>I hope you're doing well."));
		}
		else if (Memory <= 0)
		{
			Msg(FavorExpression(), L("I'm sorry, but your name is...?<br/>Mmm? <username/>? Nice to meet you."));
		}
		else if (Memory == 1)
		{
			Msg(FavorExpression(), L("Hahaha. I... Umm... I think I've met you before...<br/>Your name was...<br/>Oh, I'm sorry, <username/>. My mind went blank for a second. Hehehe."));
		}
		else if (Memory == 2)
		{
			Msg(FavorExpression(), L("<username/>, right?<br/>Hehe... I remember your name."));
		}
		else if (Memory <= 6)
		{
			Msg(FavorExpression(), L("Oh, it's you again, <username/>. How are you these days?"));
		}
		else
		{
			Msg(FavorExpression(), L("You must enjoy reading, <username/>.<br/>You come to my Bookstore all the time."));
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
					Msg(FavorExpression(), "My name? It's <npcname/>. We've never met before, have we?");
					ModifyRelation(1, 0, 0);
				}
				else
				{
					Player.GiveKeyword("shop_bookstore");
					Msg(FavorExpression(), "Hehehe... I may not look the part, but I own this Bookstore.<br/>It's okay to be casual, but<br/>at least give me some respect as a store owner.");
					ModifyRelation(Random(2), 0, Random(3));
				}
				break;

			case "rumor":
				Player.GiveKeyword("school");
				Msg(FavorExpression(), "If you want to properly train the stuff that's written on the book,<br/>why don't you first read the book in detail, then visit the school?<br/>Oh, and don't forget to talk to Stewart when you're there.");
				ModifyRelation(Random(2), 0, Random(3));
				break;

			case "about_skill":
				if (!Player.HasSkill(SkillId.MusicalKnowledge))
				{
					if (Favor < 15)
					{
						Msg(L("Oh, what a headache."));
						Msg(L("I paid a large sum of money and ordered in a ton of music theory books a while ago.<br/>I don't know if it's because they are expensive, but no one buys them.<br/>Still, they even have the skill-related green seals on them.<br/>It's not like I can just give them away to Stewart, either."));
						Msg(L("I'm tempted to give them away to someone who's nice to me.<br/>*Sigh* I should have just ordered poetry books that I like...<br/>It would be so good to have someone give me a poetry book as a gift."));
					}
					else if (!Player.HasItem(1013)) // Music Theory
					{
						Player.GiveItem(1013);
						Player.SystemNotice(L("Received Music Theory from Aeira."));

						Msg(L("It's so good to have you come by so often, <username/>!<br/>Come to think of it, you've been really nice to me,<br/>but I don't think I've returned the favor..."));
						Msg(L("Hmm... I want to make that up to you, and for that, I'd like to give you a gift.<br/>Here you go. Hope you like it!! *Giggle*"));
					}
					else
					{
						Msg(L("Oh... That book on music - did I give it to you as a gift? Hehehe. Good. It makes me happy to know that you're cherishing it."));
					}
				}
				else
				{
					Msg(L("I've talked a lot with other people regarding skills, but<br/>you seem be very knowledgeable about music, <username/>.<br/>I'm impressed. Hahaha."));
				}
				break;

			case "shop_misc":
				Msg("Hmm. The General Shop?<br/>That's where my father works at!<br/>To get to the General Shop,<br/>keep going straight towards the Square.");
				Msg("The shop is reasonably priced, with plenty of quality items...<br/>And there's just about everything you may be looking for,<br/>so don't forget to pay a visit! Hehe.");
				break;

			case "shop_grocery":
				Player.GiveKeyword("shop_restaurant");
				Msg("A grocery store?<br/>In this town, you can find the food ingredients at the Restaurant, too,<br/>so try going there instead.");
				break;

			case "shop_healing":
				Msg("A healer? That would be Manus! You're looking for Manus' place!<br/>His house is down there.<br/>Head south along the main road to the Square.");
				break;

			case "shop_inn":
				Player.GiveKeyword("skill_campfire");
				Msg("Oh, no. There is no inn in our town.<br/>There's really no good place to rest either.<br/>Why don't you stay with people who can use the Campfire skill for now?");
				break;

			case "shop_bank":
				Msg("Are you looking for Austeyn?<br/>The Bank is on the west end of the Square.<br/>You should be able to spot the sign easily.");
				Msg("Go there right now!");
				break;

			case "shop_smith":
				Player.GiveKeyword("shop_armory");
				Msg("Do we have a blacksmith's shop in this town? I think Nerys might know.<br/>It's just that I've never seen Nerys hammering<br/>or using the bellow.");
				Msg("You might want to go visit the Weapons Shop first.");
				break;

			case "skill_range":
				Msg("Hmm. A long range attack is...!");
				Msg("It's a way of attacking a hostile<br/>enemy at a certain distance by using<br/>devices such as a bow, spear, spells, etc...");
				Msg("Oh, pardon me.<br/>I got excited whenever I hear something I know.");
				break;

			case "skill_instrument":
				Msg("Hehe... I guess I'm not very good at something like that.");
				Msg("But those who bought instruments at my father's General Shop<br/>quickly picked it up after playing it only a few times.");
				break;

			case "skill_tailoring":
				Msg("Why don't you go ask Simon?<br/>It would also help to buy a Tailoring Kit from my father's shop.");
				break;

			case "skill_gathering":
				Msg("There was a time when I followed Stewart around<br/>to pick herbs all over the town.<br/>It was so much fun!");
				break;

			case "square":
				Msg("The Square is right over there<br/>through the alley.");
				break;

			case "farmland":
				Msg("The farmland is near here but everyone is stressed out over<br/>all the rats that are showing up there. Yuck!<br/>If you see them, please get them out of there.");
				break;

			case "brook":
				Msg("Adelia Stream?<br/>Yes, I've heard of it.");
				Msg("It's supposed to be a stream that flows near a town called Tir Chonaill,<br/>which is located just up north.");
				Msg("I don't think it flows<br/>all the way down here, though.");
				break;

			case "shop_headman":
				Msg("Hmm. Our town Chief?<br/>Never heard of one.<br/>Umm... Do we have one?");
				break;

			case "temple":
				Msg("Oh...<br/>Church is located in the<br/>northwest part of town.");
				break;

			case "school":
				Msg("The School is up there! Head straight up and you'll see it.<br/>If you have a good set of eyes, you could see it from here, too. Hehehe.");
				Msg("Oh, if you get to go there,<br/>please see how Stewart is doing! Please.");
				break;

			case "skill_campfire":
				Msg("Hmm.<br/>Sometimes there are novels<br/>that feature adventurers building a campfire.");
				Msg("In the middle of the field, gazing upon twinkling stars,<br/>sitting around a bonfire that illuminates the darkness,<br/>enjoying the moment with others...<br/>It's all so romantic!");
				Msg("Wait a minute... Did I order the books on campfire?");
				break;

			case "shop_restaurant":
				Player.GiveKeyword("shop_misc");
				Msg("The Restaurant? You must be talking about Glenis' place.<br/>All you need to do is go straight to the Square.");
				Msg("While you are on the way, make sure to<br/>visit the General Shop, too. Tee hee.");
				break;

			case "shop_armory":
				Msg("The Weapons Shop is over there by the south entrance.<br/>Nerys is usually outside the store so ask her.");
				Msg("By the way, weapons or armor here might be<br/>really expensive...");
				break;

			case "shop_cloth":
				Msg("The Clothing Shop? You can find it at the Square.<br/>How do you like what I'm wearing? I bought this there, too.");
				Msg("I like the clothes there so much that I bought several more from them!");
				break;

			case "shop_bookstore":
				Msg("Yes, this is the Bookstore! Ta-da!<br/>Feel free to look around, and press the 'Shop' button when you're ready.");
				Msg("I know this place isn't that big so you may not find the book you like,<br/>but we often bring in new shipments so don't be too disappointed<br/>if you can't find what you like today, okay?");
				break;

			case "shop_goverment_office":
				Msg("The Town Office? Are you looking for Eavan?<br/>Are you close friends with her?<br/>What's the relationship between you two?");
				Msg("Oh, nothing. It's just strange to see someone<br/>who's looking for someone who's as cold as ice. Hehe.<br/>...");
				break;

			case "bow":
				Msg("So, you're looking for a bow?<br/>You can buy a bow at the Weapons Shop.<br/>Ask Nerys and she'll kindly tell you where it is.");
				Msg("My father made a toy bow for me way back,<br/>but I couldn't hit the target very well with it.<br/>...<br/>...");
				Msg("But still,<br/>Aranwen once told me that<br/>I have the potential to be a good warrior.");
				Msg("Hehe...<br/>Bows are all good, but<br/>I wish I could learn how to shoot the arrow of love, like Cupid.");
				break;

			case "lute":
				Player.GiveKeyword("shop_misc");
				Msg("Right! The instrument my dad sold at his shop was called the Lute!<br/>You WILL go stop by his shop later, right?");
				Msg("What, you forgot already? It's the General Shop!!. I can't believe you forgot already.");
				break;

			case "tir_na_nog":
				Msg("Tir Na Nog?<br/>It's a paradise that people dream of, a world void of hatred and fighting, and full of love...");
				Msg("A place where the three major gods of this world maintain beautiful harmony,<br/>and praise the blessings of Aton Cimeni.<br/>It's also a place where many heroes gain new life after they pass away.");
				Msg("That's how the legend goes... But I can't trust anyone<br/>who claims to have been there and back.");
				break;

			case "mabinogi":
				Msg("Mabinogi is a song sung by bards<br/>about the heroes and the old gods.");
				Msg("It's a song that commemorates those who fought against<br/>the Fomors to establish the peaceful world we live in today.");
				Msg("Occasionally, we get books that talk about such stories here.<br/>Take a look. Hehe.");
				break;

			case "musicsheet":
				Player.GiveKeyword("shop_misc");
				Msg("The Music Scores?<br/>My father's shop carries them!<br/>It's over on the other side of the Square across the street from here.");
				Msg("They may sell out, so hurry!");
				break;

			default:
				RndFavorMsg(
					"...?",
					"Umm... What did you just say?",
					"Oh... Umm... That... I don't know.",
					"I'm not sure I know. Maybe Stewart knows.",
					"I don't know too much about that. Sorry...",
					"I don't really understand what you just said...",
					"Yeah, but... I don't really know anything about that.",
					"Hahaha. Well, it's not really my area of expertise...",
					"I don't know much about it, but let me know if you find out more.",
					"I'm not sure exactly what that is but it seems important,<br/>seeing how so many people inquire about it...",
					"Heh. Just because I own a bookstore doesn't mean that I've read all the books here.<br/>Please be patient with me."
				);
				ModifyRelation(0, 0, Random(3));
				break;
		}
	}
}

public class AeiraShop : NpcShopScript
{
	public override void Setup()
	{
		Add("Skill Book", 1006); // Introduction to Music Composition
		Add("Skill Book", 1012); // Campfire Manual
		Add("Skill Book", 1302); // Your first Glass of Wine Vol. 1
		Add("Skill Book", 1303); // Your first Glass of Wine Vol. 2
		Add("Skill Book", 1011); // Improving Your Composing Skill
		Add("Skill Book", 1304); // Wine for the Everyman
		Add("Skill Book", 1018); // The History of Music in Erinn (1)
		Add("Skill Book", 1305); // Tin's Liquor Drop
		Add("Skill Book", 1083); // Campfire Skill: Beyond the Kit
		Add("Skill Book", 1064); // Master Chef's Cooking Class: Baking
		Add("Skill Book", 1065); // Master Chef's Cooking Class: Simmering
		Add("Skill Book", 1019); // The History of Music in Erinn (2)
		Add("Skill Book", 1066); // About Kneading
		Add("Skill Book", 1020); // Composition Lessons with Helene (1)
		Add("Skill Book", 1123); // The Great Camping Companion: Camp Kit
		Add("Skill Book", 1007); // Healing: The Basics of Magic
		Add("Skill Book", 1029, 1, 9900); // A Campfire Memory
		Add("Skill Book", 1114); // The History of Music in Erinn (3)
		Add("Skill Book", 1111); // The Path of Composing
		Add("Skill Book", 1013, 1, 80000); // Music Theory
		Add("Skill Book", 1115); // Effective Meditation

		Add("Life Skill Book", 1055); // The Road to Becoming a Magic Warrior
		Add("Life Skill Book", 1056); // How to Enjoy Field Hunting
		Add("Life Skill Book", 1092); // Enchant, Another Mysterious Magic
		Add("Life Skill Book", 1124); // An Easy Guide to Taking Up Residence in a Home
		Add("Life Skill Book", 1102); // Your Pet
		Add("Life Skill Book", 1052); // How to milk a Cow
		Add("Life Skill Book", 1050); // An Unempolyed Man's Memoir of Clothes
		Add("Life Skill Book", 1040); // Facial Expressions Require Practice too
		Add("Life Skill Book", 1046); // Fire Arrow, The Ultimate Archery
		Add("Life Skill Book", 1021); // The Tir Chonaill Environs
		Add("Life Skill Book", 1022); // The Dunbarton Environs
		Add("Life Skill Book", 1043); // Wizards Love the Dark
		Add("Life Skill Book", 1057); // Introduction to Field Bosses
		Add("Life Skill Book", 1058); // Understanding Whisps
		Add("Life Skill Book", 1015); // Seal Stone Research Almanac: Rabbie Dungeon
		Add("Life Skill Book", 1016); // Seal Stone Research Almanac: Ciar Dungeon
		Add("Life Skill Book", 1017); // Seal Stone Research Almanac: Dugald Aisle
		Add("Life Skill Book", 1033); // Guidebook for Dungeon Exploration - Theory
		Add("Life Skill Book", 1034); // Guidebook for Dungeon Exploration - Practicum
		Add("Life Skill Book", 1035); // An Adventurer's Memoir
		Add("Life Skill Book", 1077); // Wanderer of the Fiodh Forest
		Add("Life Skill Book", 1090); // How Am I Going to Survive Like This?
		Add("Life Skill Book", 1031); // Understanding Elementals
		Add("Life Skill Book", 1036); // Records of the Bangorr Seal Stone Investigation
		Add("Life Skill Book", 1072); // Cooking on Your Own Vol. 1
		Add("Life Skill Book", 1073); // Cooking on Your Own Vol. 2

		Add("Literature", 1023);  // The Story of Spiral Hill
		Add("Literature", 1025);  // Mystery of the Dungeon
		Add("Literature", 1026);  // A Report on Astralium
		Add("Literature", 1027);  // I Hate Cuteness
		Add("Literature", 1028);  // Tracy's Secret
		Add("Literature", 1032);  // The Shadow Mystery
		Add("Literature", 1140);  // It's a 'paper airplane' that flies.
		Add("Literature", 1001);  // The Story of a White Doe
		Add("Literature", 1059);  // A Campfire Story
		Add("Literature", 1060);  // Imp's Diary
		Add("Literature", 1061);  // The Tale of Ifan the Rich
		Add("Literature", 1042);  // Animal-loving Healer
		Add("Literature", 1103);  // The Story of a Lizard
		Add("Literature", 1104);  // The Origin of Moon Gates
		Add("Literature", 74028); // The Forgotten Legend of Fiodh Forest
		Add("Literature", 74029); // The Tragedy of Emain Macha
		Add("Literature", 74027); // The Knight of Light Lugh, The Hero of Mag Tuireadh

		if (IsEnabled("Handicraft"))
			Add("Skill Book", 1505); // The World of Handicrafts
	}
}