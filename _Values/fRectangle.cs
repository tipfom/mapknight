using System;

namespace mapKnight.Values
{
	public struct fRectangle
	{
		private fPoint iPosition;

		public fPoint Position{ get { return iPosition; } set { iPosition = value; } }

		private fSize iSize;

		public fSize Size{ get { return iSize; } set { iSize = value; } }

		public float Left{ get { return Position.X; } set { iPosition.X = value; } }

		public float Right{ get { return Position.X + Width; } set { iPosition.X = value - Width; } }

		public float Top{ get { return Position.Y + Height; } set { iPosition.Y = value - Height; } }

		public float Bottom{ get { return Position.Y; } set { iPosition.Y = value; } }

		public float Width{ get { return Size.Width; } set { iSize.Width = value; } }

		public float Height { get { return Size.Height; } set { iSize.Height = value; } }

		public float[] Verticies { get { return new float[]{ Left, Top, Left, Bottom, Right, Bottom, Right, Top }; } }

		public fRectangle (fPoint position, fSize size) : this ()
		{
			this.Position = position;
			this.Size = size;
		}

	}
}

