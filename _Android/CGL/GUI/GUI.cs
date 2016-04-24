using System.Collections.Generic;
using Android.Views;
using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public class GUI : Java.Lang.Object, View.IOnTouchListener {
        const int MAX_TOUCH_COUNT = 3;

        // text
        // buttons ( nur text, text und standartbild, nur bild)
        // bar
        // muss touches handeln
        private List<Touch> activeTouches = new List<Touch> ();
        private List<GUIElement> addedElements = new List<GUIElement> ();

        public GUI () {

        }

        public bool OnTouch (View v, MotionEvent e) {
            int pointerIndex = ((int)(e.Action & MotionEventActions.PointerIdMask) >>
                               (int)MotionEventActions.PointerIdShift);
            int pointerId = e.GetPointerId (pointerIndex);

            switch (e.Action & MotionEventActions.Mask) {
            case MotionEventActions.Down:
            case MotionEventActions.PointerDown:
                // user touched the screen
                fVector2D touchPosition = new fVector2D (e.GetX (pointerIndex) / Content.ScreenSize.Width, e.GetY (pointerIndex) / Content.ScreenSize.Height);

                if (activeTouches.Count < MAX_TOUCH_COUNT) {
                    activeTouches.Add (new Touch (pointerId, touchPosition));

                    foreach (GUIElement gui in addedElements.FindAll ((GUIElement gui) => gui.Collides (touchPosition))) {
                        // iterates through each colliding gui
                        gui.HandleTouchBegin ();
                    }
                }
                break;
            case MotionEventActions.Up:
            case MotionEventActions.Cancel:
            case MotionEventActions.PointerUp:
                // user lifted the finger of the screen
                int touchIndex = activeTouches.FindIndex ((Touch touch) => touch.ID == pointerId);
                if (touchIndex != -1) {
                    activeTouches.RemoveAt (touchIndex);
                }
                break;
            case MotionEventActions.Move:
                // user moved the finger
                for (int i = 0; i < activeTouches.Count; i++) {
                    fVector2D activeTouchPosition = new fVector2D (e.GetX (i) / Content.ScreenSize.Width, e.GetY (i) / Content.ScreenSize.Height);
                    if (activeTouches[i].Position - activeTouchPosition != fVector2D.Zero) {
                        // touch moved
                        activeTouches[i].Position = activeTouchPosition;
                    }
                }
                break;
            }

            return true;
        }

        private class Touch {
            public readonly int ID;
            public fVector2D Position;

            public Touch (int id, fVector2D initialposition) {
                ID = id;
                Position = initialposition;
            }
        }
    }
}