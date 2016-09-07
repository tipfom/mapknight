using System;
using System.Collections.Generic;
using System.Text;
using Android.Gestures;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public class UIGesturePanel : UIPanel {
        const double SCORE_THRESHOLD = 1.0;

        // CURRENTLY ONLY WITH SUPPORT FOR ANDROID
        private int currentTouchID;
        private IList<GesturePoint> trackedGesturePoints = new List<GesturePoint>(100);
        private GestureStore gestureStore;

        public UIGesturePanel (Screen owner, UIMargin hmargin, UIMargin vmargin, Vector2 size, GestureStore gesturestore) : base(owner, hmargin, vmargin, size, false) {
            gestureStore = gesturestore;
        }

        public override void HandleTouch (UITouchAction action, UITouch touch) {
            switch (action) {
                case UITouchAction.Move:
                    if (touch.ID == currentTouchID) {
                        trackedGesturePoints.Add(new GesturePoint(touch.Position.X, touch.Position.Y, Environment.TickCount));
                    }
                    break;
                case UITouchAction.Begin:
                case UITouchAction.Enter:
                    if (currentTouchID == -1) {
                        currentTouchID = touch.ID;
                        trackedGesturePoints.Add(new GesturePoint(touch.Position.X, touch.Position.Y, Environment.TickCount));
                    }
                    break;
                case UITouchAction.End:
                case UITouchAction.Leave:
                    if(touch.ID == currentTouchID) {
                        trackedGesturePoints.Add(new GesturePoint(touch.Position.X, touch.Position.Y, Environment.TickCount));
                        GestureCompleted( );
                        currentTouchID = -1;
                    }
                    break;
            }
            base.HandleTouch(action, touch);
        }

        private void GestureCompleted ( ) {
            // construct gesture
            Gesture gesture = new Gesture( );
            gesture.AddStroke(new GestureStroke(trackedGesturePoints));

            // compute gesture
            IList<Prediction> predictions = gestureStore.Recognize(gesture);
            if (predictions.Count > 0 && predictions[0].Score > SCORE_THRESHOLD) {
                Debug.Print(this, predictions[0].Name);
                global::Android.Widget.Toast.MakeText(Assets.Context, predictions[0].Name, global::Android.Widget.ToastLength.Short).Show();
            }
            trackedGesturePoints.Clear( );
        }
    }
}
