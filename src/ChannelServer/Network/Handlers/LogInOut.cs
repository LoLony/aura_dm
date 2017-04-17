﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System.Linq;
using Aura.Channel.Database;
using Aura.Channel.Network.Sending;
using Aura.Shared.Network;
using Aura.Shared.Util;
using Aura.Channel.World;
using Aura.Mabi.Const;
using Aura.Channel.World.Entities;
using System;
using Aura.Mabi;
using Aura.Mabi.Network;
using Aura.Channel.World.Dungeons;

namespace Aura.Channel.Network.Handlers
{
	public partial class ChannelServerHandlers : PacketHandlerManager<ChannelClient>
	{
		/// <summary>
		/// Login.
		/// </summary>
		/// <example>
		/// 001 [................] String : admin
		/// 002 [................] String : admin
		/// 003 [.79D55246A240C89] Long   : 548688344496999561
		/// 004 [..10000000000002] Long   : 4503599627370498
		/// </example>
		[PacketHandler(Op.ChannelLogin)]
		public void ChannelLogin(ChannelClient client, Packet packet)
		{
			// Refuse connection if the ChannelServer is currently shutting down
			if (ChannelServer.Instance.ShuttingDown)
			{
				Log.Info("Refused connection because the server is currently shutting down.");
				client.Kill();
				return;
			}

			var accountId = packet.GetString();
			// [160XXX] Double account name
			{
				packet.GetString();
			}
			var sessionKey = packet.GetLong();
			var characterId = packet.GetLong();
			var secondaryLogin = (packet.Peek() == PacketElementType.Byte && packet.GetByte() == 0x0B);

			// Check state
			if (client.State != ClientState.LoggingIn && !secondaryLogin)
				return;

			// Check account
			var account = ChannelServer.Instance.Database.GetAccount(accountId);
			if (account == null || account.SessionKey != sessionKey)
			{
				// This doesn't autoban because the client is not yet "authenticated",
				// so an evil person might be able to use it to inflate someone's
				// autoban score without knowing their password
				Log.Warning("ChannelLogin handler: Invalid account ({0}) or session ({1}).", accountId, sessionKey);
				client.Kill();
				return;
			}

			// Normal login if not secondary or client isn't logged
			// in yet (fallback).
			if (!secondaryLogin || client.State == ClientState.LoggingIn)
			{
				// Check character
				var character = account.GetCharacterOrPetSafe(characterId) as Creature;

				// Free premium
				account.PremiumServices.EvaluateFreeServices(ChannelServer.Instance.Conf.Premium);

				client.Account = account;
				client.Controlling = character;
				client.Creatures.Add(character.EntityId, character);
				character.Client = client;

				client.State = ClientState.LoggedIn;

				// Update online status
				ChannelServer.Instance.Database.SetAccountLoggedIn(account.Id, true);

				var playerCreature = character as PlayerCreature;
				if (playerCreature != null)
					ChannelServer.Instance.Database.UpdateOnlineStatus(playerCreature.CreatureId, true);
				ChannelServer.Instance.Database.UpdateOnlineStatus((character as PlayerCreature).CreatureId, true);

				// Response
				Send.ChannelLoginR(client, character.EntityId);

				// Special login to Soul Stream for new chars and on birthdays
				if (!character.Has(CreatureStates.Initialized) || character.CanReceiveBirthdayPresent)
				{
					var npcEntityId = (character.IsCharacter ? MabiId.Nao : MabiId.Tin);
					var npc = ChannelServer.Instance.World.GetCreature(npcEntityId);
					if (npc == null)
						Log.Warning("ChannelLogin: Intro NPC not found ({0:X16}).", npcEntityId);

					character.Temp.InSoulStream = true;
					character.Activate(CreatureStates.Initialized);

					Send.SpecialLogin(character, 1000, 3200, 3200, npcEntityId);
				}
				// Log into world
				else
				{
					// Fallback for invalid region ids, like 0, dynamics, and dungeons.
					if (character.RegionId == 0 || Math2.Between(character.RegionId, 35000, 40000) || Math2.Between(character.RegionId, 10000, 11000))
						character.SetLocation(1, 12800, 38100);

					character.Warp(character.GetLocation());
				}
			}
			else
			{
				// Try to get character from controlle creatures.
				Creature character;
				if (!client.Creatures.TryGetValue(characterId, out character))
				{
					Log.Warning("ChannelLogin: Secondary login failed, creature not found.");
					client.Kill();
					return;
				}

				Send.ChannelLoginR(client, character.EntityId);

				character.Warp(character.GetLocation());
			}
		}

