﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Channel.Util;
using Aura.Shared.Network;
using Aura.Shared.Util;
using Aura.Channel.Network.Sending;
using Aura.Channel.Scripting;
using System.Text.RegularExpressions;
using Aura.Data.Database;
using Aura.Mabi.Const;
using Aura.Data;
using Aura.Channel.World.Entities;
using Aura.Channel.Scripting.Scripts;
using Aura.Channel.World.Inventory;
using Aura.Mabi.Network;
using Aura.Mabi;
using Aura.Shared.Database;

namespace Aura.Channel.Network.Handlers
{
	public partial class ChannelServerHandlers : PacketHandlerManager<ChannelClient>
	{
		/// <summary>
		/// Request to talk to an NPC.
		/// </summary>
		/// <example>
		/// 0001 [0010F0000000032A] Long   : 4767482418037546
		/// </example>
		[PacketHandler(Op.NpcTalkStart)]
		public void NpcTalkStart(ChannelClient client, Packet packet)
		{
			var npcEntityId = packet.GetLong();

			// Check creature
			var creature = client.GetCreatureSafe(packet.Id);

			// Check lock
			if (!creature.Can(Locks.TalkToNpc))
			{
				Log.Debug("TalkToNpc locked for '{0}'.", creature.Name);
				Send.NpcTalkStartR_Fail(creature);
				return;
			}

			// Check NPC
			var target = ChannelServer.Instance.World.GetNpc(npcEntityId);
			if (target == null)
			{
				throw new ModerateViolation("Tried to talk to non-existant NPC 0x{0:X}", npcEntityId);
			}

			// Special NPC requirements
			// The Soulstream version of Nao and Tin are only available
			// in the Soulstream, and in there we can't check range.
			var bypassDistanceCheck = false;
			var disallow = false;
			if (npcEntityId == MabiId.Nao || npcEntityId == MabiId.Tin)
			{
				bypassDistanceCheck = creature.Temp.InSoulStream;
				disallow = !creature.Temp.InSoulStream;
			}

			// Some special NPCs require special permission.
			if (disallow)
			{
				throw new ModerateViolation("Tried to talk to NPC 0x{0:X} ({1}) without permission.", npcEntityId, target.Name);
			}

			// Check script
			if (target.ScriptType == null)
			{
				Send.NpcTalkStartR_Fail(creature);

				Log.Warning("NpcTalkStart: Creature '{0}' tried to talk to NPC '{1}', that doesn't have a script.", creature.Name, target.Name);
				return;
			}

			// Check distance
			if (!bypassDistanceCheck && (creature.RegionId != target.RegionId || target.GetPosition().GetDistance(creature.GetPosition()) > 1000))
			{
				Send.MsgBox(creature, Localization.Get("You're too far away."));
				Send.NpcTalkStartR_Fail(creature);

				Log.Warning("NpcTalkStart: Creature '{0}' tried to talk to NPC '{1}' out of range.", creature.Name, target.Name);
				return;
			}

			// Respond
			Send.NpcTalkStartR(creature, npcEntityId);

			// Start NPC dialog
			client.NpcSession.StartTalk(target, creature);
		}

		/// <summary>
		/// Sent when "End Conversation" button is clicked.
		/// </summary>
		/// <remarks>
		/// Not every "End Conversation" button is the same. Some send this,
		/// others, like the one you get while the keywords are open,
		/// send an "@end" response to Select instead.
		/// </remarks>
		/// <example>
		/// 001 [0010F00000000003] Long   : 4767482418036739
		/// 002 [..............01] Byte   : 1
		/// </example>
		[PacketHandler(Op.NpcTalkEnd)]
		public void NpcTalkEnd(ChannelClient client, Packet packet)
		{
			var npcId = packet.GetLong();
			var unkByte = packet.GetByte();

			// Check creature
			var creature = client.GetCreatureSafe(packet.Id);

			// Check session
			if (!client.NpcSession.IsValid(npcId) && creature.Temp.CurrentShop == null)
			{
				Log.Warning("Player '{0}' tried ending invalid NPC session.", creature.Name);

				// Don't return, there's no harm in closing a dialog,
				// and the player could get stuck because of a bug or
				// something.
				//return;
			}

			client.NpcSession.Clear();
			creature.Temp.CurrentShop = null;

			Send.NpcTalkEndR(creature, npcId);
		}

