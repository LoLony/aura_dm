//--- Aura Script -----------------------------------------------------------
// Endelyon's Church Part-Time Job
//--- Description -----------------------------------------------------------
// All quests used by Endelyon's PTJ, and a script to handle the PTJ
// via hooks.
//---------------------------------------------------------------------------

public class EndelyonPtjScript : GeneralScript
{
	const PtjType JobType = PtjType.Church;

	const int Start = 12;
	const int Report = 16;
	const int Deadline = 21;
	const int PerDay = 20;

	int remaining = PerDay;

	readonly int[] QuestIds = new int[]
	{
		502102, // Basic - 10 Barley
		502103, // Basic - 10 Wheat
		502105, // Basic - 15 Eggs
		502132, // Int - 15 Barley
		502133, // Int - 15 Wheat
		502135, // Int - 20 Eggs
		502162, // Adv - 20 Barley
		502163, // Adv - 20 Wheat
		502165, // Adv - 30 Eggs
	};

	public override void Load()
	{
		AddHook("_endelyon", "after_intro", AfterIntro);
		AddHook("_endelyon", "before_keywords", BeforeKeywords);
	}

	public async Task<HookResult> AfterIntro(NpcScript npc, params object[] args)
	{
		if (npc.Player.IsDoingPtjFor(npc.NPC) && ErinnHour(Report, Deadline))
		{
			await AboutArbeit(npc);
			return HookResult.Break;
		}

		return HookResult.Continue;
	}

	[On("ErinnMidnightTick")]
	public void OnErinnMidnightTick(ErinnTime time)
	{
		remaining = PerDay;
	}

	public async Task<HookResult> BeforeKeywords(NpcScript npc, params object[] args)
	{
		var keyword = args[0] as string;

		if (keyword == "about_arbeit")
		{
			await AboutArbeit(npc);
			await npc.Conversation();
			npc.End();

			return HookResult.End;
		}

		return HookResult.Continue;
	}

	public async Task AboutArbeit(NpcScript npc)
	{
		if (npc.Player.IsDoingPtjNotFor(npc.NPC))
		{
			npc.Msg(L("You have other things to do, right?<br/>If you need the Holy Water of Lymilark, can you come back after you are finished with your work?"));
			return;
		}

		if (npc.Player.IsDoingPtjFor(npc.NPC))
		{
			var result = npc.Player.GetPtjResult();

			if (!ErinnHour(Report, Deadline))
			{
				if (result == QuestResult.Perfect)
					npc.Msg(L("It seems you took care of your end of the bargain.<br/>I'm a little busy right now, but come back later so I can compensate you for your work."));
				else
					npc.Msg(L("How are you doing with the part-time job for the Church today?<br/>I have the utmost faith in you, <username/>."));
				return;
			}

			npc.Msg(L("Did you finish the part-time job I gave you?<br/>If you are done, you can report the results to me. Do you want to do so now?"), npc.Button(L("Report Now"), "@report"), npc.Button(L("Report Later"), "@later"));

			if (await npc.Select() != "@report")
			{
				npc.Msg(L("You don't want to report yet?<br/>Please make sure to come back and report before the deadline."));
				return;
			}

			if (result == QuestResult.None)
			{
				npc.Player.GiveUpPtj();

				npc.Msg(npc.FavorExpression(), L("I'm sorry,<br/>but I cannot give you the Holy Water of Lymilark unless you complete the task I've asked you to take care of.<br/>Please work harder next time."));
				npc.ModifyRelation(0, -Random(3), 0);
			}
			else
			{
				npc.Msg(L("Well done, <username/>. I feel very relieved thanks to you.<br/>In appreciation of all the hard work you've put in for the Church,<br/>I prepared some things for you.<br/>I'd love to give you all these if I could, but I can't. Please pick one."), npc.Button(L("Report Later"), "@later"), npc.PtjReport(result));
				var reply = await npc.Select();

				if (!reply.StartsWith("@reward:"))
				{
					npc.Msg(L("You don't want to report yet?<br/>Please make sure to come back and report before the deadline."));
					return;
				}

				npc.Player.CompletePtj(reply);
				remaining--;

				if (result == QuestResult.Perfect)
				{
					npc.Msg(npc.FavorExpression(), L("Thanks. You took care of everything I've asked for.<br/>As promised, I will give you the Holy Water of Lymilark."));
					npc.ModifyRelation(0, Random(3), 0);
				}
				else if (result == QuestResult.Mid)
				{
					npc.Msg(npc.FavorExpression(), L("It's a bit short of what I asked for,<br/>but I appreciate your help.<br/>I will give you the Holy Water of Lymilark in return."));
					npc.ModifyRelation(0, Random(1), 0);
				}
				else if (result == QuestResult.Low)
				{
					npc.Msg(npc.FavorExpression(), L("Did you run out of time?<br/>You completed only a portion of what I asked for.<br/>I'm sorry, but that's not good enough for me to give you the Holy Water of Lymilark."));
					npc.ModifyRelation(0, -Random(2), 0);
				}
			}
			return;
		}

		if (!ErinnHour(Start, Deadline))
		{
			npc.Msg(L("Are you willing to help the Church?<br/>It's a bit early, though. Please come back at a later time."));
			return;
		}

		if (!npc.Player.CanDoPtj(JobType, remaining))
		{
			npc.Msg(L("Today's part-time jobs are all taken.<br/>If you need some Holy Water of Lymilark, please come back tomorrow."));
			return;
		}

		var randomPtj = GetRandomPtj(npc.Player, JobType, QuestIds);
		var msg = "";

		if (npc.Player.GetPtjDoneCount(JobType) == 0)
			msg = L("Our Church is looking for a kind soul to help take care of our crops.<br/>The main job is to harvest wheat or barley from the farmland located south of the Church.<br/>One thing to note: because of our tight budget, we cannot afford to pay in gold.<p/>Instead, anyone who completes the job will receive some Holy Water of Lymilark,<br/>which can be used to bless items.<br/>Blessed items do not fall to the ground<br/>when its owner is knocked unconscious.<br/>Now, what do you say?");
		else
			msg = L("Are you here for the Holy Water of Lymilark again?<br/>Please take a look at today's part-time job and tell me if you want it.");

		npc.Msg(msg, npc.PtjDesc(randomPtj, L("Endelyon's Church Part-Time Job"), L("Looking for help with delivering goods to Church."), PerDay, remaining, npc.Player.GetPtjDoneCount(JobType)));

		if (await npc.Select() == "@accept")
		{
			npc.Msg(L("Thank you.<br/>Please take care of this on time."));
			npc.Player.StartPtj(randomPtj, npc.NPC.Name);
		}
		else
		{
			npc.Msg(L("If you don't want to do it, then I guess that's that."));
		}
	}
}

