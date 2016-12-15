using System.Collections.Generic;
using Android.Views;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI;
using Window = mapKnight.Extended.Graphics.Window;

namespace mapKnight.Android {
    public class TouchHandler : Java.Lang.Object, View.IOnTouchListener {
        const int MAX_TOUCH_COUNT = 4;

        public static TouchHandler Instance { get; } = new TouchHandler( );

        private List<UITouch> activeTouches = new List<UITouch>( );
        
        public bool OnTouch (global::Android.Views.View v, MotionEvent e) {
            int pointerIndex = ((int)(e.Action & MotionEventActions.PointerIdMask) >> (int)MotionEventActions.PointerIdShift);
            int pointerId = e.GetPointerId(pointerIndex);

            switch (e.Action & MotionEventActions.Mask) {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    // user touched the screen
                    if (activeTouches.Count < MAX_TOUCH_COUNT) {
                        UITouch touch = new UITouch(pointerId, new Vector2(e.GetX(pointerIndex), e.GetY(pointerIndex)));
                        activeTouches.Add(touch);

                        foreach (UIItem UI in UIRenderer.Current.FindAll((UIItem UI) => UI.Collides(touch.RelativePosition))) {
                            // iterates through each colliding UI
                            UI.HandleTouch(UITouchAction.Begin, touch);
                        }
                    }
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                case MotionEventActions.PointerUp:
                    // user lifted the finger of the screen
                    int touchIndex = activeTouches.FindIndex((UITouch touch) => touch.ID == pointerId);
                    if (touchIndex != -1) {
                        foreach (UIItem UI in UIRenderer.Current.FindAll((UIItem UI) => UI.Collides(activeTouches[touchIndex].RelativePosition))) {
                            UI.HandleTouch(UITouchAction.End, activeTouches[touchIndex]);
                        }
                        activeTouches.RemoveAt(touchIndex);
                    }
                    break;
                case MotionEventActions.Move:
                    // user moved the finger
                    for (int i = 0; i < activeTouches.Count; i++) {
                        int activePointerIndex = e.FindPointerIndex(activeTouches[i].ID);
                        Vector2 activeTouchPosition = new Vector2(e.GetX(activePointerIndex), e.GetY(activePointerIndex));
                        if (activeTouches[i].Position - activeTouchPosition != Vector2.Zero) {
                            // touch moved
                            Vector2 activeTouchRelativePosition = new Vector2((activeTouchPosition.X / Window.Size.Width - 0.5f) * 2 * Window.Ratio, (activeTouchPosition.Y / Window.Size.Height - 0.5f) * -2);

                            foreach (UIItem UI in UIRenderer.Current) {
                                if (UI.Collides(activeTouchRelativePosition) && !UI.Collides(activeTouches[i].RelativePosition)) {
                                    // collides with the current position, but not with the last
                                    UI.HandleTouch(UITouchAction.Enter, activeTouches[i]);
                                } else if (!UI.Collides(activeTouchRelativePosition) && UI.Collides(activeTouches[i].RelativePosition)) {
                                    // collides with the last position, but not with the current -> touch moved out of UIelement
                                    UI.HandleTouch(UITouchAction.Leave, activeTouches[i]);
                                } else if (UI.Collides(activeTouchRelativePosition)) {
                                    // touch moved inside UI
                                    UI.HandleTouch(UITouchAction.Move, activeTouches[i]);
                                }
                            }

                            activeTouches[i].Position = activeTouchPosition;
                            activeTouches[i].RelativePosition = activeTouchRelativePosition;
                        }
                    }
                    break;
            }

            return true;
        }
    }
}