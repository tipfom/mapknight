using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public class GUITouchHandler : Java.Lang.Object, View.IOnTouchListener {
        const int MAX_TOUCH_COUNT = 4;

        public delegate List<GUIItem> GetCurrentGUIItems ();
        private GetCurrentGUIItems getCurrentGUIItems;
        private List<Touch> activeTouches = new List<Touch>();

        public GUITouchHandler (GetCurrentGUIItems getcurrentguiitems) {
            this.getCurrentGUIItems = getcurrentguiitems;
        }

        public bool OnTouch (View v, MotionEvent e) {
            int pointerIndex = ((int)(e.Action & MotionEventActions.PointerIdMask) >>
                               (int)MotionEventActions.PointerIdShift);
            int pointerId = e.GetPointerId (pointerIndex);

            switch (e.Action & MotionEventActions.Mask) {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    // user touched the screen
                    fVector2D touchPosition = new fVector2D (e.GetX (pointerIndex) / Screen.ScreenSize.X, e.GetY (pointerIndex) / Screen.ScreenSize.Y);

                    if (activeTouches.Count < MAX_TOUCH_COUNT) {
                        activeTouches.Add (new Touch (pointerId, touchPosition));

                        foreach (GUIItem gui in getCurrentGUIItems().FindAll ((GUIItem gui) => gui.Collides (touchPosition))) {
                            // iterates through each colliding gui
                            gui.HandleTouch (Touch.Action.Begin);
                        }
                    }
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                case MotionEventActions.PointerUp:
                    // user lifted the finger of the screen
                    int touchIndex = activeTouches.FindIndex ((Touch touch) => touch.ID == pointerId);
                    if (touchIndex != -1) {
                        foreach (GUIItem gui in getCurrentGUIItems().FindAll ((GUIItem gui) => gui.Collides (activeTouches[touchIndex].Position))) {
                            gui.HandleTouch (Touch.Action.End);
                        }
                        activeTouches.RemoveAt (touchIndex);
                    }
                    break;
                case MotionEventActions.Move:
                    // user moved the finger
                    for (int i = 0; i < activeTouches.Count; i++) {
                        fVector2D activeTouchPosition = new fVector2D (e.GetX (i) / Screen.ScreenSize.X, e.GetY (i) / Screen.ScreenSize.Y);
                        if (activeTouches[i].Position - activeTouchPosition != fVector2D.Zero) {
                            // touch moved
                            foreach (GUIItem gui in getCurrentGUIItems()) {
                                if (gui.Collides (activeTouchPosition) && !gui.Collides (activeTouches[i].Position)) {
                                    // collides with the current position, but not with the last
                                    gui.HandleTouch (Touch.Action.Enter);
                                } else if (!gui.Collides (activeTouchPosition) && gui.Collides (activeTouches[i].Position)) {
                                    // collides with the last position, but not with the current -> touch moved out of guielement
                                    gui.HandleTouch (Touch.Action.Leave);
                                } else if (gui.Collides (activeTouchPosition)) {
                                    // touch moved inside gui
                                    gui.HandleTouch (Touch.Action.Move);
                                }
                            }

                            activeTouches[i].Position = activeTouchPosition;
                        }
                    }
                    break;
            }

            return true;
        }

        public class Touch {
            public enum Action {
                Move,
                Begin,
                End,
                Leave,
                Enter
            }

            // class to be able to change the position
            public readonly int ID;
            public fVector2D Position;

            public Touch (int id, fVector2D initialposition) {
                ID = id;
                Position = initialposition;
            }
        }
    }
}