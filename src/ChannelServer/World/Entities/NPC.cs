﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aura.Mabi.Const;
using Aura.Channel.Scripting.Scripts;
using Aura.Shared.Util;
using System;
using Aura.Channel.Network.Sending;
using Aura.Data.Database;
using Aura.Data;

namespace Aura.Channel.World.Entities
{
	public class NPC : Creature
	{
		/// <summary>
		/// Unique entity id increased and used for each NPC.
		/// </summary>
		private static long _npcId = MabiId.Npcs;

		/// <summary>
		/// Time in seconds it takes a creature (i.e. a monster)
		/// to disappear after dying.
		/// </summary>
		public const int DisappearDelay = 20;

		/// <summary>
		/// Type of the NpcScript used by the NPC.
		/// </summary>
		public Type ScriptType { get; set; }

		/// <summary>
		/// AI controlling the NPC
		/// </summary>
		public AiScript AI { get; set; }

		/// <summary>
		/// Creature spawn id, used for respawning.
		/// </summary>
		public int SpawnId { get; set; }

		/// <summary>
		/// NPCs preferences regarding gifts.
		/// </summary>
		public GiftWeightInfo GiftWeights { get; set; }

		/// <summary>
		/// Location the NPC was spawned at.
		/// </summary>
		public Location SpawnLocation { get; set; }

		/// <summary>
		/// Custom portrait in dialog.
		/// </summary>
		public string DialogPortrait { get; set; }

		/// <summary>
		/// Creates new NPC
		/// </summary>
		public NPC()
		{
			this.EntityId = GetNewNpcEntityId();

			// Some default values to prevent errors
			this.Name = "_undefined";
			this.RaceId = 190140; // Wood dummy
			this.Height = this.Weight = this.Upper = this.Lower = 1;
			this.RegionId = 0;
			this.Life = this.LifeMaxBase = 1000;
			this.Color1 = this.Color2 = this.Color3 = 0x808080;
			this.GiftWeights = new GiftWeightInfo();
		}

		/// <summary>
		/// Creates new NPC and loads defaults for race.
		/// </summary>
		/// <param name="raceId"></param>
		public NPC(int raceId)
			: this()
		{
			this.RaceId = raceId;
			this.LoadDefault();

			// Technically the following would belong in LoadDefault,
			// but NPCs in NPC scripts are loaded a little weird,
			// so we only load the following for NPCs who's race id
			// we get in advance.

			var rnd = RandomProvider.Get();

			// Set some base information
			this.Name = this.RaceData.Name;
			this.Color1 = this.RaceData.Color1;
			this.Color2 = this.RaceData.Color2;
			this.Color3 = this.RaceData.Color3;
			this.Height = (float)(this.RaceData.SizeMin + rnd.NextDouble() * (this.RaceData.SizeMax - this.RaceData.SizeMin));
			this.Life = this.LifeMaxBase = this.RaceData.Life;
			this.Mana = this.ManaMaxBase = this.RaceData.Mana;
			this.Stamina = this.StaminaMaxBase = this.RaceData.Stamina;
			this.State = (CreatureStates)this.RaceData.DefaultState;
			this.Direction = (byte)rnd.Next(256);

			// Set drops
			this.Drops.GoldMin = this.RaceData.GoldMin;
			this.Drops.GoldMax = this.RaceData.GoldMax;
			this.Drops.Add(this.RaceData.Drops);

			// Give skills
			foreach (var skill in this.RaceData.Skills)
				this.Skills.Add((SkillId)skill.SkillId, (SkillRank)skill.Rank, this.RaceId);

			// Equipment
			foreach (var itemData in this.RaceData.Equip)
			{
				var item = new Item(itemData.GetRandomId(rnd));
				if (itemData.Color1s.Count > 0) item.Info.Color1 = itemData.GetRandomColor1(rnd);
				if (itemData.Color2s.Count > 0) item.Info.Color2 = itemData.GetRandomColor2(rnd);
				if (itemData.Color3s.Count > 0) item.Info.Color3 = itemData.GetRandomColor3(rnd);

				var pocket = (Pocket)itemData.Pocket;
				if (pocket != Pocket.None)
					this.Inventory.Add(item, pocket);
			}

			// Face
			if (this.RaceData.Face.EyeColors.Count > 0) this.EyeColor = (byte)this.RaceData.Face.GetRandomEyeColor(rnd);
			if (this.RaceData.Face.EyeTypes.Count > 0) this.EyeType = (short)this.RaceData.Face.GetRandomEyeType(rnd);
			if (this.RaceData.Face.MouthTypes.Count > 0) this.MouthType = (byte)this.RaceData.Face.GetRandomMouthType(rnd);
			if (this.RaceData.Face.SkinColors.Count > 0) this.SkinColor = (byte)this.RaceData.Face.GetRandomSkinColor(rnd);

			// Set AI
			if (!string.IsNullOrWhiteSpace(this.RaceData.AI) && this.RaceData.AI != "none")
			{
				this.AI = ChannelServer.Instance.ScriptManager.AiScripts.CreateAi(this.RaceData.AI, this);
				if (this.AI == null)
					Log.Warning("ScriptManager.Spawn: Missing AI '{0}' for '{1}'.", this.RaceData.AI, this.RaceId);
			}
		}