		/// <summary>
		/// Sent whenever a button, other than "Continue", is pressed
		/// while the client is in "SelectInTalk" mode.
		/// </summary>
		/// <example>
		/// 001 [................] String : <result session='1837'><this type="character">4503599627370498</this><return type="string">@end</return></result>
		/// 002 [........0000072D] Int    : 1837
		/// </example>
		[PacketHandler(Op.NpcTalkSelect)]
		public void NpcTalkSelect(ChannelClient client, Packet packet)
		{
			var result = packet.GetString();
			var sessionid = packet.GetInt();

			var creature = client.GetCreatureSafe(packet.Id);

			// Check session
			if (!client.NpcSession.IsValid())
			{
				// We can't throw a violation here because the client sends
				// NpcTalkSelect *after* NpcTalkEnd if you click the X in Eiry
				// while a list is open... maybe on other occasions as well,
				// so let's make it a debug msg, to not confuse admins.

				Log.Debug("NpcTalkSelect: Player '{0}' sent NpcTalkSelect for an invalid NPC session.", creature.Name);
				return;
			}

			// Check result string
			var match = Regex.Match(result, "<return type=\"string\">(?<result>[^<]*)</return>");
			if (!match.Success)
			{
				throw new ModerateViolation("Invalid NPC talk selection: {0}", result);
			}

			var response = match.Groups["result"].Value;

			// Cut @input "prefix" added for <input> element.
			if (response.StartsWith("@input"))
				response = response.Substring(7).Trim();

			// TODO: Do another keyword check, in case modders bypass the
			//   actual check below.

			// Check conversation state
			if (client.NpcSession.Script.ConversationState != ConversationState.Select)
				Log.Debug("Received Select without being in Select mode ({0}).", client.NpcSession.Script.GetType().Name);

			// Continue dialog
			client.NpcSession.Script.Resume(response);
		}

		/// <summary>
		/// Sent when selecting a keyword, to check the validity.
		/// </summary>
		/// <remarks>
		/// Client blocks until the server answers it.
		/// Failing it unblocks the client and makes it not send Select,
		/// effectively ignoring the keyword click.
		/// </remarks>
		/// <example>
		/// 001 [................] String : personal_info
		/// </example>
		[PacketHandler(Op.NpcTalkKeyword)]
		public void NpcTalkKeyword(ChannelClient client, Packet packet)
		{
			var keyword = packet.GetString();

			var creature = client.GetCreatureSafe(packet.Id);

			// Check session
			client.NpcSession.EnsureValid();

			// Check keyword
			if (!creature.Keywords.Has(keyword))
			{
				Send.NpcTalkKeywordR_Fail(creature);
				Log.Warning("NpcTalkKeyword: Player '{0}' tried using keyword '{1}', without knowing it.", creature.Name, keyword);
				return;
			}

			Send.NpcTalkKeywordR(creature, keyword);
		}

		/// <summary>
		/// Sent when buying an item from a regular NPC shop.
		/// </summary>
		/// <example>
		/// 0001 [005000CBB3152F26] Long   : 22518873019723558
		/// 0002 [..............00] Byte   : 0
		/// 0003 [..............00] Byte   : 0
		/// </example>
		[PacketHandler(Op.NpcShopBuyItem)]
		public void NpcShopBuyItem(ChannelClient client, Packet packet)
		{
			var entityId = packet.GetLong();
			var moveToInventory = packet.GetBool(); // 0:cursor, 1:inv
			var directBankTransaction = packet.GetBool();

			var creature = client.GetCreatureSafe(packet.Id);

			// Check session
			// Not compatible with remote shop access, unless we bind it to NPCs.
			//client.NpcSession.EnsureValid();

			// Check open shop
			if (creature.Temp.CurrentShop == null)
			{
				throw new ModerateViolation("Tried to buy an item with a null shop.");
			}

			var success = creature.Temp.CurrentShop.Buy(creature, entityId, moveToInventory, directBankTransaction);

			Send.NpcShopBuyItemR(creature, success);
		}

