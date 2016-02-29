using System;
using System.Collections.Generic;

using Android.Content;

using mapKnight.Basic;

namespace mapKnight.Android.CGL
{
	public class CGLEntityPreset: EntityPreset
	{
		protected List<CGLSet> sets;
		protected List<CGLAnimation> animations;

		protected List<CGLBoundedPoint> boundedPoints;

		protected int weight;
		protected fSize bounds;

		public CGLEntityPreset (XMLElemental entityConfig, Context context) : base (entityConfig)
		{
			sets = new List<CGLSet> ();
			animations = new List<CGLAnimation> ();
			boundedPoints = new List<CGLBoundedPoint> ();

			foreach (XMLElemental set in entityConfig.GetAll("set")) {
				sets.Add (new CGLSet (set, context));
			}
			foreach (XMLElemental animation in entityConfig.GetAll("anim")) {
				animations.Add (new CGLAnimation (animation));
			}

			foreach (XMLElemental def in entityConfig["def"].GetAll()) {
				switch (def.Name) {
				case "slot":
					for (int i = 0; i < Convert.ToInt32 (def.Attributes ["bpcount"]); i++) {
						boundedPoints.Add (new CGLBoundedPoint (CGLTools.ParseCoordinates (def ["texture"].Attributes, sets [0].TextureSize), (Slot)Enum.Parse (typeof(Slot), def.Attributes ["name"], true), def.Attributes ["name"] + "_" + i.ToString (), new fSize (float.Parse (def ["drawsize"].Attributes ["width"]), float.Parse (def ["drawsize"].Attributes ["height"]))));
					}
					break;
				}
			}

			weight = int.Parse (entityConfig ["physx"] ["bounds"].Attributes ["weight"]);
			bounds = new fSize (float.Parse (entityConfig ["physx"] ["bounds"].Attributes ["width"]), float.Parse (entityConfig ["physx"] ["bounds"].Attributes ["height"]));
		}

		public CGLEntity Instantiate (uint level, string set)
		{
			return Instantiate (level, set, new fPoint (0, 0));
		}

		public CGLEntity Instantiate (uint level, string set, fPoint position)
		{
			return new CGLEntity (defaultAttributes [Attribute.Health] + (int)((level - 1) * attributeIncrease [Attribute.Health]), position, name,
				weight, bounds,
				boundedPoints, animations, sets.Find ((CGLSet obj) => obj.Name == set));
		}
	}
}

