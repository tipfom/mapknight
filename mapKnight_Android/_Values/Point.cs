using System;

namespace mapKnight_Android
{
	public struct Point
	{
		public int X;
		public int Y;

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		public Point(Size size)
		{
			X = size.Width;
			Y = size.Height;
		}
	}
}

