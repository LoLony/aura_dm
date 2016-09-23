The Art of Becoming: Adding Transformations to Project Aura

Intro
------

Packet Analysis
----------------
Client:
	SkillStart(id, str): sends command to server to begin skill 'id'. If the transformation is not on cooldown, continue. Otherwise, the server will return an error and halt the skill startup.
Server:
	StatUpdatePublic(): sends local players the most recent non-transformed dimensions/HP/CP from the user, presumably for continuity.
	StatUpdatePrivate(): sends the player the most recent non-transformed complete statistics, presumably for continuity.
	TitleUpdate(): the player's title is updated to the transformation title.
	SetLocation(): updates the player's location, presumably in an attempt to place the user at whole number x/y points for the cutscene/animation.
	ToggleSkill(): enables the appropriate transformation skill to be marked as ON/OFF. Passives aren't applied and skills can't be used in the OFF state.
	Transformation(byte, short, short, byte): unsure.
	StatUpdatePublic(): updates the user's HP and other Life statistics.
	StatUpdatePrivate(): updates all stats affected by now online transformation skills.
	EffectDelayed(): activates the visual effect that occurs when the user's passives are announced.
	Notice(): a notice on the screen (not in chatlog) that announces the user's passives are online.
	Chat(): if the user has PvP enabled, the user is informed in the chatlog that PvP based on transformation is online.
	0x00007534(byte, byte, byte, byte): unsure.
	SkillStart(): informs the client that the skill has been started successfully.