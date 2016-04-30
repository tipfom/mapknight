using System;
using System.Collections.Generic;
using Android.Views;
using Java.Nio;
using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public class GUI : Java.Lang.Object, View.IOnTouchListener {
        const int MAX_TOUCH_COUNT = 3;
        const int MAX_QUADS = 400;

        // buttons ( text und standartbild, nur bild)
        // bar

        private List<Touch> activeTouches = new List<Touch> ( );
        private List<GUIItem> addedItems = new List<GUIItem> ( );

        private CGLSprite2D sprite;
        private Stack<int> freeIndicies = new Stack<int> ( );
        private Dictionary<GUIItem, Stack<int>> usedIndicies = new Dictionary<GUIItem, Stack<int>> ( );
        private FloatBuffer vertexBuffer;
        private FloatBuffer textureBuffer;
        private FloatBuffer colorBuffer;
        private ShortBuffer indexBuffer;

        private float[ ] vertexBufferData;
        private float[ ] textureBufferData;
        private float[ ] colorBufferData;
        private bool areBufferUpToDate = true;

        public GUI () {
            GUIItem.Changed += OnGUIChanged;

            sprite = Assets.Load<CGLSprite2D> ("interface.png", "interface.json");
            vertexBuffer = CGLTools.CreateFloatBuffer (MAX_QUADS * 8);
            vertexBufferData = new float[MAX_QUADS * 8];
            textureBuffer = CGLTools.CreateFloatBuffer (MAX_QUADS * 8);
            textureBufferData = new float[MAX_QUADS * 8];
            colorBuffer = CGLTools.CreateFloatBuffer (MAX_QUADS * 4 * 4);
            colorBufferData = new float[MAX_QUADS * 4 * 4];

            for (int i = 0; i < colorBufferData.Length; i++) {
                colorBufferData[i] = 1f;
            }

            short[ ] indicies = new short[MAX_QUADS * 6];
            for (int i = 0; i < MAX_QUADS; i++) {
                indicies[i * 6 + 0] = (short)(i * 4 + 0);
                indicies[i * 6 + 1] = (short)(i * 4 + 1);
                indicies[i * 6 + 2] = (short)(i * 4 + 2);
                indicies[i * 6 + 3] = (short)(i * 4 + 0);
                indicies[i * 6 + 4] = (short)(i * 4 + 2);
                indicies[i * 6 + 5] = (short)(i * 4 + 3);
            }
            indexBuffer = CGLTools.CreateBuffer (indicies);

            for (int i = MAX_QUADS - 1; i > -1; i--) {
                // fill up free verticies
                freeIndicies.Push (i);
            }

            Screen.Changed += updateAllGUIElements;
        }

        private void updateAllGUIElements () {
            foreach (GUIItem element in addedItems) {
                OnGUIChanged (element);
            }
        }

        private void OnGUIChanged (GUIItem sender) {
            if (!usedIndicies.ContainsKey (sender)) {
                // add guielement
                addedItems.Add (sender);
                usedIndicies.Add (sender, new Stack<int> ( ));
            }

            while (usedIndicies[sender].Count > 0) {
                // free up used space
                int usedIndex = usedIndicies[sender].Pop ( );
                freeIndicies.Push (usedIndex);
                Array.Copy (new float[ ] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, vertexBufferData, usedIndex * 8, 8);
            }

            List<GUIItem.VertexData> senderVertexData = sender.GetVertexData ( );

            if (senderVertexData.Count <= freeIndicies.Count) {
                for (int i = 0; i < senderVertexData.Count; i++) {
                    int newIndex = freeIndicies.Pop ( );
                    usedIndicies[sender].Push (newIndex);

                    Array.Copy (senderVertexData[i].Verticies, 0, vertexBufferData, newIndex * 8, 8);
                    Array.Copy (sprite.Get (senderVertexData[i].Texture), 0, textureBufferData, newIndex * 8, 8);

                    Array.Copy (senderVertexData[i].Color.ToOpenGL ( ), 0, colorBufferData, newIndex * 16, 16);
                }
            } else {
                Log.Print (typeof (GUI), "guielement couldnt be updated, there were not enough free indicies");
            }

            areBufferUpToDate = false;
        }

        public void Draw () {
            Content.ColorProgram.Begin ( );
            Content.ColorProgram.Draw (vertexBuffer, textureBuffer, colorBuffer, indexBuffer, sprite.Texture, Content.Camera.DefaultMVPMatrix, true);
            Content.ColorProgram.End ( );
        }

        public void Update (float dt) {
            if (!areBufferUpToDate) {
                textureBuffer.Put (textureBufferData);
                vertexBuffer.Put (vertexBufferData);
                colorBuffer.Put (colorBufferData);
                textureBuffer.Position (0);
                vertexBuffer.Position (0);
                colorBuffer.Position (0);
                areBufferUpToDate = true;
            }

            foreach (GUIItem item in addedItems) {
                item.Update (dt);
            }
        }

        public void Remove (GUIItem element) {
            if (addedItems.Contains (element)) {
                addedItems.Remove (element);
                usedIndicies.Remove (element);
            }
        }

        #region touch
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

                    foreach (GUIItem gui in addedItems.FindAll ((GUIItem gui) => gui.Collides (touchPosition))) {
                        // iterates through each colliding gui
                        gui.HandleTouch (GUIItem.Action.Begin);
                    }
                }
                break;
            case MotionEventActions.Up:
            case MotionEventActions.Cancel:
            case MotionEventActions.PointerUp:
                // user lifted the finger of the screen
                int touchIndex = activeTouches.FindIndex ((Touch touch) => touch.ID == pointerId);
                if (touchIndex != -1) {
                    foreach (GUIItem gui in addedItems.FindAll ((GUIItem gui) => gui.Collides (activeTouches[touchIndex].Position))) {
                        gui.HandleTouch (GUIItem.Action.End);
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
                        foreach (GUIItem gui in addedItems) {
                            if (gui.Collides (activeTouchPosition) && !gui.Collides (activeTouches[i].Position)) {
                                // collides with the current position, but not with the last
                                gui.HandleTouch (GUIItem.Action.Enter);
                            } else if (!gui.Collides (activeTouchPosition) && gui.Collides (activeTouches[i].Position)) {
                                // collides with the last position, but not with the current -> touch moved out of guielement
                                gui.HandleTouch (GUIItem.Action.Leave);
                            } else if (gui.Collides (activeTouchPosition)) {
                                // touch moved inside gui
                                gui.HandleTouch (GUIItem.Action.Move);
                            }
                        }

                        activeTouches[i].Position = activeTouchPosition;
                    }
                }
                break;
            }

            return true;
        }

        private class Touch {
            // class to be able to change the position
            public readonly int ID;
            public fVector2D Position;

            public Touch (int id, fVector2D initialposition) {
                ID = id;
                Position = initialposition;
            }
        }
        #endregion
    }
}