		/// <summary>
		/// Sent when selling an item from the inventory to a regular NPC shop.
		/// </summary>
		/// <example>
		/// 0001 [005000CBB3154E13] Long   : 22518873019731475
		/// 0002 [..............00] Byte   : 0
		/// </example>
		[PacketHandler(Op.NpcShopSellItem)]
		public void NpcShopSellItem(ChannelClient client, Packet packet)
		{
			var entityId = packet.GetLong();
			var directTransaction = packet.GetBool();

			var creature = client.GetCreatureSafe(packet.Id);

			// Check session
			// Not compatible with remote shop access, unless we bind it to NPCs.
			//client.NpcSession.EnsureValid();

			// Check open shop
			if (creature.Temp.CurrentShop == null)
			{
				throw new ModerateViolation("Tried to sell something with current shop being null");
			}

			// Get item
			var item = creature.Inventory.GetItemSafe(entityId);

			// Check for Pon, the client doesn't let you sell items that were
			// bought with them.
			if (item.OptionInfo.PointPrice != 0)
			{
				Send.MsgBox(creature, Localization.Get("You cannot sell items bought with Pon at the shop."));
				goto L_End;
			}

			// Calculate selling price
			var sellingPrice = item.GetSellingPrice();

			// Disable direct bank transaction if price is less than 50k
			if (directTransaction && sellingPrice < 50000)
				directTransaction = false;

			// Remove item from inv
			if (!creature.Inventory.Remove(item))
			{
				Log.Warning("NpcShopSellItem: Failed to remove item '{0:X16}' from '{1}'s inventory.", entityId, creature.Name);
				goto L_End;
			}

			// Add gold
			if (!directTransaction)
			{
				creature.Inventory.AddGold(sellingPrice);
			}
			else
			{
				// Fee
				// Unofficial, will not match *exactly* what the client
				// displays in the sell item window in all cases.
				var fee = (sellingPrice * (8.958 + ((sellingPrice / 10000 - 5) * 1.002)) / 100);
				sellingPrice = (int)(sellingPrice - fee);

				client.Account.Bank.AddGold(creature, sellingPrice);
			}

			// Respond in any case, to unlock the player
		L_End:
			Send.NpcShopSellItemR(creature);
		}

		/// <summary>
		/// Sent when trying to sell an item worth more than 50k, to check
		/// if it can be sold via Direct Transaction.
		/// </summary>
		/// <example>
		/// 001 [0050F00000000652] Long   : 22781880927520338
		/// </example>
		[PacketHandler(Op.CheckDirectBankSelling)]
		public void CheckDirectBankSelling(ChannelClient client, Packet packet)
		{
			var itemEntityId = packet.GetLong();

			var creature = client.GetCreatureSafe(packet.Id);

			// Check item
			var item = creature.Inventory.GetItem(itemEntityId);
			if (item == null)
			{
				Send.CheckDirectBankSellingR(creature, false);
				return;
			}

			// Check price
			var sellingPrice = item.GetSellingPrice();
			if (sellingPrice < 50000)
			{
				Send.CheckDirectBankSellingR(creature, false);
				return;
			}

			// Check space
			var goldMax = Math.Min((long)int.MaxValue, client.Account.Characters.Count * (long)ChannelServer.Instance.Conf.World.BankGoldPerCharacter);
			if ((long)client.Account.Bank.Gold + sellingPrice > goldMax)
			{
				Send.CheckDirectBankSellingR(creature, false);
				return;
			}

			Send.CheckDirectBankSellingR(creature, true);
		}

		/// <summary>
		/// Sent when clicking on close button in bank.
		/// </summary>
		/// <remarks>
		/// Doesn't lock the character if response isn't sent.
		/// </remarks>
		/// <example>
		/// 0001 [..............00] Byte   : 0
		/// </example>
		[PacketHandler(Op.CloseBank)]
		public void CloseBank(ChannelClient client, Packet packet)
		{
			var creature = client.GetCreatureSafe(packet.Id);

			Send.CloseBankR(creature);
		}

