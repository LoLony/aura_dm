﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Channel.World.Entities;
using Aura.Data;
using Aura.Data.Database;
using Aura.Mabi.Const;
using Aura.Mabi.Network;
using Aura.Shared.Network;
using System;
using System.Linq;

namespace Aura.Channel.Network.Sending.Helpers
{
	public static class CreatureHelper
	{
		public static Packet AddCreatureInfo(this Packet packet, Creature creature, CreaturePacketType type)
		{
			var pos = creature.GetPosition();
			var account = creature.Client.Account;

			// Start
			// --------------------------------------------------------------

			packet.PutLong(creature.EntityId);
			packet.PutByte((byte)type);

			// Looks/Location
			// --------------------------------------------------------------
			packet.PutString(creature.Name);
			packet.PutString("");				 // Title
			packet.PutString("");				 // Eng Title
			packet.PutInt(creature.RaceId);
			packet.PutByte(creature.SkinColor);
			packet.PutShort(creature.EyeType); // [180600, NA187 (25.06.2014)] Changed from byte to short
			packet.PutByte(creature.EyeColor);
			packet.PutByte(creature.MouthType);
			packet.PutUInt((uint)creature.State);
			if (type == CreaturePacketType.Public)
			{
				packet.PutUInt((uint)creature.StateEx);

				// [180300, NA166 (18.09.2013)]
				{
					packet.PutInt(0);
				}
			}
			packet.PutFloat(creature.Height);
			packet.PutFloat(creature.Weight);
			packet.PutFloat(creature.Upper);
			packet.PutFloat(creature.Lower);
			packet.PutInt(creature.RegionId);
			packet.PutInt(pos.X);
			packet.PutInt(pos.Y);
			packet.PutByte(creature.Direction);
			packet.PutInt(Convert.ToInt32(creature.IsInBattleStance));
			packet.PutByte((byte)creature.Inventory.WeaponSet);
			packet.PutUInt(creature.Color1);
			packet.PutUInt(creature.Color2);
			packet.PutUInt(creature.Color3);

			// Stats
			// --------------------------------------------------------------
			packet.PutFloat(creature.CombatPower);
			packet.PutString(creature.StandStyle);

			if (type == CreaturePacketType.Private)
			{
				packet.PutFloat(creature.Life);
				packet.PutFloat(creature.LifeInjured);
				packet.PutFloat(creature.LifeMaxBaseTotal);
				packet.PutFloat(creature.LifeMaxMod);
				packet.PutFloat(creature.Mana);
				packet.PutFloat(creature.ManaMaxBaseTotal);
				packet.PutFloat(creature.ManaMaxMod);
				packet.PutFloat(creature.Stamina);
				packet.PutFloat(creature.StaminaMaxBaseTotal);
				packet.PutFloat(creature.StaminaMaxMod);
				packet.PutFloat(creature.StaminaHunger);
				packet.PutFloat(0.5f);
				packet.PutShort(creature.Level);
				packet.PutInt(creature.TotalLevel - creature.Level);
				packet.PutShort(0);                  // Max Level (reached ever?)
				packet.PutShort((short)creature.RebirthCount);
				packet.PutShort(0);
				packet.PutLong(AuraData.ExpDb.CalculateRemaining(creature.Level, creature.Exp) * 1000);
				packet.PutShort(creature.Age);
				packet.PutFloat(creature.StrBaseTotal);
				packet.PutFloat(creature.StrMod);
				packet.PutFloat(creature.DexBaseTotal);
				packet.PutFloat(creature.DexMod);
				packet.PutFloat(creature.IntBaseTotal);
				packet.PutFloat(creature.IntMod);
				packet.PutFloat(creature.WillBaseTotal);
				packet.PutFloat(creature.WillMod);
				packet.PutFloat(creature.LuckBaseTotal);
				packet.PutFloat(creature.LuckMod);
				packet.PutFloat(creature.LifeFoodMod);
				packet.PutFloat(creature.ManaFoodMod);
				packet.PutFloat(creature.StaminaFoodMod);
				packet.PutFloat(creature.StrFoodMod);
				packet.PutFloat(creature.DexFoodMod);
				packet.PutFloat(creature.IntFoodMod);
				packet.PutFloat(creature.WillFoodMod);
				packet.PutFloat(creature.LuckFoodMod);
				packet.PutInt(creature.AbilityPoints); // [200100, NA229 (2016-06-16)] Changed from short to int
				packet.PutShort((short)creature.AttackMinBase);
				packet.PutShort((short)creature.AttackMinMod);
				packet.PutShort((short)creature.AttackMaxBase);
				packet.PutShort((short)creature.AttackMaxMod);
				packet.PutShort((short)creature.InjuryMinBase);
				packet.PutShort((short)creature.InjuryMinMod);
				packet.PutShort((short)creature.InjuryMaxBase);
				packet.PutShort((short)creature.InjuryMaxMod);
				packet.PutShort((short)creature.LeftAttackMinMod);
				packet.PutShort((short)creature.LeftAttackMaxMod);
				packet.PutShort((short)creature.RightAttackMinMod);
				packet.PutShort((short)creature.RightAttackMaxMod);
				packet.PutShort((short)creature.LeftInjuryMinMod);
				packet.PutShort((short)creature.LeftInjuryMaxMod);
				packet.PutShort((short)creature.RightInjuryMinMod);
				packet.PutShort((short)creature.RightInjuryMaxMod);
				packet.PutFloat(creature.LeftCriticalMod);
				packet.PutFloat(creature.RightCriticalMod);
				packet.PutShort((short)creature.LeftBalanceMod);
				packet.PutShort((short)creature.RightBalanceMod);
				packet.PutFloat(0);			         // MagicDefenseMod
				// [180300, NA166 (18.09.2013)] Magic Protection
				{
					packet.PutFloat(0);			     // MagicProtectMod
				}
				packet.PutFloat(0);			         // MagicAttackMod
				packet.PutShort(15);		         // MeleeAttackRateMod
				packet.PutShort(15);		         // RangeAttackRateMod
				packet.PutFloat(creature.CriticalBase);
				packet.PutFloat(0);			         // CriticalMod
				packet.PutFloat((short)creature.ProtectionBase);
				packet.PutFloat(creature.ProtectionMod);
				packet.PutShort((short)creature.DefenseBase);
				packet.PutShort((short)creature.DefenseMod);
				packet.PutShort((short)creature.BalanceBase);
				packet.PutShort(0);			         // RateMod
				packet.PutShort(0);			         // Rank1
				packet.PutShort(0);			         // Rank2
				// [180300, NA166 (18.09.2013)] Armor Pierce
				{
					packet.PutShort(0);			     // ArmorPierceMod
				}
				packet.PutLong(0);			         // Score
				packet.PutShort((short)creature.AttackMinBaseMod);
				packet.PutShort((short)creature.AttackMaxBaseMod);
				packet.PutShort((short)creature.InjuryMinBaseMod);
				packet.PutShort((short)creature.InjuryMaxBaseMod);
				packet.PutFloat(creature.CriticalBaseMod);
				packet.PutFloat(creature.ProtectionBaseMod);
				packet.PutShort((short)creature.DefenseBaseMod);
				packet.PutShort((short)creature.BalanceBaseMod);

				// In some tests the damage display would be messed up if
				// those two weren't set to something.
				// In recent tests they were simply added to the min/max dmg,
				// purpose unknown.
				packet.PutShort(0);                  // MeleeAttackMinBaseMod (8 / 3)
				packet.PutShort(0);                  // MeleeAttackMaxBaseMod (18 / 4)

				packet.PutShort(0);                  // MeleeInjuryMinBaseMod
				packet.PutShort(0);                  // MeleeInjuryMaxBaseMod
				packet.PutShort(0);                  // RangeAttackMinBaseMod (10)
				packet.PutShort(0);                  // RangeAttackMaxBaseMod (25)
				packet.PutShort(0);                  // RangeInjuryMinBaseMod
				packet.PutShort(0);                  // RangeInjuryMaxBaseMod
				// [180100] Guns
				{
					packet.PutShort(0);			     // DualgunAttackMinBaseMod
					packet.PutShort(0);			     // DualgunAttackMaxBaseMod
					packet.PutShort(0);			     // DualgunInjuryMinBaseMod
					packet.PutShort(0);			     // DualgunInjuryMaxBaseMod
				}
				// [180800, NA189 (23.07.2014)] Ninja?
				{
					packet.PutShort(0);			     // ? AttackMinBaseMod
					packet.PutShort(0);			     // ? AttackMaxBaseMod
					packet.PutShort(0);			     // ? InjuryMinBaseMod
					packet.PutShort(0);			     // ? InjuryMaxBaseMod
				}
				packet.PutShort(0);			         // PoisonBase
				packet.PutShort(0);			         // PoisonMod
				packet.PutShort(67);		         // PoisonImmuneBase
				packet.PutShort(0);			         // PoisonImmuneMod
				packet.PutFloat(0.5f);		         // PoisonDamageRatio1
				packet.PutFloat(0);			         // PoisonDamageRatio2
				packet.PutFloat(creature.ToxicStr);
				packet.PutFloat(creature.ToxicInt);
				packet.PutFloat(creature.ToxicDex);
				packet.PutFloat(creature.ToxicWill);
				packet.PutFloat(creature.ToxicLuck);
				packet.PutString(creature.LastTown);
				packet.PutShort(1);					 // ExploLevel
				packet.PutShort(0);					 // ExploMaxKeyLevel
				packet.PutInt(0);					 // ExploCumLevel
				packet.PutLong(0);					 // ExploExp
				packet.PutInt(0);					 // DiscoverCount
				packet.PutFloat(0);					 // conditionStr
				packet.PutFloat(0);					 // conditionInt
				packet.PutFloat(0);					 // conditionDex
				packet.PutFloat(0);					 // conditionWill
				packet.PutFloat(0);					 // conditionLuck
				packet.PutByte(9);					 // ElementPhysical
				packet.PutByte(0);					 // ElementLightning
				packet.PutByte(0);					 // ElementFire
				packet.PutByte(0);					 // ElementIce

				// [180800, NA196 (14.10.2014)] ?
				{
					packet.PutByte(0);
					packet.PutByte(0);
				}

				// [190200, NA203 (22.04.2015)] ?
				{
					packet.PutByte(0);
				}

				// [200200, NA233 (2016-08-12)] ?
				{
					packet.PutByte(0);
				}

				var regens = creature.Regens.GetList();
				packet.PutInt(regens.Count);
				foreach (var regen in regens)
					packet.AddRegen(regen);
			}
			else if (type == CreaturePacketType.Public || type == CreaturePacketType.Minimal)
			{
				packet.PutFloat(creature.Life);
				packet.PutFloat(creature.LifeMaxBaseTotal);
				packet.PutFloat(creature.LifeMaxMod);
				packet.PutFloat(creature.LifeInjured);

				// [180800, NA196 (14.10.2014)] ?
				{
					packet.PutShort(0);
				}

				var regens = creature.Regens.GetPublicList();
				packet.PutInt(regens.Count);
				foreach (var regen in regens)
					packet.AddRegen(regen);

				// Another 6 elements list?
				packet.PutInt(0);
			}

			// Titles
			// --------------------------------------------------------------
			packet.PutUShort(creature.Titles.SelectedTitle);
			packet.PutLong(creature.Titles.Applied);
			if (type == CreaturePacketType.Private)
			{
				// List of available titles
				var titles = creature.Titles.GetList();
				packet.PutShort((short)titles.Count);
				foreach (var title in titles)
				{
					packet.PutUShort(title.Key);
					packet.PutByte((byte)title.Value);
					packet.PutLong(0); // [190100, NA200 (2014-01-15)] Changed from Int to Long
				}
			}
			if (type == CreaturePacketType.Private || type == CreaturePacketType.Public)
			{
				packet.PutUShort(creature.Titles.SelectedOptionTitle);
			}

			// Items and expiring? (Last part of minimal)
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Minimal)
			{
				packet.PutString("");
				packet.PutByte(0);

				var items = creature.Inventory.GetAllEquipment();

				packet.PutInt(items.Length);
				foreach (var item in items)
				{
					packet.PutLong(item.EntityId);
					packet.PutBin(item.Info);
				}

				packet.PutInt(0);  // PetRemainingTime
				packet.PutLong(0); // PetLastTime
				packet.PutLong(0); // PetExpireTime

				return packet;
			}

