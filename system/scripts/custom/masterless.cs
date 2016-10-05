//--- Aura Script -----------------------------------------------------------
// Masterless
//--- Description -----------------------------------------------------------
// A man who gave it all up for power. He can teach you to become a Paladin.
//---------------------------------------------------------------------------

public class MasterlessScript : NpcScript
{
	public override void Load()
	{
		SetRace(10002);
		SetName("Masterless");
		SetFace(skinColor: 21, eyeType: 190, eyeColor: 176, mouthType: 2);
		SetLocation(27, 3206, 4351, 60);

		EquipItem(Pocket.Face, 4909, 0x00000015, 0x00000000, 0x00000000);
		EquipItem(Pocket.Hair, 4158, 0x10000008, 0x00000000, 0x00000000);
		EquipItem(Pocket.Armor, 210009, 0x00A58E74, 0x00A58E74, 0x00000000);
		EquipItem(Pocket.Glove, 16537, 0x00A58E74, 0x00000000, 0x00000000);
		EquipItem(Pocket.Shoe, 17510, 0x00A58E74, 0x00000000, 0x00000000);
		EquipItem(Pocket.Head, 18518, 0x00A58E74, 0x00000000, 0x00000000);
		EquipItem(Pocket.RightHand1, 40907, 0x00A58E74, 0x00A58E74, 0x00000000);


		AddPhrase("This place isn't safe, you know.");
		AddPhrase("You should think twice about entering Albey Dungeon.");
		AddPhrase("You dare challenge me?!");
	}

	protected override async Task Talk()
	{
		await Intro(L("A golden knight stands before you, his gaze fixed upon the stone mural.<br/>After a second, he turns to you, and his voice bellows out from the armor."));

		Msg("Greetings, wayward one...", Button("Start a Conversation", "@talk"), Button("Shop", "@shop"), Button("Duel", "@duel"));

		switch (await Select())
		{
			case "@talk":
				Greet();
				Msg(Hide.Name, GetMoodString(), FavorExpression());

				await Conversation();
				break;

			case "@shop":
				Msg("All I sell are these potions used to further your abilities...<br/>They are quite pricey- such power doesn't come free, after all...");
				OpenShop("MasterlessShop");
				return;

			case "@duel":
				Msg("So, you would like to duel me, eh?<br/>I tell you what... for 100 coins, I'll duel you, and if you win, I'll teach you the mystical art of transformation.<br/>You'll need Red Coins for the powers of darkness, and Blue Coins for the power of light.<br/>What do you say, <username/>? Do we have a deal?", Button("Yes", "@yes"), Button("No", "@no"));
				switch (await Select()) {
					case "@yes":
						Msg("Very well then... what will you choose?<br/>The powers of darkness, or the powers of light?", Button("Light", "@light"), Button("Darkness", "@dark"));
						switch (await Select()) {
							case "@dark":
								if (HasItem(52033, 100) && !HasQuest(333333)) // 100 Red Coins and not running the Paladin Quest
								{
									RemoveItem(52033, 100);
									StartQuest(333334);	//Battle for Darkness quest
									Msg("Very well, child of darkness... I look forward to our duel.<br/>Meet me in Alby Dungeon... drop a Silver Gem on the altar to find me.");
								}
								else
								{
									Msg("Alas... you don't have enough coins.<br/>Come back to me again when you have 100 Red Coins...");
								}
								return;
							case "@light":
								if (HasItem(52032, 100) && !HasQuest(333334)) // 100 Blue Coins and not running the Dark Knight Quest
								{
									RemoveItem(52032, 100);
									StartQuest(333333);	//Battle for Light quest
									Msg("Very well, child of light... I look forward to our duel.<br/>Meet me in Alby Dungeon... drop a Silver Gem on the altar to find me.");
								}
								else
								{
									Msg("Alas... you don't have enough coins.<br/>Come back to me again when you have 100 Blue Coins...");
								}
								return;
						}
						return;
					case "@no":
						Msg("That's too bad... it would have been quite the battle, hahaha.<br/>Do come back if you change your mind, though. I look forward to it...");
						return;
				}
				break;
		}

		End("Goodbye, <npcname/>.");
	}

