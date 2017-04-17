//--- Aura Script -----------------------------------------------------------
// [PQ] Hunt Down the Dire Wolves (40)
//--- Description -----------------------------------------------------------
// Party quest to kill certain monsters.
//---------------------------------------------------------------------------

public class DireWolf40PartyQuest : QuestScript
{
	public override void Load()
	{
		SetId(100052);
		SetScrollId(70025);
		SetName(L("[PQ] Hunt Down the Dire Wolves"));
		SetDescription(L("Recently, Dire Wolves and Skeleton Wolves which are under a more powerful evil spell than regular Wolves are threatening travelers near the Dragon Ruins, Please [Hunt 10 Brown Dire Wolves and 30 Skeleton Wolves]."));
		SetType(QuestType.Collect);
		SetCancelable(true);

		SetIcon(QuestIcon.Party);
		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.Repeat);

		AddObjective("obj1", L("Hunt 10 Brown Dire Wolves"), 0, 0, 0, Kill(10, "/browndirewolf/"));
		AddObjective("obj2", L("Hunt 30 Skeleton Wolves"), 0, 0, 0, Kill(30, "/skeletonwolf/"));

		AddReward(Exp(1680));
		AddReward(Gold(2991));
	}
}
