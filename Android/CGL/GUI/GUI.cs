using System;
using System.Collections.Generic;
using Java.Nio;

namespace mapKnight.Android.CGL.GUI {
    public class GUI {
        const int MAX_TOUCH_COUNT = 3;
        const int MAX_QUADS = 400;

        // buttons ( text und standartbild, nur bild)
        // bar

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
            if (!usedIndicies.ContainsKey (sender))
                return;

            while (usedIndicies[sender].Count > 0) {
                // free up used space
                int usedIndex = usedIndicies[sender].Pop ( );
                freeIndicies.Push (usedIndex);
                Array.Copy (new float[ ] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, vertexBufferData, usedIndex * 8, 8);
            }

            List<CGLVertexData> senderVertexData = sender.GetVertexData ( );

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

        public List<GUIItem> GetAllItems () {
            return addedItems;
        }

        public void Draw () {
            Content.ProgramCollection.Color.Begin ( );
            Content.ProgramCollection.Color.Draw (vertexBuffer, textureBuffer, colorBuffer, indexBuffer, sprite.Texture, Screen.DefaultMatrix.MVP, true);
            Content.ProgramCollection.Color.End ( );
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

        public T Add<T> (T item) where T : GUIItem {
            if (!addedItems.Contains (item)) {
                this.addedItems.Add (item);
                usedIndicies.Add (item, new Stack<int> ( ));
                OnGUIChanged (item);
            }
            return item;
        }
    }
}