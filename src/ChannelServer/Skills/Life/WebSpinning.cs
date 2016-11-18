﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Channel.Network.Sending;
using Aura.Channel.Skills.Base;
using Aura.Channel.World.Entities;
using Aura.Mabi.Const;
using Aura.Mabi.Network;
using Aura.Shared.Network;
using Aura.Shared.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.Channel.Skills.Life
{
	/// <summary>
	/// Skill used by spiders to drop cobweb.
	/// </summary>
	[Skill(SkillId.WebSpinning)]
	public class WebSpinning : IPreparable, ICompletable, ICancelable
	{
		/// <summary>
		/// Id of the item to be dropped.
		/// </summary>
		private const int ItemId = 60008;

		/// <summary>
		/// Prepares skill, goes straight to use to skip readying and using it.
		/// </summary>
		/// <remarks>
		/// The client will take a moment to send the Complete packet,
		/// as if it would cast the skill first.
		/// </remarks>
		/// <param name="creature"></param>
		/// <param name="skill"></param>
		/// <param name="packet"></param>
		public bool Prepare(Creature creature, Skill skill, Packet packet)
		{
			Send.SkillUse(creature, skill.Info.Id, 0);
			skill.State = SkillState.Used;

			return true;
		}

		/// <summary>
		/// Cancels skill (do nothing).
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="skill"></param>
		public void Cancel(Creature creature, Skill skill)
		{
		}

		/// <summary>
		/// Completes skill, dropping a cobweb.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="skill"></param>
		/// <param name="packet"></param>
		public void Complete(Creature creature, Skill skill, Packet packet)
		{
			var cobweb = new Item(ItemId);
			cobweb.Drop(creature.Region, creature.GetPosition(), 200, creature, true);

			Send.SkillComplete(creature, skill.Info.Id);
		}
	}
}
