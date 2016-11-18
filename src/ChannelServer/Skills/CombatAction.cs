﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Aura.Channel.World;
using Aura.Channel.World.Entities;
using Aura.Mabi.Const;
using Aura.Channel.Network.Sending;
using Aura.Shared.Util;
using Aura.Channel.Skills.Life;
using Aura.Channel.Skills.Base;
using Aura.Data;

namespace Aura.Channel.Skills
{
	/// <summary>
	/// Collection of combat actions
	/// </summary>
	/// <remarks>
	/// A combat action (eg a player hitting a monster) consits of multiple
	/// actions, depending on the amount involved creatures. Each action
	/// has information about one creature, to make the client display
	/// the result of the attack. An attack action shows the creature
	/// hitting, a target action shows it receiving a hit.
	/// </remarks>
	public class CombatActionPack
	{
		private static int _actionId = 0;

		/// <summary>
		/// Id of this action pack.
		/// </summary>
		public int Id { get; protected set; }

		/// <summary>
		/// Id of previous pack, in multi pack situations (dual wield).
		/// </summary>
		public int PrevId { get; set; }

		/// <summary>
		/// Hit x of MaxHits.
		/// </summary>
		public byte Hit { get; set; }

		/// <summary>
		/// 1 or 2 (normal vs dual wield)
		/// </summary>
		public CombatActionPackType Type { get; set; }

		/// <summary>
		/// Attacking creature.
		/// </summary>
		public Creature Attacker { get; set; }

		/// <summary>
		/// Skill used by the attacker.
		/// </summary>
		public SkillId SkillId { get; set; }

		/// <summary>
		/// Unknown Flags, magic shield related?
		/// </summary>
		public byte Flags { get; set; }

		public int BlockedByShieldPosX { get; set; }
		public int BlockedByShieldPosY { get; set; }
		public long ShieldCasterId { get; set; }

		/// <summary>
		/// Attacker and Target actions.
		/// </summary>
		public List<CombatAction> Actions { get; protected set; }

		/// <summary>
		/// Initializes combat action pack.
		/// </summary>
		private CombatActionPack()
		{
			this.Id = Interlocked.Increment(ref _actionId);
			this.Hit = 1;
			this.Type = CombatActionPackType.NormalAttack;
			this.Actions = new List<CombatAction>();
		}

		/// <summary>
		/// Creates new combat action pack.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="skillId"></param>
		public CombatActionPack(Creature attacker, SkillId skillId)
			: this()
		{
			this.Attacker = attacker;
			this.SkillId = skillId;
		}

		/// <summary>
		/// Creates new combat action pack.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="skillId"></param>
		/// <param name="actions"></param>
		public CombatActionPack(Creature attacker, SkillId skillId, params CombatAction[] actions)
			: this(attacker, skillId)
		{
			this.Add(actions);
		}

		/// <summary>
		/// Adds combat actions.
		/// </summary>
		/// <param name="actions"></param>
		public void Add(params CombatAction[] actions)
		{
			foreach (var action in actions)
				action.Pack = this;

			this.Actions.AddRange(actions);
		}

		/// <summary>
		/// Returns all creatures found in this pack's target actions.
		/// </summary>
		/// <returns></returns>
		public Creature[] GetTargets()
		{
			return this.Actions.Where(a => a.Category == CombatActionCategory.Target).Select(a => a.Creature).ToArray();
		}