		/// <summary>
		/// Creates new NPC from actor data.
		/// </summary>
		/// <param name="actorData"></param>
		public NPC(ActorData actorData)
			: this(actorData.RaceId)
		{
			if (actorData.HasColors)
			{
				this.Color1 = actorData.Color1;
				this.Color2 = actorData.Color2;
				this.Color3 = actorData.Color3;
			}

			this.Height = actorData.Height;
			this.Weight = actorData.Weight;
			this.Upper = actorData.Upper;
			this.Lower = actorData.Lower;
			this.EyeColor = (byte)actorData.EyeColor;
			this.EyeType = (short)actorData.EyeType;
			this.MouthType = (byte)actorData.MouthType;
			this.SkinColor = (byte)actorData.SkinColor;

			this.Titles.SelectedTitle = (ushort)actorData.Title;
			this.Age = (short)actorData.Age;
			this.Level = (short)actorData.Level;
			this.AbilityPoints = (short)actorData.AP;
			this.LifeMaxBase = actorData.Life;
			this.ManaMaxBase = actorData.Mana;
			this.StaminaMaxBase = actorData.Stamina;
			this.StrBase = actorData.Str;
			this.IntBase = actorData.Int;
			this.DexBase = actorData.Dex;
			this.WillBase = actorData.Will;
			this.LuckBase = actorData.Luck;

			// Hair and Face
			if (actorData.FaceItemId != 0)
			{
				var face = new Item(actorData.FaceItemId);
				face.Info.Color1 = (byte)actorData.SkinColor;
				this.Inventory.Add(face, Pocket.Face);
			}
			if (actorData.HairItemId != 0)
			{
				var hair = new Item(actorData.HairItemId);
				hair.Info.Color1 = actorData.HairColor;
				this.Inventory.Add(hair, Pocket.Hair);
			}

			// Items
			foreach (var itemData in actorData.Items)
			{
				var item = new Item(itemData.ItemId);
				item.Info.State = (byte)itemData.State;

				item.Info.Amount = (ushort)itemData.Amount;
				if (item.Data.StackType != StackType.Sac && item.Info.Amount < 1)
					item.Info.Amount = 1;

				if (itemData.HasColors)
				{
					item.Info.Color1 = itemData.Color1;
					item.Info.Color2 = itemData.Color2;
					item.Info.Color3 = itemData.Color3;
				}

				var pocket = (Pocket)itemData.Pocket;
				if (pocket != Pocket.None)
					this.Inventory.Add(item, pocket);
			}

			// Skills
			foreach (var skillData in actorData.Skills)
				this.Skills.Add(skillData.SkillId, skillData.Rank, this.RaceId);

			// Max out after skills and items were added (bonuses)
			this.Life = this.LifeMax;
			this.Mana = this.ManaMax;
			this.Stamina = this.StaminaMax;
		}

