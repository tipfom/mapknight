using System;

namespace mapKnight_Android
{
	public struct Rectangle
	{
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
	}
}