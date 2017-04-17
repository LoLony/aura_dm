//--- Aura Script -----------------------------------------------------------
// [PQ] Hunt Down the Red Skeletons (30)
//--- Description -----------------------------------------------------------
// Party quest to kill certain monsters.
//---------------------------------------------------------------------------

public class RedSkeletons30PartyQuest : QuestScript
{
	public override void Load()
	{
		SetId(100028);
		SetScrollId(70025);
		SetName(L("[PQ] Hunt Down the Red Skeletons"));
		SetDescription(L("Have you ever seen a Red Skeleton? The Red Skeletons living in Rabbie Dungeon are controlled by a more powerful evil force than regular Skeletons. Please hunt [30 Red Skeletons]."));
		SetType(QuestType.Collect);
		SetCancelable(true);

		SetIcon(QuestIcon.Party);
		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.Repeat);

		AddObjective("obj", L("Hunt 30 Red Skeletons"), 0, 0, 0, Kill(30, "/redskeleton/"));

		AddReward(Exp(882));
		AddReward(Gold(2352));
	}
}
