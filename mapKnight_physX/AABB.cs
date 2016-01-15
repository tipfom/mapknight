using System;

using mapKnight.Values;

namespace mapKnight.PhysX
{
	public struct AABB // axis aligned bounding box, my friend
	{
		Vector2D Min;
		Vector2D Max;

		public AABB (Vector2D min, Vector2D max) : this ()
		{
			Min = Vector2D.Min (min, max);
			Max = Vector2D.Max (min, max);
		}
	}
}