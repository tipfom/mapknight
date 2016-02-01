using System;
using System.Collections.Generic;
using System.Linq;

using Android.Views;

using mapKnight.Values;

namespace mapKnight.Android
{
	public class TouchManager : Java.Lang.Object, View.IOnTouchListener
	{
		public delegate void HandleOnTouchEvent (TouchManager manager, Touch touch);

		public event HandleOnTouchEvent OnTouchMoved;
		public event HandleOnTouchEvent OnTouchBegan;
		public event HandleOnTouchEvent OnTouchEnded;

		public const int MaximumTouchCount = 4;

		public Touch[] Touches;
		public List<int> ActiveTouches;

		public TouchManager ()
		{
			Touches = new Touch[MaximumTouchCount];
			ActiveTouches = new List<int> ();
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
					Touches [pointerId] = new Touch (pointerId, (int)e.GetX (pointerIndex), (int)e.GetY (pointerIndex));
					ActiveTouches.Add (pointerId);

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
				foreach (int index in ActiveTouches) {
					Touches [index].Update ((int)e.GetX (ActiveTouches.IndexOf (index)), (int)e.GetY (ActiveTouches.IndexOf (index)));
				}
				break;
			case MotionEventActions.Up:
			case MotionEventActions.Cancel:
			case MotionEventActions.PointerUp:
//				Utils.Log.All (this, pointerId.ToString (), MessageType.Debug);
				if (ActiveTouches.Contains (pointerId)) {
					ActiveTouches.Remove (pointerId);
					Touches [pointerId].Dispose ();
				}
				break;
			}
		
			return true;
		}
	}
}