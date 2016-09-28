// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Channel.Network.Sending;
using Aura.Channel.Skills.Base;
using Aura.Channel.World.Entities;
using Aura.Mabi;
using Aura.Mabi.Const;

namespace Aura.Channel.Skills.Magic		
{
	/// <summary>
	/// Handles the Soul of Chaos skill.
	/// </summary>
	/// <remarks>
	/// Variables currently unknown.
	/// </remarks>
	[Skill(SkillId.SoulOfChaos)]
	public class SoulOfChaos : StartStopSkillHandler
	{
		/// <summary>
		/// Starts the skill.
		/// </summary>
		/// <param name="creature"></param>
		/// <returns></returns>
		public override StartStopResult Start(Creature creature)
		{
			// After duration of DK, use this to engage the Disarm FX.
			// See the captures to determine the name of the effect for implementation.
			// creature.Conditions.Activate(ConditionsA.ManaShield);
			Send.EffectDelayed(creature, 6500, Effect.DkPassiveEngage);

			if (true)	// not used today
			{

			}
			else // used today
			{
				Send.ServerMessage(creature, Localization.Get("You can't use this any more today."));
				Send.SkillUseSilentCancel(creature);
				// return; 		needed?
			}

			return StartStopResult.Okay;
		}

		/// <summary>
		/// Stops the skill.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="skill"></param>
		/// <param name="dict"></param>
		/// <returns></returns>
		public override StartStopResult Stop(Creature creature)
		{
			if (creature.Conditions.Has(ConditionsA.CancelDarkKnight)) {creature.Conditions.Deactivate(ConditionsA.CancelDarkKnight);}

			return StartStopResult.Okay;
		}

		/// <summary>
		/// Checks if target's Mana Shield is active, calculates mana
		/// damage, and sets target action's Mana Damage property if applicable.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="damage"></param>
		/// <param name="tAction"></param>
		public static void Handle(Creature target, ref float damage, TargetAction tAction)
		{
			// Mana Shield active?
			if (!target.Conditions.Has(ConditionsA.ManaShield))
				return;

			/* Get Soul of Chaos skill to get the rank's vars [I DON'T THINK WE NEED THIS]
			var dkRank = target.Skills.Get(SkillId.SoulOfChaos);
			if (dkRank == null) // Checks for things that should never ever happen, yay.
				return;

			// Var 1 = Efficiency
			var manaDamage = damage / manaShield.RankData.Var1;
			if (target.Mana >= manaDamage)
			{
				// Damage is 0 if target's mana is enough to cover it
				damage = 0;
			}
			else
			{
				// Set mana damage to target's mana and reduce the remaining
				// damage from life if the mana is not enough.
				damage -= (manaDamage - target.Mana) * manaShield.RankData.Var1;
				manaDamage = target.Mana;
			} */

			// while experiencing DK Disarm
			if (target.Life <= 0)
				ChannelServer.Instance.SkillManager.GetHandler<StartStopSkillHandler>(SkillId.ManaShield).Stop(target, dkRank);

			tAction.ManaDamage = manaDamage;
		}
	}
}
