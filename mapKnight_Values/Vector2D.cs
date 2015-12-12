using System;

namespace mapKnight.Values
{
	public struct Vector2D
	{
		public static Vector2D operator + (Vector2D vec1, Vector2D vec2)
		{
			return new Vector2D (vec1.X + vec2.X, vec1.Y + vec2.Y);
		}

		public static Vector2D operator + (Vector2D vec1, fVector2D vec2)
		{
			return new Vector2D (vec1.X + (int)vec2.X, vec1.Y + (int)vec2.Y);
		}

		public static Vector2D operator * (Vector2D vec1, int multiplier)
		{
			return new Vector2D (vec1.X * multiplier, vec1.Y * multiplier);
		}

		public int X;
		public int Y;

		public Vector2D (int x, int y)
		{
			X = x;
			Y = y;
		}
	}
}

