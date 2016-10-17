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

/*
ordinary chest
---------------
champion knuckle
shyllien mana knuckle
steel claw
bipennis
francisca
broad stick
ghost hammer
mace
warhammer
wild troll cudgel
elven long bow
highlander long bow
ring bow
wing bow
arbalest
trinity staff
wisdom of snakes
andris wing staff
combat wand
fire wand
ice wand
lightning wand
metal (element) wand
phoenix/crown/crystal wand
eweca short sword
soul-searcher sword
cursed killer sword
taillteann two-handed sword
masamune
leminia's holy moon sword
katana
highlander claymore
gargoyle sword
executioner's sword
dustin silver knight sword
dragon blade
noble's sword
war sword
wakizashi
tanto
royal crystal wing sword
muramasa
machete
hooked cutlass
falcata
crystal sword
bone marine sword
beholder's sword

premium chest
--------------
champion sheep knuckle
merlin's shyllien mana knuckle (type 2)
smiley knuckle
upgraded tangerine
skull battle axe
celtic royal warrior hammer
celtic warrior hammer
duster
sea troll cudgel
toy hammer
luxurious stage punch glove
royal crystal wing bow
salvation bow
bhafel hunter
black dragon knight's bow
masterpiece bow
celtic crossbow
celtic royal crossbow
black mask staff
hermit's staff
original sin staff
royal crystal wing staff
white wing staff
celtic druid staff
master lich wand
savage (element) wand
fairy (element) wand
(element) ice cream
emission/quercus/par wand
eweca's light short sword
krutta broadsword
sword of the goddess
ivory sword
laertes' sword
venom sword
glowing ice sword

next: clothes (hats/body/hands/feet/robe)
next: shields?
next: training potions for allowed skills
next: dura hammers
next: holy water/mass HW
next: instruments? blacksmith manuals/sewing patterns? potions? crafting mats? fomor scrolls? dyes? pouches? 
next: spirit weapon repair potion
next: dungeon passes? 

*/

public class TreasureChestScript : GeneralScript
{
	[On("ErinnDaytimeTick")]
	public void OnErinnDaytimeTick(ErinnTime now)
	{
		Send.Notice(NoticeType.TopGreen, "The Treasure Chest Aura is currently in effect. Revised 10/13/16.");
		// these don't seem to appear, rescript?
	}

	[On("PlayerLoggedIn")]
	public void OnPlayerLoggedIn(Creature creature)
	{
		Send.Notice(creature, NoticeType.TopGreen, "The Treasure Chest Aura is currently in effect. REVISION 1 10/14/16: lowered drop rates for chests further. Removeded DEBUG messages. Removed most items. New items to come.");
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
			else if (keyVar * 100 < 2 * ChannelServer.Instance.Conf.World.DropRate)
			{
				var eventDropPos = pos.GetRandomInRange(50, rnd);
				var eventItem = new Item(70155);							// Ordinary Chest Key
				eventItem.Drop(creature.Region, eventDropPos, Item.DropRadius, killer, true);
			}
			if (chestVar * 100 < 0.25 * ChannelServer.Instance.Conf.World.DropRate)
			{
				var eventDropPos = pos.GetRandomInRange(50, rnd);
				var eventItem = new Item(91039);							// Premium Chest
				eventItem.Drop(creature.Region, eventDropPos, Item.DropRadius, killer, true);
			}
			else if (chestVar * 100 < 1 * ChannelServer.Instance.Conf.World.DropRate)
			{
				var eventDropPos = pos.GetRandomInRange(50, rnd);
				var eventItem = new Item(91038);							// Ordinary Chest
				eventItem.Drop(creature.Region, eventDropPos, Item.DropRadius, killer, true);
			}
		}
	}
}