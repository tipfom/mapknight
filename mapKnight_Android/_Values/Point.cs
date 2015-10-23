using System;

namespace mapKnight_Android
{
	public struct Point
	{
		public static Point operator + (Point point1, Point point2)
		{
			return new Point (point1.X + point2.X, point1.Y + point2.Y);
		}

		public static Point operator - (Point point1, Point point2)
		{
			return new Point (point1.X - point2.X, point1.Y - point2.Y);
		}

		public int X;
		public int Y;

		public Point (int x, int y)
		{
			X = x;
			Y = y;
		}

		public Point (Size size)
		{
			X = size.Width;
			Y = size.Height;
		}

		public override string ToString ()
		{
			return String.Format ("X={0}; Y={1}", X.ToString (), Y.ToString ());
		}
	}
}

