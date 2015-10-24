using System;

namespace mapKnight_Android
{
	public struct fSize
	{
		public float Width;
		public float Height;

		public fSize (float width, float height)
		{
			Width = width;
			Height = height;
		}

		public fSize (fPoint point)
		{
			Width = point.X;
			Height = point.Y;
		}
	}
}

