using System;

using mapKnight.Basic;

namespace mapKnight.Android.PhysX
{
	public class PhysXEntity : Entity
	{
		public fSize Bounds;
		public fVector2D Velocity;
		public fVector2D Acceleration;
		public readonly int Weight;
		public PhysXFlag CollisionMask;

		public PhysXEntity (int weight, fSize bounds, int health, fPoint position, string name) : base (health, position, name)
		{
			Bounds = bounds;
			Weight = weight; 

			Velocity = new fVector2D (0, 0);
			Acceleration = new fVector2D (0, 0);
			CollisionMask = PhysXFlag.None;
		}
	}
}

