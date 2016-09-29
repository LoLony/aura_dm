//--- Aura Script -----------------------------------------------------------
// Good Luck Potion (with big air quotes this is for levels kek)
//--- Description -----------------------------------------------------------
// Handles items like wings, that warp you to a pre-defined location.
//---------------------------------------------------------------------------

[ItemScript("/bees/")]
public class LuckPotionItemScript : ItemScript
{

	public override void OnUse(Creature creature, Item item, string parameter)
	{
		creature.Pet.GiveExp(7500000);
		Send.ServerMessage(creature, Localization.Get("Generated 7.5M XP to pet. ;-)"));
	}
}
