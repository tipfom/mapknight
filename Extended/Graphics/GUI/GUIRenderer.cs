using System;
using System.Collections.Generic;
using mapKnight.Extended.Graphics.Buffer;
using static mapKnight.Extended.Graphics.Programs.ColorProgram;

namespace mapKnight.Extended.Graphics.GUI {
    public static class GUIRenderer {
        const int MAX_QUADS = 500;

        private static float[ ] nullArray = new float[ ] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };

        public static SpriteBatch Texture;
        public static List<GUIItem> Current { get { return guiItems[currentScreen]; } }

        private static Screen currentScreen;
        private static Dictionary<Screen, List<GUIItem>> guiItems;
        private static Dictionary<GUIItem, Stack<int>> usedIndicies;
        private static Stack<int> freeIndicies;
        private static BufferBatch buffer;
        private static CachedGPUBuffer vertexBuffer { get { return (CachedGPUBuffer)buffer.VertexBuffer; } }
        private static CachedGPUBuffer textureBuffer { get { return (CachedGPUBuffer)buffer.TextureBuffer; } }
        private static CachedGPUBuffer colorBuffer { get { return (CachedGPUBuffer)buffer.ColorBuffer; } }
        private static bool bufferUpdated = false;

        static GUIRenderer ( ) {
            buffer = new BufferBatch(new IndexBuffer(MAX_QUADS), new CachedGPUBuffer(2, MAX_QUADS), new CachedGPUBuffer(4, MAX_QUADS), new CachedGPUBuffer(2, MAX_QUADS));
            guiItems = new Dictionary<Screen, List<GUIItem>>( );
            ResetIndicies( );
        }

        public static void Prepare (Screen target) {
            currentScreen = target;
            ResetIndicies( );
            vertexBuffer.Cache = new float[buffer.VertexBuffer.Length];
            vertexBuffer.Put( );

            if (guiItems.ContainsKey(target)) {
                foreach (GUIItem item in guiItems[target]) {
                    usedIndicies.Add(item, new Stack<int>( ));
                    Update(item);
                }
            }
        }

        private static void ResetIndicies ( ) {
            usedIndicies = new Dictionary<GUIItem, Stack<int>>( );
            freeIndicies = new Stack<int>( );
            for (int i = MAX_QUADS - 1; i >= 0; i--) {
                freeIndicies.Push(i);
            }
        }

        public static void Add (Screen screen, GUIItem item) {
            if (!guiItems.ContainsKey(screen))
                guiItems.Add(screen, new List<GUIItem>( ));
            guiItems[screen].Add(item);
            usedIndicies.Add(item, new Stack<int>( ));
        }

        public static void Delete ( ) {
            guiItems.Clear( );
        }

        public static void Delete (Screen target) {
            guiItems.Remove(target);
        }

        public static void Update (GUIItem item) {
            bufferUpdated = true;
            while (usedIndicies[item].Count > 0) {
                int index = usedIndicies[item].Pop( );
                freeIndicies.Push(index);
                Array.Copy(nullArray, 0, vertexBuffer.Cache, index * 8, 8);
            }
            if (!item.Visible)
                return;
            foreach (VertexData vertexData in item.GetVertexData( )) {
                int index = freeIndicies.Pop( );
                Array.Copy(vertexData.Verticies, 0, vertexBuffer.Cache, index * 8, 8);
                Array.Copy(vertexData.Color.ToOpenGL( ), 0, colorBuffer.Cache, index * 16, 16);
                Array.Copy(Texture.Get(vertexData.Texture), 0, textureBuffer.Cache, index * 8, 8);

                usedIndicies[item].Push(index);
            }
        }

        public static void Update (TimeSpan dt) {
            if (bufferUpdated) {
                bufferUpdated = false;
                vertexBuffer.Put( );
                colorBuffer.Put( );
                textureBuffer.Put( );
            }
            foreach (GUIItem item in guiItems[currentScreen])
                item.Update(dt);
        }

        public static void Draw ( ) {
            Program.Begin( );
            Program.Draw(buffer, Texture, Matrix.Default, true);
            Program.End( );
        }
    }
}
