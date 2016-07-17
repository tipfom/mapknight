using System.Collections.Generic;
using Android.Views;
using mapKnight.Core;
using mapKnight.Extended.Graphics.GUI;
using Window = mapKnight.Extended.Graphics.Window;

namespace mapKnight.Android {
    public class TouchHandler : Java.Lang.Object, View.IOnTouchListener {
        const int MAX_TOUCH_COUNT = 4;

        public static TouchHandler Instance { get; } = new TouchHandler( );

        private List<Touch> activeTouches = new List<Touch>( );

        public bool OnTouch (global::Android.Views.View v, MotionEvent e) {
            int pointerIndex = ((int)(e.Action & MotionEventActions.PointerIdMask) >>
                               (int)MotionEventActions.PointerIdShift);
            int pointerId = e.GetPointerId(pointerIndex);

            switch (e.Action & MotionEventActions.Mask) {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    // user touched the screen
                    Vector2 touchPosition = new Vector2((e.GetX(pointerIndex) / Window.Size.Width - 0.5f) * 2 * Window.Ratio, (e.GetY(pointerIndex) / Window.Size.Height - 0.5f) * -2);

                    if (activeTouches.Count < MAX_TOUCH_COUNT) {
                        activeTouches.Add(new Touch(pointerId, touchPosition));

                        foreach (GUIItem gui in GUIRenderer.Current.FindAll((GUIItem gui) => gui.Collides(touchPosition))) {
                            // iterates through each colliding gui
                            gui.HandleTouch(GUITouchAction.Begin);
                        }
                    }
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                case MotionEventActions.PointerUp:
                    // user lifted the finger of the screen
                    int touchIndex = activeTouches.FindIndex((Touch touch) => touch.ID == pointerId);
                    if (touchIndex != -1) {
                        foreach (GUIItem gui in GUIRenderer.Current.FindAll((GUIItem gui) => gui.Collides(activeTouches[touchIndex].Position))) {
                            gui.HandleTouch(GUITouchAction.End);
                        }
                        activeTouches.RemoveAt(touchIndex);
                    }
                    break;
                case MotionEventActions.Move:
                    // user moved the finger
                    for (int i = 0; i < activeTouches.Count; i++) {
                        Vector2 activeTouchPosition = new Vector2((e.GetX(pointerIndex) / Window.Size.Width - 0.5f) * 2 * Window.Ratio, (e.GetY(pointerIndex) / Window.Size.Height - 0.5f) * -2);
                        if (activeTouches[i].Position - activeTouchPosition != Vector2.Zero) {
                            // touch moved
                            foreach (GUIItem gui in GUIRenderer.Current) {
                                if (gui.Collides(activeTouchPosition) && !gui.Collides(activeTouches[i].Position)) {
                                    // collides with the current position, but not with the last
                                    gui.HandleTouch(GUITouchAction.Enter);
                                } else if (!gui.Collides(activeTouchPosition) && gui.Collides(activeTouches[i].Position)) {
                                    // collides with the last position, but not with the current -> touch moved out of guielement
                                    gui.HandleTouch(GUITouchAction.Leave);
                                } else if (gui.Collides(activeTouchPosition)) {
                                    // touch moved inside gui
                                    gui.HandleTouch(GUITouchAction.Move);
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
            public Vector2 Position;

            public Touch (int id, Vector2 initialposition) {
                ID = id;
                Position = initialposition;
            }
        }
    }
}