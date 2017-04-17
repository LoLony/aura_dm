﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Aura.Data.Database
{
	[Serializable]
	public class CollectingData
	{
		public int Id { get; set; }
		public string Target { get; set; }
		public string RightHand { get; set; }
		public string LeftHand { get; set; }
		public int SkillId { get; set; }
		public int Delay { get; set; }
		public int DurabilityLoss { get; set; }
		public float SuccessRate { get; set; }
		public float ResourceReduction { get; set; }
		public float ResourceRecovering { get; set; }
		public float ResourceReductionRainBonus { get; set; }
		public int Source { get; set; }
		public List<CollectingItemData> Products { get; set; }
		public List<CollectingItemData> Products2 { get; set; }
		public List<CollectingItemData> FailProducts { get; set; }
		public List<CollectingItemData> FailProducts2 { get; set; }

		public int GetRndProduct(Random rnd, ProductBonus bonus)
		{
			return this.GetRndFrom(this.Products, rnd, bonus);
		}

		public int GetRndProduct2(Random rnd, ProductBonus bonus)
		{
			return this.GetRndFrom(this.Products2, rnd, bonus);
		}

		public int GetRndFailProduct(Random rnd, ProductBonus bonus)
		{
			return this.GetRndFrom(this.FailProducts, rnd, bonus);
		}

		public int GetRndFailProduct2(Random rnd, ProductBonus bonus)
		{
			return this.GetRndFrom(this.FailProducts2, rnd, bonus);
		}

		private int GetRndFrom(List<CollectingItemData> items, Random rnd, ProductBonus productBonus)
		{
			if (items.Count == 0)
				return 0;

			// Create a new list, with the adjusted chances if necessary
			List<CollectingItemData> list;
			if (productBonus != null)
			{
				list = new List<CollectingItemData>();
				foreach (var item in items)
				{
					var itemId = item.Id;
					var chance = item.Chance;

					if (itemId == productBonus.ItemId)
						chance += productBonus.Bonus;

					list.Add(new CollectingItemData() { Id = itemId, Chance = chance });
				}
			}
			else
			{
				list = items;
			}

			var total = list.Sum(cls => cls.Chance);

			var randVal = rnd.NextDouble() * total;
			var i = 0;
			for (; randVal > 0; ++i)
				randVal -= list[i].Chance;

			return list[i - 1].Id;
		}
	}

	public class ProductBonus
	{
		public readonly int ItemId;
		public readonly int Bonus;

		public ProductBonus(int itemId, int bonus)
		{
			this.ItemId = itemId;
			this.Bonus = bonus;
		}
	}

	[Serializable]
	public class CollectingItemData
	{
		public int Id { get; set; }
		public int Chance { get; set; }
	}

	public class CollectingDb : DatabaseJsonIndexed<int, CollectingData>
	{
		protected override void ReadEntry(JObject entry)
		{
			entry.AssertNotMissing("id", "target", "rightHand", "leftHand", "skillId", "delay", "durabilityLoss", "successRate", "resourceReduction", "resourceRecovering", "resourceReductionRainBonus");

			var data = new CollectingData();
			data.Id = entry.ReadInt("id");
			data.Target = entry.ReadString("target");
			data.RightHand = entry.ReadString("rightHand");
			data.LeftHand = entry.ReadString("leftHand");
			data.SkillId = entry.ReadInt("skillId");
			data.Delay = entry.ReadInt("delay");
			data.DurabilityLoss = entry.ReadInt("durabilityLoss");
			data.SuccessRate = entry.ReadFloat("successRate");
			data.ResourceReduction = entry.ReadFloat("resourceReduction");
			data.ResourceRecovering = entry.ReadFloat("resourceRecovering");
			data.ResourceReductionRainBonus = entry.ReadInt("resourceReductionRainBonus");
			data.Source = entry.ReadInt("source");

			data.Products = this.ReadList(entry, "products");
			data.Products2 = this.ReadList(entry, "products2");
			data.FailProducts = this.ReadList(entry, "failProducts");
			data.FailProducts2 = this.ReadList(entry, "failProducts2");

			this.Entries[data.Id] = data;
		}

		private List<CollectingItemData> ReadList(JObject entry, string name)
		{
			var result = new List<CollectingItemData>();

			if (!entry.ContainsKeys(name) || !(entry[name] is JArray))
				return result;

			foreach (JObject item in entry[name])
			{
				item.AssertNotMissing("itemId", "chance");

				var itemId = item.ReadInt("itemId");
				var chance = item.ReadInt("chance");

				result.Add(new CollectingItemData() { Id = itemId, Chance = chance });
			}

			return result;
		}
	}
}
