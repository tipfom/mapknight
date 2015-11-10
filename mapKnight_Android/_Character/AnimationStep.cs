using System;
using System.Collections.Generic;

namespace mapKnight_Android
{
	public class AnimationStep : ICloneable
	{
		public readonly int Time;
		public Dictionary<string,Point> Movements;

		public AnimationStep (XMLElemental config)
		{
			Time = Convert.ToInt32 (config.Attributes ["time"]);
			Movements = new Dictionary<string, Point> ();

			foreach (XMLElemental move in config.GetAll("move")) {
				Movements.Add (move.Attributes ["point"].ToUpper (), new Point (Convert.ToInt32 (move.Attributes ["x"]), Convert.ToInt32 (move.Attributes ["y"])));
			}
		}

		protected AnimationStep (int time, Dictionary<string,Point> movements)
		{
			
		}

		public object Clone ()
		{
			return (object)new AnimationStep (Time, Movements.Clone ());
		}
	}
}

