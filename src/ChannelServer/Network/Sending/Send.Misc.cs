﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Channel.World.Entities;
using Aura.Shared.Network;
using Aura.Channel.World;
using Aura.Mabi.Const;
using Aura.Shared.Util;
using Aura.Channel.Network.Sending.Helpers;
using Aura.Mabi.Network;
using Aura.Channel.Scripting.Scripts;

namespace Aura.Channel.Network.Sending
{
	public static partial class Send
	{
		/// <summary>
		/// Sends MailsRequestR to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		public static void MailsRequestR(Creature creature)
		{
			var packet = new Packet(Op.MailsRequestR, creature.EntityId);
			// ...

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends SosButtonRequestR to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="enabled"></param>
		public static void SosButtonRequestR(Creature creature, bool enabled)
		{
			var packet = new Packet(Op.SosButtonRequestR, creature.EntityId);
			packet.PutByte(enabled);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends HomesteadInfoRequestR to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		public static void HomesteadInfoRequestR(Creature creature)
		{
			var packet = new Packet(Op.HomesteadInfoRequestR, creature.EntityId);
			packet.PutByte(0);
			packet.PutByte(0);
			packet.PutByte(1);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends negative HomesteadEnterRequestR dummy to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		public static void HomesteadEnterRequestR(Creature creature)
		{
			var packet = new Packet(Op.HomesteadEnterRequestR, creature.EntityId);
			packet.PutByte(false);
			packet.PutByte((byte)HomesteadEnterRequestResponse.FailedToEnter);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends Disappear to creature's client.
		/// </summary>
		/// <remarks>
		/// Should this be broadcasted? What does it even do? TODO.
		/// </remarks>
		/// <param name="creature"></param>
		public static void Disappear(Creature creature)
		{
			var packet = new Packet(Op.Disappear, MabiId.Channel);
			packet.PutLong(creature.EntityId);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends CollectionRequestR to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		public static void CollectionRequestR(Creature creature)
		{
			var packet = new Packet(Op.CollectionRequestR, creature.EntityId);
			packet.PutByte(1); // success?
			packet.PutInt(0);
			packet.PutInt(0);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends ContinentWarpCoolDownR to creature's client.
		/// </summary>
		/// <remarks>
		/// On login the first parameter always seems to be a 1 byte.
		/// If it's not, after a continent warp for example, the packet
		/// has two more date parameters, with times 18 minutes apart
		/// from each other.
		/// The first date is the time of the last continent warp reset,
		/// 00:00 or 12:00. The second date is the time of the next reset.
		/// Based on those two times the skill icon cool down is displayed.
		/// </remarks>
		/// <param name="creature"></param>
		public static void ContinentWarpCoolDownR(Creature creature)
		{
			var packet = new Packet(Op.ContinentWarpCoolDownR, creature.EntityId);
			packet.PutByte(1);

			// Alternative structure: (Conti and Nao warps)
			// 001 [..............00]  Byte   : 0
			// 002 [000039BA86EA43C0]  Long   : 000039BA86EA43C0 // 2012-May-22 15:30:00
			// 003 [000039BA86FABE80]  Long   : 000039BA86FABE80 // 2012-May-22 15:48:00
			//packet.PutByte(0);
			//packet.PutLong(DateTime.Now.AddMinutes(1));
			//packet.PutLong(DateTime.Now.AddMinutes(5));

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Broadcasts PlayDead in range of creature.
		/// </summary>
		/// <param name="creature"></param>
		public static void PlayDead(Creature creature)
		{
			var pos = creature.GetPosition();

			var packet = new Packet(Op.PlayDead, creature.EntityId);
			packet.PutByte(true); // ?
			packet.PutFloat(pos.X);
			packet.PutFloat(pos.Y);
			packet.PutInt(5000);

			creature.Region.Broadcast(packet, creature);
		}

		/// <summary>
		/// Broadcasts RemoveDeathScreen in range of creature.
		/// </summary>
		/// <remarks>
		/// Removes black bars and unlocks player.
		/// 
		/// Update: This has to be broadcasted, otherwise other players
		///   are visually stuck in death mode. TODO: Maybe change name.
		/// </remarks>
		/// <param name="creature"></param>
		public static void RemoveDeathScreen(Creature creature)
		{
			var packet = new Packet(Op.RemoveDeathScreen, creature.EntityId);

			creature.Region.Broadcast(packet);
		}

		/// <summary>
		/// Broadcasts RiseFromTheDead in range of creature.
		/// </summary>
		/// <remarks>
		/// Makes creature stand up.
		/// </remarks>
		/// <param name="creature"></param>
		public static void RiseFromTheDead(Creature creature)
		{
			var packet = new Packet(Op.RiseFromTheDead, creature.EntityId);

			creature.Region.Broadcast(packet, creature);
		}

		/// <summary>
		/// Broadcasts DeadFeather in range of creature.
		/// </summary>
		/// <param name="creature"></param>
		public static void DeadFeather(Creature creature)
		{
			var bits = (int)creature.DeadMenu.Options;
			var flags = new List<int>();
			flags.Add(0);

			// Break down options bit by bit, and add them to flags if set.
			for (var i = 1; bits != 0; ++i, bits >>= 1)
			{
				if ((bits & 1) != 0)
					flags.Add(i);
			}

			var packet = new Packet(Op.DeadFeather, creature.EntityId);

			packet.PutShort((short)flags.Count);
			foreach (var flag in flags)
				packet.PutInt(flag);

			packet.PutByte((byte)creature.NaoOutfit);

			creature.Region.Broadcast(packet, creature);
		}

		/// <summary>
		/// Sends PlayCutscene to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="cutscene"></param>
		public static void PlayCutscene(Creature creature, Cutscene cutscene)
		{
			var packet = new Packet(Op.PlayCutscene, MabiId.Channel);
			packet.PutLong(creature.EntityId);
			packet.PutLong(cutscene.Leader.EntityId);
			packet.PutString(cutscene.Name);

			packet.PutInt(cutscene.Actors.Count);
			foreach (var actor in cutscene.Actors)
			{
				var subPacket = Packet.Empty();
				subPacket.AddCreatureInfo(actor.Value, CreaturePacketType.Public);
				var bArr = subPacket.Build();

				packet.PutString(actor.Key);
				packet.PutShort((short)bArr.Length);
				packet.PutBin(bArr);
			}

			packet.PutInt(1); // count?
			packet.PutLong(creature.EntityId);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends CutsceneEnd to cutscene's leader.
		/// </summary>
		/// <param name="cutscene"></param>
		public static void CutsceneEnd(Creature creature)
		{
			var packet = new Packet(Op.CutsceneEnd, MabiId.Channel);
			packet.PutLong(creature.EntityId);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends CutsceneUnk to cutscene's leader.
		/// </summary>
		/// <remarks>
		/// Doesn't seem to be required, but it's usually sent after unlocking
		/// the character after watching the cutscene.
		/// </remarks>
		/// <param name="cutscene"></param>
		public static void CutsceneUnk(Creature creature)
		{
			var packet = new Packet(Op.CutsceneUnk, MabiId.Channel);
			packet.PutLong(creature.EntityId);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends UseGestureR to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="success"></param>
		public static void UseGestureR(Creature creature, bool success)
		{
			var packet = new Packet(Op.UseGestureR, creature.EntityId);
			packet.PutByte(success);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Broadcasts UseMotion and CancelMotion (if cancel is true) in creature's region.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="category"></param>
		/// <param name="type"></param>
		/// <param name="loop"></param>
		/// <param name="cancel"></param>
		public static void UseMotion(Creature creature, int category, int type, bool loop = false, bool cancel = false)
		{
			if (cancel)
				CancelMotion(creature);

			// Do motion
			var packet = new Packet(Op.UseMotion, creature.EntityId);
			packet.PutInt(category);
			packet.PutInt(type);
			packet.PutByte(loop);
			packet.PutShort(0);

			// XXX: Why is it region and not range again...? Maybe so you see
			//   the motion when coming into range? ... does that work?

			creature.Region.Broadcast(packet, creature);
		}

		/// <summary>
		/// Broadcasts CancelMotion in creature's region.
		/// </summary>
		/// <param name="creature"></param>
		public static void CancelMotion(Creature creature)
		{
			var cancelPacket = new Packet(Op.CancelMotion, creature.EntityId);
			cancelPacket.PutByte(0);

			creature.Region.Broadcast(cancelPacket, creature);
		}

		/// <summary>
		/// Broadcasts MotionCancel2 in creature's region.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="unkByte"></param>
		public static void MotionCancel2(Creature creature, byte unkByte)
		{
			var packet = new Packet(Op.MotionCancel2, creature.EntityId);
			packet.PutByte(unkByte);

			creature.Region.Broadcast(packet);
		}

		/// <summary>
		/// Sends SetBgm to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="file"></param>
		/// <param name="type"></param>
		public static void SetBgm(Creature creature, string file, BgmRepeat type)
		{
			var packet = new Packet(Op.SetBgm, creature.EntityId);
			packet.PutString(file);
			packet.PutInt((int)type);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends UnsetBgm to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="file"></param>
		public static void UnsetBgm(Creature creature, string file)
		{
			var packet = new Packet(Op.UnsetBgm, creature.EntityId);
			packet.PutString(file);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends ViewEquipmentR to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="targetEntityId"></param>
		/// <param name="items"></param>
		public static void ViewEquipmentR(Creature creature, Creature target)
		{
			var packet = new Packet(Op.ViewEquipmentR, creature.EntityId);
			packet.PutByte(target != null);
			if (target != null)
			{
				packet.PutLong(target.EntityId);

				var items = target.Inventory.GetEquipment();
				packet.PutInt(items.Length);
				foreach (var item in items)
					packet.AddItemInfo(item, ItemPacketType.Private);
			}

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends Inquiry to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="id"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public static void Inquiry(Creature creature, byte id, string format, params object[] args)
		{
			var packet = new Packet(Op.Inquiry, creature.EntityId);
			packet.PutByte(id);
			packet.PutString(format, args);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends InquiryResponseR to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="success"></param>
		public static void InquiryResponseR(Creature creature, bool success)
		{
			var packet = new Packet(Op.InquiryResponseR, creature.EntityId);
			packet.PutByte(success);

			creature.Client.Send(packet);
		}

		public static void SpinColorWheelR(Creature creature, float result)
		{
			var packet = new Packet(Op.SpinColorWheelR, creature.EntityId);
			packet.PutFloat(result);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends NaoRevivalEntrance to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="result"></param>
		public static void NaoRevivalEntrance(Creature creature)
		{
			var packet = new Packet(Op.NaoRevivalEntrance, creature.EntityId);
			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends NaoRevivalExit to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="result"></param>
		public static void NaoRevivalExit(Creature creature)
		{
			var packet = new Packet(Op.NaoRevivalExit, creature.EntityId);
			packet.PutByte(0);
			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends RequestNpcNamesR to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="npcs"></param>
		public static void RequestNpcNamesR(Creature creature, ICollection<NPC> npcs)
		{
			var packet = new Packet(Op.RequestNpcNamesR, creature.EntityId);
			packet.PutInt(npcs.Count);
			foreach (var npc in npcs)
			{
				var pos = npc.GetPosition();

				packet.PutInt(npc.RegionId);
				packet.PutInt(pos.X);
				packet.PutInt(pos.Y);
				packet.PutString(npc.Name);
				packet.PutString(""); // ?
				packet.PutString(""); // ?
				packet.PutByte(0); // ?
			}

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends negative DressingRoomOpenR dummy to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		public static void DressingRoomOpenR(Creature creature, bool success)
		{
			var packet = new Packet(Op.DressingRoomOpenR, creature.EntityId);
			packet.PutByte(success);
			if (success)
			{
				packet.PutString(creature.Client.Account.Id);
				packet.PutInt(0); // item count?
				// for(item count)
				// {
				//		Item's ID
				//		Item's private info
				//		something more?
				// }

				packet.PutInt(0); // index count?
				// for(index count)
				//{
				//	packet.PutInt(0);
				//	packet.PutInt(0); // increments
				//	packet.PutByte(0);
				//}
			}

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends DressingRoomCloseR to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="success"></param>
		public static void DressingRoomCloseR(Creature creature, bool success)
		{
			var packet = new Packet(Op.DressingRoomCloseR, creature.EntityId);
			packet.PutByte(success);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends GameEventStateUpdate to all clients connected to the channel.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="isActive"></param>
		public static void GameEventStateUpdate(string gameEventId, bool isActive)
		{
			var packet = new Packet(Op.GameEventStateUpdate, MabiId.Broadcast);
			packet.PutString(gameEventId);
			packet.PutByte(isActive);
			packet.PutInt(0);

			ChannelServer.Instance.World.Broadcast(packet);
		}

		/// <summary>
		/// Sends GameEventStateUpdate creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="isActive"></param>
		public static void GameEventStateUpdate(Creature creature, string gameEventId, bool isActive)
		{
			var packet = new Packet(Op.GameEventStateUpdate, MabiId.Broadcast);
			packet.PutString(gameEventId);
			packet.PutByte(isActive);
			packet.PutInt(0);

			creature.Client.Send(packet);
		}
	}
}
