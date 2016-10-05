public class TreasureChestScript : GeneralScript
{
	[On("ErinnDaytimeTick")]
	public void OnErinnDaytimeTick(ErinnTime now)
	{
		Send.Notice(NoticeType.TopGreen, "The Treasure Chest Aura is currently in effect.");
	}

	[On("PlayerLoggedIn")]
	public void OnPlayerLoggedIn(Creature creature)
	{
		Send.Notice(creature, NoticeType.TopGreen, "The Treasure Chest Aura is currently in effect.");
	}

	[On("CreatureKilled")]
	public void OnCreatureKilled(Creature creature, Creature killer)
	{
		if (!creature.IsPlayer && !creature.IsPartner && killer.IsPlayer)
		{
			var rnd = RandomProvider.Get();
			var pos = creature.GetPosition();
			var keyVar = rnd.NextDouble();
			var chestVar = rnd.NextDouble();
			if (keyVar * 100 < 0.5 * ChannelServer.Instance.Conf.World.DropRate)
			{
				var eventDropPos = pos.GetRandomInRange(50, rnd);
				var eventItem = new Item(70156);
				eventItem.Drop(creature.Region, eventDropPos, Item.DropRadius, killer, true);
			}
			else if (keyVar * 100 < 2.0 * ChannelServer.Instance.Conf.World.DropRate)
			{
				var eventDropPos = pos.GetRandomInRange(50, rnd);
				var eventItem = new Item(70155);
				eventItem.Drop(creature.Region, eventDropPos, Item.DropRadius, killer, true);
			}
			if (chestVar * 100 < 0.25 * ChannelServer.Instance.Conf.World.DropRate)
			{
				var eventDropPos = pos.GetRandomInRange(50, rnd);
				var eventItem = new Item(90139);
				eventItem.Drop(creature.Region, eventDropPos, Item.DropRadius, killer, true);
			}
			else if (chestVar * 100 < 0.5 * ChannelServer.Instance.Conf.World.DropRate)
			{
				var eventDropPos = pos.GetRandomInRange(50, rnd);
				var eventItem = new Item(90138);
				eventItem.Drop(creature.Region, eventDropPos, Item.DropRadius, killer, true);
			}
		}
	}
}