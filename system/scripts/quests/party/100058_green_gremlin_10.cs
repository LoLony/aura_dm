//--- Aura Script -----------------------------------------------------------
// [PQ] Hunt Down the Green Gremlins (10)
//--- Description -----------------------------------------------------------
// Party quest to kill certain monsters.
//---------------------------------------------------------------------------

public class GreenGremlin10PartyQuest : QuestScript
{
	public override void Load()
	{
		SetId(100058);
		SetScrollId(70025);
		SetName(L("[PQ] Hunt Down the Green Gremlins"));
		SetDescription(L("Green Gremlins are under an evil spell and are attacking travelers. Please [Hunt 10 Green Gremlins]."));
		SetType(QuestType.Collect);
		SetCancelable(true);

		SetIcon(QuestIcon.Party);
		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.Repeat);

		AddObjective("obj", L("Hunt 10 Green Gremlins"), 0, 0, 0, Kill(10, "/greengremlin/"));

		AddReward(Exp(600));
		AddReward(Gold(1710));
	}
}
