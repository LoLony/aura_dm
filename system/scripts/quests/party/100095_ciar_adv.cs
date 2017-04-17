//--- Aura Script -----------------------------------------------------------
// [PQ] Defeat the Golem (Ciar Adv.)
//--- Description -----------------------------------------------------------
// Party quest to kill certain monsters.
//---------------------------------------------------------------------------

public class CiarAdvPartyQuest : QuestScript
{
	public override void Load()
	{
		SetId(100095);
		SetScrollId(70025);
		SetName(L("[PQ] Defeat the Golem"));
		SetDescription(L("Please offer [Ciar Adv. Fomor Pass] on the altar of Ciar Dungeon, and defeat a [Golem] that can be found at the deepest part of the dungeon. The reward will be given to you outside the dungeon after completing the quest."));
		SetType(QuestType.Collect);
		SetCancelable(true);

		SetIcon(QuestIcon.Party);
		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.Repeat);

		AddObjective("obj1", L("Eliminate 2 Golems"), 0, 0, 0, Kill(2, "/golem/boss/mini_golem/ensemble/golem5/|/golem/boss/mini_golem/ensemble/golem4/"));
		AddObjective("obj2", L("Eliminate 2 Giant Lightning Sprites"), 0, 0, 0, Kill(2, "/elemental/giantlightningelemental/not_swallow/sprite2/"));
		AddObjective("obj3", L("Clear Ciar Adv. Dungeon"), 0, 0, 0, ClearDungeon("TirCho_Ciar_High_Dungeon"));

		AddReward(Exp(19800));
		AddReward(Gold(20000));
	}
}
