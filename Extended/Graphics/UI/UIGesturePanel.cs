using System;
using System.Collections.Generic;
using Android.Gestures;
using mapKnight.Extended.Graphics.UI.Layout;
using mapKnight.Extended.Graphics.Buffer;
using static mapKnight.Extended.Graphics.Programs.LineProgram;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.UI {
    public class UIGesturePanel : UIPanel {
        public const string SWIPE_LEFT = "l", SWIPE_RIGHT = "r", SWIPE_UP = "u", SWIPE_DOWN = "d";

        const double SCORE_THRESHOLD = 1.0;
        const int SWIPE_TIME = 400;
        const int SWIPE_MIN_DIST = 100;

        public bool AcceptingGestures = true;
        public event Action<string> OnGesturePerformed;

        // CURRENTLY ONLY WITH SUPPORT FOR ANDROID
        private int currentTouchID = -1;
        private IList<GesturePoint> trackedStrokeBuffer = new List<GesturePoint>(100);
        private GestureStore gestureStore;
        private int currentVertexIndex = -1;
        private ClientBuffer vertexBuffer;

        public UIGesturePanel (Screen owner, UILayout layout, GestureStore gesturestore) : base(owner, layout, false) {
            gestureStore = gesturestore;

            vertexBuffer = new ClientBuffer(2, 1000, PrimitiveType.Point);
        }

        public override bool HandleTouch (UITouchAction action, UITouch touch) {
            if (AcceptingGestures) {
                switch (action) {
                    case UITouchAction.Move:
                        if (touch.ID == currentTouchID) {
                            trackedStrokeBuffer.Add(new GesturePoint(touch.Position.X, touch.Position.Y, Environment.TickCount));
                            PushCurrentTouch(touch);
                        }
                        break;
                    case UITouchAction.Begin:
                    case UITouchAction.Enter:
                        if (currentTouchID == -1) {
                            currentTouchID = touch.ID;
                            trackedStrokeBuffer.Add(new GesturePoint(touch.Position.X, touch.Position.Y, Environment.TickCount));
                            currentVertexIndex = 0;
                            PushCurrentTouch(touch);
                        }
                        break;
                    case UITouchAction.End:
                    case UITouchAction.Leave:
                        if (touch.ID == currentTouchID) {
                            trackedStrokeBuffer.Add(new GesturePoint(touch.Position.X, touch.Position.Y, Environment.TickCount));
                            OnGesturePerformed?.Invoke(ComputeGesture( ));
                            trackedStrokeBuffer.Clear( );
                            currentTouchID = -1;
                            currentVertexIndex = -1;
                        }
                        break;
                }
            } else {
                OnGesturePerformed?.Invoke(string.Empty);
            }

            return base.HandleTouch(action, touch);
        }

        private void PushCurrentTouch (UITouch touch) {
            int p = currentVertexIndex * 2;

            vertexBuffer.Data[p] = touch.RelativePosition.X / Window.Ratio;
            vertexBuffer.Data[p + 1] = touch.RelativePosition.Y;

            currentVertexIndex++;
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

        public void DrawGestures (Color color) {
            if (currentVertexIndex >= 0) {
                Program.Begin( );
                Program.Draw(vertexBuffer, color, 10f, currentVertexIndex, true);
                Program.End( );
            }
        }
    }
}
