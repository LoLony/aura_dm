//--- Aura Script -----------------------------------------------------------
// [PQ] Hunt Down the Laghodessas (30)
//--- Description -----------------------------------------------------------
// Party quest to kill certain monsters.
//---------------------------------------------------------------------------

public class Laghodessas30PartyQuest : QuestScript
{
	public override void Load()
	{
		SetId(100056);
		SetScrollId(70025);
		SetName(L("[PQ] Hunt Down the Laghodessas"));
		SetDescription(L("Laghodessas under the control of an evil power are attacking travelers. Please do us a favor and [hunt 30 Laghodessass]."));
		SetType(QuestType.Collect);
		SetCancelable(true);

		SetIcon(QuestIcon.Party);
		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.Repeat);

		AddObjective("obj", L("Hunt 30 Laghodessas"), 0, 0, 0, Kill(30, "/laghodessa/"));

		AddReward(Exp(1200));
		AddReward(Gold(2631));
	}
}
