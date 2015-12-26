using System;

namespace mapKnight.Android
{
	public struct Level
	{
		public readonly int Health;
		public readonly int Power;
		public readonly int ExperienceNeeded;

		public Level (int health, int power, int expneeded)
		{
			Health = health;
			Power = power;
			ExperienceNeeded = expneeded;
		}
	}
}

