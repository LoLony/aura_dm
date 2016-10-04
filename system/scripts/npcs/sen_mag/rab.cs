//--- Aura Script -----------------------------------------------------------
// Fleta's Rab
//--- Description -----------------------------------------------------------
// The canine companion of the lost girl
//---------------------------------------------------------------------------

public class RabScript : NpcScript
{
	public override void Load()
	{
		SetRace(20);
		SetName("_rab");
		SetBody(height: 0.9f);
		SetColor(0x00000000, 0x00404040, 0x00C0C0C0);
		SetLocation(53, 102918, 108990, 0);

		AddPhrase("Ruff");
		AddPhrase("Ruff, ruff");
		AddPhrase("Kmmmph");
		AddPhrase("...");
		AddPhrase("......");
		AddPhrase("Whimper");
	}

	public void OnErinnTimeTick(ErinnTime time)
    {
        if (time.Minute == 0 && (time.Hour == 9 | time.Hour == 15 | time.Hour == 19)) // Warp in
        {
        	int plusX = Convert.ToInt32(Random(0,4000) % 100);
        	int plusY = Convert.ToInt32(Random(0,4000) % 100);
        	NPC.WarpFlash(53, 102000 + plusX, 106000 + plusY);
        }
        else if (time.Minute == 0 && (time.Hour == 11 | time.Hour == 17 | time.Hour == 21)) // Warp out
        {
        	NPC.WarpFlash(22, 6000,5000);
        }
        else        // If it is not x:00, there's a 0~6 chance to move
        {
            int chanceToMove = Convert.ToInt32(Random() % 6);
            if (chanceToMove == 1)
            {
                Position currentPos = this.NPC.GetPosition();
                int newX = currentPos.X;
                int newY = currentPos.Y;
                newX += Convert.ToInt32(Random(-300, 300) % 100);
                newY += Convert.ToInt32(Random(-300, 300) % 100);
                Position newPos = new Position(newX, newY);
                this.NPC.Move(newPos, true);
            }
        }
    }

	protected override async Task Talk()
	{
		//if (Player.RaceID < 10000) {
			Msg("(Fleta's dog. I think it's name is Rab)");
		//}
		//else
		//{
		//	//womp
		//}
	}
}