		/// <summary>
		/// Returns a new, unused entity id in the NPC range.
		/// </summary>
		public static long GetNewNpcEntityId()
		{
			return Interlocked.Increment(ref _npcId);
		}

		/// <summary>
		/// Disposes AI.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			if (this.AI != null)
				this.AI.Dispose();
		}

		/// <summary>
		/// Moves NPC to target location and adds it to the region.
		/// Returns false if region doesn't exist.
		/// </summary>
		/// <param name="regionId"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Warp(int regionId, int x, int y)
		{
			var region = ChannelServer.Instance.World.GetRegion(regionId);
			if (region == null)
			{
				Log.Error("NPC.Warp: Region '{0}' doesn't exist.", regionId);
				return false;
			}

			if (this.Region != Region.Limbo)
				this.Region.RemoveCreature(this);

			this.SetLocation(regionId, x, y);

			region.AddCreature(this);

			return true;
		}

		/// <summary>
		/// Like <see cref="Warp"/>, except it sends a screen flash
		/// and sound effect to the departing region and arriving region.
		/// </summary>
		/// <param name="regionId"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <remarks>Ideal for NPCs like Tarlach. Be careful not to "double flash"
		/// if you're swapping two NPCs. Only ONE of the NPCs needs to use this method,
		/// the other can use the regular <see cref="Warp"/>.</remarks>
		/// <returns></returns>
		public bool WarpFlash(int regionId, int x, int y)
		{
			// "Departing" effect
			if (this.Region != Region.Limbo)
			{
				Send.Effect(this, Effect.ScreenFlash, 3000, 0);
				Send.PlaySound(this, "data/sound/Tarlach_change.wav");
			}

			if (!this.Warp(regionId, x, y))
				return false;

			// "Arriving" effect
			Send.Effect(this, Effect.ScreenFlash, 3000, 0);
			Send.PlaySound(this, "data/sound/Tarlach_change.wav");

			return true;
		}

		/// <summary>
		/// Returns whether the NPC can target the given creature.
		/// </summary>
		/// <param name="creature"></param>
		/// <returns></returns>
		public override bool CanTarget(Creature creature)
		{
			if (!base.CanTarget(creature))
				return false;

			// Named NPCs (normal dialog ones) can't be targeted.
			// Important because AIs target /pc/ and most NPCs are humans.
			if (creature.Has(CreatureStates.NamedNpc))
				return false;

			return true;
		}

		/// <summary>
		/// Kills NPC, rewarding the killer.
		/// </summary>
		/// <param name="killer"></param>
		public override void Kill(Creature killer)
		{
			base.Kill(killer);

			this.DisappearTime = DateTime.Now.AddSeconds(NPC.DisappearDelay);

			if (killer == null)
				return;

			// Exp
			var exp = (long)(this.RaceData.Exp * ChannelServer.Instance.Conf.World.ExpRate);
			var expRule = killer.Party.ExpRule;

			if (!killer.IsInParty || expRule == PartyExpSharing.AllToFinish)
			{
				killer.GiveExp(exp);
				Send.CombatMessage(killer, "+{0} EXP", exp);
			}
			else
			{
				var members = killer.Party.GetMembers();
				var eaExp = 0L;
				var killerExp = 0L;
				var killerPos = killer.GetPosition();

				// Apply optional exp bonus
				if (members.Length > 1)
				{
					var extra = members.Length - 1;
					var bonus = ChannelServer.Instance.Conf.World.PartyExpBonus;

					exp += (long)(exp * ((extra * bonus) / 100f));
				}

				// Official simply ALWAYS divides by party member total,
				// even if they cannot recieve the experience.
				if (expRule == PartyExpSharing.Equal)
				{
					eaExp = exp / members.Length;
					killerExp = eaExp;
				}
				else if (expRule == PartyExpSharing.MoreToFinish)
				{
					exp /= 2;
					eaExp = exp / members.Length;
					killerExp = exp;
				}

				// Killer's exp
				killer.GiveExp(killerExp);
				Send.CombatMessage(killer, "+{0} EXP", killerExp);

				// Exp for members in range of killer, the range is unofficial
				foreach (var member in members.Where(a => a != killer && a.GetPosition().InRange(killerPos, 3000)))
				{
					member.GiveExp(eaExp);
					Send.CombatMessage(member, "+{0} EXP", eaExp);
				}
			}
		}

