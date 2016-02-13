using System;

namespace mapKnight.Basic
{
	public class fPoint
	{
		public static fPoint operator + (fPoint point1, fPoint point2)
		{
			return new fPoint (point1.X + point2.X, point1.Y + point2.Y);
		}

		public static fPoint operator - (fPoint point1, fPoint point2)
		{
			return new fPoint (point1.X - point2.X, point1.Y - point2.Y);
		}

		public float X{ get; set; }

		public float Y{ get; set; }

		public fPoint (float x, float y)
		{
			X = x;
			Y = y;
		}

		public fPoint (fSize size)
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

