//--- Aura Script -----------------------------------------------------------
// [PQ] Hunt White Spiders (10)
//--- Description -----------------------------------------------------------
// Party quest to kill certain monsters.
//---------------------------------------------------------------------------

public class WhiteSpider10PartyQuest : QuestScript
{
	public override void Load()
	{
		SetId(100013);
		SetScrollId(70025);
		SetName(L("[PQ] Hunt White Spiders"));
		SetDescription(L("The White Spiders are a threat for people trying to collect cobwebs. I will reward you for hunting [10 White Spiders]."));
		SetType(QuestType.Collect);
		SetCancelable(true);

		SetIcon(QuestIcon.Party);
		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.Repeat);

		AddObjective("obj", L("Hunt 10 White Spiders"), 0, 0, 0, Kill(10, "/whitespider/"));

		AddReward(Exp(66));
		AddReward(Gold(201));
		AddReward(QuestScroll(100014)); // [PQ] Hunt White Spiders (30)
	}
}