		/// <summary>
		/// Sent after EnterRegion.
		/// </summary>
		/// <example>
		/// No parameters.
		/// </example>
		[PacketHandler(Op.EnterRegionRequest)]
		public void EnterRegionRequest(ChannelClient client, Packet packet)
		{
			var creature = client.GetCreatureSafe(packet.Id);
			var firstSpawn = (creature.Region == Region.Limbo);
			var prevRegionId = creature.RegionId;
			var regionId = creature.WarpLocation.RegionId;

			// Check permission
			// This can happen from time to time, client lag?
			if (!creature.Warping)
			{
				Log.Warning("Unauthorized warp attemp from '{0}'.", creature.Name);
				return;
			}

			creature.Warping = false;

			// Get region
			var region = ChannelServer.Instance.World.GetRegion(regionId);
			if (region == null)
			{
				Log.Warning("EnterRegionRequest: Player '{0}' tried to enter unknown region '{1}'.", creature.Name, regionId);
				return;
			}

			// Characters that spawned at least once need to be saved.
			var playerCreature = creature as PlayerCreature;
			if (playerCreature != null)
				playerCreature.Save = true;

			// Remove creature from previous region.
			if (creature.Region != Region.Limbo)
				creature.Region.RemoveCreature(creature);

			// Add to region
			creature.SetLocation(creature.WarpLocation);
			region.AddCreature(creature);

			// Unlock and warp
			creature.Unlock(Locks.Default, true);
			if (firstSpawn)
				Send.EnterRegionRequestR(client, creature);
			else
				Send.WarpRegion(creature);

			// Activate AIs around spawn
			var pos = creature.GetPosition();
			creature.Region.ActivateAis(creature, pos, pos);

			// Warp pets and other creatures as well if creature isn't an
			// RP character, since that would bring the actual creature back
			// to the map, which messes things up.
			if (!creature.IsRpCharacter)
			{
				foreach (var cr in client.Creatures.Values.Where(a => a.RegionId != creature.RegionId))
					cr.Warp(creature.RegionId, pos.X, pos.Y);
			}

			// Automatically done by the world update
			//Send.EntitiesAppear(client, region.GetEntitiesInRange(creature));

			// Inform dungeon about player entering the lobby
			// Originally this was done inside the dungeon, via event on
			// the lobby region, fired from AddCreature, but AddCreature is
			// supposed to run before WarpRegion, and we need this to run
			// afterwards, so the client is done with the warping process,
			// when things like cutscenes are started from the OnEnter
			// events in  the dungeon script.
			// Needs to be delayed for RP characters, because they can't
			// watch cutscenes before receiving ChannelCharacterInfoRequestR
			// in reply to ChannelCharacterInfoRequest.
			if (!creature.IsRpCharacter)
			{
				var dungeonLobbyRegion = creature.Region as DungeonLobbyRegion;
				if (dungeonLobbyRegion != null)
					dungeonLobbyRegion.Dungeon.OnPlayerEntersLobby(creature);
			}

			// Raise entered event after sending the response packets,
			// so the client is ready to receive things like cutscenes.
			region.OnPlayerEntered(creature, prevRegionId);
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <remarks>
		/// Judging by the name I'd guess you normally get the entities here.
		/// Sent when logging in, spawning a pet, etc.
		/// </remarks>
		/// <example>
		/// Op: 000061A8, Id: 200000000000000F
		/// 0001 [0010010000000001] Long   : 4504699138998273
		/// </example>
		[PacketHandler(Op.AddObserverRequest)]
		public void AddObserverRequest(ChannelClient client, Packet packet)
		{
			var id = packet.GetLong();

			// ...
		}

		/// <summary>
		/// Request for character information.
		/// </summary>
		/// <example>
		/// No parameters.
		/// </example>
		[PacketHandler(Op.ChannelCharacterInfoRequest)]
		public void ChannelCharacterInfoRequest(ChannelClient client, Packet packet)
		{
			var creature = client.GetCreatureSafe(packet.Id);

			if (creature.Master != null)
			{
				var pos = creature.GetPosition();
				Send.SpawnEffect(SpawnEffect.Pet, creature.RegionId, pos.X, pos.Y, creature.Master, creature);
			}

			// Infamous 5209, aka char info
			Send.ChannelCharacterInfoRequestR(client, creature);

			// Send any necessary "feature enabled" packets, grant extra items, update quests, etc.
			ChannelServer.Instance.Events.OnCreatureConnecting(creature);

			// Special treatment for pets
			if (creature.Master != null)
			{
				// Send vehicle info to make mounts mountable
				if (creature.RaceData.VehicleType > 0)
					Send.VehicleInfo(creature);
			}

			var now = DateTime.Now;

			// Update last login
			creature.LastLogin = now;

			// Age check
			var lastSaturday = ErinnTime.Now.GetLastSaturday();
			var lastAging = creature.LastAging;
			var diff = (lastSaturday - lastAging).TotalDays;

			if (lastAging < lastSaturday)
				creature.AgeUp((short)(1 + diff / 7));

			// Name/Chat color conditions
			if (creature.Vars.Perm["NameColorEnd"] != null)
			{
				var dt = (DateTime)creature.Vars.Perm["NameColorEnd"];
				if (now > dt)
				{
					creature.Vars.Perm["NameColorIdx"] = null;
					creature.Vars.Perm["NameColorEnd"] = null;
				}
			}
			if (creature.Vars.Perm["ChatColorEnd"] != null)
			{
				var dt = (DateTime)creature.Vars.Perm["ChatColorEnd"];
				if (now > dt)
				{
					creature.Vars.Perm["ChatColorIdx"] = null;
					creature.Vars.Perm["ChatColorEnd"] = null;
				}
			}
			if (creature.Vars.Perm["NameColorIdx"] != null)
			{
				var extra = new MabiDictionary();
				extra.SetInt("IDX", (int)creature.Vars.Perm["NameColorIdx"]);

				creature.Conditions.Activate(ConditionsB.NameColorChange, extra);
			}
			if (creature.Vars.Perm["ChatColorIdx"] != null)
			{
				var extra = new MabiDictionary();
				extra.SetInt("IDX", (int)creature.Vars.Perm["ChatColorIdx"]);

				creature.Conditions.Activate(ConditionsB.ChatColorChange, extra);
			}

			// Chat sticker hack
			// You don't see your own chat stickers on Aura without this packet
			// for unknown reasons.
			if (creature.Vars.Perm["ChatStickerId"] != null)
			{
				var sticker = (ChatSticker)creature.Vars.Perm["ChatStickerId"];
				var end = (DateTime)creature.Vars.Perm["ChatStickerEnd"];

				if (now < end)
					Send.ChatSticker(creature, sticker, end);
			}

			// Update Pon
			Send.PointsUpdate(creature, creature.Points);

			// Send UrlUpdate packets?
			// - UrlUpdateChronicle
			// - UrlUpdateAdvertise
			// - UrlUpdateGuestbook
			// - UrlUpdatePvp
			// - UrlUpdateDungeonBoard

			// Update dead menu, in case creature is dead
			creature.DeadMenu.Update();

			// Any extra ChannelInfo initialization from scripts
			// Actual first update of features
			ChannelServer.Instance.Events.OnCreatureConnected(creature);

			// Delayed OnPlayerEntersLobby for RP characters.
			// (See EnterRegionRequest handler.)
			if (creature.IsRpCharacter)
			{
				var dungeonLobbyRegion = creature.Region as DungeonLobbyRegion;
				if (dungeonLobbyRegion != null)
					dungeonLobbyRegion.Dungeon.OnPlayerEntersLobby(creature);
			}
		}

		/// <summary>
		/// Disconnection request.
		/// </summary>
		/// <remarks>
		/// Client doesn't disconnect till we answer.
		/// </remarks>
		/// <example>
		/// ...
		/// </example>
		[PacketHandler(Op.DisconnectRequest)]
		public void DisconnectRequest(ChannelClient client, Packet packet)
		{
			var unk1 = packet.GetByte(); // 1 | 2 (maybe login vs exit?)

			Log.Info("'{0}' is closing the connection. Saving...", client.Account.Id);

			client.CleanUp();

			Send.ChannelDisconnectR(client);
		}

		/// <summary>
		/// Sent when entering the Soul Stream.
		/// </summary>
		/// <remarks>
		/// Purpose unknown, no answer sent in logs.
		/// </remarks>
		/// <example>
		/// No parameters.
		/// </example>
		[PacketHandler(Op.EnterSoulStream)]
		public void EnterSoulStream(ChannelClient client, Packet packet)
		{
		}

		/// <summary>
		/// Sent after ending the conversation with Nao.
		/// </summary>
		/// <example>
		/// No parameters.
		/// </example>
		[PacketHandler(Op.LeaveSoulStream)]
		public void LeaveSoulStream(ChannelClient client, Packet packet)
		{
			var creature = client.GetCreatureSafe(packet.Id);
			if (!creature.Temp.InSoulStream)
				return;

			creature.Temp.InSoulStream = false;

			Send.LeaveSoulStreamR(creature);

			creature.Warp(creature.GetLocation());
		}

		/// <summary>
		/// Sent repeatedly while channel list is open to update it.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		[PacketHandler(Op.GetChannelList)]
		public void GetChannelList(ChannelClient client, Packet packet)
		{
			var server = ChannelServer.Instance.ServerList.GetServer(ChannelServer.Instance.Conf.Channel.ChannelServer);
			if (server == null) return; // Should never happen

			Send.GetChannelListR(client, server);
		}

		/// <summary>
		/// Request for switching channels or entering rebirth from channel.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		[PacketHandler(Op.SwitchChannel)]
		public void SwitchChannel(ChannelClient client, Packet packet)
		{
			var channelName = packet.GetString();
			var rebirth = packet.GetBool();

			var creature = client.GetControlledCreatureSafe();

			// Get channel
			var channel = ChannelServer.Instance.ServerList.GetChannel(ChannelServer.Instance.Conf.Channel.ChannelServer, channelName);
			if (channel == null)
			{
				Log.Debug("Warning: Player '{0}' tried to switch to non-existent channel '{1}'.", creature.Name, channelName);
				Send.SwitchChannelR(creature, null);
				return;
			}

			// XXX: Check for same channel switch?

			// Deactivate Initialized state, so we can reach Nao.
			if (rebirth)
				creature.Deactivate(CreatureStates.Initialized);

			// Make visible entities disappear, otherwise they will still
			// be there after the channel switch. Can't be handled automatically
			// like normal (dis)appearing because the other channel doesn't
			// know what the player saw before.
			var playerCreature = creature as PlayerCreature;
			if (playerCreature != null)
			{
				playerCreature.Watching = false;
				Send.EntitiesDisappear(playerCreature.Client, playerCreature.Region.GetVisibleEntities(playerCreature));
			}

			if (!rebirth)
				Log.Info("'{0}' is switching channels. Saving...", client.Account.Id);
			else
				Log.Info("'{0}' is reconnecting for rebirth. Saving...", client.Account.Id);

			client.CleanUp();

			// Success
			Send.SwitchChannelR(creature, channel);
		}

		/// <summary>
		/// Dummy handler for DcUnk.
		/// </summary>
		/// <remarks>
		/// Sent on logout, purpose unknown. Client waits for an answer,
		/// which makes logging out impossible without sending DcUnkR.
		/// </remarks>
		/// <example>
		/// No parameters.
		/// </example>
		[PacketHandler(Op.DcUnk)]
		public void DcUnk(ChannelClient client, Packet packet)
		{
			Send.DcUnkR(client, 0);
		}
	}
}
