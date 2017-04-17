//--- Aura Script -----------------------------------------------------------
// Alissa
//--- Description -----------------------------------------------------------
// Mill Operator
//---------------------------------------------------------------------------

public class AlissaScript : NpcScript
{
	const string WindmillPropName = "Uladh_main/town_TirChonaill/windmill_tircho";

	static bool WindmillActive { get; set; }

	static Prop _windmillProp = null;
	Prop WindmillProp { get { return _windmillProp ?? (_windmillProp = ChannelServer.Instance.World.GetProp(WindmillPropName)); } }

	public override void Load()
	{
		SetRace(10001);
		SetName("_alissa");
		SetBody(height: 0.1f, weight: 1.3f, upper: 1.3f, lower: 1.4f);
		SetFace(skinColor: 19, eyeType: 10, eyeColor: 148, mouthType: 2);
		SetStand("human/female/anim/female_natural_stand_npc_alissa");
		SetLocation(1, 15765, 31015, 120);
		SetGiftWeights(beauty: 1, individuality: 2, luxury: -1, toughness: 2, utility: 2, rarity: 0, meaning: -1, adult: 2, maniac: -1, anime: 2, sexy: 0);

		EquipItem(Pocket.Face, 3900, 0x00596131, 0x00FFEEC6, 0x006F0017);
		EquipItem(Pocket.Hair, 3143, 0x00D57527, 0x00D57527, 0x00D57527);
		EquipItem(Pocket.Armor, 15654, 0x00DECDB0, 0x006C7553, 0x009B9E7B);
		EquipItem(Pocket.Shoe, 17012, 0x00693F1E, 0x00000000, 0x00000000);
		EquipItem(Pocket.Head, 18406, 0x00DECDB0, 0x00000000, 0x00000000);

		AddPhrase("Hmm... Ferghus must have made another mistake.");
		AddPhrase("How are you going to make flour without any wheat?");
		AddPhrase("La la la la.");
		AddPhrase("La la la, one leaf, la la la, two leaves.");
		AddPhrase("La la la, three leaves, la la la, fooour leaves.");
		AddPhrase("My sister needs to grow up...");
		AddPhrase("There's a guard at the wheat field, and I'm watching the Windmill.");
		AddPhrase("When is Caitin going to teach me how to bake bread?");
		AddPhrase("You can gather wheat at the wheat field.");

		DeactivateWindmill();
	}

	protected override async Task Talk()
	{
		SetBgm("NPC_Alissa.mp3");

		await Intro(L("A young girl stands with her hands on her hips like she's a person of great importance.<br/>She wears a worn out hat that frames her soft hair, round face, and button nose.<br/>As she stands there, you notice that her apron is actually too big and she's discreetly trying to keep it from slipping.<br/>In spite of all that, her cherry eyes sparkle with curiosity."));

		Msg("So, what can I do for you?", Button("Start a Conversation", "@talk"), Button("Operate the Windmill", "@windmill"));

		switch (await Select())
		{
			case "@talk":
				Greet();
				Msg(Hide.Name, GetMoodString(), FavorExpression());

				var today = ErinnTime.Now.ToString("yyyyMMdd");
				if (today != Player.Vars.Perm["alissa_title_gift"])
				{
					switch (Player.Title)
					{
						case 10060: // is a friend of Deian
						case 10062: // is a friend of Nora
							Msg(L("Yay! Welcome back, <username/>.<br/>I was expecting you."));

							Player.Vars.Perm["alissa_title_gift"] = today;

							Player.GiveItem(71021); // Berry
							Player.Notice(L("Received Berry from Alissa."));
							Player.SystemMsg(L("Received Berry from Alissa."));

							Msg(L("I wanted to give you this..."));
							break;
					}
				}

				if (Player.IsUsingTitle(11001))
				{
					Msg("You rescued the Goddess?<br/>You?");
					Msg("Umm... Well, I guess I'll have to take your word for it.<br/>So then, can you say that our world has become a paradise?");
					Msg("Well, will I still need to look after my big sister in this paradise?<br/>Oh... I see... Right. Paradox. It's a paradox.");
				}
				else if (Player.IsUsingTitle(11002))
				{
					Msg("Huh? <username/>...<br/>You're the Guardian of Erinn?<br/>When did this happen...?<br/>Even I can see that there's something different about you.");
				}

				await Conversation();
				break;

			case "@windmill":
				if (WindmillActive)
				{
					Msg("The Mill is already working.");
					break;
				}

				Msg("How long do you want to use the Mill?<br/>It's 100 Gold for one minute and 450 Gold for 5 minutes.<br/>Once it starts working, anyone can use the Mill.", Button("1 Minute", "@1minute"), Button("5 Minutes", "@5minute"), Button("Forget It", "@quit"));

				switch (await Select())
				{
					case "@1minute":
						BuyWindmill(100, 1);
						break;

					case "@5minute":
						BuyWindmill(450, 5);
						break;

					case "@quit":
						Msg("Whatever, it's your choice...<br/>Just remember that this is the only place where you can grind your crops into flour.");
						break;
				}
				break;
		}

		End();
	}

