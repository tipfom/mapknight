using System;
using System.Collections.Generic;

using Android.Views;
using GL = Android.Opengl.GLES20;

namespace mapKnight_Android
{
	public class ButtonManager : TouchManager
	{
		public ButtonManager () : base ()
		{
		}

		public Button Create (int x, int y, int width, int height)
		{
			return new Button (this, x, y, width, height);
		}

		public Button Create (Point position, Size size)
		{
			return new Button (this, position.X, position.Y, size.Width, size.Height);
		}

		public Button Create (Rectangle hitbox)
		{
			return new Button (this, hitbox.Position.X, hitbox.Position.Y, hitbox.Size.Width, hitbox.Size.Height);
		}

		public class Button : IDisposable
		{
			public delegate void ClickHandler ();

			public event ClickHandler OnClick;
			public event ClickHandler OnLeave;
			public event ClickHandler OnTouchChanged;

			public readonly Rectangle Hitbox;

			public bool Clicked{ get; private set; }

			private List<int> connectedTouches;
			private int activeTouch;

			public Button (ButtonManager manager, int x, int y, int width, int height)
			{
				Hitbox = new Rectangle (x, y, width, height);
				Clicked = false;
				connectedTouches = new List<int> ();
				activeTouch = -1;

				manager.OnTouchBegan += HandleOnTouchBegan;
				manager.OnTouchEnded += HandleOnTouchEnded;
				manager.OnTouchMoved += HandleOnTouchMoved;
			}

			private void HandleOnTouchBegan (TouchManager manager, Touch touch)
			{
				if (Hitbox.Collides (touch.Position)) {
					connectedTouches.Add (touch.TouchID);
					if (!Clicked) {
						Clicked = true;
						if (OnClick != null)
							OnClick ();
					}
					if (activeTouch == -1) {
						activeTouch = connectedTouches [0];
					}
				}
			}

			private void HandleOnTouchEnded (TouchManager manager, Touch touch)
			{
				if (connectedTouches.Contains (touch.TouchID)) {
					connectedTouches.Remove (touch.TouchID);
					if (connectedTouches.Count > 0) {
						if (OnLeave != null)
							OnLeave ();
						activeTouch = connectedTouches [0];
						if (OnClick != null)
							OnClick ();
					} else {
						Clicked = false;
						activeTouch = -1;
						if (OnLeave != null)
							OnLeave ();
					}
				}
			}

			private void HandleOnTouchMoved (TouchManager manager, Touch touch)
			{
				if (Hitbox.Collides (touch.Position)) {
					if (!connectedTouches.Contains (touch.TouchID)) {
						connectedTouches.Add (touch.TouchID);
					}
				} else if (connectedTouches.Contains (touch.TouchID)) {
					connectedTouches.Remove (touch.TouchID);
					if (activeTouch == touch.TouchID) {
						if (connectedTouches.Count > 0) {
							activeTouch = connectedTouches [0];
							if (OnTouchChanged != null)
								OnTouchChanged ();
						} else {
							Clicked = false;
							activeTouch = -1;
							if (OnLeave != null)
								OnLeave ();
						}
					}
				}
			}

			public void Dispose ()
			{
				GlobalContent.TouchManager.OnTouchBegan -= HandleOnTouchBegan;
				GlobalContent.TouchManager.OnTouchEnded -= HandleOnTouchEnded;
				GlobalContent.TouchManager.OnTouchMoved -= HandleOnTouchMoved;

				Clicked = false;
				activeTouch = -1;
				connectedTouches.Clear ();
				OnClick = null;
				OnLeave = null;
				OnTouchChanged = null;
			}
		}
	}
}