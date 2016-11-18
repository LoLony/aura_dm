//--- Aura Script -----------------------------------------------------------
// Save my Sheep
//--- Description -----------------------------------------------------------
// Automatically started after Rescue Resident, involves the player being
// warped to a dynamic region, where they have to protect sheep from being
// killed by wolves.
//---------------------------------------------------------------------------

public class SaveMySheepQuestScript : QuestScript
{
	public override void Load()
	{
		SetId(202003);
		SetName("Save my Sheep");
		SetDescription("I am Deian, a shepherd in the Northeast pasture across the bridge. I have to take care of my sheep all day, and it leaves me no time for anything else. But I have to go somewhere today, so can you guard the sheep from the wolves for me till I come back? - Deian -");

		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.Tutorial);

		SetReceive(Receive.Automatically);
		AddPrerequisite(Completed(202002));

		AddObjective("talk_deian1", "Talk with Deian from the Pasture", 1, 27953, 42287, Talk("deian"));
		AddObjective("protect_sheep", "Guard the Sheep from the Gray Wolves", 1, 27953, 42287, GetKeyword("TirChonaill_Tutorial_Thinking")); // ProtectSheep(5)?
		AddObjective("talk_deian2", "Talk with Deian", 1, 27953, 42287, Talk("deian"));

		AddReward(Exp(900));
		AddReward(Gold(2600));
		if (!IsEnabled("G1EasyOverseas"))
			AddReward(AP(3));
		else
			AddReward(AP(6)); // 6 AP in EU and during "g1_easy_overseas" (212003)

		AddHook("_deian", "after_intro", TalkDeian);
	}

	public async Task<HookResult> TalkDeian(NpcScript npc, params object[] args)
	{
		if (npc.QuestActive(this.Id, "talk_deian1") || npc.QuestActive(this.Id, "protect_sheep"))
		{
			npc.Msg("I'm glad to see you. I've been stuck here all day!<br/>Can you look after my sheep for a few minutes? I got some business to take care of.<br/>It should be easy, as long as the wolves don't show up.");
			npc.Msg("Just make sure to keep my sheep safe if wolves show up.<br/>The number of sheep and the time left will display<br/>on the top right corner.");
			npc.Msg("I hear you're pretty strong, so this should be pretty simple.<br/>Thanks!<button title='Look After Sheep' keyword='@protect'/><button title='Start Another Topic' keyword='@end'/>");
			var response = await npc.Select();

			if (response != "@protect")
				return HookResult.Break;

			npc.Close2();
			npc.FinishQuest(this.Id, "talk_deian1");

			CreateRegionAndWarp(npc.Player);

			return HookResult.End;
		}
		else if (npc.QuestActive(this.Id, "talk_deian2"))
		{
			npc.FinishQuest(this.Id, "talk_deian2");

			npc.Msg("Wow, good job.<br/>I got everything done thanks to you.<br/>You'll do this again next time, right? Thanks!");

			return HookResult.Break;
		}

		return HookResult.Continue;
	}

	const int Minute = 60 * 1000; // A minute in milliseconds
	const int SheepAmount = 20; // Sheeps to protect
	const int SheepMinAmount = 5; // Amount you need to save to succeed
	const int WolfAmount = 10; // Wolves to (re)spawn
	static readonly Position Center = new Position(60000, 58000);

	public void CreateRegionAndWarp(Creature creature)
	{
		// Get duration
		var time = 5;
		if (IsEnabled("ShortSheepProtection"))
			time = 3;

		time *= Minute;

		// Create region
		var region = new DynamicRegion(118);
		ChannelServer.Instance.World.AddRegion(region);

		var rnd = RandomProvider.Get();
		var sheepAmount = SheepAmount;

		// After x ms (success)
		var timer = SetTimeout(time, () =>
		{
			// Unofficial, I think the msg also depends on how well you did.
			// Official >10: Thanks to my dilligent supervision, over 10 sheep are safe.
			Send.Notice(creature, NoticeType.MiddleSystem, L("The time is over, you did it."));
			Send.RemoveQuestTimer(creature);
			creature.Keywords.Give("TirChonaill_Tutorial_Thinking");
			creature.Warp(1, 27622, 42125);
		});

		// Spawn sheep
		for (int i = 0; i < sheepAmount; ++i)
		{
			var pos = Center.GetRandomInRect(6000, 4000, rnd);

			var npc = new NPC(40001); // Sheep
			npc.Death += (killed, killer) =>
			{
				sheepAmount--;

				// Cancel if success is not possible anymore.
				if (sheepAmount < SheepMinAmount)
				{
					Send.Notice(creature, NoticeType.MiddleSystem, L("You've failed to save the sheep."));
					Send.RemoveQuestTimer(creature);
					StopTimer(timer);
					creature.Warp(1, 27622, 42125);
					return;
				}

				Send.UpdateQuestTimer(creature, L("Remaining sheep: {0}"), sheepAmount);
			};
			npc.Spawn(region.Id, pos.X, pos.Y);
		}

		// Spawn wolves
		for (int i = 0; i < WolfAmount; ++i)
			SpawnWolf(region.Id, rnd);

		// Warp to region and start visible timer
		creature.Warp(region.Id, 60000, 58000);
		Send.SetQuestTimer(creature, time, L("Protect the sheep from wolves"), L("Deadline: {0}"), L("Remaining sheep: {0}"), sheepAmount);
	}

	private void SpawnWolf(int regionId, Random rnd)
	{
		var pos = Center.GetRandomInRect(6000, 4000, rnd);

		// Spawn wolf on random position and respawn it if it dies.
		var npc = new NPC(20001); // Gray Wolf
		npc.Death += (killed, killer) =>
		{
			SpawnWolf(regionId, rnd);
		};
		npc.Spawn(regionId, pos.X, pos.Y);
	}
}