			// Mate
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutLong(0);					 // MateID
				packet.PutString("");				 // MateName
				packet.PutLong(0);					 // MarriageTime
				packet.PutShort(0);					 // MarriageCount
			}
			else if (type == CreaturePacketType.Public)
			{
				packet.PutString("");				 // MateName
			}

			// Destiny
			// --------------------------------------------------------------
			packet.PutByte(0);			             // (0:Venturer, 1:Knight, 2:Wizard, 3:Bard, 4:Merchant, 5:Alchemist)

			// Inventory
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutInt(creature.InventoryWidth);
				packet.PutInt(creature.InventoryHeight);

				var items = creature.Inventory.GetItems();
				packet.PutInt(items.Length);
				foreach (var item in items)
					packet.AddItemInfo(item, ItemPacketType.Private);
			}
			else if (type == CreaturePacketType.Public)
			{
				var items = creature.Inventory.GetAllEquipment();

				packet.PutInt(items.Length);
				foreach (var item in items)
				{
					packet.PutLong(item.EntityId);
					packet.PutBin(item.Info);
				}
			}

			// [180300, NA169 (23.10.2013)] ?
			// Strange one, it's in the logs, but stucks the char in
			// casting animation. Dependent on something?
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				//packet.PutInt(2); // Count?
				//packet.PutInt(36);
				//packet.PutInt(8);
				//packet.PutInt(38);
				//packet.PutInt(4);
			}

			// Keywords
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				var keywords = creature.Keywords.GetList();
				packet.PutShort((short)keywords.Count);
				foreach (var keyword in keywords)
					packet.PutUShort(keyword);
			}

			// Skills
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				var skills = creature.Skills.GetList();
				packet.PutShort((short)skills.Count);
				foreach (var skill in skills)
					packet.PutBin(skill.Info);
				packet.PutInt(0);			     // SkillVarBufferList
				// loop						         
				//   packet.PutInt
				//   packet.PutFloat
			}
			else if (type == CreaturePacketType.Public)
			{
				packet.PutShort(0);			     // CurrentSkill
				packet.PutByte(0);			     // SkillStackCount
				packet.PutInt(0);			     // SkillProgress

				// Wrong?
				//packet.PutInt(0);			     // SkillSyncList
				// loop						         
				//   packet.PutShort
				//   packet.PutShort

				// Not 100% sure what this is, Yiting added the above years
				// ago, now it looks like this is a list of skill bins.
				// The skills listed seem to be skills of type "7",
				// which seem to be skills that have their Start/Stop
				// packets being broadcasted.
				// It's possible that it was two shorts originally,
				// the skill id + the flags. [exec]
				var skills = creature.Skills.GetList(s => s.Data.Type == SkillType.BroadcastStartStop);
				packet.PutInt(skills.Count);
				foreach (var skill in skills)
					packet.PutBin(skill.Info);
			}

			// [150100] ?
			{
				packet.PutByte(0);			     // {PLGCNT}
			}

			// [190200, NA203 (24.04.2015)] ?
			{
				packet.PutInt(0);
			}

			// Party
			// --------------------------------------------------------------
			if (creature.IsInParty)
			{
				packet.PutByte(creature.Party.IsOpen && creature.Party.Leader == creature);
				packet.PutString(creature.Party.ToString());
			}
			else
			{
				packet.PutByte(0);
				packet.PutString("");
			}

			// PvP
			// --------------------------------------------------------------
			packet.AddPvPInfo(creature);

			// [180800, NA196 (14.10.2014)] ?
			{
				packet.PutByte(0);
			}

			// Conditions
			// --------------------------------------------------------------
			packet.AddConditions(creature.Conditions);

			// Guild
			// --------------------------------------------------------------
			if (creature.Guild != null)
			{
				packet.PutLong(creature.Guild.Id);
				packet.PutString(creature.Guild.Name);
				packet.PutInt((int)creature.GuildMember.Rank);
				if (creature.Guild.HasRobe)
				{
					packet.PutByte(creature.Guild.Robe.EmblemMark);
					packet.PutByte(creature.Guild.Robe.EmblemOutline);
					packet.PutByte(creature.Guild.Robe.Stripes);
					packet.PutUInt(creature.Guild.Robe.RobeColor);
					packet.PutByte(creature.Guild.Robe.BadgeColor);
					packet.PutByte(creature.Guild.Robe.EmblemMarkColor);
					packet.PutByte(creature.Guild.Robe.EmblemOutlineColor);
					packet.PutByte(creature.Guild.Robe.StripesColor);
				}
				else
				{
					packet.PutByte(0);
					packet.PutByte(0);
					packet.PutByte(0);
					packet.PutInt(0);
					packet.PutByte(0);
					packet.PutByte(0);
					packet.PutByte(0);
					packet.PutByte(0);
				}
				packet.PutString(creature.Guild.Title);
			}
			else
			{
				packet.PutLong(0);
				packet.PutString("");
				packet.PutInt(0);
				packet.PutByte(0);
				packet.PutByte(0);
				packet.PutByte(0);
				packet.PutInt(0);
				packet.PutByte(0);
				packet.PutByte(0);
				packet.PutByte(0);
				packet.PutByte(0);
				packet.PutString("");
			}

			// PTJ
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutLong(0);				     // ArbeitID

				var records = creature.Quests.GetPtjTrackRecords();
				packet.PutInt(records.Length);
				foreach (var record in records)
				{
					packet.PutShort((short)record.Type);
					packet.PutShort((short)record.Done);
					packet.PutShort((short)record.Success);
				}
			}

			// Following a master
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				if (creature.Master != null)
				{
					packet.PutLong(creature.Master.EntityId);
					packet.PutByte((byte)SubordinateType.Pet);
					packet.PutByte(0);				 // SubType
				}
				else if (creature.IsRpCharacter)
				{
					var rpCharacter = creature as RpCharacter;
					packet.PutLong(rpCharacter.Actor.EntityId);
					packet.PutByte((byte)SubordinateType.RpCharacter);
					packet.PutByte(0);
				}
				else
				{
					packet.PutLong(0);
					packet.PutByte(0);
					packet.PutByte(0);
				}
			}

			// [170100] ?
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutFloat(1);
				packet.PutLong(0);
			}

			// Transformation
			// --------------------------------------------------------------
			packet.PutByte(0);				     // Type (1:Paladin, 2:DarkKnight, 3:SubraceTransformed, 4:TransformedElf, 5:TransformedGiant)
			packet.PutShort(0);				     // Level
			packet.PutShort(0);				     // SubType

			// Pet
			// --------------------------------------------------------------
			if (creature.Master != null)
			{
				packet.PutString(creature.Master.Name);

				if (type == CreaturePacketType.Private)
				{
					packet.PutInt(2000000000);			// RemainingSummonTime
					packet.PutLong(0);					// LastSummonTime
					packet.PutLong(0);					// PetExpireTime
					packet.PutByte(0);					// Loyalty
					packet.PutByte(0);					// Favor
					packet.PutLong(DateTime.Now);		// SummonTime
					packet.PutByte(0);					// KeepingMode
					packet.PutLong(0);					// KeepingProp
					packet.PutLong(creature.Master.EntityId);
					packet.PutByte(0);					// PetSealCount {PSCNT}
				}
				else if (type == CreaturePacketType.Public)
				{
					packet.PutLong(creature.Master.EntityId);
					packet.PutByte(0);				 // KeepingMode
					packet.PutLong(0);				 // KeepingProp
				}
			}
			else
			{
				packet.PutString("");

				if (type == CreaturePacketType.Private)
				{
					packet.PutInt(0);
					packet.PutLong(0);
					packet.PutLong(0);
					packet.PutByte(0);
					packet.PutByte(0);
					packet.PutLong(0);
					packet.PutByte(0);
					packet.PutLong(0);
					packet.PutLong(0);
					packet.PutByte(0);
				}
				else if (type == CreaturePacketType.Public)
				{
					packet.PutLong(0);
					packet.PutByte(0);
					packet.PutLong(0);
				}
			}

			// House
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
				packet.PutLong(0);				 // HouseID

			// Taming
			// --------------------------------------------------------------
			packet.PutLong(0);					 // MasterID
			packet.PutByte(0);					 // IsTamed
			packet.PutByte(0);					 // TamedType (1:DarkKnightTamed, 2:InstrumentTamed, 3:AnimalTraining, 4:MercenaryTamed, 5:Recalled, 6:SoulStoneTamed, 7:TamedFriend)
			packet.PutByte(1);					 // IsMasterMode
			packet.PutInt(0);					 // LimitTime

			// Vehicle
			// --------------------------------------------------------------
			packet.PutInt(0);					 // Type
			packet.PutInt(0);					 // TypeFlag (0x1:Driver, 0x4:Owner)
			packet.PutLong(0);					 // VehicleId
			packet.PutInt(0);					 // SeatIndex
			packet.PutByte(0);					 // PassengerList
			// loop
			//   packet.PutLong

			// Showdown
			// --------------------------------------------------------------
			packet.PutInt(0);	                 // unknown at 0x18
			packet.PutLong(0);                   // unknown at 0x08
			packet.PutLong(0);	                 // unknown at 0x10
			packet.PutByte(1);	                 // IsPartyPvpDropout

			// Transport
			// --------------------------------------------------------------
			packet.PutLong(0);					 // TransportID
			packet.PutInt(0);					 // HuntPoint

			// Aviation
			// --------------------------------------------------------------
			packet.PutByte(0); // --v
			//packet.PutByte(creature.IsFlying);
			//if (creature.IsFlying)
			//{
			//    packet.PutFloat(pos.X);
			//    packet.PutFloat(pos.H);
			//    packet.PutFloat(pos.Y);
			//    packet.PutFloat(creature.Destination.X);
			//    packet.PutFloat(creature.Destination.H);
			//    packet.PutFloat(creature.Destination.Y);
			//    packet.PutFloat(creature.Direction);
			//}

			// Skiing
			// --------------------------------------------------------------
			packet.PutByte(0);					 // IsSkiing
			// loop
			//   packet.PutFloat
			//   packet.PutFloat
			//   packet.PutFloat
			//   packet.PutFloat
			//   packet.PutInt
			//   packet.PutInt
			//   packet.PutByte
			//   packet.PutByte

			// Farming
			// [150100-170400] Public too
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutLong(0);					 // FarmId
				//   packet.PutLong
				//   packet.PutLong
				//   packet.PutLong
				//   packet.PutShort
				//   packet.PutShort
				//   packet.PutShort
				//   packet.PutShort
				//   packet.PutShort
				//   packet.PutShort
				//   packet.PutByte
				//   packet.PutLong
				//   packet.PutByte
				//   packet.PutLong
			}

			// Event (CaptureTheFlag, WaterBalloonBattle)
			// --------------------------------------------------------------
			packet.PutByte(0);				     // EventFullSuitIndex
			packet.PutByte(0);				     // TeamId
			// if?
			//   packet.PutInt					 // HitPoint
			//   packet.PutInt					 // MaxHitPoint

			// [170300] ?
			{
				packet.PutString("");
				packet.PutByte(0);
			}

			// Heartstickers
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutShort(0);
				packet.PutShort(0);
			}

			// Joust
			// --------------------------------------------------------------
			packet.PutInt(0);					 // JoustId
			if (type == CreaturePacketType.Private)
			{
				packet.PutInt(0);					 // JoustPoint
				packet.PutByte(0);					 // unknown at 0x1D
				packet.PutByte(0);					 // unknown at 0x1C
				packet.PutByte(0);					 // WeekWinCount
				packet.PutShort(0);					 // DailyWinCount
				packet.PutShort(0);					 // DailyLoseCount
				packet.PutShort(0);					 // ServerWinCount
				packet.PutShort(0);					 // ServerLoseCount
			}
			else if (type == CreaturePacketType.Public)
			{
				packet.PutLong(0);			         // HorseId
				packet.PutFloat(0);	                 // Life
				packet.PutInt(100);		             // LifeMax
				packet.PutByte(9);			         // unknown at 0x6C
				packet.PutByte(0);			         // IsJousting
			}

			// Achievements
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutInt(0);	                 // TotalScore
				packet.PutShort(0);                  // AchievementList
				// loop
				//   packet.PutShort achievementId
			}

			// PrivateFarm
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutInt(0);					 // FavoriteFarmList
				// loop
				//   packet.PutLong                  // FarmId
				//   packet.PutInt                   // ZoneId
				//   packet.PutShort                 // PosX
				//   packet.PutShort                 // PoxY
				//   packet.PutString                // FarmName
				//   packet.PutString                // OwnerName
			}

			// Family
			// --------------------------------------------------------------
			packet.PutLong(0);					 // FamilyId
			// if
			//   packet.PutString				 // FamilyName
			//   packet.PutShort
			//   packet.PutShort
			//   packet.PutShort
			//   packet.PutString				 // FamilyTitle

			// Demigod
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutInt(0);					 // SupportType (0:None, 1:Neamhain, 2:Morrighan)
			}

			// [150100] NPC options
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Public && creature is NPC)
			{
				packet.PutShort(0);		         // OnlyShowFilter
				packet.PutShort(0);		         // HideFilter
			}

			// [150100] Commerce
			// --------------------------------------------------------------
			{
				packet.PutByte(1);               // IsInCommerceCombat
				packet.PutLong(0);               // TransportCharacterId
				packet.PutFloat(1);              // ScaleHeight
			}

			// [170100] Talents
			// --------------------------------------------------------------
			{
				if (type == CreaturePacketType.Public)
				{
					packet.PutLong(0);
					packet.PutByte(0);
					packet.PutByte(0);
					packet.PutFloat(1);
					packet.PutLong(0);

					packet.PutShort(0); // --v
					packet.PutByte(0);  // --v
					//packet.PutShort((ushort)creature.Talents.SelectedTitle);
					//packet.PutByte((byte)creature.Talents.Grandmaster);
				}
				else if (type == CreaturePacketType.Private)
				{
					packet.AddPrivateTalentInfo(creature);
				}
			}

			// [170300] Shamala
			// --------------------------------------------------------------
			{
				if (type == CreaturePacketType.Private)
				{
					// Transformation Diary
					packet.PutInt(0); // --v
					//packet.PutSInt(character.Shamalas.Count);
					//foreach (var trans in character.Shamalas)
					//{
					//    packet.PutInt(trans.Id);
					//    packet.PutByte(trans.Counter);
					//    packet.PutByte((byte)trans.State);
					//}
				}
				else if (type == CreaturePacketType.Public)
				{
					// Current transformation info
					//if (creature.Shamala != null)
					//{
					//    packet.PutInt(creature.Shamala.Id);
					//    packet.PutByte(0);
					//    packet.PutInt(creature.ShamalaRace.Id);
					//    packet.PutFloat(creature.Shamala.Size);
					//    packet.PutInt(creature.Shamala.Color1);
					//    packet.PutInt(creature.Shamala.Color2);
					//    packet.PutInt(creature.Shamala.Color3);
					//}
					//else
					{
						packet.PutInt(0);
						packet.PutByte(0);
						packet.PutInt(0);
						packet.PutFloat(1);
						packet.PutInt(0x808080);
						packet.PutInt(0x808080);
						packet.PutInt(0x808080);
					}
					packet.PutByte(0);
					packet.PutByte(0);
				}
			}

			// [180100] ?
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutInt(0);
				packet.PutInt(0);
			}

			// [NA170403, TW170300] ?
			// --------------------------------------------------------------
			{
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutLong(0);

				// Rock/Paper/Scissors?
				packet.PutString(""); // Banner text?
				packet.PutByte(0);    // Banner enabled?
			}

			// [190100, NA198 (11.12.2014)] ?
			// --------------------------------------------------------------
			{
				packet.PutInt(0);
			}

			// [180300, NA166 (18.09.2013)] ?
			// Required, even though it looks like a list.
			// --------------------------------------------------------------
			{
				packet.PutInt(10); // Count?
				packet.PutLong(4194304);
				packet.PutInt(1347950097);
				packet.PutLong(34359771136);
				packet.PutInt(1346340501);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
			}

			// [180500, NA181 (12.02.2014)] ?
			// Without this the "me" creature in the Smash cutscene had a
			// red aura.
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Public)
			{
				packet.PutByte(0);
			}

			// Character
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Public)
			{
				packet.PutLong(0);			         // AimingTarget
				packet.PutLong(0);			         // Executor
				packet.PutShort(0);			         // ReviveTypeList
				// loop						         
				//   packet.PutInt	

				// < int g18 monsters?
			}

			packet.PutByte(0);					 // IsGhost

			// SittingProp
			if (creature.Temp.SittingProp == null)
				packet.PutLong(0);
			else
				packet.PutLong(creature.Temp.SittingProp.EntityId);

			packet.PutInt(-1);					 // SittedSocialMotionId

			// ? (Last Part of public, except for something at the very end)
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Public)
			{
				packet.PutLong(0);			         // DoubleGoreTarget (Doppelganger condition)
				packet.PutInt(0);			         // DoubleGoreTargetType

				// [180300, NA169 (23.10.2013)] ?
				{
					packet.PutLong(0);
				}

				if (!creature.IsMoving)
				{
					packet.PutByte(0);
				}
				else
				{
					var dest = creature.GetDestination();

					packet.PutByte((byte)(!creature.IsWalking ? 2 : 1));
					packet.PutInt(dest.X);
					packet.PutInt(dest.Y);
				}

				if (creature is NPC)
				{
					packet.PutString(creature.StandStyleTalking);
				}

				// [150100] Bomb Event
				{
					packet.PutByte(0);			     // BombEventState
				}

				// [170400] ?
				{
					packet.PutByte(0);
				}

				// [180?00] ?
				{
					packet.PutByte(1);
				}

				// [180500, NA181 (12.02.2014)] ?
				{
					packet.PutByte(1);
				}
			}

			if (type == CreaturePacketType.Private)
			{
				// private:

				// [JP] ?
				// This int is needed in the JP client (1704 log),
				// but doesn't appear in the NA 1704 or KR test 1801 log.
				{
					//packet.PutInt(4);
				}

				// Premium stuff
				// 
				// - Bags: Access to bags in inventory.
				// - Account Bank: Access to bank tabs of other characters.
				// - Premium Gestures: Access to premium gestures.
				// - VIP tab: Access to VIP inventory tab.
				// - Style tab: Access to Style inventory tab.
				// 
				// Last update of what the bytes do: 2015-11-02
				// --------------------------------------------------------------
				// [180600, NA187 (25.06.2014)] ?
				{
					packet.PutByte(0);
				}
				packet.PutByte(false);                                           // ? (formerly IsUsingExtraStorage)
				packet.PutByte(account.PremiumServices.HasVipService);           // Style tab (formerly IsUsingNaosSupport)
				packet.PutByte(false);                                           // ? (formerly IsUsingAdvancedPlay)
				packet.PutByte(false);                                           // ?
				packet.PutByte(account.PremiumServices.HasPremiumService);       // Bags, Account Bank, Premium Gestures
				packet.PutByte(false);                                           // ? (formerly Premium Gestures?)
				packet.PutByte(true);                                            // ? (Default 1 on NA?)
				packet.PutByte(account.PremiumServices.HasInventoryPlusService); // Bags, Account Bank
				// [170402, TW170300] New premium thing
				{
					packet.PutByte(account.PremiumServices.HasVipService);       // Bags, Account Bank, Premium Gestures, VIP tab
				}
				// [180300, NA166 (18.09.2013)] ?
				{
					packet.PutByte(false);                                       // Bags, Account Bank, Premium Gestures, VIP tab
					packet.PutByte(false);                                       // Bags, Account Bank, Premium Gestures, VIP tab
				}
				// [180800, NA196 (14.10.2014)] ?
				{
					packet.PutByte(0);
				}
				packet.PutInt(0);
				packet.PutByte(0);
				packet.PutInt(0);
				packet.PutInt(0);
				packet.PutInt(0);

				// Quests
				// --------------------------------------------------------------
				var quests = creature.Quests.GetIncompleteList();
				packet.PutInt(quests.Count);
				foreach (var quest in quests)
					packet.AddQuest(quest);

				// Char
				// --------------------------------------------------------------
				packet.PutByte(0);					 // NaoDress (0:normal, 12:??, 13:??)
				packet.PutLong(creature.CreationTime);
				packet.PutLong(creature.LastRebirth);
				packet.PutString("");
				packet.PutByte(0); // "true" makes character lie on floor?
				packet.PutByte(2);

				// [150100] Pocket ExpireTime List
				// Apperantly a list of "pockets"?, incl expiration time.
				// Ends with a long 0?
				// --------------------------------------------------------------
				{
					// Style
					// This is how the style tab was enabled in the past,
					// but now it seems to use the service bools above,
					// this doesn't have any effect anymore.
					//packet.PutLong(DateTime.Now.AddMonths(1));
					//packet.PutShort(72);

					// ?
					//packet.PutLong(0);
					//packet.PutShort(73);

					packet.PutLong(0);
				}
			}

			// [190200, NA215 (18.11.2015)] Chat Sticker
			{
				var stickerId = 0;
				var end = DateTime.MinValue;
				if (creature.Vars.Perm["ChatStickerId"] != null)
				{
					stickerId = creature.Vars.Perm["ChatStickerId"];
					end = creature.Vars.Perm["ChatStickerEnd"];
				}

				packet.PutInt((int)stickerId);
				packet.PutLong(end);
			}

			// [200100, NA229 (2016-06-16)] ?
			{
				packet.PutByte(0);
				packet.PutInt(0);
			}

			return packet;
		}
	}
}