		/// <summary>
		/// Handles actions and broadcasts action pack.
		/// </summary>
		public void Handle()
		{
			foreach (var action in this.Actions)
			{
				// Stat update
				Send.StatUpdate(action.Creature, StatUpdateType.Private, Stat.Life, Stat.LifeInjured, Stat.Mana, Stat.Stamina);
				Send.StatUpdate(action.Creature, StatUpdateType.Public, Stat.Life, Stat.LifeInjured);

				// If target action
				if (action.Category == CombatActionCategory.Target)
				{
					var tAction = action as TargetAction;

					if (action.Creature.IsPlayer)
					{
						// Max target stun for players == 2000?
						action.Stun = Math.Min((short)2000, action.Stun);

						// Break eggs on knock back
						if (tAction.IsKnockBack && ChannelServer.Instance.Conf.World.BrokenEggs)
						{
							// Search for usable (non-broken) eggs, sparing VIP eggs.
							var eggs = action.Creature.Inventory.GetItems(a => a.HasTag("/usable/food/cooking/solid/*egg/") && a.Info.Pocket != Pocket.VIPInventory);
							foreach (var egg in eggs)
							{
								// Replace egg
								var brokenEgg = new Item(50011);
								brokenEgg.Amount = egg.Amount;

								// Remove egg and place the broken one at the
								// same spot.
								if (action.Creature.Inventory.Remove(egg))
									action.Creature.Inventory.Add(brokenEgg, egg.Info.Pocket);
							}
						}
					}

					// Put target into battle stance if it was hit.
					// Fix for #435, Mimics not opening after being got hit
					// by Windmill "passively", because they didn't aggro.
					if (!action.Creature.IsInBattleStance)
						action.Creature.IsInBattleStance = true;

					// Reduce stun if pinged
					// If the second hit of the second set of dual wield hits
					// pings, the monster's stun on the client is longer than
					// on the server, which causes a little glitch, where a
					// monster might attack while it's still visibly stunned.
					// Should stun from combat be *added*, instead of *set*?
					if (!tAction.IsKnockBack && tAction.Has(EffectFlags.HeavyStander))
						action.Stun = Math.Min((short)1000, action.Stun);

					action.Creature.Stun = action.Stun;

					// Mana Shield flag
					if (tAction.ManaDamage > 0 && tAction.Damage == 0)
						tAction.Set(TargetOptions.ManaShield);

					// On attack events
					ChannelServer.Instance.Events.OnCreatureAttacked(tAction);
					if (this.Attacker.IsPlayer)
						ChannelServer.Instance.Events.OnCreatureAttackedByPlayer(tAction);

					// OnHit AI event
					//action.Creature.Aggro(tAction.Attacker);
					var npc = action.Creature as NPC;
					if (npc != null && npc.AI != null)
					{
						npc.AI.OnTargetActionHit(tAction);
					}

					// Cancel target's skill
					// Don't cancel Defense, which is still active at this point,
					// a Complete for it is sent by the client shortly after.
					// That won't happen if Smash is the attacker skill,
					// but that's official behavior.
					var activeSkill = action.Creature.Skills.ActiveSkill;
					if (activeSkill != null && activeSkill.Info.Id != SkillId.Defense)
					{
						var custom = ChannelServer.Instance.SkillManager.GetHandler(activeSkill.Info.Id) as ICustomHitCanceler;
						var stackMax = activeSkill.RankData.StackMax;
						var cancel = false;

						// Cancel non stackable skills on hit, wait for a
						// knock back for stackables
						if (stackMax > 1)
							cancel = action.IsKnockBack;
						else
							cancel = true;

						if (cancel)
						{
							if (custom == null)
								action.Creature.Skills.CancelActiveSkill();
							else
								custom.CustomHitCancel(action.Creature, tAction);
						}
					}

					// Cancel rest
					if (action.Creature.Has(CreatureStates.SitDown))
					{
						var restHandler = ChannelServer.Instance.SkillManager.GetHandler<Rest>(SkillId.Rest);
						if (restHandler != null)
							restHandler.Stop(action.Creature, action.Creature.Skills.Get(SkillId.Rest));
					}

					// Remember knock back/down
					tAction.Creature.WasKnockedBack = tAction.Has(TargetOptions.KnockBack) || tAction.Has(TargetOptions.KnockDown) || tAction.Has(TargetOptions.Smash);

					if (tAction.Has(TargetOptions.KnockDown))
						tAction.Creature.KnockDownTime = DateTime.Now.AddMilliseconds(tAction.Stun);

					// Stability meter
					// TODO: Limit it to "targetees"?
					var visibleCreatures = tAction.Creature.Region.GetVisibleCreaturesInRange(tAction.Creature);
					foreach (var cr in visibleCreatures)
						Send.StabilityMeterUpdate(cr, tAction.Creature);
				}
				// If attacker action
				else if (action.Category == CombatActionCategory.Attack)
				{
					var aAction = action as AttackerAction;

					if (action.Creature.IsPlayer && action.SkillId >= SkillId.RangedAttack && action.SkillId <= SkillId.UrgentShot && !AuraData.FeaturesDb.IsEnabled("CombatSystemRenewal"))
					{
						// Players don't seem to be stunned from ranging
						// on old officials. While the proper stun is sent
						// to the client, for some reason you didn't need
						// to wait. Since we don't know how officials did
						// that, we'll just cap the stun time.
						action.Stun = 0;
					}

					action.Creature.Stun = action.Stun;

					var npc = action.Creature as NPC;
					if (npc != null && npc.AI != null && action.SkillId != SkillId.CombatMastery)
						npc.AI.OnUsedSkill(aAction);

					ChannelServer.Instance.Events.OnCreatureAttacks(aAction);
				}
			}

			// Send combat action
			Send.CombatAction(this);

			// Skill used
			if (this.SkillId != SkillId.CombatMastery)
				Send.CombatUsedSkill(this.Attacker, this.SkillId);

			// End combat action
			Send.CombatActionEnd(this.Attacker, this.Id);
		}
	}

