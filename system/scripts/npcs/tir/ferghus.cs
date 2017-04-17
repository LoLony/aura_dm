//--- Aura Script -----------------------------------------------------------
// Ferghus
//--- Description -----------------------------------------------------------
// The Blacksmith/Weapons Dealer in Southeast Tir
//---------------------------------------------------------------------------

public class FerghusScript : NpcScript
{
	public override void Load()
	{
		SetRace(10002);
		SetName("_ferghus");
		SetBody(height: 1.1f, upper: 1.4f, lower: 1.1f);
		SetFace(skinColor: 23, eyeType: 3, eyeColor: 112, mouthType: 4);
		SetStand("human/male/anim/male_natural_stand_npc_Ferghus_retake");
		SetLocation(1, 18075, 29960, 80);
		SetGiftWeights(beauty: -1, individuality: 0, luxury: 0, toughness: 2, utility: 1, rarity: 1, meaning: 0, adult: 0, maniac: 0, anime: 0, sexy: 2);

		EquipItem(Pocket.Face, 4950, 0x00C6C561, 0x00E1C210, 0x00E8A14A);
		EquipItem(Pocket.Hair, 4153, 0x002E303F, 0x002E303F, 0x002E303F);
		EquipItem(Pocket.Armor, 15650, 0x001F2340, 0x00988486, 0x009E9FAC);
		EquipItem(Pocket.Shoe, 17283, 0x0077564A, 0x00F2A03A, 0x008A243D);
		EquipItem(Pocket.RightHand1, 40024, 0x00808080, 0x00212121, 0x00808080);

		AddPhrase("(Spits out a loogie)");
		AddPhrase("Beard! Oh, beard! A true man never forgets how to grow a beard, yeah!");
		AddPhrase("How come they are so late? I've been expecting armor customers for hours now.");
		AddPhrase("Hrrrm");
		AddPhrase("I am running out of Iron Ore. I guess I should wait for more.");
		AddPhrase("I feel like working while singing songs.");
		AddPhrase("I probably did too much hammering yesterday. Now my arm is sore.");
		AddPhrase("I really need a pair of bellows... The sooner the better.");
		AddPhrase("Ouch, I yawned too big. I nearly ripped my mouth open!");
		AddPhrase("Scratching");
		AddPhrase("What am I going to make today?");
	}

