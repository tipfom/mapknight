namespace mapKnight.Android
{
	public class CharacterInfo
	{
		public readonly int Health;
		public readonly int Power;
		// ähnlich dem mana

		public CharacterInfo (CharacterPreset preset, int level)
		{
			Health = preset.loadedLevel [level].Health;
			Power = preset.loadedLevel [level].Power;
		}
	}
}

