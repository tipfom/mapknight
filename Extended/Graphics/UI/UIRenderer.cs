using System;
using System.Collections.Generic;
using mapKnight.Extended.Graphics.Buffer;
using static mapKnight.Extended.Graphics.Programs.ColorProgram;

namespace mapKnight.Extended.Graphics.UI {
    public static class UIRenderer {
        const int MAX_QUADS = 500;

        public static SpriteBatch Texture;
        public static List<UIItem> Current { get { return UIItems[currentScreen]; } }

        private static Screen currentScreen;
        private static Dictionary<Screen, List<UIItem>> UIItems;
        private static Dictionary<UIItem, Stack<int>> usedIndicies;
        private static Stack<int> freeIndicies;
        private static BufferBatch buffer;
        private static CachedGPUBuffer vertexBuffer { get { return (CachedGPUBuffer)buffer.VertexBuffer; } }
        private static CachedGPUBuffer textureBuffer { get { return (CachedGPUBuffer)buffer.TextureBuffer; } }
        private static CachedGPUBuffer colorBuffer { get { return (CachedGPUBuffer)buffer.ColorBuffer; } }
        private static bool bufferUpdated = false;

        static UIRenderer ( ) {
            buffer = new BufferBatch(new IndexBuffer(MAX_QUADS), new CachedGPUBuffer(2, MAX_QUADS), new CachedGPUBuffer(4, MAX_QUADS), new CachedGPUBuffer(2, MAX_QUADS));
            UIItems = new Dictionary<Screen, List<UIItem>>( );
            ResetIndicies( );
        }

        public static void Prepare (Screen target) {
            currentScreen = target;
            ResetIndicies( );
            Array.Clear(vertexBuffer.Cache, 0, vertexBuffer.Cache.Length);
            vertexBuffer.Put( );

            if (UIItems.ContainsKey(target)) {
                foreach (UIItem item in UIItems[target]) {
                    usedIndicies.Add(item, new Stack<int>( ));
                    Update(item);
                }
            }
        }

        private static void ResetIndicies ( ) {
            usedIndicies = new Dictionary<UIItem, Stack<int>>( );
            freeIndicies = new Stack<int>( );
            for (int i = MAX_QUADS - 1; i >= 0; i--) {
                freeIndicies.Push(i);
            }
        }

        public static void Add (Screen screen, UIItem item) {
            if (!UIItems.ContainsKey(screen))
                UIItems.Add(screen, new List<UIItem>( ));
            UIItems[screen].Add(item);
            usedIndicies.Add(item, new Stack<int>( ));
            item.Changed += Update;
        }

        public static void Delete ( ) {
            UIItems.Clear( );
        }

        public static void Delete (Screen target) {
            UIItems.Remove(target);
        }

        public static void Update (UIItem item) {
            bufferUpdated = true;
            while (usedIndicies[item].Count > 0) {
                int index = usedIndicies[item].Pop( );
                freeIndicies.Push(index);
                Array.Clear(vertexBuffer.Cache, index * 8, 8);
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
            foreach (UIItem item in UIItems[currentScreen])
                item.Update(dt);
        }

        public static void Draw ( ) {
            Program.Begin( );
            Program.Draw(buffer, Texture, Matrix.Default, true);
            Program.End( );
        }
    }
}
