using System;

using mapKnight.Values;

namespace mapKnight.Android
{
	public struct Touch : IDisposable
	{
		public delegate void OnMoveHandler (Touch sender, Size delta);

		public delegate void OnDisposeHandler (Touch sender);

		public event OnMoveHandler OnMove;

		public event OnDisposeHandler OnDispose;

		public int TouchID;

		public Point Position;
		public Point LastPosition;

		public Size Delta;

		public int X{ get { return Position.X; } }

		public int Y { get { return Position.Y; } }

		public Touch (int id, int x, int y) : this ()
		{
			TouchID = id;
			Position = new Point (x, y);
		}

		public void Update (int x, int y)
		{
			if (!(Position.X - x == 0) && !(Position.Y - y == 0)) {
				LastPosition = new Point (Position.X, Position.Y);
				Position = new Point (x, y);
				Delta = -new Size (LastPosition - Position);
				#if LOGTOUCH
				Log.All (this, "touch (id=" + TouchID.ToString () + ") moved to " + Position.ToString () + " from " + LastPosition.ToString () + " delta " + Delta.ToString (), MessageType.Debug);
				#endif

				if (OnMove != null)
					OnMove (this, Delta);
			}
		}

		public void Dispose ()
		{
			#if LOGTOUCH
			Log.All (this, "touch (id=" + TouchID.ToString () + ") disposed", MessageType.Debug);
			#endif
			if (OnDispose != null) {
				OnDispose (this);
			}
		}
	}
}