	private void Greet()
	{
		if (Memory <= 0)
		{
			Msg(FavorExpression(), L("Hello, we haven't met. My name is <npcname/>. Your name is <username/>, right?<br/>How did I know?<br/>Haha, it's written above your head. Don't tell me you don't see it?"));
		}
		else if (Memory == 1)
		{
			Msg(FavorExpression(), L("Hey, you're back. Hmm...wasn't your name different last time?<br/>Maybe I saw it wrong."));
		}
		else if (Memory == 2)
		{
			Msg(FavorExpression(), L("I've missed you...hehe. If you have nothing to do, chat with me!"));
		}
		else if (Memory <= 6)
		{
			Msg(FavorExpression(), L("You come here pretty often.<br/>It's 'cause you like me huh? Hehe!"));
		}
		else
		{
			Msg(FavorExpression(), L("Hey. It's <username/>! Let's play. Please?"));
		}

		UpdateRelationAfterGreet();
	}

	protected override async Task Keywords(string keyword)
	{
		switch (keyword)
		{
			case "personal_info":
				if (Memory >= 15 && Favor >= 50 && Stress <= 5)
				{
					Msg(FavorExpression(), "Did you know that you're an awesome person?<br/>Actually... Nevermind, forget I just said that.");
					ModifyRelation(Random(2), 0, Random(2));
				}
				else if (Memory >= 15 && Favor >= 30 && Stress <= 5)
				{
					Msg(FavorExpression(), "I like Caitin so much.<br/>She's pretty, kind...<br/>And always gives me some of her delicious food when I visit.<br/>And she also teaches me how to cook.");
					Msg("I wonder why she doesn't have a boyfriend.<br/>These boys don't know what they're missing.");
					ModifyRelation(Random(2), 0, Random(2));
				}
				else if (Favor >= 10 && Stress <= 10)
				{
					Msg(FavorExpression(), "I want to be an awesome baker when I grow up.<br/>So it's important that I know all the ingredients for baking bread.<br/>I know it's not going to be easy, but you'll see one day...<br/>I can't wait to become as good as Caitin, hehe.");
					ModifyRelation(Random(2), Random(2), Random(2));
				}
				else if (Favor <= -10)
				{
					Msg(FavorExpression(), "When I look at the Windmill,<br/>I'm amazed by the power of wind.<br/>What do you think, <username/>?");
					ModifyRelation(Random(2), 0, Random(1, 3));
				}
				else if (Favor <= -30 && Stress <= 10)
				{
					Msg(FavorExpression(), "<username/>... I get the feeling<br/>that you're looking down on me because I'm younger than you.<br/>I don't like that...");
					ModifyRelation(Random(2), 0, Random(1, 3));
				}
				else if (Favor <= -30 && Stress > 10)
				{
					Msg(FavorExpression(), "Eh... I don't feel like talking. Forget it.");
					ModifyRelation(Random(2), -Random(2), Random(1, 4));
				}
				else
				{
					Player.GiveKeyword("school");
					Msg(FavorExpression(), "My name? I am <npcname/>.<br/>I work here at the mill, helping around with chores.<br/>Have you seen my sister? She's at the School.<br/>If you happen to go there, go inside the left building.<br/>She'll be in the magic class.");
					ModifyRelation(Random(2), 0, Random(3));
				}
				break;

			case "rumor":
				if (Memory >= 15 && Favor >= 50 && Stress <= 5)
				{
					Msg(FavorExpression(), "I don't know if you already know this...<br/>Ranald is in love with Priestess Endelyon.<br/>And everybody knows Trefor has a crush on Dilys.<br/>And Malcolm...when he's near Nora, his face just gets red and he can't say a word.<br/>Is it that hard to just confess your love?");
					Msg("Anyhow, I shouldn't be gossiping about their love problems. It's none of my business.");
					ModifyRelation(Random(2), 0, Random(2));
				}
				if (Memory >= 15 && Favor >= 30 && Stress <= 5)
				{
					Msg(FavorExpression(), "Did you know? My sister's hair. She dyed it.<br/>She sometimes goes to the city to get her hair done.<br/>Then she comes home with some expensive hair dye.");
					Msg("She can afford all that expensive dye for herself, but never buys me anything.<br/>Sometimes I wonder if she's really my sister.<br/>Does she even remember that I work here at the Mill? *sob*");
					ModifyRelation(Random(2), 0, Random(2));
				}
				if (Favor >= 10 && Stress <= 10)
				{
					Msg(FavorExpression(), "Tell me, <username/>... Who's your favorite person in this town?");
					ModifyRelation(Random(2), Random(2), Random(2));
				}
				else if (Favor <= -10)
				{
					Msg(FavorExpression(), "Deian?<br/>Well, he's not bad.<br/>But you know...he's really immature!<br/>Don't tell him that I said that though.<br/>I mean, it's not that big a deal...");
					ModifyRelation(Random(2), 0, Random(1, 3));
				}
				else if (Favor <= -30 && Stress <= 10)
				{
					Msg(FavorExpression(), "Have you met Tracy, the lumberjack?<br/>He comes here sometimes, and calls me an old lady trapped in a kid's body. Argh!<br/>He has no common sense... Doesn't understand a girl's heart...<br/>I wish someone will just pluck off his beard!");
					ModifyRelation(Random(2), 0, Random(1, 3));
				}
				else if (Favor <= -30 && Stress > 10)
				{
					Msg(FavorExpression(), "No matter how bored I am, I don't want to talk about that...");
					ModifyRelation(Random(2), -Random(2), Random(1, 4));
				}
				else
				{
					Player.GiveKeyword("shop_smith");
					Msg(FavorExpression(), "Ferghus?<br/>I don't know if he's a good blacksmith, but he's a nice person.<br/>Usually, when you ask him a question, he kindly answers everything...<br/>Go find out for yourself.");
					ModifyRelation(Random(2), 0, Random(3));
				}
				break;

			case "about_skill":
				if (!Player.HasSkill(SkillId.ProductionMastery))
				{
					Player.GiveSkill(SkillId.ProductionMastery, SkillRank.RF);
					Msg("Have you heard of the Production Mastery skill? Learning it will give you more Stamina and Dexterity,<br/>and you'll be more successful at gathering and crafting.");
					Msg("It's not a skill you have to activate manually, either. Simply gathering and crafting will trigger the skill,<br/>so it'll improve without any particular effort on your part.");
				}
				else
					Msg("Um... I only know about Production Mastery,<br/>you know.");
				break;

			case "shop_misc":
				Msg("Are you looking for Malcolm's General Shop?<br/>Take the bridge from here and go up the hill.<br/>I would take you, but I have to stay here and work...");
				break;

			case "shop_grocery":
				Msg("OK, so you're looking for Caitin?<br/>Her Grocery Shop is very close to the General Shop.<br/>She used to come here very often...<br/>But now, I hardly see her at all.");
				Msg("Sometimes I need someone to get a few things from Caitin,<br/>so talk to me about 'Regarding Jobs'.");
				break;

			case "shop_healing":
				Msg("Dilys's place? Go straight up the hill and follow the road.<br/>If you can't find it, ask someone nearby.<br/>Ahh, I think she's so pretty.<br/>I don't know why she's with Trefor.");
				break;

			case "shop_inn":
				Msg("Nora's Inn?<br/>It's...right in front of you, silly!<br/>You should read signs.<br/>Or at least read the NPC names.<br/>Even I know how to read... Hehe!");
				break;

			case "shop_bank":
				Msg("You mean Bebhinn's Bank?<br/>...<br/>You have that much money? You sure don't look it...");
				break;

			case "shop_smith":
				Msg("Ferghus? He's right in front of you. Can't you see?<br/>You should get glasses, thick ones...<br/>But, his beard smells...like alcohol, so get nose plugs too.");
				break;

			case "skill_rest":
				Msg("I want to learn that, too.<br/>It's tiring to stand all day.<br/>I heard Nora uses hotkeys to make it easier.<br/>I wonder why she doesn't teach it to me...");
				break;

			case "skill_range":
				Msg("Hmph...I'm disappointed!<br/>Can't you tell the difference between something you should say<br/>and something you shouldn't?<br/>Go and talk to Trefor or something.<br/>He's probably snooping around Dilys's house again.");
				break;

			case "skill_instrument":
				Player.GiveKeyword("lute");
				Msg("You can practice if you have a lute.<br/>Were you planning to learn the skill without an instrument?");
				break;

			case "skill_composing":
				Msg("If it were me, I would rather buy a book.<br/>I don't remember which book exactly, though.<br/>But I think you can buy it at Malcolm's General Shop.");
				break;

			case "skill_tailoring":
				Msg("Well, if I knew how, I would make my own clothes...<br/>If you ever learn how, make me an outfit, yea? Yea?");
				break;

			case "skill_magnum_shot":
				Msg("How dare you ask a girl that...<br/>What do I look like, huh!?");
				break;

			case "skill_counter_attack":
				Msg("It's so annoying to see all these 10 year olds bragging about how they hunted a bear.<br/>Why would they kill animals for fun?<br/>I admit it's tough...<br/>But it's such a senseless act...<br/>A very childish thing to be proud of.");
				break;

			case "skill_smash":
				Msg("...<br/>Why do boys like fighting so much? I don't like it.<br/>You're not like that, are you?");
				break;

			case "skill_gathering":
				Msg("You need the right tools to gather things.<br/>You need a Sickle to harvest wheat.<br/>You need a Gathering Knife to shear wool.<br/>You need an Axe to gather firewood.");
				Msg("In the end, nothing is free. You have to work for it.");
				break;

			case "square":
				Msg("You will see a big dip when you go up the hill.<br/>There will also be lots of people.<br/>You can't miss it.");
				break;

			case "pool":
				Msg("Hello. If it weren't for the Windmill, there would be no water in the reservoir.<br/>Do you see how important my job is?");
				break;

			case "farmland":
				Msg("Right. You can harvest wheat from there. Of course, you need permission first.<br/>Then you can grind the crops into flour here.");
				Msg("But, watch out...<br/>You can't see what's in front of you at the field...<br/>And no one will be there to help you if you accidentally trip and fall.<br/>Be careful.");
				break;

			case "windmill":
				Msg("The Windmill is right in front of you. It was built by Ferghus.<br/>Be careful. If it weren't for me watching over it, it would've broken down by now.");
				break;

			case "brook":
				Msg("It's the stream right in front of you.<br/>The Windmill is also used to send water up to the reservoir.<br/>Isn't the water so clear? Hehe...");
				break;

			case "shop_headman":
				Msg("Chief Duncan's beard feels very soft and nice.<br/>Even better than wool.<br/>Try feeling it next time. I always do!");
				break;

			case "temple":
				Msg("Priest Meven is boring because he doesn't have a beard.<br/>You don't really care about him?<br/>I know...it's Priestess Endelyon you're after, right?");
				break;

			case "school":
				Msg("By the way, did I tell you?<br/>My sister teaches magic at the School.<br/>I don't know if she's any good though...hehe...");
				Msg("You know what else? She is crazy about school uniforms.<br/>Her wardrobe is full of them!");
				Msg("Why does a teacher need to wear a uniform?");
				break;

			case "skill_windmill":
				Msg("You can't just operate a windmill.<br/>You need skills. Someone who really knows how. Hehe...<br/>Maybe...someone by the name of <npcname/>?<br/>If you see someone saying they want to learn the Windmill skill,<br/>bring them to me! I'll show them!");
				break;

			case "skill_campfire":
				Msg("Deian boasted to me that he could do it<br/>but he never showed me.<br/>He's always like that though...");
				break;

			case "shop_restaurant":
				Msg("Are you hungry? If I weren't working I would make you some food...<br/>I have this secret recipe I got from Caitin!");
				break;

			case "shop_armory":
				Msg("If you're looking for weapons, Ferghus at the Blacksmith's Shop sells them...<br/>...<br/>As long as it's brand new, it should be okay...");
				break;

			case "shop_cloth":
				Msg("Talking about the Clothing Shop makes me want to go shopping...<br/>My sister is so cheap.<br/>I spent all my money to help her with her tuition...<br/>and she won't even buy me a single dress!<br/>Look at me. Even my apron is worn out...");
				Msg("I don't even know where she spends all the money she earns from her lessons and textbooks.<br/>What about me...?");
				break;

			case "shop_bookstore":
				Player.GiveKeyword("school");
				Msg("The Bookstore? Ah, you're looking for a place to buy books?<br/>Well, I don't know if we have one in town.<br/>My sister sells some books at the School...<br/>But it's not exactly a bookstore...");
				break;

			case "shop_goverment_office":
				Msg("It's so weird. We don't have anything like that here.<br/>But everyone always asks about the Town Office.<br/>Maybe I am the only one who doesn't know about it.<br/>Is it just me?");
				break;

			case "graveyard":
				Msg("You're so mean. It's too scary to go there by myself.");
				break;

			case "skill_fishing":
				Msg("Fishing?<br/>Umm... People around here fish at the Reservoir over there.<br/>I went up there with Deian before. He said he'd show me how to fish,<br/>but he always caught some weird things instead.<br/>...I don't know. He doesn't seem to be able to do anything right.");
				break;

			case "bow":
				Msg("You should ask Ferghus those questions, not me.");
				break;

			case "lute":
				Msg("I think Malcolm sells lutes!<br/>But, I've never seen him play one.<br/>He would be so popular with the girls if he did.");
				break;

			case "complicity":
				Msg("Hmm... Do you mean the Windmill blades?<br/>Probably not, right?");
				break;

			case "tir_na_nog":
				Player.GiveKeyword("temple");
				Msg("Tir Na Nog? I remember Endelyon talked to me about that place before...<br/>If I remember correctly, she said it's a place of eternal happiness and life.<br/>If you want to know more about it, you should go to Church.");
				Msg("It's not that far from here. Just go around the farmland, and it should be right there.");
				break;

			case "mabinogi":
				Msg("Mabi... nogi? Yes, my grandma used to tell me stories about that.<br/>I remember she used to say Mabinogi ends right here.");
				Msg("I thought it was just an old bedtime story...<br/>Guess I was wrong.");
				break;

			case "musicsheet":
				Msg("I heard you need it in order to compose.<br/>But I don't really know much about it. Maybe someone who has all the time in the world would...");
				break;

			default:
				if (Memory >= 15 && Favor >= 30 && Stress <= 5)
				{
					Msg(FavorExpression(), "Oh. So that's what happened. I never knew.");
					ModifyRelation(0, 0, Random(2));
				}
				else if (Favor >= 10 && Stress <= 10)
				{
					Msg(FavorExpression(), "Haha... I don't know anything about that...");
					ModifyRelation(0, 0, Random(2));
				}
				else if (Favor <= -10)
				{
					Msg(FavorExpression(), "Jeez... Stop showing off! Whatever... I know that too.");
					ModifyRelation(0, 0, Random(4));
				}
				else if (Favor <= -30)
				{
					Msg(FavorExpression(), "Stop using hard words! Are you making fun of me? I warn you, I'll cry...");
					ModifyRelation(0, 0, Random(5));
				}
				else
				{
					RndFavorMsg(
						"You're not testing me, are you?",
						"You're expecting too much from me.",
						"Perhaps Caitin might know. Well... Anyway...",
						"Ah... well, I don't know anything about that.",
						"Eh... It feels like you're treating me like a child.",
						"Hmm... I think Ferghus would be able to explain it better. He's across the stream."
					);
					ModifyRelation(0, 0, Random(3));
				}
				break;
		}
	}