		/// <summary>
		/// NPCs may survive randomly.
		/// </summary>
		/// <remarks>
		/// http://wiki.mabinogiworld.com/view/Stats#Life
		/// More Will supposedly increases the chance. Unknown if this
		/// applies to players as well. Before certain Gs, NPCs weren't
		/// able to survive attacks under any circumstances.
		/// </remarks>
		/// <param name="damage"></param>
		/// <param name="from"></param>
		/// <param name="lifeBefore"></param>
		/// <returns></returns>
		protected override bool ShouldSurvive(float damage, Creature from, float lifeBefore)
		{
			// No surviving once you're in deadly
			if (lifeBefore < 0)
				return false;

			if (!AuraData.FeaturesDb.IsEnabled("DeadlyNPCs"))
				return false;

			// Chance = Will/10, capped at 50%
			// (i.e 80 Will = 8%, 500+ Will = 50%)
			// Actual formula unknown
			var chance = Math.Min(50, this.Will / 10);
			return (RandomProvider.Get().Next(101) < chance);
		}

		/// <summary>
		/// Returns how well the NPC remembers the other creature.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int GetMemory(Creature other)
		{
			// Get NPC memory and last change date
			var memory = other.Vars.Perm["npc_memory_" + this.Name] ?? 0;
			var change = other.Vars.Perm["npc_memory_change_" + this.Name];

			// Reduce memory by 1 each day
			if (change != null && memory > 0)
			{
				TimeSpan diff = DateTime.Now - change;
				memory = Math.Max(0, memory - Math.Floor(diff.TotalDays));
			}

			return (int)memory;
		}

		/// <summary>
		/// Modifies how well the NPC remembers the other creature.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="value"></param>
		/// <returns>New memory value</returns>
		public int SetMemory(Creature other, int value)
		{
			value = Math.Max(0, value);

			other.Vars.Perm["npc_memory_" + this.Name] = value;
			other.Vars.Perm["npc_memory_change_" + this.Name] = DateTime.Now;

			return value;
		}

		/// <summary>
		/// Sets how well the NPC remembers the other creature.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="value"></param>
		/// <returns>New memory value</returns>
		public int ModifyMemory(Creature other, int value)
		{
			return this.SetMemory(other, this.GetMemory(other) + value);
		}

		/// <summary>
		/// Returns favor of the NPC towards the other creature.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int GetFavor(Creature other)
		{
			// Get NPC favor and last change date
			var favor = other.Vars.Perm["npc_favor_" + this.Name] ?? 0;
			var change = other.Vars.Perm["npc_favor_change_" + this.Name];

			// Reduce favor by 1 each hour
			if (change != null && favor > 0)
			{
				TimeSpan diff = DateTime.Now - change;
				favor = Math.Max(0, favor - Math.Floor(diff.TotalHours));
			}

			return (int)favor;
		}

		/// <summary>
		/// Sets favor of the NPC towards the other creature.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="value"></param>
		/// <returns>New favor value</returns>
		public int SetFavor(Creature other, int value)
		{
			other.Vars.Perm["npc_favor_" + this.Name] = value;
			other.Vars.Perm["npc_favor_change_" + this.Name] = DateTime.Now;

			return value;
		}

		/// <summary>
		/// Modifies favor of the NPC towards the other creature.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="value"></param>
		/// <returns>New favor value</returns>
		public int ModifyFavor(Creature other, int value)
		{
			return this.SetFavor(other, this.GetFavor(other) + value);
		}

