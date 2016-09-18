using System;
using System.Collections.Generic;
using System.Text;
using Android.Gestures;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public class UIGesturePanel : UIPanel {
        public const string SWIPE_LEFT = "SL", SWIPE_RIGHT = "SR", SWIPE_UP = "SU", SWIPE_DOWN = "SD";

        const double SCORE_THRESHOLD = 1.0;
        const int SWIPE_TIME = 400;
        const int SWIPE_MIN_DIST = 100;

        // CURRENTLY ONLY WITH SUPPORT FOR ANDROID
        private int currentTouchID;
        private IList<GesturePoint> trackedStrokeBuffer = new List<GesturePoint>(100);
        private GestureStore gestureStore;

        public event Action<string> OnGesturePerformed;
        public UIGesturePanel (Screen owner, UIMargin hmargin, UIMargin vmargin, Vector2 size, GestureStore gesturestore) : base(owner, hmargin, vmargin, size, false) {
            gestureStore = gesturestore;
        }

        public override void HandleTouch (UITouchAction action, UITouch touch) {
            switch (action) {
                case UITouchAction.Move:
                    if (touch.ID == currentTouchID) {
                        trackedStrokeBuffer.Add(new GesturePoint(touch.Position.X, touch.Position.Y, Environment.TickCount));
                    }
                    break;
                case UITouchAction.Begin:
                case UITouchAction.Enter:
                    if (currentTouchID == -1) {
                        currentTouchID = touch.ID;
                        trackedStrokeBuffer.Add(new GesturePoint(touch.Position.X, touch.Position.Y, Environment.TickCount));
                    }
                    break;
                case UITouchAction.End:
                case UITouchAction.Leave:
                    if (touch.ID == currentTouchID) {
                        trackedStrokeBuffer.Add(new GesturePoint(touch.Position.X, touch.Position.Y, Environment.TickCount));
                        OnGesturePerformed?.Invoke(ComputeGesture( ));
                        trackedStrokeBuffer.Clear( );
                        currentTouchID = -1;
                    }
                    break;
            }
            base.HandleTouch(action, touch);
        }

        private string ComputeGesture ( ) {
            // check if its a complex gesture or a swipe
            if (trackedStrokeBuffer[trackedStrokeBuffer.Count - 1].Timestamp - trackedStrokeBuffer[0].Timestamp < SWIPE_TIME) {
                GesturePoint first = trackedStrokeBuffer[0], last = trackedStrokeBuffer[trackedStrokeBuffer.Count - 1];
                float dx = Math.Abs(first.X - last.X), dy = Math.Abs(first.Y - last.Y);
                if (dx > dy && dx > SWIPE_MIN_DIST) {
                    if (first.X < last.X) {
                        return SWIPE_RIGHT;
                    } else {
                        return SWIPE_LEFT;
                    }
                } else if (dy > dx && dy > SWIPE_MIN_DIST) {
                    if (first.Y < last.Y) {
                        return SWIPE_DOWN;
                    } else {
                        return SWIPE_UP;
                    }
                }
            } else {
                // construct gesture
                Gesture gesture = new Gesture( );
                gesture.AddStroke(new GestureStroke(trackedStrokeBuffer));

                // compute gesture
                IList<Prediction> predictions = gestureStore.Recognize(gesture);
                if (predictions.Count > 0 && predictions[0].Score > SCORE_THRESHOLD) {
                    return predictions[0].Name;
                }
            }
            return string.Empty;
        }
    }
}