	protected override async Task Gift(Item item, GiftReaction reaction)
	{
		switch (reaction)
		{
			case GiftReaction.Love:
				RndMsg(
					"Wow. I wasn't expecting this.<br/>Thank you!",
					"Are you really giving me this?<br/>Thank you!",
					"Thank you.<br/>Lassar should learn from you.<br/>You care more about me than she does!"
				);
				break;

			// everything seems to be neutral for her except likeability pots
			default:
				RndMsg(
					"Now I've got more gifts than Deian!",
					"I guess I am quite popular, getting gifts like this all the time. Hehe...",
					"I am so sorry... I don't have anything to give you. <br/>Is it really okay to just accept this?",
					"To tell the truth, I was a little bit jealous when Caitin and Deian told me they got gifts from other people... <br/>I guess I didn't need to feel jealous after all, haha."
				);
				break;
		}
	}

	protected void BuyWindmill(int gold, int minutes)
	{
		if (Gold < gold)
		{
			Msg("You don't have enough money. I'm sorry, you can't use it for free.");
			return;
		}

		Gold -= gold;
		ActivateWindmill(minutes);

		RndMsg(
			"Okay!<br/>Anyone can use the Mill now for the next " + minutes + " minutes.<br/>I'm counting, haha.",
			"Yay! I got some pocket money!"
		);
	}

	protected void ActivateWindmill(int minutes)
	{
		if (WindmillActive)
			return;

		WindmillActive = true;

		WindmillProp.State = "on";
		WindmillProp.Xml.SetAttributeValue("EventText", Player.Name + " has activated the Windmill. Anybody can use it now to grind crops into flour.");

		SetTimeout(minutes * 60 * 1000, DeactivateWindmill);

		Send.PropUpdate(WindmillProp);
	}

	protected void DeactivateWindmill()
	{
		WindmillActive = false;

		WindmillProp.State = "off";
		WindmillProp.Xml.SetAttributeValue("EventText", "The Mill is currently not in operation.\nOnce you operate it, you can grind the crops into flour.");

		Send.PropUpdate(WindmillProp);
	}
}
