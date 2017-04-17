//--- Aura Script -----------------------------------------------------------
// Nao
//--- Description -----------------------------------------------------------
// First NPC players encounter. Also met on every rebirth.
// Located on an unaccessible map, can be talked to from anywhere,
// given the permission.
//---------------------------------------------------------------------------

public class NaoScript : NpcScript
{
	int privateStoryCount;

	public override void Load()
	{
		SetRace(1);
		SetId(MabiId.Nao);
		SetName("_nao");
		SetLocation(22, 6013, 5712);
	}

	protected override async Task Talk()
	{
		SetBgm("Nao_talk.mp3");

		await Intro(L("A beautiful girl in a black dress with intricate patterns.<br/>Her deep azure eyes remind everyone of an endless blue sea full of mystique.<br/>With her pale skin and her distinctively sublime silhouette, she seems like she belongs in another world."));

		if (!Player.HasEverEnteredWorld)
			await FirstTime();
		else if (Player.CanReceiveBirthdayPresent)
			await Birthday();
		else
			await Rebirth();
	}

	private async Task FirstTime()
	{
		await Introduction();
		await Questions();
		// Destiny/Talent...
		await EndIntroduction();
	}

	private async Task Introduction()
	{
		if (Player.IsMale)
			Msg(L("Hello, there... You are <username/>, right?<br/>I have been waiting for you.<br/>It's good to see a gentleman like you here."));
		else
			Msg(L("Hello, there... You are <username/>, right?<br/>I have been waiting for you.<br/>It's good to see a lady like you here."));

		Msg(L("My name is Nao.<br/>It is my duty to lead pure souls like yours to Erinn."));
	}

	private async Task Questions()
	{
		Msg(L("<username/>, we have some time before I guide you to Erinn.<br/>Do you have any questions for me?"), Button(L("No"), "@no"), Button(L("Yes"), "@yes"));
		if (await Select() != "@yes")
			return;

		while (true)
		{
			var msg = Rnd(
				L("If there is something you'd like to know more of, please ask me now."),
				L("Do not hesitate to ask questions. I am more than happy to answer them for you."),
				L("If you have any questions before heading off to Erinn, please feel free to ask.")
			);

			Msg(msg,
				Button(L("End Conversation"), "@endconv"),
				List(L("Talk to Nao"), 4, "@endconv",
					Button(L("About Mabinogi"), "@mabinogi"),
					Button(L("About Erinn"), "@erinn"),
					Button(L("What to do?"), "@what"),
					Button(L("About Adventures"), "@adventures")
				)
			);

			switch (await Select())
			{
				case "@mabinogi":
					Msg(L("Mabinogi can be defined as the songs of bards, although in some cases, the bards themselves are referred to as Mabinogi.<br/>To the residents at Erinn, music is a big part of their lives and nothing brings joy to them quite like music and Mabinogi.<br/>Once you get there, I highly recommend joining them in composing songs and playing musical instruments."));
					break;

				case "@erinn":
					Msg(L("Erinn is the name of the place you will be going to, <username/>.<br/>The place commonly known as the world of Mabinogi is called Erinn.<br/>It has become so lively since outsiders such as yourself began to come."));
					Msg(L("Some time ago, adventurers discovered a land called Iria,<br/>and others even conquered Belvast Island, between the continents.<br/>Now, these places have become home to adventurers like yourself, <username/>.<p/>You can go to Tir Chonaill of Uladh now,<br/>but you should try catching a boat from Uladh and<br/>crossing the ocean to Iria or Belvast Island."));
					break;

				case "@what":
					Msg(L("That purely depends on what you wish to do.<br/>You are not obligated to do anything, <username/>.<br/>You set your own goals in life, and pursue them during your adventures in Erinn.<p/>Sure, it may be nice to be recognized as one of the best, be it the most powerful, most resourceful, etc., but <br/>I don't believe your goal in life should necessarily have to be becoming 'the best' at everything.<br/>Isn't happiness a much better goal to pursue?<p/>I think you should experience what Erinn has to offer <br/>before deciding what you really want to do there."));
					break;

				case "@adventures":
					Msg(L("There are so many things to do and adventures to go on in Erinn.<br/>Hunting and exploring dungeons in Uladh...<br/>Exploring the ruins of Iria...<br/>Learning the stories of the Fomors in Belvast...<p/>Explore all three regions to experience brand new adventures!<br/>Whatever you wish to do, <username/>, if you follow your heart,<br/>I know you will become a great adventurer before you know it!"));
					break;

				default:
					return;
			}
		}
	}