	private void Greet()
	{
		if (Memory <= 0)
		{
			Msg(FavorExpression(), L("I don't believe we've met before...<br/>I am... Masterless."));
		}
		else if (Memory == 1)
		{
			Msg(FavorExpression(), L("Greetings once again, <username/>. What brings you back here?"));
		}
		else if (Memory == 2)
		{
			Msg(FavorExpression(), L("Ah, <username/>. I am glad you've survived this long."));
		}
		else if (Memory <= 6)
		{
			Msg(FavorExpression(), L("I am glad you've returned, <username/>. Where have your travels taken you?"));
		}
		else
		{
			Msg(FavorExpression(), L("It has been a long time, <username/>. I'm glad you're well."));
		}

		UpdateRelationAfterGreet();
	}

	protected override async Task Keywords(string keyword)
	{
		switch (keyword)
		{
			/* case "personal_info":
				GiveKeyword("shop_smith");
				Msg(FavorExpression(), "I'm the blacksmith of Tir Chonaill. We'll see each other often, <username/>.");
				ModifyRelation(Random(2), 0, Random(3));
				break;

			case "rumor":
				GiveKeyword("windmill");
				Msg(FavorExpression(), "The wind around Tir Chonaill is very strong. It even breaks the windmill blades.<br/>And I'm the one to fix them.<br/>Malcolm's got some skills,<br/>but I'm the one who deals with iron.");
				Msg("I made those extra blades out there just in case.<br/>When the Windmill stops working, it's really inconvenient around here.<br/>It's always better to be prepared, isn't it?");
				ModifyRelation(Random(2), 0, Random(3));
				break;

			case "about_skill":
				GiveKeyword("skill_fishing");
				Msg("Hmm... Well, <username/>, since you ask,<br/>I might as well answer you. Let's see.<br/>Fishing. Do you know about the Fishing skill?");
				Msg("I'm not sure about the details, but<br/>I've seen a lot of people fishing up there.<br/>I'm not sure if fishing would be considered a skill, though.");
				Msg("From what I've seen, all you need is<br/>a Fishing Rod and a Bait Tin.");
				break;

			case "shop_misc":
				GiveKeyword("shop_smith");
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
				GiveKeyword("temple");
				Msg("You want to write music?<br/>Priestess Endelyon at the Church<br/>knows a bit about composing, I think.<br/>You can talk to her.");
				Msg("She's such a nice lady.<br/>I'm sure she'll help you a lot.");
				break;

			case "skill_tailoring":
				GiveKeyword("shop_misc");
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
				RemoveKeyword("skill_smash");
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
				GiveKeyword("brook");
				Msg("The reservoir is not on this side.<br/>Cross the Adelia Stream out there,<br/>take a left, and then go straight.");
				Msg("When you think you're close to the School, that's where it is.");
				break;

			case "farmland":
				GiveKeyword("brook");
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
				GiveKeyword("farmland");
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
				GiveKeyword("shop_headman");
				Msg("The graveyard is near the Chief's House.<br/>Walk to the north of his house and you'll see it.<br/>Several days ago, I came home and slept like a log after drinking.<br/>But it turns out I slept in the graveyard, not in my bed!");
				Msg("It was a bit chilly and more than a little creepy! But it was fun too.<br/>If there were no spiders, I could have a real good drinking binge there.");
				break;

			case "skill_fishing":
				Msg("Based on what I've seen, all you need<br/>is a Fishing Rod and a Bait Tin in each hand.");
				break;

			case "bow":
				RemoveKeyword("bow");
				RemoveKeyword("skill_range");
				Msg("Ha, ha. You are looking for bows. You came to the right place.<br/>I certainly have bows. In fact, you know what?<br/>This is a great chance to get your own bow!<br/>By the way, you know that you need arrows too, right?<br/>I mean, what can we do with just a bow and a string?<br/>Play with it?");
				break;

			case "lute":
				GiveKeyword("shop_misc");
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
				GiveKeyword("shop_misc");
				Msg("If you are looking for Music Scores, you came too far down.<br/>Malcolm's General Shop is near the Square.<br/>Looks like someone wasted their time, haha.");
				break;
			*/
			default:
				RndMsg(
					"I don't see why I should concern myself with such frivelous things.",
					"It matters not to me.",
					"What makes you think I care about this?",
					"I've never heard anything of this across all my travels."
				);
				ModifyRelation(0, 0, Random(3));
				break;
		}
	}
}

public class MasterlessShop : NpcShopScript
{
	public override void Setup()
	{
		Add("Training Potion", 91219, 1, 250000);      // Spirit of Order Training Potion
		Add("Training Potion", 91220, 1, 250000);      // Soul of Chaos Training Potion
	}
}
