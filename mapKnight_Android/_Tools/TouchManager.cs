using System;
using System.Collections.Generic;
using System.Linq;

using Android.Views;

namespace mapKnight_Android
{
	public class TouchManager : Java.Lang.Object, View.IOnTouchListener
	{
		public delegate void HandleOnTouchEvent (TouchManager manager, Touch touch);

		public event HandleOnTouchEvent OnTouchMoved;
		public event HandleOnTouchEvent OnTouchBegan;
		public event HandleOnTouchEvent OnTouchEnded;

		private const int MaximumTouchCount = 4;

		Touch[] Touches;
		List<int> activeTouches;

		public TouchManager ()
		{
			Touches = new Touch[MaximumTouchCount];
			activeTouches = new List<int> ();
		}

		public bool OnTouch (View v, MotionEvent e)
		{
			var pointerIndex = ((int)(e.Action & MotionEventActions.PointerIdMask) >>
			                   (int)MotionEventActions.PointerIdShift);
			var pointerId = e.GetPointerId (pointerIndex);
			var action = (e.Action & MotionEventActions.Mask);

			switch (action) {
			case MotionEventActions.Down:
			case MotionEventActions.PointerDown:
				if (e.PointerCount <= MaximumTouchCount) {
					Touches [pointerId] = new Touch (pointerId, (int)e.GetX (pointerId), (int)e.GetY (pointerId));
					activeTouches.Add (pointerId);

					// handle events
					if (OnTouchBegan != null)
						OnTouchBegan (this, Touches [pointerId]);
					Touches [pointerId].OnMove += (Touch sender, Size delta) => {
						if (OnTouchMoved != null)
							OnTouchMoved (this, sender);
					};
					Touches [pointerId].OnDispose += (Touch sender) => {
						if (OnTouchEnded != null)
							OnTouchEnded (this, sender);
					};
				}
				break;
			case MotionEventActions.Move:
				foreach (int index in activeTouches) {
					Touches [index].Update ((int)e.GetX (activeTouches.IndexOf (index)), (int)e.GetY (activeTouches.IndexOf (index)));
				}
				break;
			case MotionEventActions.Up:
			case MotionEventActions.Cancel:
			case MotionEventActions.PointerUp:
				Utils.Log.All (this, pointerId.ToString (), MessageType.Debug);
				Touches [pointerId].Dispose ();
				activeTouches.Remove (pointerId);
				break;
			}
		
			return true;
		}
	}
}