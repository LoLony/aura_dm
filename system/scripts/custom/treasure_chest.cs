//--- Aura Script -----------------------------------------------------------
// Treasure Chest
//--- Description -----------------------------------------------------------
// Script to allow user to randomly find keys and chests.
// Keys: ordinary (1%), premium (0.5%)
// Chest: ordinary(0.5%), premium (0.25%)
// Check these rates to see if they can even be used. 1% may be as low as we can go.
// Check the scripting for the notices; they seem to not appear.
// Currently, there is no script for the chests. Coming soon.
//---------------------------------------------------------------------------

public class TreasureChestScript : GeneralScript
{
	[On("ErinnDaytimeTick")]
	public void OnErinnDaytimeTick(ErinnTime now)
	{
		Send.Notice(NoticeType.TopGreen, "The Treasure Chest Aura is currently in effect.");
		// these don't seem to appear, rescript?
	}

	[On("PlayerLoggedIn")]
	public void OnPlayerLoggedIn(Creature creature)
	{
		Send.Notice(creature, NoticeType.TopGreen, "The Treasure Chest Aura is currently in effect.");
		// these don't seem to appear, rescript?
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
				var eventItem = new Item(70156);							// Premium Chest Key
				eventItem.Drop(creature.Region, eventDropPos, Item.DropRadius, killer, true);
			}
			else if (keyVar * 100 < 1.0 * ChannelServer.Instance.Conf.World.DropRate)
			{
				var eventDropPos = pos.GetRandomInRange(50, rnd);
				var eventItem = new Item(70155);							// Ordinary Chest Key
				eventItem.Drop(creature.Region, eventDropPos, Item.DropRadius, killer, true);
			}
			if (chestVar * 100 < 0.25 * ChannelServer.Instance.Conf.World.DropRate)
			{
				var eventDropPos = pos.GetRandomInRange(50, rnd);
				var eventItem = new Item(90139);							// Premium Chest
				eventItem.Drop(creature.Region, eventDropPos, Item.DropRadius, killer, true);
			}
			else if (chestVar * 100 < 0.5 * ChannelServer.Instance.Conf.World.DropRate)
			{
				var eventDropPos = pos.GetRandomInRange(50, rnd);
				var eventItem = new Item(90138);							// Ordinary Chest
				eventItem.Drop(creature.Region, eventDropPos, Item.DropRadius, killer, true);
			}
		}
	}
}