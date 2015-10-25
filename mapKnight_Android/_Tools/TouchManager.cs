using System;
using System.Collections.Generic;
using System.Linq;

using Android.Views;

namespace mapKnight_Android
{
	public class TouchManager : Java.Lang.Object, View.IOnTouchListener
	{
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
				if (e.PointerCount < MaximumTouchCount - 1) {
					Touches [pointerId] = new Touch (pointerId, (int)e.GetX (pointerId), (int)e.GetY (pointerId));
					activeTouches.Add (pointerId);
				}
				break;
			case MotionEventActions.Move:
				foreach (int index in activeTouches) {
					Touches [index].Update ((int)e.GetX (index), (int)e.GetY (index));
				}
				break;
			case MotionEventActions.Up:
			case MotionEventActions.Cancel:
			case MotionEventActions.PointerUp:
				Touches [pointerId].Dispose ();
				activeTouches.Remove (pointerId);
				break;
			}
		
			return true;
		}
	}
}

