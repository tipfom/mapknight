using System.Collections.Generic;
using Android.Views;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI;
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

                        foreach (UIItem UI in UIRenderer.Current.FindAll((UIItem UI) => UI.Collides(touchPosition))) {
                            // iterates through each colliding UI
                            UI.HandleTouch(UITouchAction.Begin);
                        }
                    }
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                case MotionEventActions.PointerUp:
                    // user lifted the finger of the screen
                    int touchIndex = activeTouches.FindIndex((Touch touch) => touch.ID == pointerId);
                    if (touchIndex != -1) {
                        foreach (UIItem UI in UIRenderer.Current.FindAll((UIItem UI) => UI.Collides(activeTouches[touchIndex].Position))) {
                            UI.HandleTouch(UITouchAction.End);
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
                            foreach (UIItem UI in UIRenderer.Current) {
                                if (UI.Collides(activeTouchPosition) && !UI.Collides(activeTouches[i].Position)) {
                                    // collides with the current position, but not with the last
                                    UI.HandleTouch(UITouchAction.Enter);
                                } else if (!UI.Collides(activeTouchPosition) && UI.Collides(activeTouches[i].Position)) {
                                    // collides with the last position, but not with the current -> touch moved out of UIelement
                                    UI.HandleTouch(UITouchAction.Leave);
                                } else if (UI.Collides(activeTouchPosition)) {
                                    // touch moved inside UI
                                    UI.HandleTouch(UITouchAction.Move);
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