using System;

using mapKnight.Entity;
using mapKnight.Values;

namespace mapKnight.Android.CGL.Entity
{
	public class CGLBoundedPoint
	{
		public fRectangle TextureRectangle;
		public Slot Slot;
		public fSize Size;
		public string Name;

		public CGLBoundedPoint (fRectangle textureRectangle, Slot slot, string name, fSize size)
		{
			TextureRectangle = textureRectangle;
			Slot = slot;
			Size = size;
			Name = name;
		}
	}
}