		/// <summary>
		/// Sent when selecting which bank tabs to display (human, elf, giant).
		/// </summary>
		/// <remarks>
		/// This packet is only sent when enabling Elf or Giant, it's not sent
		/// on deactivating them, and not for Human either.
		/// It's to request data that was not sent initially,
		/// i.e. send only Human first and Elf and Giant when ticked.
		/// The client only requests those tabs once.
		/// </remarks>
		/// <example>
		/// 0001 [..............01] Byte   : 1
		/// </example>
		[PacketHandler(Op.RequestBankTabs)]
		public void RequestBankTabs(ChannelClient client, Packet packet)
		{
			var race = (BankTabRace)packet.GetByte();

			var creature = client.GetCreatureSafe(packet.Id);

			// Fall back to human when race invalid
			if (race < BankTabRace.Human || race > BankTabRace.Giant)
				race = BankTabRace.Human;

			Send.OpenBank(creature, client.Account.Bank, race, creature.Temp.CurrentBankId, creature.Temp.CurrentBankTitle);
		}

		/// <summary>
		/// Sent when depositing gold in the bank.
		/// </summary>
		/// <example>
		/// 0001 [........00000014] Int    : 20
		/// </example>
		[PacketHandler(Op.BankDepositGold)]
		public void BankDepositGold(ChannelClient client, Packet packet)
		{
			var amount = packet.GetInt();

			var creature = client.GetCreatureSafe(packet.Id);

			// Check creature's gold
			if (creature.Inventory.Gold < amount)
				throw new ModerateViolation("BankDepositGold: '{0}' ({1:X16}) tried to deposit more than he has.", creature.Name, creature.EntityId);

			// Check bank max gold
			var goldMax = Math.Min((long)int.MaxValue, client.Account.Characters.Count * (long)ChannelServer.Instance.Conf.World.BankGoldPerCharacter);
			if ((long)client.Account.Bank.Gold + amount > goldMax)
			{
				Send.MsgBox(creature, Localization.Get("The maximum amount of gold you may store in the bank is {0:n0}."), goldMax);
				Send.BankDepositGoldR(creature, false);
				return;
			}

			// Transfer gold
			creature.Inventory.RemoveGold(amount);
			client.Account.Bank.AddGold(creature, amount);

			// Response
			Send.BankDepositGoldR(creature, true);
		}

		/// <summary>
		/// Sent when withdrawing gold from the bank.
		/// </summary>
		/// <example>
		/// 0001 [..............00] Byte   : 0
		/// 0002 [........00000014] Int    : 20
		/// </example>
		[PacketHandler(Op.BankWithdrawGold)]
		public void BankWithdrawGold(ChannelClient client, Packet packet)
		{
			var createCheck = packet.GetBool();
			var withdrawAmount = packet.GetInt();

			var creature = client.GetCreatureSafe(packet.Id);

			// Add fee for checks
			var removeAmount = withdrawAmount;
			if (createCheck)
				removeAmount += withdrawAmount / 20; // +5%

			// Check bank gold
			if (client.Account.Bank.Gold < removeAmount)
			{
				// Don't throw a violation, it's possible to accidentally
				// bypass the client side check, in which case someone would
				// be banned wrongfully.
				//throw new ModerateViolation("BankWithdrawGold: '{0}' ({1}) tried to withdraw more than he has ({2}/{3}).", creature.Name, creature.EntityIdHex, removeAmount, client.Account.Bank.Gold);

				Send.MsgBox(creature, Localization.Get("Unable to pay the fee, Insufficient balance."));
				Send.BankWithdrawGoldR(creature, false);
				return;
			}

			// Add gold to inventory if no check
			if (!createCheck)
			{
				creature.Inventory.AddGold(withdrawAmount);
			}
			// Add check item to creature's cursor pocket if check
			else
			{
				var item = Item.CreateCheck(withdrawAmount);

				// Try to add check to cursor
				if (!creature.Inventory.Add(item, Pocket.Cursor))
				{
					// This shouldn't happen.
					Log.Debug("BankWithdrawGold: Unable to add check to cursor.");

					Send.BankWithdrawGoldR(creature, false);
					return;
				}
			}

			// Remove gold from bank
			client.Account.Bank.RemoveGold(creature, removeAmount);

			// Response
			Send.BankWithdrawGoldR(creature, true);
		}

