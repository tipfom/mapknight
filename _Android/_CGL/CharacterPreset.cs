using System;

using Android.Content;

using mapKnight.Basic;

namespace mapKnight.Android.CGL
{
	public class CharacterPreset : mapKnight.Android.CGL.CGLEntityPreset
	{
		private int moveSpeed;
		private int jumpSpeed;

		public CharacterPreset (XMLElemental config, Context context) : base (config, context)
		{
			moveSpeed = int.Parse (config ["physx"] ["speed"].Attributes ["move"]);
			jumpSpeed = int.Parse (config ["physx"] ["speed"].Attributes ["jump"]);
		}

		public new Character Instantiate (uint level, string set)
		{
			return new Character (defaultAttributes [Attribute.Health] + (int)((level - 1) * attributeIncrease [Attribute.Health]), defaultAttributes [Attribute.Energy] + (int)((level - 1) * attributeIncrease [Attribute.Energy]), name, weight,
				bounds, boundedPoints, animations, sets.Find (((mapKnight.Android.CGL.CGLSet obj) => obj.Name == set)), moveSpeed, jumpSpeed) { CollisionMask = mapKnight.Android.PhysX.PhysXFlag.Map };
		}
	}
}

