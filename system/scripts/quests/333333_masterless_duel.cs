//--- Aura Script -----------------------------------------------------------
// Battle for Light and Darkness
//--- Description -----------------------------------------------------------
// Battle against Masterless for the transformation of your choosing.
// Designed for humans to obtain Paladin and Dark Knight without G2-3.
// Based off the Red/Blue Coin drops in treasure_chest.cs.
//---------------------------------------------------------------------------

public class BattleForLightQuestScript : QuestScript
{
	public override void Load()
	{
		SetId(333333);
		SetName("Battle for Light");
		SetDescription("If you defeat me in combat, I'll teach you how to transform into a [Paladin]. - Masterless -");

		SetIcon(QuestIcon.ExpReward);
		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.Skill);

		AddObjective("fight_mless", "Defeat Masterless in Alby Silver Gem.", 13, 3210, 3209, Kill(1, "/masterless/"));
		AddObjective("talk_mless", "Talk to Masterless.", 27, 3206, 4351, Talk("Masterless"));

		AddReward(Exp(10000));
		AddReward(Skill(SkillId.SpiritOfOrder, SkillRank.RF));
		AddReward(Skill(SkillId.PowerOfOrder, SkillRank.RF));
		AddReward(Skill(SkillId.SwordOfOrder, SkillRank.RF));
		AddReward(Skill(SkillId.EyeOfOrder, SkillRank.RF));
		AddReward(Skill(SkillId.PaladinHeavyStander, SkillRank.RF));
		AddReward(Skill(SkillId.PaladinNaturalShield, SkillRank.RF));
		AddReward(Skill(SkillId.PaladinManaDeflector, SkillRank.RF));

		AddHook("Masterless", "after_intro", TalkMless);
	}

	public async Task<HookResult> TalkMless(NpcScript npc, params object[] args)
	{
		if (npc.QuestActive(this.Id, "talk_mless"))
		{
			npc.FinishQuest(this.Id, "talk_mless");
			npc.CompleteQuest(this.Id);
			npc.Msg("Ah... it's you, <username/>. Now that was a worthy combat...<br/>As promised, I will bestow upon you now the power of Paladin.");
			npc.Msg("Continue to hone your strength through the power of your transformation...");
			npc.End();

			return HookResult.End;
		}

		return HookResult.Continue;
	}
}

public class BattleForDarknessQuestScript : QuestScript
{
	public override void Load()
	{
		SetId(333334);
		SetName("Battle for Darkness");
		SetDescription("If you defeat me in combat, I'll teach you how to transform into a [Dark Knight]. - Masterless -");

		SetIcon(QuestIcon.ExpReward);
		if (IsEnabled("QuestViewRenewal"))
			SetCategory(QuestCategory.Skill);

		AddObjective("fight_mless", "Defeat Masterless in Alby Silver Gem.", 13, 3210, 3209, Kill(1, "/masterless/"));
		AddObjective("talk_mless", "Talk to Masterless.", 27, 3206, 4351, Talk("Masterless"));

		AddReward(Exp(10000));
		AddReward(Skill(SkillId.SoulOfChaos, SkillRank.RF));
		AddReward(Skill(SkillId.BodyOfChaos, SkillRank.RF));
		AddReward(Skill(SkillId.MindOfChaos, SkillRank.RF));
		AddReward(Skill(SkillId.HandsOfChaos, SkillRank.RF));
		AddReward(Skill(SkillId.DarkHeavyStander, SkillRank.RF));
		AddReward(Skill(SkillId.DarkNaturalShield, SkillRank.RF));
		AddReward(Skill(SkillId.DarkManaDeflector, SkillRank.RF));
		AddReward(Skill(SkillId.ControlofDarkness, SkillRank.RF));
		
		AddHook("Masterless", "after_intro", TalkMless);
	}

	public async Task<HookResult> TalkMless(NpcScript npc, params object[] args)
	{
		if (npc.QuestActive(this.Id, "talk_mless"))
		{
			npc.FinishQuest(this.Id, "talk_mless");
			npc.CompleteQuest(this.Id);
			npc.Msg("Ah... it's you, <username/>. Now that was a worthy combat...<br/>As promised, I will bestow upon you now the power of Dark Knight.");
			npc.Msg("Continue to hone your strength through the power of your transformation...");
			npc.End();

			return HookResult.End;
		}

		return HookResult.Continue;
	}
}