		/// <summary>
		/// Sent when putting an item into the bank.
		/// </summary>
		/// <example>
		/// 0001 [005000CA6F3EE634] Long   : 22518867586639412
		/// 0002 [................] String : Exec
		/// 0003 [........0000000B] Int    : 11
		/// 0004 [........00000004] Int    : 4
		/// </example>
		[PacketHandler(Op.BankDepositItem)]
		public void BankDepositItem(ChannelClient client, Packet packet)
		{
			var itemEntityId = packet.GetLong();
			var tabName = packet.GetString();
			var posX = packet.GetInt();
			var posY = packet.GetInt();

			var creature = client.GetCreatureSafe(packet.Id);

			// Check premium
			if (!client.Account.PremiumServices.CanUseAllBankTabs && tabName != creature.Name)
			{
				// Unofficial
				Send.MsgBox(creature, Localization.Get("Inventory Plus is required to access other character's bank tabs."));
				Send.BankDepositItemR(creature, false);
				return;
			}

			// Check for license
			var item = creature.Inventory.GetItemSafe(itemEntityId);
			if (item.HasTag("/personalshoplicense/"))
			{
				var amount = item.MetaData1.GetInt("EVALUE");
				if (amount != 0)
				{
					var afterFeeSum = (int)(amount * 0.99f);
					Send.BankLicenseFeeInquiry(creature, item.EntityId, amount, afterFeeSum);
					Send.BankDepositItemR(creature, false);
					return;
				}
			}

			// Deposit item
			var success = client.Account.Bank.DepositItem(creature, itemEntityId, creature.Temp.CurrentBankId, tabName, posX, posY);

			Send.BankDepositItemR(creature, success);
		}

		/// <summary>
		/// Sent after accepting fees for license deposition.
		/// </summary>
		/// <example>
		/// 001 [0050000000000AE8] Long   : 22517998136855272
		/// </example>
		[PacketHandler(Op.BankPostLicenseInquiryDeposit)]
		public void BankPostLicenseInquiryDeposit(ChannelClient client, Packet packet)
		{
			var itemEntityId = packet.GetLong();

			var creature = client.GetCreatureSafe(packet.Id);
			var item = creature.Inventory.GetItemSafe(itemEntityId);

			// Check license
			if (!item.HasTag("/personalshoplicense/"))
			{
				Log.Warning("BankPostLicenseInquiryDeposit: User '{0}' tried to post-license-inquiry-deposit invalid item.", client.Account.Id);
				Send.BankPostLicenseInquiryDepositR(creature, false);
				return;
			}

			// Deposit item
			var success = client.Account.Bank.DepositItem(creature, itemEntityId, creature.Temp.CurrentBankId, creature.Name, 0, 0);

			Send.BankPostLicenseInquiryDepositR(creature, success);
		}

		/// <summary>
		/// Sent when taking an item out of the bank.
		/// </summary>
		/// <example>
		/// 0001 [................] String : Exec
		/// 0002 [005000CA6F3EE634] Long   : 22518867586639412
		/// </example>
		[PacketHandler(Op.BankWithdrawItem)]
		public void BankWithdrawItem(ChannelClient client, Packet packet)
		{
			var tabName = packet.GetString();
			var itemEntityId = packet.GetLong();

			var creature = client.GetCreatureSafe(packet.Id);

			// Check premium
			if (!client.Account.PremiumServices.CanUseAllBankTabs && tabName != creature.Name)
			{
				// Unofficial
				Send.MsgBox(creature, Localization.Get("Inventory Plus is required to access other character's bank tabs."));
				Send.BankWithdrawItemR(creature, false);
				return;
			}

			// Withdraw item
			var success = client.Account.Bank.WithdrawItem(creature, tabName, itemEntityId);

			Send.BankWithdrawItemR(creature, success);
		}