		/// <summary>
		/// Gets how much the other creature is stressing the NPC.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int GetStress(Creature other)
		{
			// Get NPC stress and last change date
			var stress = other.Vars.Perm["npc_stress_" + this.Name] ?? 0;
			var change = other.Vars.Perm["npc_stress_change_" + this.Name];

			// Reduce stress by 1 each minute
			if (change != null && stress > 0)
			{
				TimeSpan diff = DateTime.Now - change;
				stress = Math.Max(0, stress - Math.Floor(diff.TotalMinutes));
			}

			return (int)stress;
		}

		/// <summary>
		/// Sets how much the other creature is stressing the NPC.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="value"></param>
		/// <returns>New stress value</returns>
		public int SetStress(Creature other, int value)
		{
			value = Math.Max(0, value);

			other.Vars.Perm["npc_stress_" + this.Name] = value;
			other.Vars.Perm["npc_stress_change_" + this.Name] = DateTime.Now;

			return value;
		}

		/// <summary>
		/// Modifies how much the other creature is stressing the NPC.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="value"></param>
		/// <returns>New stress value</returns>
		public int ModifyStress(Creature other, int value)
		{
			return this.SetStress(other, this.GetStress(other) + value);
		}

		/// <summary>
		/// Aggroes target, setting target and putting creature in battle stance.
		/// </summary>
		/// <param name="creature"></param>
		public override void Aggro(Creature target)
		{
			if (this.AI == null)
				return;

			// Aggro attacker if there is not current target,
			// or if there is a target but it's not a player, and the attacker is one,
			// or if the current target is not aggroed yet.
			if (this.Target == null || (this.Target != null && target != null && !this.Target.IsPlayer && target.IsPlayer) || this.AI.State != AiScript.AiState.Aggro)
				this.AI.AggroCreature(target);
		}

		/// <summary>
		/// Sets SpawnLocation and places NPC in region.
		/// </summary>
		/// <param name="regionId"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>Returns false if NPC is spawned already or region doesn't exist.</returns>
		public bool Spawn(int regionId, int x, int y)
		{
			// Already spawned
			if (this.Region != Region.Limbo)
			{
				Log.Error("NPC.Spawn: Failed to spawn '{0}', it was spawned already.", this.RaceId, this.RegionId);
				return false;
			}

			// Save spawn location
			this.SpawnLocation = new Location(this.RegionId, x, y);

			// Warp to spawn point
			if (!this.Warp(regionId, x, y))
			{
				Log.Error("NPC.Spawn: Failed to spawn '{0}', region '{1}' doesn't exist.", this.RaceId, this.RegionId);
				return false;
			}

			return true;
		}

		/// <summary>
		/// TODO: Move somewhere? =/
		/// </summary>
		public class GiftWeightInfo
		{
			public float Adult { get; set; }
			public float Anime { get; set; }
			public float Beauty { get; set; }
			public float Individuality { get; set; }
			public float Luxury { get; set; }
			public float Maniac { get; set; }
			public float Meaning { get; set; }
			public float Rarity { get; set; }
			public float Sexy { get; set; }
			public float Toughness { get; set; }
			public float Utility { get; set; }

			public int CalculateScore(Item gift)
			{
				var score = 0f;

				var taste = gift.Data.Taste;

				score += this.Adult * taste.Adult;
				score += this.Anime * taste.Anime;
				score += this.Beauty * taste.Beauty;
				score += this.Individuality * taste.Individuality;
				score += this.Luxury * taste.Luxury;
				score += this.Maniac * taste.Maniac;
				score += this.Meaning * taste.Meaning;
				score += this.Rarity * taste.Rarity;
				score += this.Sexy * taste.Sexy;
				score += this.Toughness * taste.Toughness;
				score += this.Utility * taste.Utility;

				score /= 8;

				return (int)score;
			}
		}
	}
}