// Basic Barley quest
public class EndelyonBarleyBasicPtjScript : QuestScript
{
	public override void Load()
	{
		SetId(502102);
		SetName(L("Church Part-Time Job"));
		SetDescription(L("This task is to harvest grain from the farmland. Cut [10 bundles of barley] today. Barley can be harvested using a sickle on the farmlands around town."));

		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.ById);

		SetType(QuestType.Deliver);
		SetPtjType(PtjType.Church);
		SetLevel(QuestLevel.Basic);
		SetHours(start: 12, report: 16, deadline: 21);

		AddObjective("ptj", L("Harvest 10 Bundles of Barley"), 0, 0, 0, Collect(52028, 10));

		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Exp(200));
		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Item(63016, 4));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Exp(100));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Item(63016, 2));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Exp(40));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Item(63016, 1));
	}
}

// Int Barley quest
public class EndelyonBarleyIntPtjScript : QuestScript
{
	public override void Load()
	{
		SetId(502132);
		SetName(L("Church Part-Time Job"));
		SetDescription(L("This task is to harvest grain from the farmland. Cut [15 bundles of barley] today. Barley can be harvested using a sickle on the farmlands around town."));

		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.ById);

		SetType(QuestType.Deliver);
		SetPtjType(PtjType.Church);
		SetLevel(QuestLevel.Int);
		SetHours(start: 12, report: 16, deadline: 21);

		AddObjective("ptj", L("Harvest 15 Bundles of Barley"), 0, 0, 0, Collect(52028, 15));

		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Exp(300));
		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Item(63016, 6));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Exp(150));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Item(63016, 3));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Exp(60));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Item(63016, 1));
	}
}

// Adv Barley quest
public class EndelyonBarleyAdvPtjScript : QuestScript
{
	public override void Load()
	{
		SetId(502162);
		SetName(L("Church Part-Time Job"));
		SetDescription(L("This task is to harvest grain from the farmland. Cut [20 bundles of barley] today. Barley can be harvested using a sickle on the farmlands around town."));

		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.ById);

		SetType(QuestType.Deliver);
		SetPtjType(PtjType.Church);
		SetLevel(QuestLevel.Adv);
		SetHours(start: 12, report: 16, deadline: 21);

		AddObjective("ptj", L("Harvest 20 Bundles of Barley"), 0, 0, 0, Collect(52028, 20));

		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Exp(500));
		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Item(63016, 10));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Exp(250));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Item(63016, 5));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Exp(100));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Item(63016, 2));
		AddReward(2, RewardGroupType.Item, QuestResult.Perfect, Item(40004, 1));
		AddReward(2, RewardGroupType.Item, QuestResult.Perfect, Item(19001, 1));
	}
}

// Basic Wheat quest
public class EndelyonWheatBasicPtjScript : QuestScript
{
	public override void Load()
	{
		SetId(502103);
		SetName(L("Church Part-Time Job"));
		SetDescription(L("This task is to harvest grain from the farmland. Cut [10 bundles of wheat] today. Use a sickle to harvest wheat from farmlands around town."));

		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.ById);

		SetType(QuestType.Deliver);
		SetPtjType(PtjType.Church);
		SetLevel(QuestLevel.Basic);
		SetHours(start: 12, report: 16, deadline: 21);