	/// <summary>
	/// Combat action base class.
	/// </summary>
	/// <remarks>
	/// TODO: Officially there's no differentiation between attacker and target
	///   actions, it all depends on the flags and options. Now that we have
	///   the proper enums, we might want to change this as well.
	///   This will require some changes to the way we handle skill training.
	/// </remarks>
	public abstract class CombatAction
	{
		/// <summary>
		/// Creature of this action
		/// </summary>
		public Creature Creature { get; set; }

		// Type of combat action
		public CombatActionType Flags { get; set; }

		/// <summary>
		/// Time before creature can move again.
		/// </summary>
		public short Stun { get; set; }

		/// <summary>
		/// Used skill
		/// </summary>
		public SkillId SkillId { get; set; }

		/// <summary>
		/// Used for alchemy crystals.
		/// </summary>
		public SkillId SecondarySkillId { get; set; }

		/// <summary>
		/// Don't know, seems to be always 0. Attacking with offhand weapon?
		/// </summary>
		public byte UsedWeaponSet { get; set; }

		/// <summary>
		/// Usually 1. 2 seen in dualwield, right or left weapon?
		/// </summary>
		public byte WeaponParameterType { get; set; }

		/// <summary>
		/// Returns true if action is a knock back/down.
		/// </summary>
		public abstract bool IsKnockBack { get; }

		/// <summary>
		/// Attack or Target action.
		/// </summary>
		public abstract CombatActionCategory Category { get; }

		/// <summary>
		/// Returns the combat action pack this action is part of, or null.
		/// Set automatically when adding the action to the pack.
		/// </summary>
		public CombatActionPack Pack { get; set; }

		/// <summary>
		/// Returns true if the specified flags are set.
		/// </summary>
		/// <param name="flags"></param>
		/// <returns></returns>
		public bool Is(CombatActionType flags)
		{
			return flags == this.Flags || (flags != CombatActionType.None && (this.Flags & flags) != 0);
		}
	}

	/// <summary>
	/// Contains information about the source action part of the
	/// CombatActionPack. This part is sent first, before the target actions.
	/// </summary>
	public class AttackerAction : CombatAction
	{
		/// <summary>
		/// Attacker options
		/// </summary>
		public AttackerOptions Options { get; set; }

		/// <summary>
		/// Attack Phase. Used in Pummel.
		/// </summary>
		public byte Phase { get; set; }

		/// <summary>
		/// Id of the attacked creature/area
		/// </summary>
		public long TargetId { get; set; }

		/// <summary>
		/// Id of a prop created for the skill (eg Fireball)
		/// </summary>
		public long PropId { get; set; }

		/// <summary>
		/// Returns true if KnockBackHit is set.
		/// </summary>
		public override bool IsKnockBack
		{
			get { return this.Has(AttackerOptions.KnockBackHit2) || this.Has(AttackerOptions.KnockBackHit1); }
		}

		/// <summary>
		/// Attack or Target action.
		/// </summary>
		public override CombatActionCategory Category { get { return CombatActionCategory.Attack; } }

