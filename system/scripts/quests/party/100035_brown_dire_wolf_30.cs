//--- Aura Script -----------------------------------------------------------
// [PQ] Hunt Down the Brown Dire Wolves (30)
//--- Description -----------------------------------------------------------
// Party quest to kill certain monsters.
//---------------------------------------------------------------------------

public class BrownDireWolf30PartyQuest : QuestScript
{
	public override void Load()
	{
		SetId(100035);
		SetScrollId(70025);
		SetName(L("[PQ] Hunt Down the Brown Dire Wolves"));
		SetDescription(L("Dire wolves are totally under the control of evil spirits, even more than ordinary wolves. Please do us a favor and [hunt 30 brown dire wolves]."));
		SetType(QuestType.Collect);
		SetCancelable(true);

		SetIcon(QuestIcon.Party);
		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.Repeat);

		AddObjective("obj1", L("Hunt 10 Brown Dire Wolves"), 0, 0, 0, Kill(10, "/browndirewolf/"));
		AddObjective("obj2", L("Hunt 20 Brown Dire Wolf Cubs"), 0, 0, 0, Kill(20, "/brown/direwolfkid/"));

		AddReward(Exp(636));
		AddReward(Gold(1146));
	}
}