	protected override async Task Talk()
	{
		SetBgm("NPC_Ferghus.mp3");

		await Intro(L("His bronze complexion shines with the glow of vitality. His distinctive facial outline ends with a strong jaw line covered with dark beard.<br/>The first impression clearly shows he is a seasoned blacksmith with years of experience.<br/>The wide-shouldered man keeps humming with a deep voice while his muscular torso swings gently to the rhythm of the tune."));

		Msg("Welcome to my Blacksmith's Shop.", Button("Start a Conversation", "@talk"), Button("Shop", "@shop"), Button("Repair Item", "@repair"), Button("Modify Item", "@upgrade"));

		switch (await Select())
		{
			case "@talk":
				Greet();
				Msg(Hide.Name, GetMoodString(), FavorExpression());

				if (Player.IsUsingTitle(11001))
				{
					Msg("...Hmm... Such a boast should be made in front of Priest Meven.<br/>If you'd like, I'll tell you one more thing.");
					Msg("There's no need to seek out any Goddesses.<br/>Your mother is the true Goddess.");
					Msg("...Be a good child and honor your mother.");
				}
				else if (Player.IsUsingTitle(11002))
				{
					Msg("Hm... <username/>, the Guardian of Erinn?<br/>If you want, I could guard your weapons.");
					Msg("...If you have any weapons that<br/>have become dull, I'll take care of it...");
				}

				await Conversation();
				break;

			case "@shop":
				Msg("Looking for a weapon?<br/>Or armor?");
				OpenShop("FerghusShop");
				return;

			case "@repair":
				Msg("If you want to have armor, kits or weapons repaired, you've come to the right place.<br/>I sometimes make mistakes, but I offer the best deal for repair work.<br/>For rare and expensive items, I think you should go to a big city. I can't guarantee anything.<br/><repair rate='90' stringid='(*/smith_repairable/*)' />");

				while (true)
				{
					var repair = await Select();

					if (!repair.StartsWith("@repair"))
						break;

					var result = Repair(repair, 90, "/smith_repairable/");
					if (!result.HadGold)
					{
						RndMsg(
							"Haha. You don't have enough Gold to repair that.",
							"Well, you have to bring more money to have it fixed.",
							"Do you have the Gold?"
						);
					}
					else if (result.Points == 1)
					{
						if (result.Fails == 0)
							RndMsg(
								"Alright! 1 Point repaired!",
								"Durability rose 1 point.",
								"Finished 1 point repair."
							);
						else
							Msg("Hmm... The repair didn't go well. Sorry..."); // Should be 3
					}
					else if (result.Points > 1)
					{
						if (result.Fails == 0)
							RndMsg(
								"Alright! It's perfectly repaired.",
								"Here, full repair is done.",
								"It's repaired perfectly."
							);
						else
							// TODO: Use string format once we have XML dialogues.
							Msg("Repair is over.<br/>Unfortunately, I made " + result.Fails + " mistake(s).<br/>Only " + result.Successes + " point(s) got repaired.");
					}
				}

				Msg("<repair hide='true'/>By the way, do you know you can bless your items with the Holy Water of Lymilark?<br/>I don't know why but I make fewer mistakes<br/>while repairing blessed items. Haha.");
				Msg("Well, come again when you have items to fix.");
				break;

			case "@upgrade":
				Msg("Will you select items to be modified?<br/>The number and types of modifications are different depending on the items.<br/>When I modify them, my hands never slip or make mistakes. So don't worry. Trust me.<upgrade />");

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
				Msg("If you have something to modify, let me know anytime.<upgrade hide='true'/>");
				break;
		}

		End("Goodbye, <npcname/>. I'll see you later!");
	}