	private async Task EndIntroduction()
	{
		Msg(L("Are you ready to take the next step?"));
		Msg(L("You will be headed to Erinn right now.<br/>Don't worry, once you get there, someone else is there to take care of you, my little friend by the name of Tin.<br/>After you receive some pointers from Tin, head Northeast and you will see a town."));
		Msg(L("It's a small town called Tir Chonaill.<br/>I have already talked to Chef Duncan about you, so all you need to do is show him the letter of introduction I wrote right here."), Image("tir_chonaill"));
		Msg(L("You can find Chief Duncan on the east side of the Square.<br/>When you get there, try to find a sign that says 'Chief's House'."), Image("npc_duncan"));
		Msg(L("I will give you some bread I have personally baked, and a book with some information you may find useful.<br/>To see those items, open your inventory once you get to Erinn."));
		Msg(Hide.Both, L("(Received a Bread and a Traveler's Guide from Nao.)"), Image("novice_items"));
		Msg(L("I wish you the best of luck in Erinn.<br/>See you around."), Button(L("End Conversation")));
		await Select();

		// Move to Uladh Beginner Area
		Player.SetLocation(125, 21489, 76421);
		Player.Direction = 233;

		Player.GiveItem(1000, 1);  // Traveler's Guide
		Player.GiveItem(50004, 1); // Bread

		// Add keyword, so players can't possibly get dyes without rebirth.
		Player.GiveKeyword("tutorial_present");

		Close();
	}

	private async Task Rebirth()
	{
		Msg(L("Hello, <username/>!<br/>Is life here in Erinn pleasant for you?"));

		if (!IsEnabled("Rebirth"))
		{
			// Unofficial
			Msg(L("I'm afraid I can't let you rebirth just yet, the gods won't allow it."));
			goto L_End;
		}

		if (!RebirthAllowed())
		{
			Msg(L("Barely any time has passed since your last rebirth.<br/>Why don't you enjoy your current life in Erinn for a bit longer?"));
			goto L_End;
		}

		Msg(L("If you wish, you can abandon your current body and be reborn into a new one, <username/>."));

		while (true)
		{
			Msg(L("Feel free to ask me any questions you have about rebirth.<br/>Once you've made up your mind to be reborn, press Rebirth."),
				Button("Rebirth"), Button("About Rebirths"), Button("Cancel"));

			switch (await Select())
			{
				case "@rebirth":
					Msg("<rebirth style='-1'/>");
					switch (await Select())
					{
						case "@rebirth":
							for (int i = 1; i < 10; ++i)
								Player.RemoveKeyword("Tin_ColorAmpul_" + i);
							Player.RemoveKeyword("tutorial_present");

							Player.Vars.Perm["EverRebirthed"] = true;

							// Old:
							//   Msg("Would you like to be reborn with the currently selected features?<br/><button title='Yes' keyword='@rebirthyes' /><button title='No' keyword='@rebirthhelp' />");
							//   Msg("<username/>, you have been reborn with a new appearance.<br/>Did you enjoy having Close Combat as your active Talent?<br/>Would you like to choose a different active Talent for this life?<button title='New Talent' keyword='@yes' /><button title='Keep Old Talent' keyword='@no' />");
							//   Msg("Then I will show you the different Talents available to you.<br/>Please select your new active Talent after you consider everything.<talent_select />")
							//   Msg("You have selected Close Combat.<br/>May your courage and skill grow.<br/>I will be cheering you on from afar.");
							Close(Hide.None, L("May your new appearance bring you happiness!<br/>Though you'll be different when next we meet,<br/>but I'll still be able to recognize you, <username/>.<p/>We will meet again, right?<br/>Until then, take care."));
							return;

						default:
							goto L_Cancel;
					}
					break;

				case "@about_rebirths":
					await RebirthAbout();
					break;

				default:
					goto L_Cancel;
			}
		}

	L_Cancel:
		Msg(L("There are plenty more opportunities to be reborn.<br/>Perhaps another time.") + "<rebirth hide='true'/>");

	L_End:
		Close(Hide.None, L("Until we meet again, then.<br/>I wish you the best of luck in Erinn.<br/>I'll see you around."));
	}

