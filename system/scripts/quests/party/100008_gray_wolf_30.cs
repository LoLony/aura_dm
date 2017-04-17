//--- Aura Script -----------------------------------------------------------
// [PQ] Hunt Gray Wolves (30)
//--- Description -----------------------------------------------------------
// Party quest to kill certain monsters.
//---------------------------------------------------------------------------

public class GrayWolf30PartyQuest : QuestScript
{
	public override void Load()
	{
		SetId(100008);
		SetScrollId(70025);
		SetName(L("[PQ] Hunt Gray Wolves"));
		SetDescription(L("The wolves that started to appear recently, are natural enemies of the sheep. To protect the sheep, please hunt [30 gray wolves] roaming the plains."));
		SetType(QuestType.Collect);
		SetCancelable(true);

		SetIcon(QuestIcon.Party);
		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.Repeat);

		AddObjective("obj", L("Hunt 30 Gray Wolves"), 0, 0, 0, Kill(30, "/graywolf/"));

		AddReward(Exp(372));
		AddReward(Gold(705));
	}
}