	private void Greet()
	{
		if (Player.IsDoingPtjFor(NPC))
		{
			Msg(FavorExpression(), L("Hey, part-timer!<br/>You're not just lounging around, are you? Haha."));
		}
		else if (Memory <= 0)
		{
			Msg(FavorExpression(), L("Are you new here? Good to see you."));
		}
		else if (Memory == 1)
		{
			Msg(FavorExpression(), L("Have we met? You look familiar to me."));
		}
		else if (Memory == 2)
		{
			Msg(FavorExpression(), L("What's up, <username/>?<br/>You are <username/>, right?"));
		}
		else if (Memory <= 6)
		{
			Msg(FavorExpression(), L("Good to see you, <username/>."));
		}
		else
		{
			Msg(FavorExpression(), L("Hey, regular! My dear good ol' customer!"));
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
					Msg(FavorExpression(), "In fact, this shop is my family business. My dad used to work here.<br/>When I was young, I didn't understand anything. At that time, I hated my father for being a blacksmith.<br/>I don't know why I did that. I really shouldn't have. So many regrets...");
					ModifyRelation(Random(2), 0, Random(2));
				}
				else if (Memory >= 15 && Favor >= 30 && Stress <= 5)
				{
					Msg(FavorExpression(), "Honestly, the only pleasure I get these days is to step out for a second and have a drink.<br/>When I have some time to spare, I sit out here and look at the sunset.<br/>With a drink made by Caitin's mom in my hand.<br/>Now that's what I call pleasure.");
					ModifyRelation(Random(2), 0, Random(2));
				}
				else if (Favor >= 10 && Stress <= 10)
				{
					Msg(FavorExpression(), "It's a bit difficult to keep up the orders these days. I was actually thinking of hiring someone.<br/>Will you work with me?<br/>Haha, just joking. I don't want to hire people randomly either.");
					ModifyRelation(Random(2), Random(2), Random(2));
				}
				else if (Favor <= -10)
				{
					Msg(FavorExpression(), "What was your name? I don't recall. Sorry.");
					ModifyRelation(Random(2), 0, Random(1, 3));
				}
				else if (Favor <= -30 && Stress <= 10)
				{
					Msg(FavorExpression(), "You're acting like you're a friend of mine. A bit awkward...");
					ModifyRelation(Random(2), 0, Random(1, 3));
				}
				else if (Favor <= -30 && Stress > 10)
				{
					Msg(FavorExpression(), "Hmm... This is complicated. Let's talk later.");
					ModifyRelation(Random(2), -Random(2), Random(1, 4));
				}
				else
				{
					Player.GiveKeyword("shop_smith");
					Msg(FavorExpression(), "I'm the blacksmith of Tir Chonaill. We'll see each other often, <username/>.");
					ModifyRelation(Random(2), 0, Random(3));
				}
				break;

			case "rumor":
				if (Memory >= 15 && Favor >= 50 && Stress <= 5)
				{
					Msg(FavorExpression(), "Priest Meven might be a bit stubborn sometimes,<br/>but I do like him.<br/>He once came to me with a bottle of wine he made himself,<br/>and asked me to join him for a drink.");
					ModifyRelation(Random(2), 0, Random(2));
				}
				else if (Memory >= 15 && Favor >= 30 && Stress <= 5)
				{
					Msg(FavorExpression(), "From time to time, Caitin brings me a bottle of wine her mom makes.<br/>I know she's ill. That makes me feel sorry since I can imagine how much effort she has to put in.<br/>But at the same time,<br/>I'm really grateful that she remembers a guy like me.");
					ModifyRelation(Random(2), 0, Random(2));
				}
				else if (Favor >= 10 && Stress <= 10)
				{
					Msg(FavorExpression(), "Sometimes it's much better to make a new tool<br/>than fix a broken one.<br/>But I know people don't want to throw away their old tools,<br/>and that's why I try to fix them for the owners.");
					Msg("You know the more these tools are used, the better and more valuable they become.");
					ModifyRelation(Random(2), Random(2), Random(2));
				}
				else if (Favor <= -10)
				{
					Msg(FavorExpression(), "Bebhinn sometimes teases people,<br/>but I don't think she's a bad girl.");
					ModifyRelation(Random(2), 0, Random(1, 3));
				}
				else if (Favor <= -30 && Stress <= 10)
				{
					Msg(FavorExpression(), "I know there's a rumor that some people don't really like me.<br/>I'm trying to be careful, too.<br/>If you see or hear what others say about me, please share it with me.");
					ModifyRelation(Random(2), 0, Random(1, 3));
				}
				else if (Favor <= -30 && Stress > 10)
				{
					Msg(FavorExpression(), "You just love talking about others, don't you?");
					ModifyRelation(Random(2), -Random(2), Random(1, 4));
				}
				else
				{
					Player.GiveKeyword("windmill");
					Msg(FavorExpression(), "The wind around Tir Chonaill is very strong. It even breaks the windmill blades.<br/>And I'm the one to fix them.<br/>Malcolm's got some skills,<br/>but I'm the one who deals with iron.");
					Msg("I made those extra blades out there just in case.<br/>When the Windmill stops working, it's really inconvenient around here.<br/>It's always better to be prepared, isn't it?");
					ModifyRelation(Random(2), 0, Random(3));
				}
				break;

			case "about_skill":
				Player.GiveKeyword("skill_fishing");
				Msg("Hmm... Well, <username/>, since you ask,<br/>I might as well answer you. Let's see.<br/>Fishing. Do you know about the Fishing skill?");
				Msg("I'm not sure about the details, but<br/>I've seen a lot of people fishing up there.<br/>I'm not sure if fishing would be considered a skill, though.");
				Msg("From what I've seen, all you need is<br/>a Fishing Rod and a Bait Tin.");
				break;

			case "shop_misc":
				Player.GiveKeyword("shop_smith");
				Msg("This is the Blacksmith's Shop. Surprisingly, many people think they are at the General Shop.");
				Msg("Let me tell you the biggest difference between Malcolm and me.<br/>He sells all kinds of stuff for your everyday life,<br/>but I, the best smithy in town, make metal stuff, you know.<br/>Like weapons, for example.");
				Msg("If you insist, I'll show you the way to the General Shop.<br/>Walk across the bridge, and go up the hill to the Square.");
				break;

			case "shop_grocery":
				Msg("The Grocery Store?<br/>The cute and lovely Caitin works there. Have you met her?<br/>She looks busy these days,<br/>but she cooks food for the people in town when she's got some time.");
				Msg("What? You want to go there? Go up to the Square.");
				break;

			case "shop_healing":
				Msg("I once hurt my hand while making a piece of armor.<br/>It really hurt and I went to Dilys. She cured it in a second!<br/>If you're not feeling well, you should go see her.<br/>Walk past the Square and you'll find her.");
				break;

			case "shop_inn":
				Msg("You may lose your items or even get ill<br/>if you sleep outside with no protection.<br/>If you sleep in the Inn, you don't have to worry about things like that at least.<br/>You will see Nora right in front of the Inn. Do you want to talk to her?");
				break;

			case "shop_bank":
				Msg("The Bank can be quite handy. They store your items, not just Gold.<br/>You'll probably use it a lot.<br/>The clerk at the Bank, Bebhinn, can help you in many ways if you get to know her.<br/>I'm not lying.");
				Msg("The Bank is near the Square. So you have to go up there first...<br/>Do you follow me?");
				break;

			case "shop_smith":
				Msg("Yes, this is the Blacksmith's Shop.");
				Msg("Alright, alright. You can't wait to equip yourself...<br/>But no need to rush.<br/>When you get an item, select it in your inventory, and then a slot for that item will blink.<br/>Then you just equip it. There's nothing no reason to rush.");
				Msg("What? You want to buy an item? Then you should have pressed 'Shop' instead<br/>of having this chat with me.");
				break;

			case "skill_range":
				Msg("I think Ranald knows better.<br/>Why don't you ask him directly?<br/>It's true I make good bows.<br/>But making it and using it is totally different, you know.");
				break;

			case "skill_instrument":
				Msg("Looks like you like music a lot,<br/>but I don't think I can help you with that.<br/>You know, I'm a blacksmith. I've never played any instruments before.");
				break;

			case "skill_composing":
				Player.GiveKeyword("temple");
				Msg("You want to write music?<br/>Priestess Endelyon at the Church<br/>knows a bit about composing, I think.<br/>You can talk to her.");
				Msg("She's such a nice lady.<br/>I'm sure she'll help you a lot.");
				break;

			case "skill_tailoring":
				Player.GiveKeyword("shop_misc");
				Msg("Did you buy a Tailoring Kit? You can buy one at the General Shop.<br/>I know nothing about Tailoring, but I do know you need a Tailoring Kit and Fabric to make clothes.<br/>Just like a blacksmith needs an Anvil and a Bellows.");
				break;

			case "skill_magnum_shot":
				Msg("Oh! you are interested in the Magnum Shot.<br/>Welcome. Even if you know how to use it,<br/>you have to have a bow and some arrows.<br/>If you need some, come and see me.");
				Msg("Oh, you already have one?<br/>Then you're not here to buy anything?<br/>If not, why don't you talk with Trefor<br/>or Ranald at the School.");
				Msg("I don't think I can help.");
				break;

			case "skill_counter_attack":
				Msg("Melee Counterattack skill?<br/>Have you talked to Ranald? He's at the School.<br/>What about Trefor?");
				Msg("When it comes to combat skills,<br/>you'd better talk with them. They will tell you useful stories.<br/>Chief Duncan was once a warrior.<br/>Perhaps he can give you some tips from his experience.");
				break;

			case "skill_smash":
				Player.RemoveKeyword("skill_smash");
				Msg("Did you use the Smash skill?<br/>It is like a double-edged sword. Its weakness is as big as its strength. Better use it carefully.");
				break;

			case "skill_gathering":
				Msg("The most critical aspect of successful gathering is<br/>tools, tools and tools.<br/>Let's say there's a big forest full of giant trees.<br/>You can't do anything without an axe.");
				Msg("What if there was a big flock of sheep with top quality wool?<br/>Without scissors or a knife,<br/>you can't even get a string of wool from them.");
				Msg("You got the point?<br/>Then why don't you go through my stock<br/>and pick a tool?");
				Msg("Don't you want it? Then, never mind. haha.");
				break;

			case "square":
				Msg("Haha. You must have missed it.<br/>It's nearly impossible to miss the Square.<br/>I think you need to keep your eyes open.");
				Msg("The Square farther within the town,<br/>right next to the big tree.<br/>You can see it even from that hill.");
				break;

			case "pool":
				Player.GiveKeyword("brook");
				Msg("The reservoir is not on this side.<br/>Cross the Adelia Stream out there,<br/>take a left, and then go straight.");
				Msg("When you think you're close to the School, that's where it is.");
				break;

			case "farmland":
				Player.GiveKeyword("brook");
				Msg("The farmland? Then you're on the wrong side.<br/>Cross the Adelia Stream,<br/>and follow the path to the left.");
				break;

			case "windmill":
				Msg("It's near the entrance of the town. Little lady Alissa works there.<br/>The Windmill pulls water from the stream to the reservoir.<br/>It's also used to grind crops.");
				Msg("Better be careful near the mill. It can be dangerous.");
				break;

			case "brook":
				Msg("Adelia Stream?<br/>That's flowing right there. Right in front of my shop.<br/>Have you seen any stream<br/>other than that one in this town?");
				break;

			case "shop_headman":
				if (Player.IsHuman)
				{
					Msg("You are looking for the Chief's House?<br/>His house is near the Square.<br/>Did you come straight down here<br/>without dropping by his place?");
					Msg("Then you didn't read the Quest Scroll?<br/>No, no. You should've read that.");
					Msg("Someone worked hard to create that scroll.<br/>Read it and do what it says. You've got nothing to lose.<br/>haha.");
				}
				else
				{
					Msg("You're looking for the Chief's house?<br/>The Chief's house is right next to the town square...<br/>Don't come toward here from the town square,<br/>but go up the other way. Hehehe...");
				}
				break;

			case "temple":
				Msg("Church is a bit far from here.<br/>Can you see it in your Minimap?<br/>No? Then I'll explain.<br/>First, go to the Square.");
				Msg("And then, look at your Minimap again.");
				Msg("Hmm... I think that's better than going through a long explanation.<br/>You're not upset, right?");
				break;

			case "school":
				Player.GiveKeyword("farmland");
				Msg("Did you ask because you want to know the location of the School?<br/>Then I will give you an answer.<br/>Cross the bridge first,<br/>and there's a road. Just go to the left until you see the farmland.");
				Msg("If you pass the farmland, the School is very near you.<br/>The School gate is pretty big so you can't miss it.");
				Msg("When you get there, can you tell Ranald<br/>we should get a drink together?<br/>Lassar must not find out about it, alright?");
				break;

			case "skill_windmill":
				Msg("Hmm... Are you talking about the Windmill?");
				Msg("It's at the entrance of the town.<br/>Drawing water from the stream and filling up the reservoir.<br/>Also used for grinding crops.<br/>Better stay alert around it. It can be quite dangerous.");
				Msg("Ah... You are talking about a skill name, not the Windmill.");
				break;

			case "skill_campfire":
				Msg("What? Campfire skill?<br/>I remember Deian came here and borrowed some tools,<br/>saying he would build a fire.<br/>Now I know why he needed them. haha.");
				Msg("By the way, have you met Deian?<br/>He is a clumsy boy. I'm sure he's having a hard time.<br/>Why don't you go and help him out?");
				break;

			case "shop_restaurant":
				Msg("Hmm...<br/>Are you looking for a restaurant?<br/>Sorry, but there are no restaurants in Tir Chonaill. But we have a grocery store.");
				Msg("Speaking of... I could use some booze now.");
				break;

			case "shop_armory":
				Msg("...");
				Msg("Haha,<br/>are you joking around with me?<br/>I'm sure my Blacksmith's Shop comes before the Weapons Shop<br/>in the Information Memo.");
				break;

			case "shop_cloth":
				Msg("Oh, you want to get new clothes?<br/>There's no clothing shop or anything like that here,<br/>but you can buy some at the General Shop.");
				Msg("You know what? Many people in Erinn spend their fortune on clothes, accessories and stuff.<br/>I think it's because they can wear whatever they buy here<br/>without worrying about their age or size.");
				break;

			case "shop_bookstore":
				Msg("You must be interested in books.<br/>Sorry to say this, but... we don't have a bookstore in this town.<br/>If you want to buy books,<br/>go to Lassar. I think she sells some books.");
				Msg("But don't expect too much.<br/>She probably sells expensive spellbooks. What else could she have?");
				break;

			case "shop_goverment_office":
				Msg("Town Office?<br/>I think you expect too much<br/>from a small town like Tir Chonaill.");
				Msg("If you need some help,<br/>go see the Chief.");
				break;

			case "graveyard":
				Player.GiveKeyword("shop_headman");
				Msg("The graveyard is near the Chief's House.<br/>Walk to the north of his house and you'll see it.<br/>Several days ago, I came home and slept like a log after drinking.<br/>But it turns out I slept in the graveyard, not in my bed!");
				Msg("It was a bit chilly and more than a little creepy! But it was fun too.<br/>If there were no spiders, I could have a real good drinking binge there.");
				break;

			case "skill_fishing":
				Msg("Based on what I've seen, all you need<br/>is a Fishing Rod and a Bait Tin in each hand.");
				break;

			case "bow":
				Player.RemoveKeyword("bow");
				Player.RemoveKeyword("skill_range");
				Msg("Ha, ha. You are looking for bows. You came to the right place.<br/>I certainly have bows. In fact, you know what?<br/>This is a great chance to get your own bow!<br/>By the way, you know that you need arrows too, right?<br/>I mean, what can we do with just a bow and a string?<br/>Play with it?");
				break;

			case "lute":
				Player.GiveKeyword("shop_misc");
				Msg("Malcolm's General Shop sells lutes.<br/>He also sells... Um...<br/>What was that called? Ukul... something.");
				break;

			case "complicity":
				Msg("Um... An instigator? Well...");
				break;

			case "tir_na_nog":
				Msg("Hmm... You're interested in legends too?<br/>I'm not such a good storyteller.<br/>You can ask the Chief over there,<br/>or go to Priest Meven.");
				break;

			case "mabinogi":
				Msg("A bard's song? Oh, good.<br/>You can enjoy music much better with drinks.<br/>What do you think?");
				Msg("Hmm... But minors aren't supposed to drink...");
				break;

			case "musicsheet":
				Player.GiveKeyword("shop_misc");
				Msg("If you are looking for Music Scores, you came too far down.<br/>Malcolm's General Shop is near the Square.<br/>Looks like someone wasted their time, haha.");
				break;

			default:
				if (Memory >= 15 && Favor >= 30 && Stress <= 5)
				{
					Msg(FavorExpression(), "Speaking of which, I don't think I ever paid much attention to that.<br/>I need to think about it for a while.");
					ModifyRelation(0, 0, Random(2));
				}
				else if (Favor >= 10 && Stress <= 10)
				{
					Msg(FavorExpression(), "I am sorry. I have never been interested in such things.");
					ModifyRelation(0, 0, Random(2));
				}
				else if (Favor <= -10)
				{
					Msg(FavorExpression(), "I don't have time to waste with you. There's work to do.");
					ModifyRelation(0, 0, Random(4));
				}
				else if (Favor <= -30)
				{
					Msg(FavorExpression(), "You enjoy these talks?");
					ModifyRelation(0, 0, Random(5));
				}
				else
				{
					RndFavorMsg(
						"?",
						"*Yawn* I don't know.",
						"Haha. I have no idea.",
						"That's not my concern.",
						"I don't know, man. That's just out of my league."
					);
				}
				break;
		}
	}

	protected override async Task Gift(Item item, GiftReaction reaction)
	{
		switch (reaction)
		{
			case GiftReaction.Love:
				Msg(L("Haha. I like you."));
				break;

			case GiftReaction.Like:
				Msg(L("Oh, you know who the truly skilled blacksmith is!"));
				break;

			case GiftReaction.Neutral:
				Msg(L("Hmm... I know what you're up to.<br/>You are trying to win my favor and receive discounts, right?"));
				break;

			case GiftReaction.Dislike:
				Msg(L("What do you want me to do with this?"));
				break;
		}
	}
}

