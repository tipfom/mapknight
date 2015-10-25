using System;

namespace mapKnight_Android
{
	public struct Size
	{
		public static Size operator - (Size size)
		{
			return new Size (-size.Width, -size.Height);
		}

		public int Width;
		public int Height;

		public Size (int width, int height)
		{
			Width = width;
			Height = height;
		}

		public Size (Point point)
		{
			Width = point.X;
			Height = point.Y;
		}

		public override string ToString ()
		{
			return String.Format ("Width = {0}; Height = {1}", Width.ToString (), Height.ToString ());
		}
	}
}