	private async Task RebirthAbout()
	{
		while (true)
		{
			Msg(L("When you rebirth, you will be able to have a new body.<br/>Aside from your looks, you can also change your age and starting location.<br/>Please feel free to ask me more."),
				Button(L("What is Rebirth?"), "@whatis"), Button(L("What changes after a Rebirth?"), "@whatchanges"), Button(L("What does not change after a Rebirth?"), "@whatnot"), Button(L("Done")));

			switch (await Select())
			{
				case "@whatis":
					Msg(L("You can choose a new body between the age of 10 and 17.<br/>Know that you won't receive the extra 7 AP just for being 17,<br/>as you did at the beginning of your journey.<br/>You will keep the AP that you have right now."));
					Msg(L("Also, your Level and Exploration Level will reset to 1.<br/>You'll get to keep all of your skills from your previous life, though."));
					Msg(L("You'll have to<br/>start at a low level for the Exploration Quests,<br/>but I doubt that it will be an issue for you."));
					Msg(L("If you wish, you can even just change your appearance<br/>without resetting your levels or your age.<br/>Just don't select the 'Reset Levels and Age' button<br/>to remake yourself without losing your levels."), Image("Rebirth_01_c2", true, 200, 200));
					Msg(L("You can even change your gender<br/>by clicking on 'Change Gender and Look.'<br/>If you want to maintain your current look, then don't select that button."), Image("Rebirth_02_c2", true, 200, 200));
					Msg(L("You can choose where you would like to rebirth.<br/>Choose between Tir Chonaill, Qilla Base Camp,<br/>or the last location you were at<br/>in your current life."), Image("Rebirth_03", true, 200, 200));
					break;

				case "@whatchanges":
					Msg(L("You can choose a new body between the ages of 10 and 17.<br/>though you won't receive the extra 7 AP just for being 17<br/>as you did at the beginning of your journey."));
					Msg(L("You'll keep all the AP that you have right now<br/>and your level will reset to 1.<br/>You'll keep all of your skills from your previous life, though."));
					Msg(L("If you wish, you can even change your appearance without<br/>resetting your levels or your age.<br/>Just don't select the 'Reset Levels and Age' button,<br/>and you'll be able to remake yourself without losing your current levels."), Image("Rebirth_01", true));
					Msg(L("You can even change your gender by selecting 'Change Gender and Look.'<br/>If you want to keep your current look, just don't select that button."), Image("Rebirth_02", true));
					Msg(L("Lastly, if you would like to return to your last location,<br/>select 'Move to the Last Location'.<br/>Otherwise, you'll be relocated to the Forest of Souls<br/>near Tir Chonaill."));
					break;

				case "@whatnot":
					Msg(L("First of all, know that you cannot change the<br/>name you chose upon entering Erinn.<br/>Your name is how others know you<br/>even when all else changes."));
					Msg(L("<username/>, you can also bring all the knowledge you'd earned<br/>in this life into your next one.<br/>Skills, keywords, remaining AP, titles, and guild will all be carried over.<br/>The items you have and your banking information will also remain intact."));
					break;

				default:
					return;
			}
		}
	}

	private bool RebirthAllowed()
	{
		var player = (PlayerCreature)Player;
		return (player.LastRebirth + ChannelServer.Instance.Conf.World.RebirthTime < DateTime.Now);
	}

	private async Task Birthday()
	{
		Player.GiveItem(CreateRandomBirthdayGift());
		Player.Vars.Perm["NaoLastPresentDate"] = DateTime.Now.Date;

		// Unofficial
		Msg(L("Happy Birthday, <username/>! "));
		Msg(L("I have a little something for you on this special day,<br/>please accept it."));

		if (IsEnabled("NaoDressUp") && !Player.HasKeyword("present_to_nao"))
			Player.GiveKeyword("present_to_nao");

		await Conversation();

		Close(Hide.None, "Until we meet again, then.<br/>I wish you the best of luck in Erinn.<br/>I'll see you around.");
	}

