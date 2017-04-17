//--- Aura Script -----------------------------------------------------------
// Malcolm's Ring
//--- Description -----------------------------------------------------------
// The player is asked by Malcolm to go into a special version of Alby to
// to get a ring from the boss that he lost there.
//---------------------------------------------------------------------------

public class MalcolmsRingQuestScript : QuestScript
{
	public override void Load()
	{
		SetId(202004);
		SetName("Malcolm's Ring");
		SetDescription("I'm Malcolm. I sell a variety of stuff at the General Shop near the Square. I happened to lose a ring in Alby Dungeon, but I cannot go find it myself. Can you help me find the ring? Come visit me first, though. - Malcolm -");

		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.Tutorial);

		SetReceive(Receive.Automatically);
		AddPrerequisite(Completed(202003)); // Save my Sheep

		AddObjective("talk_malcolm1", "Talk with Malcolm at the Tir Chonaill General Shop", 8, 1238, 1655, Talk("malcolm"));
		AddObjective("kill_spider", "Defeat the Giant Golden Spiderling in Alby Dungeon and get the Ring", 13, 3200, 3200, Kill(1, "/giantgoldenspiderkid/"));
		AddObjective("talk_malcolm2", "Give the Ring to Malcolm", 8, 1238, 1655, Talk("malcolm"));

		AddReward(Exp(1500));
		AddReward(Gold(1700));
		AddReward(Item(2001));
		if (!IsEnabled("G1EasyOverseas"))
			AddReward(AP(3));
		else
			AddReward(AP(6)); // 6 AP in EU and during "g1_easy_overseas" (212004)

		AddHook("_malcolm", "after_intro", TalkMalcolm);
	}

	public async Task<HookResult> TalkMalcolm(NpcScript npc, params object[] args)
	{
		if (npc.Player.QuestActive(this.Id, "talk_malcolm1"))
		{
			npc.Player.FinishQuestObjective(this.Id, "talk_malcolm1");

			npc.Msg("So, you received the quest I sent through the Owl.<br/>Thanks for coming.<br/>I think I lost my ring in Alby Dungeon,<br/>but I can't leave, because I have no one to take care of the General Shop.");
			npc.Msg("I know it's a lot to ask, but can you go find the ring for me?<br/>The dungeon is very dangerous so I suggest talking to Trefor first about the Counterattack skill.<br/><br/>Take this pass to enter the dungeon, and please find my ring.");
			npc.Player.GiveItem(63181); // Malcolm's Pass
			npc.Player.GiveKeyword("skill_counter_attack");

			return HookResult.End;
		}
		else if (npc.Player.QuestActive(this.Id, "kill_spider") && !npc.Player.HasItem(63181))
		{
			npc.Msg("Have you lost the pass?<br/>Take this one to enter the dungeon, and please find my ring.");
			npc.Player.GiveItem(63181); // Malcolm's Pass

			return HookResult.Break;
		}
		else if (npc.Player.QuestActive(this.Id, "talk_malcolm2"))
		{
			npc.Player.FinishQuestObjective(this.Id, "talk_malcolm2");
			npc.Player.GiveKeyword("Clear_Tutorial_Malcolm_Ring");
			npc.Player.RemoveItem(75058); // Malcolm's Ring

			npc.Msg("You found my Ring!<br/>You have my thanks.");

			return HookResult.Break;
		}

		return HookResult.Continue;
	}
}
