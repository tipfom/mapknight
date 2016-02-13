using System;

namespace mapKnight.Basic
{
	public struct Rectangle
	{
		public static fRectangle operator / (Rectangle rect, Size size)
		{
			return new fRectangle (new fPoint ((float)rect.Position.X / (float)size.Width, (float)rect.Position.Y / (float)size.Height), new fSize ((float)rect.Width / (float)size.Width, (float)rect.Height / (float)size.Height));
		}

		private Point iPosition;

		public Point Position{ get { return iPosition; } set { iPosition = value; } }

		private Size iSize;

		public Size Size{ get { return iSize; } set { iSize = value; } }

		public int Left{ get { return Position.X; } set { iPosition.X = value; } }

		public int Right{ get { return Position.X + Width; } set { iPosition.X = value - Width; } }

		public int Top{ get { return Position.Y + Height; } set { iPosition.Y = value - Height; } }

		public int Bottom{ get { return Position.Y; } set { iPosition.Y = value; } }

		public int Width{ get { return Size.Width; } set { iSize.Width = value; } }

		public int Height { get { return Size.Height; } set { iSize.Height = value; } }

		public Rectangle (Point position, Size size) : this ()
		{
			this.Position = position;
			this.Size = size;
		}

		public Rectangle (int x, int y, int width, int height) : this (new Point (x, y), new Size (width, height))
		{
			
		}

		public bool Collides (Point point)
		{
			if (Right > point.X && Left < point.X && Top > point.Y && Bottom < point.Y)
				return true;
			return false;
		}
	}
}