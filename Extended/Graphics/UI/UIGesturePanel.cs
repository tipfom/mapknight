using System;
using System.Collections.Generic;
using Android.Gestures;
using mapKnight.Extended.Graphics.UI.Layout;
using mapKnight.Extended.Graphics.Buffer;
using static mapKnight.Extended.Graphics.Programs.LineProgram;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.UI {
    public class UIGesturePanel : UIPanel {
        const int SWIPE_TIME = 400;
        const int SWIPE_MIN_DIST = 100;

        private Vector2[ ] _Preview;
        public Vector2[ ] Preview {
            get { return _Preview; }
            set {
                _Preview = value;
                previewLength = _Preview?.Length ?? -1;
                if (_Preview != null) {
                    for (int i = 0; i < _Preview.Length; i++) {
                        int p = i << 1;
                        previewBuffer.Data[p] = Layout.X + _Preview[i].X * Layout.Width;
                        previewBuffer.Data[p + 1] = Layout.Y - _Preview[i].Y * Layout.Height;
                    }
                }
            }
        }

        public bool AcceptingGestures = true;
        public event Action<IEnumerable<(string name, float accuracy)>> OnGesturePerformed;

        // CURRENTLY ONLY WITH SUPPORT FOR ANDROID
        private int currentTouchID = -1;
        private IList<GesturePoint> trackedStrokeBuffer = new List<GesturePoint>(100);
        private GestureStore gestureStore;
        private int currentVertexIndex = -1;
        private ClientBuffer activeBuffer;
        private int previewLength;
        private ClientBuffer previewBuffer;

        public UIGesturePanel (Screen owner, UILayout layout) : this(owner, layout, new GestureStore( )) {
        }

        public UIGesturePanel (Screen owner, UILayout layout, GestureStore gesturestore) : base(owner, layout, false) {
            gestureStore = gesturestore;
            activeBuffer = new ClientBuffer(2, 1000, PrimitiveType.Point);
            previewBuffer = new ClientBuffer(2, 10, PrimitiveType.Point);
        }

        public void Add (string name, Gesture gesture) {
            gestureStore.AddGesture(name, gesture);
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
                OnGesturePerformed?.Invoke(null);
            }

            return base.HandleTouch(action, touch);
        }

        private void PushCurrentTouch (UITouch touch) {
            int p = currentVertexIndex << 1;

            activeBuffer.Data[p] = touch.RelativePosition.X / Window.Ratio;
            activeBuffer.Data[p + 1] = touch.RelativePosition.Y;

            currentVertexIndex++;
        }

        private IEnumerable<(string name, float accuracy)> ComputeGesture ( ) {
            // construct gesture
            Gesture gesture = new Gesture( );
            gesture.AddStroke(new GestureStroke(trackedStrokeBuffer));

            // compute gesture
            IList<Prediction> predictions = gestureStore.Recognize(gesture);
            foreach(Prediction p in predictions) {
                yield return (p.Name, (float)p.Score);
            }
        }

        public void Draw (Color previewColor, Color activeColor) {
            if (previewLength >= 0) {
                Program.Begin( );
                Program.Draw(previewBuffer, previewColor, 8f, previewLength, true);
                Program.End( );
            }
            if (currentVertexIndex >= 0) {
                Program.Begin( );
                Program.Draw(activeBuffer, activeColor, 20f, currentVertexIndex, true);
                Program.End( );
            }
        }
    }
}
