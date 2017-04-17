﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using Aura.Channel.World.Entities;
using Aura.Mabi.Const;
using Aura.Shared.Network;
using Aura.Shared.Util;
using Aura.Channel.World.Entities.Creatures;
using System.Globalization;
using Aura.Channel.Network.Sending.Helpers;
using Aura.Mabi.Network;
using Aura.Channel.World;

namespace Aura.Channel.Network.Sending
{
	public static partial class Send
	{
		/// <summary>
		/// Sends CharacterLock to creature's client.
		/// </summary>
		public static void CharacterLock(Creature creature, Locks type)
		{
			var packet = new Packet(Op.CharacterLock, creature.EntityId);
			packet.PutUInt((uint)type);
			packet.PutInt(0);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends CharacterUnlock to creature's client.
		/// </summary>
		public static void CharacterUnlock(Creature creature, Locks type)
		{
			var packet = new Packet(Op.CharacterUnlock, creature.EntityId);
			packet.PutUInt((uint)type);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends CharacterLockUpdate to creature's client.
		/// </summary>
		/// <remarks>
		/// The name of this op is guessed, based on its position in the op
		/// list and its behavior. Originally I thought this might change
		/// a lock's timeout time, to, for example, reduce the time until
		/// you can move again, after you attacked something, but after
		/// testing it, it seems like it actually completely resets the
		/// locks.
		/// 
		/// The only known value for the byte is "18" (0x12), which doesn't
		/// match a known combination of locks, 0x10 being Run and 0x02 being
		/// unknown, however, 0x18 would be Run|Walk, which would match what
		/// it's doing.
		/// </remarks>
		/// <example>
		/// Resets movement stun?
		/// 001 [..............12] Byte   : 18
		/// 002 [........000005DC] Int    : 1500
		/// </example>
		public static void CharacterLockUpdate(Creature creature, byte unkByte, int unkInt)
		{
			var packet = new Packet(Op.CharacterLockUpdate, creature.EntityId);
			packet.PutByte(unkByte);
			packet.PutInt(unkInt);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends EnterRegion to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		public static void EnterRegion(Creature creature, int regionId, int x, int y)
		{
			var pos = creature.GetPosition();

			var packet = new Packet(Op.EnterRegion, MabiId.Channel);
			packet.PutLong(creature.EntityId);
			packet.PutByte(true); // success?
			packet.PutInt(regionId);
			packet.PutInt(x);
			packet.PutInt(y);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends EnterDynamicRegion to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="warpFromRegionId"></param>
		/// <param name="warpToRegion"></param>
		public static void EnterDynamicRegion(Creature creature, int warpFromRegionId, Region warpToRegion, int x, int y)
		{
			var warpTo = warpToRegion as DynamicRegion;
			if (warpTo == null)
				throw new ArgumentException("EnterDynamicRegion requires a dynamic region.");

			var pos = creature.GetPosition();

			var packet = new Packet(Op.EnterDynamicRegion, MabiId.Broadcast);
			packet.PutLong(creature.EntityId);
			packet.PutInt(warpFromRegionId); // creature's current region or 0?

			packet.PutInt(warpToRegion.Id);
			packet.PutString(warpToRegion.Name); // dynamic region name
			packet.PutUInt(0x80000000); // bitmask? (|1 = time difference?)
			packet.PutInt(warpTo.BaseId);
			packet.PutString(warpTo.BaseName);
			packet.PutInt(200); // 100|200 (100 changes the lighting?)
			packet.PutByte(0); // 1 = next is empty?
			packet.PutString("data/world/{0}/{1}", warpTo.BaseName, warpTo.Variation);

			packet.PutByte(0);
			//if (^ true)
			//{
			//	pp.PutByte(1);
			//	pp.PutInt(3100); // some region id?
			//}
			packet.PutInt(x); // target x pos
			packet.PutInt(y); // target y pos

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends EnterDynamicRegionExtended to creature's client.
		/// </summary>
		/// <remarks>
		/// From the looks of it this basically does the same as EnterDynamicRegion,
		/// but it supports the creation of multiple regions before warping to one.
		/// </remarks>
		/// <param name="creature"></param>
		/// <param name="warpFromRegionId"></param>
		/// <param name="warpToRegion"></param>
		public static void EnterDynamicRegionExtended(Creature creature, int warpFromRegionId, Region warpToRegion)
		{
			var warpTo = warpToRegion as DynamicRegion;
			if (warpTo == null)
				throw new ArgumentException("EnterDynamicRegionExtended requires a dynamic region.");

			var pos = creature.GetPosition();

			var packet = new Packet(Op.EnterDynamicRegionExtended, MabiId.Broadcast);
			packet.PutLong(creature.EntityId);
			packet.PutInt(warpFromRegionId); // creature's current region or 0?

			packet.PutInt(warpToRegion.Id); // target region id
			packet.PutInt(pos.X); // target x pos
			packet.PutInt(pos.Y); // target y pos
			packet.PutInt(0); // 0|4|8|16

			packet.PutInt(1); // count of dynamic regions v

			packet.PutInt(warpToRegion.Id);
			packet.PutString(warpToRegion.Name); // dynamic region name
			packet.PutUInt(0x80000000); // bitmask? (|1 = time difference?)
			packet.PutInt(warpTo.BaseId);
			packet.PutString(warpTo.BaseName);
			packet.PutInt(200); // 100|200
			packet.PutByte(0); // 1 = next is empty?
			packet.PutString("data/world/{0}/{1}", warpTo.BaseName, warpTo.Variation);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends RemoveDynamicRegion to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="regionId"></param>
		public static void RemoveDynamicRegion(Creature creature, int regionId)
		{
			var packet = new Packet(Op.RemoveDynamicRegion, MabiId.Broadcast);
			packet.PutInt(regionId);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends EnterRegionRequestR for creature to creature's client.
		/// </summary>
		/// <remarks>
		/// Negative response doesn't actually do anything, stucks.
		/// </remarks>
		/// <param name="client"></param>
		/// <param name="creature">Negative response if null</param>
		public static void EnterRegionRequestR(ChannelClient client, Creature creature)
		{
			var packet = new Packet(Op.EnterRegionRequestR, MabiId.Channel);
			packet.PutByte(creature != null);

			if (creature != null)
			{
				packet.PutLong(creature.EntityId);
				packet.PutLong(DateTime.Now);
			}

			client.Send(packet);
		}

		/// <summary>
		/// Sends negative ChannelCharacterInfoRequestR to client.
		/// </summary>
		/// <param name="client"></param>
		public static void ChannelCharacterInfoRequestR_Fail(ChannelClient client)
		{
			ChannelCharacterInfoRequestR(client, null);
		}

		/// <summary>
		/// Sends ChannelCharacterInfoRequestR for creature to client.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="creature">Negative response if null</param>
		public static void ChannelCharacterInfoRequestR(ChannelClient client, Creature creature)
		{
			var packet = new Packet(Op.ChannelCharacterInfoRequestR, MabiId.Channel);
			packet.PutByte(creature != null);

			if (creature != null)
			{
				packet.AddCreatureInfo(creature, CreaturePacketType.Private);
			}

			client.Send(packet);
		}

		/// <summary>
		/// Sends WarpRegion for creature to creature's client.
		/// </summary>
		/// <remarks>
		/// Makes client load the region and move the creature there.
		/// Uses current position of creature, move beforehand.
		/// </remarks>
		public static void WarpRegion(Creature creature)
		{
			var pos = creature.GetPosition();

			var packet = new Packet(Op.WarpRegion, creature.EntityId);
			packet.PutByte(true);
			packet.PutInt(creature.RegionId);
			packet.PutInt(pos.X);
			packet.PutInt(pos.Y);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends AddKeyword to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="keywordId"></param>
		public static void AddKeyword(Creature creature, ushort keywordId)
		{
			var packet = new Packet(Op.AddKeyword, creature.EntityId);
			packet.PutUShort(keywordId);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends RemoveKeyword to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="keywordId"></param>
		public static void RemoveKeyword(Creature creature, ushort keywordId)
		{
			var packet = new Packet(Op.RemoveKeyword, creature.EntityId);
			packet.PutUShort(keywordId);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends AddTitle(Knowledge) to creature's client,
		/// depending on state.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="titleId"></param>
		/// <param name="state"></param>
		public static void AddTitle(Creature creature, ushort titleId, TitleState state)
		{
			var op = (state == TitleState.Known ? Op.AddTitleKnowledge : Op.AddTitle);

			var packet = new Packet(op, creature.EntityId);
			packet.PutUShort(titleId);
			packet.PutInt(0);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Broadcasts TitleUpdate in creature's range.
		/// </summary>
		/// <param name="creature"></param>
		public static void TitleUpdate(Creature creature)
		{
			var packet = new Packet(Op.TitleUpdate, creature.EntityId);
			packet.PutUShort(creature.Titles.SelectedTitle);
			packet.PutUShort(creature.Titles.SelectedOptionTitle);

			creature.Region.Broadcast(packet, creature);
		}

		/// <summary>
		/// Sends ChangeTitleR to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="titleSuccess"></param>
		/// <param name="optionTitleSuccess"></param>
		public static void ChangeTitleR(Creature creature, bool titleSuccess, bool optionTitleSuccess)
		{
			var packet = new Packet(Op.ChangeTitleR, creature.EntityId);
			packet.PutByte(titleSuccess);
			packet.PutByte(optionTitleSuccess);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends AcquireInfo to creature's client.
		/// </summary>
		/// <remarks>
		/// Used on level up, for the green stats, floating besides the char.
		/// type can be any string, that string will be used as the name:
		/// "type +value"
		/// </remarks>
		/// <param name="creature"></param>
		/// <param name="type"></param>
		/// <param name="value"></param>
		public static void SimpleAcquireInfo(Creature creature, string type, float value)
		{
			var packet = new Packet(Op.AcquireInfo, creature.EntityId);
			packet.PutString("<xml type='{0}' value='{1}' simple='true' onlyLog='false' />", type, Math.Round(value));
			packet.PutInt(3000);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends AcquireInfo to creature's client.
		/// </summary>
		/// <remarks>
		/// Type can be various things, like "gold", "exp", or "ap".
		/// </remarks>
		/// <param name="creature"></param>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		public static void AcquireInfo(Creature creature, string type, int amount)
		{
			var packet = new Packet(Op.AcquireInfo, creature.EntityId);
			packet.PutString("<xml type='{0}' value='{1}'/>", type, amount);
			packet.PutInt(3000);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends AcquireInfo2 to creature's client.
		/// </summary>
		/// <remarks>
		/// Type can be various things, like "fishing".
		/// </remarks>
		/// <param name="creature"></param>
		/// <param name="type"></param>
		/// <param name="objectid"></param>
		public static void AcquireInfo2(Creature creature, string type, long objectid)
		{
			var packet = new Packet(Op.AcquireInfo2, creature.EntityId);
			packet.PutString("<xml type='{0}' objectid='{1}' />", type, objectid);
			packet.PutInt(3000);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends AcquireInfo2 to creature's client.
		/// </summary>
		/// <remarks>
		/// Assumingly specific to cooking.
		/// </remarks>
		/// <param name="creature"></param>
		/// <param name="objectid"></param>
		/// <param name="classid"></param>
		/// <param name="success"></param>
		public static void AcquireInfo2Cooking(Creature creature, long objectid, int classid, bool success)
		{
			var packet = new Packet(Op.AcquireInfo2, creature.EntityId);
			packet.PutString("<xml type='cooking' objectid='{0}' classid='{1}' success='{2}' />", objectid, classid, success ? 1 : 0);
			packet.PutInt(3000);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends AcquireInfo to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="itemId"></param>
		/// <param name="amount"></param>
		public static void AcquireItemInfo(Creature creature, int itemId, int amount)
		{
			var packet = new Packet(Op.AcquireInfo, creature.EntityId);
			packet.PutString("<xml type='item' classid='{0}' value='{1}'/>", itemId, amount);
			packet.PutInt(3000);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends AcquireInfo to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="itemEntityId"></param>
		public static void AcquireItemInfo(Creature creature, long itemEntityId)
		{
			var packet = new Packet(Op.AcquireInfo, creature.EntityId);
			packet.PutString("<xml type='item' objectid='{0}'/>", itemEntityId);
			packet.PutInt(3000);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends AcquireInfo2 to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="itemEntityId"></param>
		/// <param name="selected"></param>
		public static void AcquireDyedItemInfo(Creature creature, long itemEntityId, byte selected)
		{
			var packet = new Packet(Op.AcquireInfo2, creature.EntityId);
			packet.PutString("<xml type='dyeing' objectid='{0}' selected='{1}'/>", itemEntityId, selected);
			packet.PutInt(3000);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends AcquireInfo2 to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="itemEntityId"></param>
		public static void AcquireFixedDyedItemInfo(Creature creature, long itemEntityId)
		{
			var packet = new Packet(Op.AcquireInfo2, creature.EntityId);
			packet.PutString("<xml type='fixed_color_dyeing' objectid='{0}'/>", itemEntityId);
			packet.PutInt(3000);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends AcquireInfo2 to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="itemEntityId"></param>
		public static void AcquireEnchantedItemInfo(Creature creature, long itemEntityId, int itemId, int optionSetId)
		{
			var packet = new Packet(Op.AcquireInfo2, creature.EntityId);
			packet.PutString("<xml type='enchant' objectid='{0}' classid='{1}' value='{2}' optionset='{3}'/>", itemEntityId, itemId, 4, optionSetId);
			packet.PutInt(3000);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends DeadMenuR to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="menu">Negative answer if null</param>
		public static void DeadMenuR(Creature creature, CreatureDeadMenu menu)
		{
			var packet = new Packet(Op.DeadMenuR, creature.EntityId);
			packet.PutByte(menu != null);
			if (menu != null)
			{
				packet.PutString(menu.ToString());
				packet.PutInt(creature.Inventory.Count("/notTransServer/nao_coupon/")); // Beginner Nao Stone count
				packet.PutInt(creature.Inventory.Count("/nao_coupon/")); // Nao Stone Count
			}

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends negative Revived to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		public static void Revive_Fail(Creature creature)
		{
			var packet = new Packet(Op.Revived, creature.EntityId);
			packet.PutByte(false);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends Revived to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		public static void Revived(Creature creature)
		{
			var pos = creature.GetPosition();

			var packet = new Packet(Op.Revived, creature.EntityId);
			packet.PutByte(true);
			packet.PutInt(creature.RegionId);
			packet.PutInt(pos.X);
			packet.PutInt(pos.Y);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends AgeUpEffect to creature's client.
		/// </summary>
		/// <remarks>
		/// Notice + Light effect.
		/// Effect is only played for ages 1~25.
		/// </remarks>
		/// <param name="creature"></param>
		/// <param name="age"></param>
		public static void AgeUpEffect(Creature creature, short age)
		{
			var packet = new Packet(Op.AgeUpEffect, creature.EntityId);
			packet.PutShort(age);

			creature.Client.Send(packet);
		}
	}
}
