//--- Aura Script -----------------------------------------------------------
// [PQ] Hunt Down the Black Grizzly Bears (30)
//--- Description -----------------------------------------------------------
// Party quest to kill certain monsters.
//---------------------------------------------------------------------------

public class BlackGrizzlyBear30PartyQuest : QuestScript
{
	public override void Load()
	{
		SetId(100034);
		SetScrollId(70025);
		SetName(L("[PQ] Hunt Down the Black Grizzly Bears"));
		SetDescription(L("The Grizzly Bears roaming in the plains are under a mighty evil spell which can be seen in their eyes. Please [Hunt 30 Black Grizzly Bears]."));
		SetType(QuestType.Collect);
		SetCancelable(true);

		SetIcon(QuestIcon.Party);
		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.Repeat);

		AddObjective("obj1", L("Hunt 10 Black Grizzly Bears"), 0, 0, 0, Kill(10, "/blackgrizzlybear/"));
		AddObjective("obj2", L("Hunt 20 Black Grizzly Bear Cubs"), 0, 0, 0, Kill(20, "/black/grizzlybearkid/"));

		AddReward(Exp(1164));
		AddReward(Gold(2130));
	}
}