public class FerghusShop : NpcShopScript
{
	public override void Setup()
	{
		Add("Weapon", 40003);      // Short Bow
		Add("Weapon", 40005);      // Short Sword
		Add("Weapon", 40006);      // Dagger
		Add("Weapon", 40007);      // Hatchet
		Add("Weapon", 40022);      // Gathering Axe
		Add("Weapon", 40023);      // Gathering Knife
		Add("Weapon", 40024);      // Blacksmith Hammer
		Add("Weapon", 40025);      // Pickaxe
		Add("Weapon", 40026);      // Sickle
		Add("Weapon", 40027);      // Weeding Hoe
		Add("Weapon", 45001, 100); // Arrow x100
		Add("Weapon", 45001, 20);  // Arrow x20
		Add("Weapon", 45002, 50);  // Bolt x50
		Add("Weapon", 45002, 200); // Bolt x200
		Add("Weapon", 46001);      // Round Shield

		Add("Shoes && Gloves", 16000); // Leather Gloves
		Add("Shoes && Gloves", 16004); // Studded Bracelet
		Add("Shoes && Gloves", 16008); // Cores' Thief Gloves
		Add("Shoes && Gloves", 16014); // Lorica Gloves
		Add("Shoes && Gloves", 17001); // Ladies Leather Boots
		Add("Shoes && Gloves", 17005); // Hunter Boots
		Add("Shoes && Gloves", 17014); // Leather Shoes
		Add("Shoes && Gloves", 17015); // Combat Shoes
		Add("Shoes && Gloves", 17016); // Field Combat Shoes
		Add("Shoes && Gloves", 17020); // Thief Shoes
		Add("Shoes && Gloves", 17021); // Lorica Sandals

		Add("Helmet", 18503); // Cuirassier Helm

		Add("Armor", 14001); // Light Leather Mail (F)
		Add("Armor", 14003); // Studded Cuirassier
		Add("Armor", 14004); // Cloth Mail
		Add("Armor", 14008); // Full Leather Armor Set
		Add("Armor", 14010); // Light Leather Mail (M)

		Add("Event");

		if (IsEnabled("FighterJob"))
		{
			Add("Weapon", 40179); // Spiked Knuckle
			Add("Weapon", 40180); // Hobnail Knuckle
			Add("Weapon", 40244); // Bear Knuckle
		}

		if (IsEnabled("PuppetMasterJob"))
		{
			Add("Weapon", 40745); // Basic Control Bar
		}

		if (IsEnabled("Ninjajob"))
		{
			Add("Weapon", 40841); // Spiral Shuriken
		}
	}
}