		/// <summary>
		/// Creates new attacker action, setting SkillId to the currently
		/// active skill.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="creature"></param>
		/// <param name="targetId"></param>
		public AttackerAction(CombatActionType type, Creature creature, long targetId)
		{
			this.Flags = type;
			this.Creature = creature;
			this.TargetId = targetId;
			this.WeaponParameterType = 1;

			var active = creature.Skills.ActiveSkill;
			this.SkillId = (active == null ? SkillId.CombatMastery : active.Info.Id);
		}

		/// <summary>
		/// Creates new attacker action.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="creature"></param>
		/// <param name="targetId"></param>
		/// <param name="skillId"></param>
		public AttackerAction(CombatActionType type, Creature creature, long targetId, SkillId skillId)
		{
			this.Flags = type;
			this.Creature = creature;
			this.TargetId = targetId;
			this.WeaponParameterType = 1;
			this.SkillId = skillId;
		}

		/// <summary>
		/// Returns true if the specified option is set.
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public bool Has(AttackerOptions option)
		{
			return ((this.Options & option) != 0);
		}

		/// <summary>
		/// Enables option(s)
		/// </summary>
		/// <param name="option"></param>
		public void Set(AttackerOptions option)
		{
			this.Options |= option;
		}
	}

	/// <summary>
	/// Contains information about the target action part of CombatActionPack.
	/// Multiple target actions are used, depending on the amount of targets.
	/// </summary>
	public class TargetAction : CombatAction
	{
		/// <summary>
		/// Target options
		/// </summary>
		public TargetOptions Options { get; set; }

		/// <summary>
		/// Creature attacking the target
		/// </summary>
		public Creature Attacker { get; set; }

		/// <summary>
		/// Animation delay
		/// </summary>
		public int Delay { get; set; }

		/// <summary>
		/// Normal damage
		/// </summary>
		public float Damage { get; set; }

		/// <summary>
		/// Wounds inflicted in this combat action.
		/// </summary>
		public float Wound { get; set; }

		/// <summary>
		/// Mana damage (Mana Shield, blue)
		/// </summary>
		public float ManaDamage { get; set; }

		/// <summary>
		/// Action's effect flags.
		/// </summary>
		public EffectFlags EffectFlags { get; set; }

		/// <summary>
		/// Skill used by the attacker
		/// </summary>
		public SkillId AttackerSkillId { get; set; }

		/// <summary>
		/// Returns true if any option involving knocking back/down is
		/// active, including finishers.
		/// </summary>
		public override bool IsKnockBack
		{
			get { return this.Has(TargetOptions.Downed); }
		}

		/// <summary>
		/// Attack or Target action.
		/// </summary>
		public override CombatActionCategory Category { get { return CombatActionCategory.Target; } }

		/// <summary>
		/// Creates new target action.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="creature"></param>
		/// <param name="attacker"></param>
		/// <param name="attackerSkillId"></param>
		public TargetAction(CombatActionType type, Creature creature, Creature attacker, SkillId attackerSkillId)
		{
			this.Flags = type;
			this.Creature = creature;
			this.Attacker = attacker;
			this.AttackerSkillId = attackerSkillId;

			// The target's skill id is 'CombatMastery' if no skill is loaded,
			// 'None' if a skill is currently being loaded, and equal to the
			// loaded skill if it's ready.
			var activeSkill = creature.Skills.ActiveSkill;
			if (activeSkill == null)
				this.SkillId = SkillId.CombatMastery;
			else if (activeSkill.State != SkillState.Ready)
				this.SkillId = SkillId.None;
			else
				this.SkillId = activeSkill.Info.Id;
		}

		/// <summary>
		/// Returns true if the specified option is set.
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public bool Has(TargetOptions option)
		{
			return ((this.Options & option) != 0);
		}

		/// <summary>
		/// Returns true if the specified flag is set.
		/// </summary>
		/// <param name="flags"></param>
		/// <returns></returns>
		public bool Has(EffectFlags flags)
		{
			return ((this.EffectFlags & flags) != 0);
		}

		/// <summary>
		/// Enables option(s)
		/// </summary>
		/// <param name="option"></param>
		public void Set(TargetOptions option)
		{
			this.Options |= option;
		}
	}

	public enum CombatActionCategory { Attack, Target }
}