	private Item CreateRandomBirthdayGift()
	{
		var potentialGifts = new[] { 12000, 12001, 12002, 12003, 12004, 12005, 12006, 12007, 12008, 12009, 12010, 12011, 12012, 12013, 12014, 12015, 12016, 12017, 12018, 12019, 12020, 12021, 12022, 12023 };

		var rndGift = potentialGifts.Random();
		var prefix = 0;
		var suffix = 0;

		// Enchant if it's the 20th birthday.
		if (Player.Age == 20)
		{
			if (Player.IsHuman)
			{
				switch (Random(18))
				{
					case 00: prefix = 20610; break; // Shiny
					case 01: prefix = 20710; break; // Posh
					case 02: prefix = 20810; break; // Well-groomed
					case 03: prefix = 20910; break;	// Holy
					case 04: prefix = 20911; break;	// Beautiful
					case 05: prefix = 20912; break;	// Resplendent
					case 06: suffix = 30410; break;	// Capricornus
					case 07: suffix = 30510; break;	// Sagittarius
					case 08: suffix = 30511; break;	// Aquarius
					case 09: suffix = 30512; break;	// Pisces
					case 10: suffix = 30610; break;	// Libra
					case 11: suffix = 30611; break;	// Scorpius
					case 12: suffix = 30710; break;	// Taurus
					case 13: suffix = 30711; break;	// Virgo
					case 14: suffix = 30911; break;	// Aries
					case 15: suffix = 30912; break;	// Cancer
					case 16: suffix = 31010; break;	// Gemini
					case 17: suffix = 31011; break;	// Leo
				}
			}
			else if (Player.IsElf)
			{
				switch (Random(18))
				{
					case 00: prefix = 20610; break; // Shiny
					case 01: prefix = 20710; break; // Posh
					case 02: prefix = 20810; break; // Well-groomed
					case 03: prefix = 20910; break;	// Holy
					case 04: prefix = 20911; break;	// Beautiful
					case 05: prefix = 20912; break;	// Resplendent
					case 06: suffix = 30413; break;	// Sundrop
					case 07: suffix = 30518; break;	// Violet
					case 08: suffix = 30519; break;	// Forget-me-not
					case 09: suffix = 30520; break;	// Rose
					case 10: suffix = 30621; break;	// Clover
					case 11: suffix = 30622; break;	// Sweet Pea
					case 12: suffix = 30721; break;	// Otter
					case 13: suffix = 30722; break;	// Lilly
					case 14: suffix = 31012; break;	// Cornflower
					case 15: suffix = 31013; break;	// Cosmos
					case 16: suffix = 30816; break;	// Marguerite
					case 17: suffix = 30817; break;	// Hyacinth
				}
			}
			else if (Player.IsGiant)
			{
				switch (Random(18))
				{
					case 00: prefix = 20610; break; // Shiny
					case 01: prefix = 20710; break; // Posh
					case 02: prefix = 20810; break; // Well-groomed
					case 03: prefix = 20910; break;	// Holy
					case 04: prefix = 20911; break;	// Beautiful
					case 05: prefix = 20912; break;	// Resplendent
					case 06: suffix = 31501; break;	// Freezing
					case 07: suffix = 31502; break;	// Frost
					case 08: suffix = 31503; break;	// Hurricane's
					case 09: suffix = 31504; break;	// Hail
					case 10: suffix = 31505; break;	// Sleet
					case 11: suffix = 31506; break;	// Whirlpool
					case 12: suffix = 31507; break;	// Earthquake's
					case 13: suffix = 31508; break;	// Downpour
					case 14: suffix = 31509; break;	// Blizzard's
					case 15: suffix = 31510; break;	// Thunder
					case 16: suffix = 31511; break;	// Tempest
					case 17: suffix = 31512; break;	// Snowfield
				}
			}
		}

		return Item.CreateEnchanted(rndGift, prefix, suffix);
	}

