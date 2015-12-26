using System;
using System.Collections.Generic;

using mapKnight.Utils;
using mapKnight.Values;

namespace mapKnight.Android
{
	public class AnimationStep : ICloneable
	{
		public readonly int Time;
		public Dictionary<int,Point> Movements;

		public AnimationStep (XMLElemental config)
		{
			Time = Convert.ToInt32 (config.Attributes ["time"]);
			Movements = new Dictionary<int, Point> ();

			foreach (XMLElemental move in config.GetAll("move")) {
				//Movements.Add (Content.Character.GetID (move.Attributes ["point"].ToUpper ()), new Point (Convert.ToInt32 (move.Attributes ["x"]), Convert.ToInt32 (move.Attributes ["y"])));
			}
		}

		protected AnimationStep (int time, Dictionary<int,Point> movements)
		{
			Time = time;
			Movements = movements;
		}

		//		public Vector2D[] GetFinalPosition ()
		//		{
		//			Vector2D[] result = new Vector2D[Content.Character.BodyParts];
		//
		//			for (int i = 0; i < Content.Character.BodyParts; i++) {
		//				if (Movements.ContainsKey (i)) {
		//					result [i] = new Vector2D (Movements [i].X, Movements [i].Y);
		//				} else {
		//					result [i] = new Vector2D (0, 0);
		//				}
		//			}
		//
		//			return result;
		//		}
		//
		//		public Vector2D[] GetFinalPosition (Vector2D[] last)
		//		{
		//			Vector2D[] result = new Vector2D[Content.Character.BodyParts];
		//
		//			for (int i = 0; i < Content.Character.BodyParts; i++) {
		//				if (Movements.ContainsKey (i)) {
		//					result [i] = new Vector2D (Movements [i].X, Movements [i].Y) + last [i];
		//				} else {
		//					result [i] = last [i];
		//				}
		//			}
		//
		//			return result;
		//		}

		public object Clone ()
		{
			return (object)new AnimationStep (Time, Movements);
		}
	}
}

