using System;

namespace mapKnight.Basic
{
	public class ChangingProperty
	{
		public int Current{ get; set; }

		public int Max{ get; private set; }

		public float Percent{ get { return (float)Current / (float)Max; } }

		public ChangingProperty (int maxValue)
		{
			Max = maxValue;
		}
	}
}