	protected override async Task Keywords(string keyword)
	{
		switch (keyword)
		{
			// Gifts and clothes
			// --------------------------------------------------------------

			case "present_to_nao":
				await KeywordPresentToNao();
				break;

			case "nao_cloth0":
				Msg(L("The old black clothes are better?<br/>Ok... from now on I will wear these clothes when we meet, <username/>."), Button(L("Thank you"), "@yes"), Button(L("It's not like that..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.BlackDress;
					Msg(L("Ok, if you think these are better, <username/>."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth1":
				Msg(L("Is it ok for me to accept these clothes?<br/>Alright, from now on I'll wear these clothes when I meet you, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.RuasDress;
					Msg(L("Ok, if you think it's better, <username/>..."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth2":
				Msg(L("Do you like this pink coat you gave me?<br/>Ok... from now on I will wear the pink coat when we meet again, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.PinkCoat;
					Msg(L("I understand, if you think these are better, <username/>..."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth3":
				Msg(L("Do you like this black coat you gave me?<br/>Ok... from now on I will wear the black coat when we meet again, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.BlackCoat;
					Msg(L("Ok. If you think it's better, <username/>...<br/>Since it's warm I also love the black coat."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth4":
				Msg(L("Do you like the spring dress better?<br/>Alright, from now on I'll wear the yellow spring dress just for you, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.YellowSpringDress;
					Msg(L("Ok. If you think it's better, <username/>...<br/>I also love the spring clothes, they are so cute."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth5":
				Msg(L("Do you like the spring dress better?<br/>Alright, from now on I'll wear the white spring dress just for you, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.WhiteSpringDress;
					Msg(L("Ok. If you think it's better, <username/>...<br/>I also love the spring clothes, they are so cute."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth6":
				Msg(L("Do you like the spring dress better?<br/>Alright, from now on I'll wear the pink spring dress just for you, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.PinkSpringDress;
					Msg(L("Ok. If you think it's better, <username/>...<br/>I also love the spring clothes, they are so cute."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth7":
				Msg(L("Do you think the explorer suit you gifted me are better?<br/>Ok then, <username/>, I'll wear the explorer suit for you when we meet again."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.ExplorerSuit;
					Msg(L("Ok. If you think it's better, <username/>..."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth8":
				Msg(L("Do you prefer the Iria Casual Look that you gave me as a present? Ok, from now on I will wear this outfit just for you, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.IriaCasualWear;
					Msg(L("Ok. If you think it's better, <username/>..."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_yukata":
				Msg(L("Do you like the Yukata clothing?<br/>Alright then, I will wear the Yukata outfit whenever we meet, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.Yukata;
					Msg(L("Ok. If you think it's better, <username/>..."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth_santa":
				Msg(L("During this time would you like me to wear the Santa Clothes?<br/>Ok then, I'll wear the Santa Suit whenever we meet, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.SantaSuit;
					Msg(L("Ok. If you think it's better, <username/>..."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth_summer":
				Msg(L("Do you like the White Dress you gave me before?<br/>Ok then, I'll be wearing a White Dress when we meet again, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.WhiteDress;
					Msg(L("Ok. If you think it's better, <username/>..."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth_kimono":
				Msg(L("Do you prefer the Kimono you gave me before?<br/>Alright, I'll wear that outfit just for you when we meet again, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.Kimono;
					Msg(L("Ok. If you think it's better, <username/>..."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth_summer_2008":
				Msg(L("Do you like the sky-blue dress that you gave me as a present?<br/>Ok then, I'll wear that dress just for you when we meet again, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.SkyBlueDress;
					Msg(L("Ok. If you think it's better, <username/>..."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth_shakespeare":
				Msg(L("Do you like the Playwright Costume you gave me before?<br/>Ok then, I'll be wearing it when we meet again, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.PlaywrightCostume;
					Msg(L("Ok. If you think it's better, <username/>..."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			case "nao_cloth_farmer":
				Msg(L("Do you like the Farming Outfit you gave me before?<br/>Ok then, I'll be wearing it when we meet again, <username/>."), Button(L("Thank you"), "@yes"), Button(L("Nevermind..."), "@no"));
				if (await Select() == "@yes")
				{
					Player.NaoOutfit = NaoOutfit.FarmingOutfit;
					Msg(L("Ok. If you think it's better, <username/>..."));
				}
				else
					Msg(L("<username/>, you must not be interested in my clothes..."));
				break;

			// Breast
			// http://mabination.com/threads/85165-quot-Breast-quot-and-other-keywords.
			// --------------------------------------------------------------

			case "nao_blacksuit":
				Player.GiveKeyword("breast");

				Msg(L("I really like these clothes.<br/>I think the skirt is sort of erotic but, despite the appearance, it's very comfortable.<br/>But...the chest is probably a bit tight."));
				break;

			case "breast":
				Player.RemoveKeyword("breast");

				Msg(L("Uhm... <username/>, this discussion is a little..."));
				Msg(Hide.Name, L("(Nao is blushing uncomfortably.)"));
				Msg(L("...<p/>......<p/>A long time ago my friends would poke fun at me for that like you...<br/>It makes those friends spring to mind.<br/>In some ways it's similar to those feelings after all..."));
				Msg(L("I don't think they had any ill intent when they said it.<br/>Honestly, because that was thought of me ever since I was a child,<br/>I had a complex about it."));
				Msg(L("Do you think that way too, <username/>?"), Button(L("They look big to me"), "@big"), Button(L("It is not like that"), "@notlikethat"), Button(L("I think it is adorable"), "@adorable"), Button(L("They are not all that big"), "@notbig"), Button(L("Can I touch them just once?"), "@touch"));

				switch (await Select())
				{
					case "@big":
						NPC.ModifyFavor(Player, -3);
						Msg(L("...<br/>You really do think that way, huh? Fuu......<br/>Even though I didn't fatten up in other places..."));
						break;

					case "@notlikethat":
						NPC.ModifyFavor(Player, +3);
						Msg(Hide.Name, L("(After hearing that, Nao smiled cutely while avoiding my eyes.)"));
						Msg(L("Thank you for giving me courage. There's nothing else to really say..."));
						break;

					case "@adorable":
						NPC.ModifyFavor(Player, +1);
						Msg(Hide.Name, L("(Nao looked surprised after hearing that.)"));
						Msg(L("Umm... r-really? Thank you. That made me a little more confident."));
						break;

					case "@notbig":
						NPC.ModifyFavor(Player, -7);
						Msg(Hide.Name, L("(After hearing that, Nao looked a little displeased and avoided my eyes.)"));
						Msg(L("I-is that so? ...sh-shall we stop this conversation now?"));
						break;

					case "@touch":
						NPC.ModifyFavor(Player, -10);
						Msg(L("Whaaa!! <username/>! What do you think you're saying!? Th-that could never happen!"));
						Msg(Hide.Name, L("(Nao looks really angry.)"));
						Msg(L("...<p/>...Ah, I got so upset, sorry... I went overboard, huh..."));
						break;
				}
				break;

			// Others
			// --------------------------------------------------------------

			case "personal_info":
				switch (privateStoryCount)
				{
					case 0:
						Msg(L("My full name is 'Nao Mariota Pryderi'.<br/>I know it is not the easiest name to pronounce.<br/>Don't worry, <username/>, you can just call me Nao."));
						break;

					case 1:
						Msg(L("If you right-click and drag your cursor during the conversation,<br/>you can view different angles. You are staring at me while we're talking,<br/>and honestly, it's a little embarrassing. Please roll down the<br/>mouse wheel to zoom out and take a few steps back."));
						break;

					case 2:
						Msg(L("I believe everyone should cultivate his or her own unique style instead of simply following trends.<br/>I'm not just talking about hair style or fashion. I'm talking about lifestyle.<br/>It's about doing what you want to do, in a style that's uniquely yours."));
						break;

					case 3:
						Player.GiveKeyword("nao_owl");
						Msg(L("I have a pet owl. He's a great friend that takes care of many things for me."));
						break;

					case 4:
						Msg(L("I love to exchange gifts.<br/>I can tell from the gift how the other person really thinks of me.<br/>The people in Erinn are very fond of exchanging gifts."));
						Msg(L("<username/>, what's your opinion on exchanging gifts with others?<br/>Do you like it?"), Button(L("Of course, I do!"), "@yes"), Button(L("I like receiving gifts."), "@receiving"), Button(L("If it is someone I like, then yes."), "@like"), Button(L("I think it is a waste."), "@waste"), Button(L("No, not really."), "@no"));

						switch (await Select())
						{
							case "@yes":
								Msg(L("Wow! I knew it! I think you and I have something in common.<br/>Personally, I feel that people from other worlds are<br/>generally not so used to the idea of exchanging gifts.<br/>I even heard it from some people that they<br/>were surprised to hear such a question."));
								break;

							case "@receiving":
							case "@like":
								Msg(L("That's an interesting answer. If you're currently with someone,<br/>then I definitely envy that lucky person.<br/>If not, then I sincerely hope you'll find someone soon."));
								break;

							case "@waste":
								Msg(L("What? Really? I am sorry. I shouldn't have asked."));
								break;

							case "@no":
								Msg(L("Oh, I see. But I'm sure you will change your mind<br/>if someone surprises you with an unexpected gift."));
								break;
						}
						break;

					case 5:
						Player.GiveKeyword("nao_friend");
						Msg(L("A few years ago, I was locked in a dungeon by the evil Fomor.<br/>I do not ever want to go near a dungeon now...<br/>I don't even want to think about it.<br/>Fortunately, a friend of mine rescued me from there."));
						Msg(L("Dungeons are very dark and dangerous, but some claim that they<br/>are some of the best places for training and adrenaline rush.<br/>The power of the evil Fomors can change a dungeon every time it's visited,<br/>but that is the exact reason why the daredevil adventurers who prefer constant<br/>changes are that much more attracted to dungeons."));
						break;

					case 6:
						Msg(L("Perhaps those that aspire to quickly become the most powerful usually<br/>end up unhappy and unsatisfied as their lust for power grows.<br/>My friend was one of those that constantly pursued limitless power,<br/>and he had it at the very end. Unfortunately,<br/>that was the seed that brought his downfall in the end.<p/>....."));
						break;

					case 7:
						Player.GiveKeyword("nao_blacksuit");
						Msg(L("There are some people who suspect I might be one of the Fomors because<br/>of my black dress. I mean, what I wear is none of their business,<br/>but someone even speculated that I was the messenger of death.<br/>Honestly, I felt really weird when I heard that."));
						Msg(L("These days, I don't even know who I am anymore.<br/>Maybe I really am one of them, you know."));
						Msg(L("...<p/>Please don't tell me you believe that..."));
						break;

					case 8:
						RndMsg(
							L("Hehe, that's funny."),
							L("I like it, thank you for caring about me."),
							L("It's a funny thing."),
							L("Yes..."),
							L("Umm, do you want to talk about something else?")
						);
						break;
				}

				if (privateStoryCount < 8)
					privateStoryCount++;

				break;

			// Default
			// --------------------------------------------------------------

			default:
				RndFavorMsg(
					L("Ummm...why don't we talk about something else?")
				);
				break;
		}
	}

	protected async Task KeywordPresentToNao()
	{
		Msg(L("You're giving me a present?<br/>I'm not that special...<br/>But, it'd be impolite to refuse. I'm very grateful.<p/>What kind of gift is it?"), SelectItem("Present for Nao", "Select a gift.", "*/nao_dress/*"));

		var selection = await Select();
		Item item = null;

		// If an item was selected.
		if (selection.StartsWith("@select:"))
		{
			var args = selection.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
			long itemEntityId;

			if (!long.TryParse(args[1], out itemEntityId))
			{
				Log.Error("NaoScript: Invalid item selection response '{0}'.", selection);
			}
			else
			{
				item = Player.Inventory.GetItem(itemEntityId);

				if (item == null)
					Log.Warning("NaoScript: Player '{0:X16}' (Account: {1}) tried to gift item they don't possess.", Player.EntityId, Player.Client.Account.Id);

				if (!item.HasTag("/nao_dress/"))
				{
					item = null;
					Log.Warning("NaoScript: Player '{0:X16}' (Account: {1}) tried to use an invalid item, without nao_dress tag.", Player.EntityId, Player.Client.Account.Id);
				}
			}
		}
		else
		{
			Msg(L("Thanks for feeling that way, at least..."));
			return;
		}

		// If no item selected or error.
		if (item == null)
		{
			Msg(L("Ah...I'm not getting anything..."));
			return;
		}

		// Nao's outfits are in the id range 80,000~80256, with the
		// first byte corresponding with the outfit id.
		var itemId = item.Info.Id;
		var outfit = (NaoOutfit)(item.Info.Id - 80000);

		switch (outfit)
		{
			case NaoOutfit.BlackDress:
				Player.GiveItem(80012); // White Dress

				Msg(L("Thank you very much. I usually wear the same clothes every day.<br/>As a token of gratitude, <username/>, I'll wear these clothes just for you.<br/>Thank you very much."));
				break;

			case NaoOutfit.RuasDress:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth1");

				Msg(L("Huh...? You're giving me this outfit?<br/>Thank you...they're very nice clothes.<br/>From now on, <username/>, I'll wear these clothes whenever I see you."));
				break;

			case NaoOutfit.PinkCoat:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth2");

				Msg(L("Thank you very much. Beautiful pink clothes...<br/>From now on, <username/>, I'll wear these clothes whenever I see you."));
				break;

			case NaoOutfit.BlackCoat:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth3");

				Msg(L("Thank you very much...it's a warm coat.<br/>From now on, <username/>, I'll be wearing this coat when we meet."));
				break;

			case NaoOutfit.YellowSpringDress:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth4");

				Msg(L("Thank you very much. These spring clothes are cute. I'm very happy.<br/>So, <username/>, when we meet again, I'll wear this outfit."));
				break;

			case NaoOutfit.WhiteSpringDress:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth5");

				Msg(L("Thank you very much. These spring clothes are cute. I'm very happy.<br/>So, <username/>, when we meet again, I'll wear this outfit."));
				break;

			case NaoOutfit.PinkSpringDress:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth6");

				Msg(L("Thank you very much. These spring clothes are cute. I'm very happy.<br/>So, <username/>, when we meet again, I'll wear this outfit."));
				break;

			case NaoOutfit.ExplorerSuit:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth7");

				Msg(L("Thank you very much...these explorer clothes are pretty.<br/>From now on, <username/>, I'll wear this when we meet."));
				break;

			case NaoOutfit.IriaCasualWear:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth8");

				Msg(L("Thank you very much... these clothes are cute.<br/>From now on, <username/>, I'll wear these when we meet."));
				break;

			case NaoOutfit.Yukata:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_yukata");

				Msg(L("Thank you very much.<br/>Are these clothes from your world, <username/>? Very pretty...<br/>From now on, <username/>, I'll wear these clothes when I see you."));
				break;

			case NaoOutfit.SantaSuit:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth_santa");

				Msg(L("Thank you very much. Are these what you wear at the time of christmas in your world <username/>? Very cute...<br/>When we meet from now on, <username/>,<br/>I'll be sure to wear these clothes you gifted to me."));
				break;

			case NaoOutfit.WhiteDress:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth_summer");

				Msg(L("Thank you very much. This white dress is nice.<br/>From now on, <username/>, I'll wear this when we meet."));
				break;

			case NaoOutfit.Kimono:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth_kimono");

				Msg(L("Thank you very much. It's a long-sleeved kimono.<br/>From now on, <username/>, I'll wear it when we meet."));
				break;

			case NaoOutfit.SkyBlueDress:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth_summer_2008");

				Msg(L("Thank you very much. It's a sky-blue dress.<br/>From now on, <username/>, I'll wear this when we meet."));
				break;

			case NaoOutfit.PlaywrightCostume:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth_shakespeare");

				Msg(L("Thank you very much. It's the clothes of a playwright.<br/>From now on, <username/>, I'll wear this when we meet."));
				break;

			case NaoOutfit.FarmingOutfit:
				Player.GiveKeyword("nao_cloth0");
				Player.GiveKeyword("nao_cloth_farmer");

				Msg(L("Thank you very much. It's a farmer's outfit.<br/>From now on, <username/>, I'll wear this when we meet."));
				break;

			default:
				Msg(L("(Error: Unknown outfit."));
				return;
		}

		Player.NaoOutfit = outfit;
		Player.RemoveItem(itemId);
	}
}
