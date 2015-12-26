using System;

using mapKnight.Entity;
using mapKnight.Values;

namespace mapKnight.Android
{
	public class CGLBoundedPoint
	{
		public fRectangle TextureRectangle;
		public Slot Slot;

		public CGLBoundedPoint (fRectangle textureRectangle, Slot slot)
		{
			TextureRectangle = textureRectangle;
			Slot = slot;
		}
	}
}