		AddObjective("ptj", L("Harvest 10 Bundles of Wheat"), 0, 0, 0, Collect(52027, 10));

		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Exp(200));
		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Item(63016, 4));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Exp(100));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Item(63016, 2));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Exp(40));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Item(63016, 1));
	}
}

// Int Wheat quest
public class EndelyonWheatIntPtjScript : QuestScript
{
	public override void Load()
	{
		SetId(502133);
		SetName(L("Church Part-Time Job"));
		SetDescription(L("This task is to harvest grain from the farmland. Cut [15 bundles of wheat] today. Use a sickle to harvest wheat from farmlands around town."));

		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.ById);

		SetType(QuestType.Deliver);
		SetPtjType(PtjType.Church);
		SetLevel(QuestLevel.Int);
		SetHours(start: 12, report: 16, deadline: 21);

		AddObjective("ptj", L("Harvest 15 Bundles of Wheat"), 0, 0, 0, Collect(52027, 15));

		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Exp(300));
		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Item(63016, 6));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Exp(150));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Item(63016, 3));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Exp(60));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Item(63016, 1));
	}
}

// Adv Wheat quest
public class EndelyonWheatAdvPtjScript : QuestScript
{
	public override void Load()
	{
		SetId(502163);
		SetName(L("Church Part-Time Job"));
		SetDescription(L("This task is to harvest grain from the farmland. Cut [20 bundles of wheat] today. Use a sickle to harvest wheat from farmlands around town."));

		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.ById);

		SetType(QuestType.Deliver);
		SetPtjType(PtjType.Church);
		SetLevel(QuestLevel.Adv);
		SetHours(start: 12, report: 16, deadline: 21);

		AddObjective("ptj", L("Harvest 20 Bundles of Wheat"), 0, 0, 0, Collect(52027, 20));

		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Exp(500));
		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Item(63016, 10));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Exp(250));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Item(63016, 5));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Exp(100));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Item(63016, 2));
		AddReward(2, RewardGroupType.Item, QuestResult.Perfect, Item(40004, 1));
		AddReward(2, RewardGroupType.Item, QuestResult.Perfect, Item(19001, 1));
	}
}

// Basic Eggs quest
public class EndelyonEggsBasicPtjScript : QuestScript
{
	public override void Load()
	{
		SetId(502105);
		SetName(L("Church Part-Time Job"));
		SetDescription(L("This job is to gather eggs from chickens. Please bring [15 eggs] today. Gather eggs from the chickens around town."));

		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.ById);

		SetType(QuestType.Deliver);
		SetPtjType(PtjType.Church);
		SetLevel(QuestLevel.Basic);
		SetHours(start: 12, report: 16, deadline: 21);

		AddObjective("ptj", L("Collect 15 Eggs"), 0, 0, 0, Collect(50009, 15));

		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Exp(200));
		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Item(63016, 4));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Exp(100));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Item(63016, 2));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Exp(40));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Item(63016, 1));
	}
}

// Int Eggs quest
public class EndelyonEggsIntPtjScript : QuestScript
{
	public override void Load()
	{
		SetId(502135);
		SetName(L("Church Part-Time Job"));
		SetDescription(L("This job is to gather eggs from chickens. Please bring [20 eggs] today. Gather eggs from the chickens around town."));

		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.ById);

		SetType(QuestType.Deliver);
		SetPtjType(PtjType.Church);
		SetLevel(QuestLevel.Int);
		SetHours(start: 12, report: 16, deadline: 21);

		AddObjective("ptj", L("Collect 20 Eggs"), 0, 0, 0, Collect(50009, 20));

		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Exp(300));
		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Item(63016, 6));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Exp(150));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Item(63016, 3));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Exp(60));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Item(63016, 1));
	}
}

// Adv Eggs quest
public class EndelyonEggsAdvPtjScript : QuestScript
{
	public override void Load()
	{
		SetId(502165);
		SetName(L("Church Part-Time Job"));
		SetDescription(L("This job is to gather eggs from chickens. Please bring [30 eggs] today. Gather eggs from the chickens around town."));

		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.ById);

		SetType(QuestType.Deliver);
		SetPtjType(PtjType.Church);
		SetLevel(QuestLevel.Adv);
		SetHours(start: 12, report: 16, deadline: 21);

		AddObjective("ptj", L("Collect 30 Eggs"), 0, 0, 0, Collect(50009, 30));

		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Exp(500));
		AddReward(1, RewardGroupType.Item, QuestResult.Perfect, Item(63016, 10));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Exp(250));
		AddReward(1, RewardGroupType.Item, QuestResult.Mid, Item(63016, 5));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Exp(100));
		AddReward(1, RewardGroupType.Item, QuestResult.Low, Item(63016, 2));
		AddReward(2, RewardGroupType.Item, QuestResult.Perfect, Item(40004, 1));
		AddReward(2, RewardGroupType.Item, QuestResult.Perfect, Item(19001, 1));
	}
}
