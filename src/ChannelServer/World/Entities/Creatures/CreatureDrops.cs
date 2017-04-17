﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aura.Data.Database;

namespace Aura.Channel.World.Entities.Creatures
{
	public class CreatureDrops
	{
		/// <summary>
		/// Circle pattern for dropping 21 stacks of gold.
		/// </summary>
		public static int[,] MaxGoldPattern = new int[,] { 
			            {-50,100},  {0,100},  {50,100},
			{-100,50},  {-50,50},   {0,50},   {50,50},   {100,50},
			{-100,0},   {-50,0},    {0,0},    {50,0},    {100,0},
			{-100,-50}, {-50,-50},  {0,-50},  {50,-50},  {100,-50},
			            {-50,-100}, {0,-100}, {50,-100},
		};

		private Creature _creature;
		private List<DropData> _drops;
		private List<Item> _staticDrops;

		public int GoldMin { get; set; }
		public int GoldMax { get; set; }
		public ICollection<DropData> Drops { get { lock (_drops) return _drops.ToArray(); } }

		/// <summary>
		/// List of unique items that are dropped once.
		/// </summary>
		public ICollection<Item> StaticDrops { get { lock (_staticDrops) return _staticDrops.ToArray(); } }

		public CreatureDrops(Creature creature)
		{
			_creature = creature;
			_drops = new List<DropData>();
			_staticDrops = new List<Item>();
		}

		/// <summary>
		/// Adds drop
		/// </summary>
		/// <param name="itemId"></param>
		/// <param name="chance"></param>
		public void Add(int itemId, float chance)
		{
			lock (_drops)
				_drops.Add(new DropData(itemId, chance));
		}

		/// <summary>
		///  Adds all drops
		/// </summary>
		/// <param name="drops"></param>
		public void Add(ICollection<DropData> drops)
		{
			lock (_drops)
			{
				foreach (var drop in drops)
					_drops.Add(drop.Copy());
			}
		}

		/// <summary>
		///  Adds item as drop.
		/// </summary>
		/// <param name="drops"></param>
		public void Add(Item item)
		{
			lock (_staticDrops)
				_staticDrops.Add(item);
		}

		/// <summary>
		/// Clears static drops, so they can only ever be dropped once.
		/// </summary>
		public void ClearStaticDrops()
		{
			lock (_staticDrops)
				_staticDrops.Clear();
		}

		/// <summary>
		/// Removes items from static drops that match the given predicate.
		/// </summary>
		/// <param name="predicate"></param>
		public void RemoveFromStaticDrops(Predicate<Item> predicate)
		{
			lock (_staticDrops)
				_staticDrops.RemoveAll(predicate);
		}
	}
}
