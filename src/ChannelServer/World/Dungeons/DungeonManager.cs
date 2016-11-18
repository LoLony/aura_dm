﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Channel.Network.Sending;
using Aura.Channel.Scripting.Scripts;
using Aura.Channel.World.Dungeons.Generation;
using Aura.Channel.World.Entities;
using Aura.Data;
using Aura.Data.Database;
using Aura.Mabi;
using Aura.Mabi.Const;
using Aura.Shared.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aura.Channel.World.Dungeons
{
	/// <summary>
	/// Manages dynamic regions.
	/// </summary>
	public class DungeonManager
	{
		private static long _instanceId = MabiId.Instances;

		private static readonly HashSet<int> _entryRegionIds = new HashSet<int>()
		{
			11,    // Uladh_Dungeon_Black_Wolfs_Hall1 (Ciar)
			13,    // Uladh_Dungeon_Beginners_Hall1 (Alby)
			24,    // Ula_DgnHall_Dunbarton_before1 (Rabbie)
			25,    // Ula_DgnHall_Dunbarton_before2 (Math)
			27,    // Ula_hardmode_DgnHall_TirChonaill_before (Alby Hard)
			32,    // Ula_DgnHall_Bangor_before1 (Bangor)
			33,    // Ula_DgnHall_Bangor_before2 (Bangor)
			44,    // Ula_DgnHall_Tirnanog_before1 (Albey)
			45,    // Ula_DgnHall_Tirnanog_before2 (Albey)
			49,    // Ula_DgnHall_Danu_before (Fiodh)
			54,    // Ula_DgnHall_Coill_before (Coil)
			64,    // Ula_DgnHall_Runda_before (Rundal)
			74,    // Ula_Dgnhall_Peaca_before (Peaca)
			78,    // Ula_DgnHall_Tirnanog_G3_before (Baol)
			121,   // Ula_hardmode_DgnHall_Ciar_before (Ciar Hard)
			123,   // Ula_hardmode_DgnHall_Runda_before (Rundal Hard)
			205,   // Dugald_Aisle_keep_DgnHall_before (Dugald Castle)
			207,   // Sen_Mag_keep_DgnHall_before (Sen Mag Castle)
			217,   // Abb_Neagh_keep_DgnHall_before (Abb Neagh Castle)
			600,   // JP_Nekojima_islet (Nekojima)
			60206, // Tara_keep_DgnHall_before (Tara Castle)
		};

		private Dictionary<long, Dungeon> _dungeons;
		private HashSet<int> _regionIds;

		/// <summary>
		/// Lock for the manager's lists.
		/// </summary>
		private object _syncLock = new Object();

		/// <summary>
		/// Lock for creating new dungeons and removing them, so a player
		/// isn't accidentally warped into a dungeon that is removed
		/// a nano second later.
		/// </summary>
		private object _createAndCleanUpLock = new Object();

		/// <summary>
		/// Creates new dungeon manager.
		/// </summary>
		public DungeonManager()
		{
			_dungeons = new Dictionary<long, Dungeon>();
			_regionIds = new HashSet<int>();

			ChannelServer.Instance.Events.MabiTick += this.OnMabiTick;
		}

		/// <summary>
		/// Raised every 5 minutes, removes empty dungeons.
		/// </summary>
		/// <remarks>
		/// TODO: Is removing on MabiTick what we want? How long do dungeons
		///   stay active before they're removed? This could remove a dungeon
		///   the minute, even the second, the last player leaves it.
		/// </remarks>
		/// <param name="time"></param>
		private void OnMabiTick(ErinnTime time)
		{
			lock (_createAndCleanUpLock)
			{
				List<long> remove;
				lock (_syncLock)
					remove = _dungeons.Values.Where(a => a.CountPlayers() == 0).Select(b => b.InstanceId).ToList();

				foreach (var instanceId in remove)
					this.Remove(instanceId);
			}
		}

		/// <summary>
		/// Generates instance id and creates dungeon.
		/// </summary>
		/// <param name="dungeonName"></param>
		/// <param name="itemId"></param>
		/// <param name="creature"></param>
		/// <returns></returns>
		private Dungeon CreateDungeon(string dungeonName, int itemId, Creature creature)
		{
			Dungeon dungeon;
			long instanceId = 0;
			var rnd = RandomProvider.Get();
			var itemData = AuraData.ItemDb.Find(itemId);

			// Create new dungeon for passes (includes quest items).
			// Since some "passes" don't have the dungeon_pass tag, but do
			// have quest_item, and quest items are generally supposed to
			// go to an NPC or onto an altar, we'll assume those are passes
			// as well.
			// If this assumption turnes out to be incorrect, we have to
			// check for some items specifically, like the Goddess Pass in G1.
			if (itemData != null && itemData.HasTag("/dungeon_pass/|/quest_item/"))
			{
				instanceId = this.GetInstanceId();
				dungeon = new Dungeon(instanceId, dungeonName, itemId, rnd.Next(), rnd.Next(), creature);
			}
			else
			{
				// Create new dungeon if there's not one yet
				var existing = this.Get(a => a.Name == dungeonName && a.ItemId == itemId);
				if (existing == null || ChannelServer.Instance.Conf.World.PrivateDungeons)
				{
					// Random floor plan on Tuesday
					var day = ErinnTime.Now.Month;
					var floorPlan = (day == 2 || ChannelServer.Instance.Conf.World.RandomFloors ? rnd.Next() : day);

					instanceId = this.GetInstanceId();
					dungeon = new Dungeon(instanceId, dungeonName, itemId, rnd.Next(), floorPlan, creature);
				}
				else
					dungeon = existing;
			}

			// Add new dungeon to list
			if (instanceId != 0)
			{
				lock (_syncLock)
					_dungeons.Add(instanceId, dungeon);
			}

			return dungeon;
		}

		/// <summary>
		/// Removes dungeon with given instance id, incl all regions.
		/// </summary>
		/// <param name="instanceId"></param>
		/// <returns></returns>
		private bool Remove(long instanceId)
		{
			Dungeon dungeon;

			lock (_syncLock)
			{
				if (!_dungeons.TryGetValue(instanceId, out dungeon))
					return false;

				foreach (var region in dungeon.Regions)
				{
					_regionIds.Remove(region.Id);
					ChannelServer.Instance.World.RemoveRegion(region.Id);
				}

				_dungeons.Remove(instanceId);
			}

			return true;
		}

		/// <summary>
		/// Returns first dungeon that matches the predicate, or null.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		private Dungeon Get(Func<Dungeon, bool> predicate)
		{
			lock (_syncLock)
				return _dungeons.Values.FirstOrDefault(predicate);
		}

		/// <summary>
		/// Generates and reserves a new dungeon region id.
		/// </summary>
		/// <returns></returns>
		public int GetRegionId()
		{
			var id = -1;

			lock (_syncLock)
			{
				for (int i = MabiId.DungeonRegions; i < MabiId.DynamicRegions; ++i)
				{
					if (!_regionIds.Contains(i))
					{
						id = i;
						break;
					}
				}

				_regionIds.Add(id);
			}

			if (id == -1)
				throw new Exception("No dungeon region ids available.");

			return id;
		}

		/// <summary>
		/// Generates and reserves a new dungeon instance id.
		/// </summary>
		/// <returns></returns>
		public long GetInstanceId()
		{
			return Interlocked.Increment(ref _instanceId);
		}

		/// <summary>
		/// Checks if creature is able to enter a dungeon with the given item,
		/// at his current position, if so, a dungeon is created and the
		/// party is moved inside.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool CheckDrop(Creature creature, Item item)
		{
			var currentRegionId = creature.RegionId;
			if (!_entryRegionIds.Contains(currentRegionId))
				return false;

			var pos = creature.GetPosition();

			var clientEvent = creature.Region.GetClientEvent(a => a.Data.IsAltar);
			if (clientEvent == null)
			{
				Log.Warning("DungeonManager.CheckDrop: No altar found.");
				return false;
			}

			if (!clientEvent.IsInside(pos.X, pos.Y))
			{
				// Tell player to step on altar?
				return false;
			}

			var parameter = clientEvent.Data.Parameters.FirstOrDefault(a => a.EventType == EventType.Altar);
			if (parameter == null || parameter.XML == null || parameter.XML.Attribute("dungeonname") == null)
			{
				Log.Warning("DungeonManager.CheckDrop: No dungeon name found in altar event '{0:X16}'.", clientEvent.EntityId);
				return false;
			}

			var dungeonName = parameter.XML.Attribute("dungeonname").Value.ToLower();

			// Check script
			var dungeonScript = ChannelServer.Instance.ScriptManager.DungeonScripts.Get(dungeonName);
			if (dungeonScript == null)
			{
				Send.ServerMessage(creature, "This dungeon hasn't been added yet.");
				Log.Warning("DungeonManager.CheckDrop: No routing dungeon script found for '{0}'.", dungeonName);
				return false;
			}

			// Check arenas
			if (dungeonScript.Name == "tircho_alby_dungeon" && item.HasTag("/alby_battle_arena/"))
			{
				creature.Warp(28, 1174, 795);
				return true;
			}

			// Check route
			if (!dungeonScript.Route(creature, item, ref dungeonName))
			{
				// The response in case of a fail is handled by the router.
				return false;
			}

			// Check party
			if (creature.IsInParty && creature.Party.Leader != creature)
			{
				// Unofficial
				Send.Notice(creature, Localization.Get("Only the leader may create the dungeon."));
				return false;
			}

			return this.CreateDungeonAndWarp(dungeonName, item.Info.Id, creature);
		}

		/// <summary>
		/// Creates a dungeon with the given parameters and warps the creature's
		/// party inside.
		/// </summary>
		/// <param name="dungeonName"></param>
		/// <param name="itemId"></param>
		/// <param name="leader"></param>
		/// <returns></returns>
		public bool CreateDungeonAndWarp(string dungeonName, int itemId, Creature leader)
		{
			lock (_createAndCleanUpLock)
			{
				try
				{
					var dungeon = this.CreateDungeon(dungeonName, itemId, leader);
					var regionId = dungeon.Regions.First().Id;

					// Warp the party currently standing on the altar into the dungeon.
					var party = leader.Party;
					var creators = party.GetCreaturesOnAltar(leader.RegionId);

					// Add creature to list in case something went wrong.
					// For example, there might be no altar, because the call
					// came from the dungeon command.
					if (creators.Count == 0)
						creators.Add(leader);

					// RP dungeon
					if (dungeon.HasRoles)
					{
						// Get roles
						var roles = dungeon.GetRoles();

						if (roles.Count < creators.Count)
						{
							Send.Notice(leader, Localization.Get("Your party has too few members for this role-playing dungeon."));
							return false;
						}

						// Create RP characters
						var rpCharacters = new List<RpCharacter>();
						for (int i = 0, j = 1; i < creators.Count; ++i)
						{
							var creator = creators[i];
							var pos = creator.GetPosition();

							// Get first role for leader or next available
							// one for members.
							var role = (creator == leader ? roles[0] : roles[j++]);

							// Get actor data
							var actorData = AuraData.ActorDb.Find(role);
							if (actorData == null)
							{
								Log.Error("DungeonManager.CreateDungeonAndWarp: Actor data not found for '{0}'.", role);
								return false;
							}

							try
							{
								var rpCharacter = new RpCharacter(actorData, creator, null);
								rpCharacter.SetLocation(regionId, pos.X, pos.Y);

								dungeon.RpCharacters.Add(rpCharacter.EntityId);
								dungeon.Script.OnRpCharacterCreated(dungeon, rpCharacter);

								rpCharacters.Add(rpCharacter);
							}
							catch
							{
								Log.Error("DungeonManager.CreateDungeonAndWarp: Failed to create RP character '{0}'.", role);
								throw;
							}
						}

						// Start RP sessions, which makes the players switch
						// to the RP characters inside the dungeon.
						foreach (var character in rpCharacters)
							character.Start();
					}
					// Normal dungeon
					else
					{
						// Warp in
						foreach (var creator in creators)
						{
							// Warp member to same position in the lobby region.
							var pos = creator.GetPosition();
							creator.Warp(regionId, pos);

							// TODO: This is a bit hacky, needs to be moved to Creature.Warp, with an appropriate check.
							Send.EntitiesDisappear(creator.Client, creators);
						}
					}

					return true;
				}
				catch (Exception ex)
				{
					Log.Exception(ex, "Failed to create and warp to dungeon.");
					return false;
				}
			}
		}
	}
}