		/// <summary>
		/// Sent when accepting transfer to current bank.
		/// </summary>
		/// <example>
		/// 001 [005000CA6F3EE634] Long   : 22518867586639412
		/// 002 [..............00] Byte   : 0
		/// </example>
		[PacketHandler(Op.BankTransferRequest)]
		public void BankTransferRequest(ChannelClient client, Packet packet)
		{
			var itemEntityId = packet.GetLong();
			var instantTransfer = packet.GetBool();

			var creature = client.GetCreatureSafe(packet.Id);
			var success = client.Account.Bank.Transfer(creature, itemEntityId, instantTransfer);

			Send.BankTransferRequestR(creature, true);
		}

		/// <summary>
		/// Sent to speak to ego weapon.
		/// </summary>
		/// <remarks>
		/// The only parameter we get is the race of the ego the client selects.
		/// It seems to start looking for egos at the bottom right of the inv.
		/// 
		/// The client can handle multiple egos, but it's really made for one.
		/// It only shows the correct aura if you have only one equipped
		/// and since it starts looking for the ego to talk to in the inventory
		/// you would have to equip the ego you *don't* want to talk to...
		/// 
		/// If you right click the ego to talk to a specific one you get the
		/// correct ego race, but it will still show the stats of the auto-
		/// selected one.
		/// </remarks>
		/// <example>
		/// ...
		/// </example>
		[PacketHandler(Op.NpcTalkEgo)]
		public void NpcTalkEgo(ChannelClient client, Packet packet)
		{
			var egoRace = (EgoRace)packet.GetInt();

			var creature = client.GetCreatureSafe(packet.Id);
			var items = creature.Inventory.GetItems();

			// Stop if race is somehow invalid
			if (egoRace <= EgoRace.None || egoRace > EgoRace.CylinderF)
			{
				Log.Warning("NpcTalkEgo: Invalid ego race '{0}'.", egoRace);
				Send.ServerMessage(creature, Localization.Get("Invalid ego race."));
				Send.NpcTalkEgoR(creature, false, 0, null, null);
				return;
			}

			// Check multi-ego
			// TODO: We can implement multi-ego for the same ego race
			//   once we know how the client selects them.
			//   *Should* we implement that without proper support though?
			if (items.Count(item => item.EgoInfo.Race == egoRace) > 1)
			{
				Send.ServerMessage(creature, Localization.Get("Multiple egos of the same type are currently not supported."));
				Send.NpcTalkEgoR(creature, false, 0, null, null);
				return;
			}

			// Get weapon by race
			var weapon = items.FirstOrDefault(item => item.EgoInfo.Race == egoRace);
			if (weapon == null)
				throw new SevereViolation("Player tried to talk to an ego he doesn't have ({0})", egoRace);

			// Save reference for the NPC
			creature.Vars.Temp["ego"] = weapon;

			// Get NPC name by race
			var npcName = "ego_eiry";
			switch (egoRace)
			{
				case EgoRace.SwordM: npcName = "ego_male_sword"; break;
				case EgoRace.SwordF: npcName = "ego_female_sword"; break;
				case EgoRace.BluntM: npcName = "ego_male_blunt"; break;
				case EgoRace.BluntF: npcName = "ego_female_blunt"; break;
				case EgoRace.WandM: npcName = "ego_male_wand"; break;
				case EgoRace.WandF: npcName = "ego_female_wand"; break;
				case EgoRace.BowM: npcName = "ego_male_bow"; break;
				case EgoRace.BowF: npcName = "ego_female_bow"; break;
				case EgoRace.CylinderM: npcName = "ego_male_cylinder"; break;
				case EgoRace.CylinderF: npcName = "ego_female_cylinder"; break;
			}

			// Get display name
			var displayName = "Eiry";
			if (egoRace < EgoRace.EirySword || egoRace > EgoRace.EiryWind)
				displayName = string.Format(Localization.Get("{0} of {1}"), weapon.EgoInfo.Name, creature.Name);

			// Get NPC for dialog
			var npc = ChannelServer.Instance.World.GetNpc("_" + npcName);
			if (npc == null)
			{
				Log.Error("NpcTalkEgo: Ego NPC not found ({0})", npcName);
				Send.NpcTalkEgoR(creature, false, 0, null, null);
				return;
			}

			// Success
			Send.NpcTalkEgoR(creature, true, npc.EntityId, npcName, displayName);

			// Start dialog
			client.NpcSession.StartTalk(npc, creature);
		}
	}
}
